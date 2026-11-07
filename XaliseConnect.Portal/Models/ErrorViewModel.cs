namespace XaliseConnect.Portal.Models;

/// <summary>
/// Représente les informations affichées sur la page d'erreur.
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Obtient ou définit l'identifiant de la requête associée à l'erreur.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Indique si l'identifiant de requête doit être affiché.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
