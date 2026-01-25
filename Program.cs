
using Facility_Management.Converters;
using Facility_Management.Models;
using Facility_Management.Repository;
using Facility_Management.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ----------------- Controllers + JSON -----------------
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        // Global DateTime converter
        opt.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
        // opt.JsonSerializerOptions.WriteIndented = true; // optional
    });

// ----------------- Swagger -----------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Facility Management API", Version = "v1" });

    // JWT support in Swagger ("Authorize" button)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ----------------- DbContext -----------------
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// ----------------- CORS -----------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// ----------------- Identity -----------------
var core = builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 6;
});

var identityBuilder = new IdentityBuilder(core.UserType, typeof(IdentityRole), builder.Services);
identityBuilder
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager<SignInManager<ApplicationUser>>() // SignInManager available
    .AddDefaultTokenProviders();                         // Needed for password reset tokens

// ? Added: HttpContextAccessor (helps SignInManager and your services when they need HttpContext)
builder.Services.AddHttpContextAccessor();

// ----------------- JWT Auth -----------------
var jwtSection = builder.Configuration.GetSection("Jwt");
var keyStr = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
        // options.RequireHttpsMetadata = false; // optional for local http testing
    });

// ----------------- Authorization Policies -----------------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    options.AddPolicy("FacilityManagerOrAdmin", p => p.RequireRole("FacilityManager", "Admin"));
});

// ----------------- DI Registrations -----------------
builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<JwtTokenService>();

// Example hosted service you already had
builder.Services.AddHostedService<NoShowBackgroundService>();

var app = builder.Build();

// ----------------- Middleware Pipeline -----------------
app.UseCors("AllowAngular");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ----------------- Seed Roles -----------------
using (var scope = app.Services.CreateScope())
{
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = new[] { "Admin", "FacilityManager", "User" };
    foreach (var r in roles)
    {
        if (!await roleMgr.RoleExistsAsync(r))
            await roleMgr.CreateAsync(new IdentityRole(r));
    }
}

app.Run();
