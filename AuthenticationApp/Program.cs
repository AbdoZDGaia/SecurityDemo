using AuthenticationApp.Data;
using AuthenticationApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AuthDBContext>(options =>
{
    options.UseSqlite(builder.Configuration["ConnectionStrings:Default"]);
});
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/denied";
        options.Events = new CookieAuthenticationEvents()
        {
            OnSigningIn = async ctx =>
            {
                var scheme = ctx.Properties.Items.Where(k => k.Key == ".AuthScheme").FirstOrDefault();
                var claimsIdentity = ctx.Principal.Identity as ClaimsIdentity;
                var userService = ctx.HttpContext.RequestServices.GetRequiredService<IUserService>() as UserService;
                var nameIdentifier = claimsIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                if (userService is not null && nameIdentifier is not null)
                {
                    var appUser = userService.GetUserByExternalProvider(scheme.Value, nameIdentifier);
                    if (appUser is null)
                    {
                        appUser = userService.AddNewUser(scheme.Value, claimsIdentity.Claims.ToList());
                    }
                    foreach (var role in appUser.RoleList)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }
                }
                claimsIdentity.AddClaim(new Claim(scheme.Key, scheme.Value));
            }
        };
    })
    .AddOpenIdConnect("google", options =>
    {
        options.Authority = "https://accounts.google.com";
        options.ClientId = builder.Configuration["Google:ClientId"];
        options.ClientSecret = builder.Configuration["Google:ClientSecret"];
        options.CallbackPath = "/auth";
        options.SaveTokens = true;
    })
    .AddOpenIdConnect("okta", options =>
    {
        options.Authority = builder.Configuration["Okta:OktaDomain"];
        options.ClientId = builder.Configuration["Okta:ClientId"];
        options.ClientSecret = builder.Configuration["Okta:ClientSecret"];
        options.CallbackPath = builder.Configuration["Okta:CallBack"];
        options.SignedOutCallbackPath = builder.Configuration["Okta:SignOutCallback"];
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SaveTokens = true;
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("offline_access");
        //options.Events = new OpenIdConnectEvents()
        //{
        //    OnTokenValidated = async ctx =>
        //    {
        //        var principal = ctx.Principal;
        //        if (principal.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
        //        {
        //            if (principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
        //            .Value == "00u3xsji49ewesC2i5d7")
        //            {
        //                var claimsIdentity = principal.Identity as ClaimsIdentity;
        //                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
        //            }
        //        }
        //        await Task.CompletedTask;
        //    },
        //};
    });
//.AddGoogle(options =>
//{
//    options.ClientId = builder.Configuration["Google:ClientId"];
//    options.ClientSecret = builder.Configuration["Google:ClientSecret"];
//    options.CallbackPath = "/auth";
//    options.AuthorizationEndpoint += "?prompt=consent";
//});

builder.Services.AddScoped<IUserService, UserService>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
