using System.Reflection;
using Microsoft.OpenApi.Models;

namespace Surugaya.API.Settings;

/// <summary>
/// Swagger 設定
/// </summary>
public static class SwaggerSettings
{
    /// <summary>
    /// Swagger 相關註冊內容
    /// </summary>
    public static void AddSwaggerSettings(this IServiceCollection services)
    {
        services.AddSwaggerGen(setupAction =>
        {
            setupAction.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "駿河屋願望清單",
                    Version = "v1",
                    Description = "駿河屋願望清單 API",
                });

            var basePath = AppContext.BaseDirectory;
            var xmlFiles = Directory.EnumerateFiles(basePath, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (var xmlFile in xmlFiles)
            {
                setupAction.IncludeXmlComments(xmlFile, true);
            }
        });
    }

    /// <summary>
    /// Swagger 中介軟體設定
    /// </summary>
    public static void UseSwaggerSettings(this WebApplication app, bool isProduction)
    {
        if (isProduction)
        {
            return;
        }

        app.UseSwagger();

        app.UseSwaggerUI(setupAction =>
        {
            setupAction.SwaggerEndpoint($"/swagger/v1/swagger.json", $"Dotnet conf 2024 範例專案 v1");
            setupAction.DisplayRequestDuration();
            setupAction.EnableDeepLinking();
            setupAction.EnableFilter();
        });
    }
}
