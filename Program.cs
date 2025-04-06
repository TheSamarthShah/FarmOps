using FarmOps.Models;
using FarmOps.Repos;
using FarmOps.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Speckle.Newtonsoft.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddScoped<ILoginRepository, LoginRepository>();  // Correct registration
builder.Services.AddScoped<ILoginRepository>(provider =>
{
    var connectionString = provider.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");

    var dbContext = provider.GetRequiredService<DBContext>();

    return new LoginRepository(dbContext, connectionString);
});
builder.Services.AddScoped<IAttendanceRepository>(provider =>
{
    var connectionString = provider.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");

    var dbContext = provider.GetRequiredService<DBContext>();

    return new AttendanceRepository(dbContext, connectionString);
});
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<AttendanceService>();

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],  // From appsettings.json
            ValidAudience = builder.Configuration["Jwt:Audience"],  // From appsettings.json
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))  // Secret key from appsettings.json
        };
    });
builder.Services.AddAuthentication("Cookies")
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";  // Redirect to login if not authenticated
        options.LogoutPath = "/Login/Logout";  // Redirect to login if not authenticated
    });
builder.Services.AddSession(options =>
{
    // Optional: Set session timeout
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true; // Ensure session cookie is HttpOnly
    options.Cookie.IsEssential = true; // Mark session cookie as essential
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
/*app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        int statusCode = 500; // Default to InternalServerError
        if (exception is HttpRequestException httpEx)
        {
            statusCode = 400; // Example: Bad Request
        }

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonConvert.SerializeObject(new { status = "error", statuscode = statusCode, message= }));
    });
});*/

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();
