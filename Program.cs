using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CS395SI_Spring2023_Group1.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<CS395SI_Spring2023_Group1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CS395SI_Spring2023_Group1Context") ?? throw new InvalidOperationException("Connection string 'CS395SI_Spring2023_Group1Context' not found.")));


//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
//.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CS395SI_Spring2023_Group1Context") ?? throw new InvalidOperationException("Connection string 'CS395SI_Spring2023_Group1Context' not found.")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings - SECURE CONFIGURATION
    options.Cookie.HttpOnly = true;  // ✅ Prevents JavaScript access (XSS protection)
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // ✅ HTTPS only
    options.Cookie.SameSite = SameSiteMode.Lax;  // ✅ CSRF protection (Lax allows GET from external sites)
    options.ExpireTimeSpan = TimeSpan.FromHours(2);  // ✅ Reasonable session timeout
    
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;  // ✅ Extends session on activity
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;  // ✅ Secure session cookies too
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // ✅ HTTPS only
});

var app = builder.Build();
app.UseSession();

// Configure the HTTP request pipeline.
if ( !app.Environment.IsDevelopment() )
{
    app.UseExceptionHandler( "/Error" );
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CS395SI_Spring2023_Group1.Middleware.ApplicationRequiredMiddleware>();
app.MapRazorPages();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedRolesAndAdminAsync(scope.ServiceProvider);
}


app.Run();

//Required for WebApplicationFactory in integration tests
public partial class Program { }
