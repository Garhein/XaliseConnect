using System.ComponentModel;
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
    [Description("En attente")]
    EN_ATTENTE = 1,
    
    /// <summary>
    /// Demande en cours d'intégration.
    /// </summary>
    [Description("En cours")]
    EN_COURS = 2,
    
    /// <summary>
    /// Demande intégrée avec succès.
    /// </summary>
    [Description("Réussie")]
    REUSSIE = 3,

    /// <summary>
    /// Échec d'intégration de la demande.
    /// </summary>
    [ErrorStatus]
    [Description("Échec")]
    ECHEC_INTEGRATION = 4,

    /// <summary>
    /// Demande rejetée lors de l'intégration.
    /// </summary>
    [Description("Rejetée")]
    REJETEE = 5
}
