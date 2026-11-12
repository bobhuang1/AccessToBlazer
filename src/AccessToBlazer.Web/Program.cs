using AccessToBlazer.Web.Components;
using AccessToBlazer.Web.Data;
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
    .AddInteractiveServerRenderMode();

app.Run();