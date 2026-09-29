using AccessToBlazer.Web.Components;
using AccessToBlazer.Web.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("SampleDb")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=AccessToBlazerSample;Trusted_Connection=True;MultipleActiveResultSets=true";
builder.Services.AddDbContextFactory<SampleDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Create the LocalDB database and apply migrations on startup so the sample
// runs with no manual setup steps.
using (var scope = app.Services.CreateScope())
{
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