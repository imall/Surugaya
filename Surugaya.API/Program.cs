using Surugaya.API.Configuration;
using Surugaya.API.DependencyInjection;
using Surugaya.API.Jobs;
using Surugaya.API.Services;
using Surugaya.API.Settings;
using Surugaya.Repository;
using Surugaya.Service;
using Surugaya.Service.Utils;

var builder = WebApplication.CreateBuilder(args);

var productionEnvList = new HashSet<string>(
    ["AwsRelease", "Release", "Production"],
    StringComparer.OrdinalIgnoreCase
);

var isProduction = productionEnvList.Contains(builder.Environment.EnvironmentName);
builder.Services.AddHealthChecks();
// 配置 CORS
var corsSettings = new CorsSettings();
builder.Configuration.GetSection("Cors").Bind(corsSettings);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (corsSettings.AllowAllOrigins)
        {
            policy.AllowAnyOrigin();
        }
        else if (corsSettings.AllowedOrigins.Length > 0)
        {
            policy.WithOrigins(corsSettings.AllowedOrigins);
        }

        if (corsSettings.AllowedMethods.Length > 0)
        {
            policy.WithMethods(corsSettings.AllowedMethods);
        }
        else
        {
            policy.AllowAnyMethod();
        }

        if (corsSettings.AllowedHeaders.Length > 0)
        {
            policy.WithHeaders(corsSettings.AllowedHeaders);
        }
        else
        {
            policy.AllowAnyHeader();
        }

        if (corsSettings.AllowCredentials && !corsSettings.AllowAllOrigins)
        {
            policy.AllowCredentials();
        }

        policy.SetPreflightMaxAge(TimeSpan.FromSeconds(corsSettings.PreflightMaxAgeSeconds));
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerSettings();

// 配置 Supabase 設定
builder.Services.Configure<SupabaseSettings>(
    builder.Configuration.GetSection("Supabase"));

// 配置 CORS 設定
builder.Services.Configure<CorsSettings>(
    builder.Configuration.GetSection("Cors"));

builder.Services.AddSupabase(builder.Configuration);

builder.Services.AddHangFireServices(builder.Configuration);

// 註冊 HttpClient
builder.Services.AddHttpClient();

// 註冊服務
builder.Services.AddScoped<SurugayaUrlsService>();
builder.Services.AddScoped<SurugayaUrlsRepository>();
builder.Services.AddScoped<SurugayaDetailsRepository>();
builder.Services.AddScoped<ScraperUtil>();
builder.Services.AddScoped<SurugayaScraperService>();
builder.Services.AddScoped<SurugayaDetailsService>();
builder.Services.AddScoped<SurugayaCategoryService>();
builder.Services.AddScoped<SurugayaCategoryRepository>();
builder.Services.AddScoped<HealthCheckJobService>();
builder.Services.AddScoped<SurugayaScraperJob>();

var app = builder.Build();

app.UseHealthChecks("/health");
app.UseSwaggerSettings(isProduction);

// 啟用 CORS
app.UseCors();

// 安全地啟用 Hangfire Dashboard（僅在服務已啟用時）
app.UseHangfireDashboardSafely();

app.UseHttpsRedirection();

// 註冊 hangfire job
app.RegisterRecurringJobs();

app.UseAuthorization();

app.MapControllers();

app.Run();
