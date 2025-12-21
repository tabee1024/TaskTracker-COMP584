using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using TaskTracker.Api.Data;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ----------------------
// EF Core (SQLite)
// ----------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

// ----------------------
// Identity
// ----------------------
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ----------------------
// JWT Authentication
// ----------------------
var jwtKey = configuration["Jwt:Key"] ?? "ChangeThis_ReplaceIn_AppSettings";
var jwtIssuer = configuration["Jwt:Issuer"] ?? "TaskTrackerAPI";
var jwtAudience = configuration["Jwt:Audience"] ?? "TaskTrackerClient";

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // OK for local dev
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,

        ValidateAudience = true,
        ValidAudience = jwtAudience,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

// CORS not required when Angular is served from the API


// ----------------------
// Controllers + Swagger registration (middleware disabled below)
// ----------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();

// ----------------------
// Ensure DB exists
// ----------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// ----------------------
// Middleware
// ----------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // Swagger disabled (was causing 500 on swagger.json)
    // app.UseSwagger();
    // app.UseSwaggerUI();

}

// Disable HTTPS redirect for LAN/demo (prevents "Failed to determine the https port")
 // app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Serve Angular from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
