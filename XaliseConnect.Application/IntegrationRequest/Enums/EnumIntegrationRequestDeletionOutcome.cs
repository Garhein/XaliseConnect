using System.ComponentModel;

namespace XaliseConnect.Application.IntegrationRequest.Enums;

/// <summary>
/// Résultat possible de la suppression d'une demande d'intégration.
/// </summary>
public enum EnumIntegrationRequestDeletionOutcome
{
    /// <summary>
    /// Demande d'intégration supprimée avec succès.
    /// </summary>
    [Description("La demande d'intégration a été supprimée avec succès.")]
    Deleted,

    /// <summary>
    /// Demande d'intégration introuvable.
    /// </summary>
    [Description("La demande d'intégration n'existe plus ou a déjà été supprimée.")]
    NotFound,

    /// <summary>
    /// Échec de la suppression dans le stockage.
    /// </summary>
    [Description("La demande d'intégration existe, mais une erreur de stockage a empêché sa suppression.")]
    PersistenceFailure,

    /// <summary>
    /// Service de stockage indisponible.
    /// </summary>
    [Description("Le service de suppression est temporairement indisponible, veuillez réessayer ultérieurement.")]
    ServiceUnavailable,

    /// <summary>
    /// Échec inattendu de la suppression.
    /// </summary>
    [Description("Une erreur inattendue a empêché la suppression de la demande, veuillez réessayer ultérieurement.")]
    UnexpectedFailure
}
