using LoginAuthify.Common;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.Text;


var builder = WebApplication.CreateBuilder(args);
JwtTokenGenerator.Initialize(builder.Configuration);
// Add services to the container.
builder.Services.AddControllersWithViews();


// Some browsers block OAuth responses due to strict SameSite cookie policies. Try relaxing the policy in Program.cs:
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

// Load JWT settings from configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var authConfig = builder.Configuration.GetSection("Authentication");

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "issuer123",
        ValidAudience = "audience123",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my-new-secret-key"))
    };
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
})
.AddTwitter(twitterOptions =>
{
    twitterOptions.ConsumerKey = authConfig["Twitter:ClientId"];
    twitterOptions.ConsumerSecret = authConfig["Twitter:ClientSecret"];
    twitterOptions.CallbackPath = "/signin-twitter"; // Ensure this matches Twitter settings
})
.AddLinkedIn(linkedinOptions =>
{
    linkedinOptions.ClientId = authConfig["LinkedIn:ClientId"];
    linkedinOptions.ClientSecret = authConfig["LinkedIn:ClientSecret"];
    linkedinOptions.CallbackPath = "/signin-linkedin";
    linkedinOptions.Scope.Add("r_liteprofile");
    linkedinOptions.Scope.Add("r_emailaddress");
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
