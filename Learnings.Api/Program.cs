using Learnings.Api;
using Learnings.Api.Hubs;
using Learnings.Application.Dtos;
using Learnings.Application.Repositories.Interface;
using Learnings.Application.Services.CurrentLoggedInUser;
using Learnings.Application.Services.Interface;
using Learnings.Application.Services.Interface.Contracts.HUBS;
using Learnings.Application.Services.Products;
using Learnings.Domain.Entities;
using Learnings.Infrastrcuture.ApplicationDbContext;
using Learnings.Infrastrcuture.Repositories.Implementation;
using Learnings.Infrastructure.Mail.InterfaceService;
using Learnings.Infrastructure.Services;
using Learnings.Infrastructure.Services.CONTRACTS.HUBS;
using Learnings.Infrastructure.Services.CurrentUserLoggedIn;
using Learnings.Infrastructure.Services.Implementation;
using Learnings.Infrastructure.Services.Products;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:8080");

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          // Allow specific domains (you can add more domains as required)
                          builder.WithOrigins("http://localhost:4200") // Frontend URL (adjust if different)
                                 .AllowAnyHeader()  // Allow any headers
                                 .AllowAnyMethod() // Allow any HTTP methods (GET, POST, PUT, etc.)
                          .AllowCredentials();
                      });
});
var configuration = builder.Configuration;

//signalR
builder.Services.AddSignalR();

// JWT Settings
builder.Services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IMailService, MailService>();
var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();


// Configure services.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Configure DbContext.
builder.Services.AddDbContext<LearningDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
   opt.TokenLifespan = TimeSpan.FromHours(2));

// Configure Identity
builder.Services.AddIdentity<Users, IdentityRole>()
    .AddEntityFrameworkStores<LearningDbContext>()
    .AddDefaultTokenProviders();

// Configure custom services and repositories.
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRolesService, UserRolesService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPermissionsService, PermissionsService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();

//Add User Service to get logged in Id
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

//ProductAttriButes
builder.Services.AddScoped<IProductsLookUpAttributeService, ProductsLookUpAttributeService>();
builder.Services.AddScoped<ICategoryService,CategoryService> ();
//user address
builder.Services.AddScoped<IUserAddressService, UserAddressService>();
//signalR
builder.Services.AddScoped<IReviewNotificationService, ReviewNotificationService>();
builder.Services.AddScoped<IReviewService, ReviewService>();


builder.Services.AddAuthentication(options =>
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
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token was successfully validated.");
            return Task.CompletedTask;
        }
    };

});
builder.Services.AddAuthorization();

// Swagger configuration
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPITest", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

// Configure Identity options.
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

var aiConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTIONSTRING"];

// Configure Serilog
var loggerConfig = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "learnings-api")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Console();

// Add Application Insights sink only if connection string exists
if (!string.IsNullOrWhiteSpace(aiConnectionString))
{
    var telemetryConfig = new TelemetryConfiguration
    {
        ConnectionString = aiConnectionString
    };

    loggerConfig.WriteTo.ApplicationInsights(
        telemetryConfig,
        TelemetryConverter.Traces,
        Serilog.Events.LogEventLevel.Information);
}

Log.Logger = loggerConfig.CreateLogger();
builder.Host.UseSerilog();

// Register Application Insights SDK
if (!string.IsNullOrWhiteSpace(aiConnectionString))
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = aiConnectionString;
        options.EnableAdaptiveSampling = true;
        options.EnableQuickPulseMetricStream = true;
    });

    // Add custom telemetry initializer
    builder.Services.AddSingleton<ITelemetryInitializer>(sp =>
        new TelemetryInitializer
        {
            RoleName = "learnings-api",
            EnvironmentName = builder.Environment.EnvironmentName
        });
}

// Build and configure the app
var app = builder.Build();
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}
app.UseCors(MyAllowSpecificOrigins);

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ReviewHub>("/hubs/reviews");
app.MapControllers();
Log.Information("Starting Learnings API in {Environment} environment", builder.Environment.EnvironmentName);
app.Run();
