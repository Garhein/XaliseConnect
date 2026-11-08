using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using XaliseConnect.Portal.Models;

namespace XaliseConnect.Portal.Controllers;

/// <summary>
/// Traite les demandes des pages générales du portail.
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Affiche la page d'accueil du portail.
    /// </summary>
    /// <returns>La vue de la page d'accueil.</returns>
    public IActionResult Index()
    {
        return this.View();
    }

    /// <summary>
    /// Affiche la page relative à la politique de confidentialité.
    /// </summary>
    /// <returns>La vue de la politique de confidentialité.</returns>
    public IActionResult Privacy()
    {
        return this.View();
    }

    /// <summary>
    /// Affiche la page d'erreur de la requête en cours.
    /// </summary>
    /// <returns>La vue d'erreur contenant l'identifiant de la requête.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
    }
}
