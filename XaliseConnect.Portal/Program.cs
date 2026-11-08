using XaliseConnect.Application.IntegrationRequest;

namespace XaliseConnect.Portal;

/// <summary>
/// Représente le point d'entrée de l'application web Xalise Connect.
/// </summary>
public class Program
{
    /// <summary>
    /// Configure et démarre l'application web.
    /// </summary>
    /// <param name="args">Arguments transmis au démarrage de l'application.</param>
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Ajoute les services MVC au conteneur d'injection de dépendances.
        builder.Services.AddControllersWithViews();
        builder.Services.AddSingleton<IIntegrationRequestQueryService, MockIntegrationRequestQueryService>();
        WebApplication app = builder.Build();

        // Configure le pipeline HTTP de l'application.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }

        app.UseRouting();
        app.UseAuthorization();
        app.MapStaticAssets();

        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
           .WithStaticAssets();

        app.Run();
    }
}
