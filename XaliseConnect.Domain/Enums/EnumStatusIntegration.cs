using XaliseConnect.Domain.Attributes;

namespace XaliseConnect.Domain.Enums;

/// <summary>
/// Statuts d'intégration d'une demande.
/// </summary>
public enum EnumStatusIntegration : int
{
    /// <summary>
    /// Demande en attente d'intégration.
    /// </summary>
    EN_ATTENTE = 1,
    
    /// <summary>
    /// Demande en cours d'intégration.
    /// </summary>
    EN_COURS = 2,
    
    /// <summary>
    /// Demande intégrée avec succès.
    /// </summary>
    REUSSIE = 3,

    /// <summary>
    /// Échec d'intégration de la demande.
    /// </summary>
    [ErrorStatus]
    ECHEC_INTEGRATION = 4,

    /// <summary>
    /// Demande rejetée lors de l'intégration.
    /// </summary>
    REJETEE = 5
}
