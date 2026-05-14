using Microsoft.EntityFrameworkCore;
using PrepaidWallet.Data;
using PrepaidWallet.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Services ───────────────────────────────────────────────────────────────

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title       = "Prepaid Wallet API",
        Version     = "v1",
        Description = "Operator API for managing prepaid user balances."
    });
});

// ── Database ───────────────────────────────────────────────────────────────

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

// ── Application services ───────────────────────────────────────────────────

builder.Services.AddScoped<IPrepaidUserService, PrepaidUserService>();

builder.Services.AddCors(o =>
    o.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// ── Build ──────────────────────────────────────────────────────────────────

var app = builder.Build();

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Prepaid Wallet API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();

// MVC default route - Dashboard loads at root
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Keep existing API routes
app.MapControllers();

app.Run();
