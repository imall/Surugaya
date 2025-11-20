using Supabase;
using Surugaya.API.Configuration;
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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerSettings();

// 配置 Supabase 設定
builder.Services.Configure<SupabaseSettings>(
    builder.Configuration.GetSection("Supabase"));

// 配置駿河屋爬蟲設定
builder.Services.Configure<SurugayaScraperSettings>(
    builder.Configuration.GetSection("SurugayaScraper"));

// 配置 CORS 設定
builder.Services.Configure<CorsSettings>(
    builder.Configuration.GetSection("Cors"));

// 配置 Supabase
builder.Services.AddSingleton<Client>(provider =>
{
    var configuration = builder.Configuration;
    var supabaseSettings = new SupabaseSettings();
    configuration.GetSection("Supabase").Bind(supabaseSettings);

    if (string.IsNullOrEmpty(supabaseSettings.Url))
        throw new InvalidOperationException("Supabase URL 未設定");

    if (string.IsNullOrEmpty(supabaseSettings.AnonKey))
        throw new InvalidOperationException("Supabase AnonKey 未設定");

    var options = new SupabaseOptions
    {
        AutoConnectRealtime = false // 根據需要啟用即時功能
    };

    // 使用服務金鑰 (如果有設定) 或匿名金鑰
    var apiKey = supabaseSettings.GetApiKey();
    var client = new Client(supabaseSettings.Url, apiKey, options);
    client.InitializeAsync().Wait(); // 初始化客戶端
    return client;
});

// 註冊服務
builder.Services.AddScoped<SurugayaUrlsService>();
builder.Services.AddScoped<SurugayaUrlsRepository>();
builder.Services.AddScoped<SurugayaDetailsRepository>();
builder.Services.AddScoped<ScraperUtil>();
builder.Services.AddScoped<SurugayaScraperService>();
builder.Services.AddScoped<SurugayaDetailsService>();
builder.Services.AddScoped<SurugayaCategoryService>();
builder.Services.AddScoped<SurugayaCategoryRepository>();

// 註冊背景服務
builder.Services.AddHostedService<SurugayaScraperBackgroundService>();

var app = builder.Build();

app.UseHealthChecks("/health");
app.UseSwaggerSettings(isProduction);

// 啟用 CORS
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
