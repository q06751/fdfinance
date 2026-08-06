using System.Text;
using FdFinance.Application;
using FdFinance.Application.Interfaces;
using FdFinance.Infrastructure;
using FdFinance.Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls(builder.Configuration["Urls"] ?? "http://127.0.0.1:18765");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtKey = builder.Configuration["Jwt:Key"]
             ?? Environment.GetEnvironmentVariable("FDFINANCE_JWT_KEY")
             ?? "FdFinance-Dev-Secret-Key-At-Least-32-Chars!!";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "FdFinance";
var audience = builder.Configuration["Jwt:Audience"] ?? "FdFinance.Client";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FdFinance API",
        Version = "v1",
        Description = "复大财务 · 报销/付款/收款审批 API"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
                  ?? Array.Empty<string>();
builder.Services.AddCors(o =>
{
    o.AddDefaultPolicy(p =>
    {
        if (corsOrigins.Length == 0 || corsOrigins.Contains("*"))
        {
            // 演示默认放开；生产请配置 Cors:Origins 白名单
            p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
        }
        else
        {
            p.WithOrigins(corsOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        }
    });
});

var app = builder.Build();

await DbSeeder.SeedAsync(app.Services);

if (app.Configuration.GetValue("Swagger:Enabled", app.Environment.IsDevelopment()))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

var sigDir = app.Configuration["App:SignatureDir"]
             ?? Environment.GetEnvironmentVariable("FDFINANCE_SIGNATURE_DIR")
             ?? Path.Combine(app.Environment.ContentRootPath, "data", "signatures");
Directory.CreateDirectory(sigDir);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(sigDir),
    RequestPath = "/signatures"
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/api/health", (ICacheService cache, INotificationService notify, IConfiguration cfg) =>
{
    var provider = cfg["Database:Provider"] ?? "Sqlite";
    var pwdMode = cfg["Password:WriteMode"] ?? "Bcrypt";
    return Results.Ok(new
    {
        status = "ok",
        service = "FdFinance.Api",
        redis = cache.IsRedisConnected,
        dingtalk = notify.IsEnabled,
        database = provider,
        passwordWriteMode = pwdMode,
        time = DateTime.UtcNow
    });
});

app.Run();

public partial class Program { }
