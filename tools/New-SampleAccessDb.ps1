$ErrorActionPreference = 'Stop'
# docs/ next to this tools/ folder, wherever the repository is cloned.
$outDir = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\docs'))
$dbPath = Join-Path $outDir 'AccessToBlazerSample.accdb'
$acForm = 2

Add-Type -Namespace AccessToBlazer -Name Win32 -MemberDefinition @'
[DllImport("user32.dll")]
public static extern uint GetWindowThreadProcessId(System.IntPtr hWnd, out uint processId);
'@

# Starts Access and remembers the process ID of *this* instance, so the script only
# ever stops the Access it started - never a database the user has open elsewhere.
$script:accessPids = @{}
function New-Access {
    $a = New-Object -ComObject Access.Application
    $a.Visible = $true
    Start-Sleep -Seconds 3
    $accessPid = [uint32]0
    [void][AccessToBlazer.Win32]::GetWindowThreadProcessId([System.IntPtr]$a.hWndAccessApp(), [ref]$accessPid)
    $script:accessPids[$a] = $accessPid
    return $a
}
function Quit-Access($a) {
    $accessPid = $script:accessPids[$a]
    try { $a.DoCmd.Quit(0) | Out-Null } catch {}
    Start-Sleep -Seconds 2
    if ($accessPid) {
        # Quit() can leave the instance hanging after a COM failure; stop only that one.
        Get-Process -Id $accessPid -ErrorAction SilentlyContinue | Stop-Process -Force
        $script:accessPids.Remove($a)
    }
    Start-Sleep -Seconds 1
}

if (Test-Path $dbPath) {
    try { Remove-Item $dbPath -Force }
    catch { throw "Cannot replace $dbPath. Close it in Access and run the script again." }
}

# ---------------- Phase 1: database, tables, seed ----------------
$a = New-Access
try {
    $a.NewCurrentDatabase($dbPath)
    $db = $a.CurrentDb()
    $db.Execute("CREATE TABLE Products (ProductID COUNTER PRIMARY KEY, ProductName TEXT(80) NOT NULL, UnitPrice CURRENCY, UnitsInStock INTEGER)")
    $db.Execute("CREATE TABLE Customers (CustomerID COUNTER PRIMARY KEY, CustomerName TEXT(80) NOT NULL, City TEXT(60), Phone TEXT(30))")
    $db.Execute("CREATE TABLE Orders (OrderID COUNTER PRIMARY KEY, CustomerID INTEGER, ProductID INTEGER, Quantity INTEGER, OrderDate DATETIME)")
    foreach ($row in @("('Widget A', 9.99, 120)","('Widget B', 14.50, 85)","('Gadget X', 39.95, 40)","('Gizmo Basic', 5.25, 300)","('Gizmo Pro', 19.99, 150)","('Turbo Chip', 79.00, 12)")) {
        $db.Execute("INSERT INTO Products (ProductName, UnitPrice, UnitsInStock) VALUES $row")
    }
    foreach ($row in @("('Acme Corp', 'Springfield', '555-0101')","('Globex Inc', 'Shelbyville', '555-0142')","('Initech', 'Austin', '555-0177')","('Hooli', 'Palo Alto', '555-0113')","('Stark Industries', 'New York', '555-0190')")) {
        $db.Execute("INSERT INTO Customers (CustomerName, City, Phone) VALUES $row")
    }
    foreach ($row in @("(1, 1, 10, #2026-01-05#)","(1, 4, 25, #2026-01-18#)","(2, 3, 2, #2026-02-02#)","(3, 5, 8, #2026-02-14#)","(4, 6, 1, #2026-03-01#)","(5, 2, 12, #2026-03-22#)")) {
        $db.Execute("INSERT INTO Orders (CustomerID, ProductID, Quantity, OrderDate) VALUES $row")
    }
    $db.Close()
    Write-Output 'phase1: tables+seed OK'
}
finally { Quit-Access $a }

# ---------------- Phase 2: one form per fresh instance ----------------
function Build_Form {
    param($name, $recordSource, $caption, $fields)
    for ($try = 1; $try -le 5; $try++) {
        $a = New-Access
        try {
            $a.OpenCurrentDatabase($dbPath)
            $frm = $a.CreateForm()
            $gen = $frm.Name
            $frm.RecordSource = $recordSource
            $frm.Caption = $caption
            foreach ($f in $fields) {
                if ($f.combo) {
                    $ctl = $a.CreateControl($gen, 111, 0, '', $f.fld, 2040, $f.top, $f.w, 300)
                    $ctl.ControlSource = $f.fld
                    $ctl.RowSourceType = 'Table/Query'
                    $ctl.RowSource     = $f.combo
                    $ctl.BoundColumn   = 1
                    $ctl.ColumnCount   = 2
                    $ctl.ColumnWidths  = '0;2880'
                } else {
                    $ctl = $a.CreateControl($gen, 109, 0, '', $f.fld, 2040, $f.top, $f.w, 300)
                    $ctl.ControlSource = $f.fld
                }
                $null = $a.CreateControl($gen, 100, 0, $ctl.Name, ($f.fld + 'Lbl'), 480, $f.top, 1560, 300)
            }
            $a.DoCmd.Save($acForm, $gen)
            $a.DoCmd.Close($acForm, $gen)
            $a.DoCmd.Rename($name, $acForm, $gen)
            Write-Output "  form $name OK"
            Quit-Access $a
            return
        }
        catch {
            Write-Output "  form $name attempt $try failed: $($_.Exception.Message)"
            Quit-Access $a
        }
    }
    throw "form $name failed after 5 attempts"
}

Build_Form 'Products' 'SELECT * FROM Products' 'Products' @(
    @{ fld='ProductID';    top=120;  w=1200 },
    @{ fld='ProductName';  top=420;  w=3000 },
    @{ fld='UnitPrice';    top=720;  w=1500 },
    @{ fld='UnitsInStock'; top=1020; w=1200 }
)
Build_Form 'Customers' 'SELECT * FROM Customers' 'Customers' @(
    @{ fld='CustomerID';   top=120;  w=1200 },
    @{ fld='CustomerName'; top=420;  w=3000 },
    @{ fld='City';         top=720;  w=2400 },
    @{ fld='Phone';        top=1020; w=1800 }
)
Build_Form 'Orders' 'SELECT * FROM Orders' 'Orders' @(
    @{ fld='OrderID';    top=120;  w=1200 },
    @{ fld='CustomerID'; top=420;  w=2600; combo='SELECT CustomerID, CustomerName FROM Customers ORDER BY CustomerName' },
    @{ fld='ProductID';  top=720;  w=2600; combo='SELECT ProductID, ProductName FROM Products ORDER BY ProductName' },
    @{ fld='Quantity';   top=1020; w=1200 },
    @{ fld='OrderDate';  top=1320; w=1800 }
)

Write-Output ("OK: created " + $dbPath)