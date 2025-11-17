using Supabase;
using Surugaya.API.Configuration;
using Surugaya.API.Settings;
using Surugaya.Repository;
using Surugaya.Service;
using Surugaya.Service.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerSettings();

// 配置 Supabase 設定
builder.Services.Configure<SupabaseSettings>(
    builder.Configuration.GetSection("Supabase"));

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
builder.Services.AddScoped<SurugayaService>();
builder.Services.AddScoped<SurugayaRepository>();
builder.Services.AddScoped<SurugayaDetailsRepository>();
builder.Services.AddScoped<ScraperUtil>();
builder.Services.AddScoped<SurugayaScraperService>();
builder.Services.AddScoped<SurugayaDetailsService>();

var app = builder.Build();


app.UseSwaggerSettings();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
