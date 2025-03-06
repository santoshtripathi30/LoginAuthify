using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// Some browsers block OAuth responses due to strict SameSite cookie policies. Try relaxing the policy in Program.cs:
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

var authConfig = builder.Configuration.GetSection("Authentication");

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Extend session lifetime
        options.SlidingExpiration = true;
    })

.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = authConfig["Google:ClientId"];
    googleOptions.ClientSecret = authConfig["Google:ClientSecret"];
    googleOptions.CallbackPath = "/signin-google";
})
.AddGitHub(githubOptions =>
{
    githubOptions.ClientId = authConfig["GitHub:ClientId"];
    githubOptions.ClientSecret = authConfig["GitHub:ClientSecret"];
    githubOptions.CallbackPath = "/signin-github";
});


var app = builder.Build();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
