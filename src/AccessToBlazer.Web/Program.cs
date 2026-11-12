using AccessToBlazer.Web.Components;
using AccessToBlazer.Web.Data;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// The LocalDB fallback is for the local demo only; any other environment must
// configure ConnectionStrings:SampleDb explicitly and fails fast without it.
var connectionString = builder.Configuration.GetConnectionString("SampleDb")
    ?? (builder.Environment.IsDevelopment()
        ? "Server=(localdb)\\MSSQLLocalDB;Database=AccessToBlazerSample;Trusted_Connection=True;MultipleActiveResultSets=true"
        : throw new InvalidOperationException(
            "ConnectionStrings:SampleDb is not configured. Set it, or run with ASPNETCORE_ENVIRONMENT=Development to use LocalDB."));
builder.Services.AddDbContextFactory<SampleDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Create the LocalDB database and apply migrations on startup so the sample
// runs with no manual setup steps. Only in Development (or when explicitly
// enabled with MigrateOnStartup=true): the app login then needs no DDL rights
// elsewhere and multiple instances cannot race on the migration.
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("MigrateOnStartup"))
{
    using var scope = app.Services.CreateScope();
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<SampleDbContext>>();
    using var db = factory.CreateDbContext();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
// Security headers on every response. Blazor Server needs same-origin scripts and
// its websocket (covered by connect-src 'self'); style-src keeps 'unsafe-inline'
// because Blazor and the components emit inline style attributes.
string BuildContentSecurityPolicy(string nonce) => string.Join("; ",
    "default-src 'self'",
    $"script-src 'self' 'nonce-{nonce}'",
    "style-src 'self' 'unsafe-inline'",
    "img-src 'self' data:",
    "font-src 'self'",
    "connect-src 'self'",
    "object-src 'none'",
    "base-uri 'self'",
    "form-action 'self'",
    "frame-ancestors 'none'");
app.Use(async (context, next) =>
{
    // The only inline script is the import map Blazor emits in App.razor; it gets this
    // per-request nonce (an import map cannot be loaded from a file).
    var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
    context.Items[CspNonce.ItemKey] = nonce;
    var headers = context.Response.Headers;
    headers.ContentSecurityPolicy = BuildContentSecurityPolicy(nonce);
    headers.XContentTypeOptions = "nosniff";
    headers.XFrameOptions = "DENY";
    headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(options =>
        // Blazor adds its own frame-ancestors CSP header (default 'self'); keep it consistent with ours.
        options.ContentSecurityFrameAncestorsPolicy = "'none'");

app.Run();

/// <summary>Where the per-request CSP nonce is stored in <c>HttpContext.Items</c>.</summary>
internal static class CspNonce
{
    public const string ItemKey = "csp-nonce";
}
