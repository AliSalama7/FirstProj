    using Microsoft.EntityFrameworkCore;
using Movies.Domain.Interfaces;
using Movies.EF.Repositories;
using NToastNotify;
using Microsoft.AspNetCore.Identity;
using Movies.Domain.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using MoviesApp.Services;
using Movies.EF.Data;
using Microsoft.AspNetCore.Authorization;
using Movies.EF.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});
//Add services to the container.
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MoviesDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection"),
b => b.MigrationsAssembly(typeof(MoviesDbContext).Assembly.FullName)));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<MoviesDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

builder.Services.AddTransient(typeof(IGenericRepository<> ), typeof(GenericRepository<>));
builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddMvc().AddNToastNotifyToastr(new NToastNotify.ToastrOptions()
{
    ProgressBar = true,
    PositionClass = ToastPositions.TopRight,
    PreventDuplicates =true,
    CloseButton = true
}) ;
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;
});


var app = builder.Build();
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var loggerFactory = services.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger("app");
try
{
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await Movies.EF.Seeds.SeedRoles.SeedAsync(roleManager);
    await Movies.EF.Seeds.SeedUsers.SeedBasicUser(userManager);
    await Movies.EF.Seeds.SeedUsers.SeedModerator(userManager, roleManager);

    logger.LogInformation("Data Seeding ");
}
catch (Exception ex)
{
    logger.LogWarning(ex, "Error Occurred while Seeding data");
}




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

 app.UseEndpoints(endpoints => endpoints.MapRazorPages());

app.Run();
