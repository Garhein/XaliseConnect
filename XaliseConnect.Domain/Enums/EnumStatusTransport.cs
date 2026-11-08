using XaliseConnect.Domain.Attributes;

namespace XaliseConnect.Domain.Enums;

/// <summary>
/// Statuts de transport d'une demande d'intégration.
/// </summary>
public enum EnumStatusTransport : int
{
    /// <summary>
    /// Demande d'intégration en attente de transport.
    /// </summary>
    EN_ATTENTE = 1,
    
    /// <summary>
    /// Demande d'intégration en cours de transport.
    /// </summary>
    EN_COURS = 2,
    
    /// <summary>
    /// Demande d'intégration envoyée.
    /// </summary>
    ENVOYEE = 3,

    /// <summary>
    /// Échec du transport de la demande d'intégration.
    /// </summary>
    [ErrorStatus]
    ECHEC_TRANSPORT = 4,

    /// <summary>
    /// Délai d'accusé de réception dépassé.
    /// </summary>
    [ErrorStatus]
    DELAI_ACCUSE_RECEPTION_DEPASSE = 5
}
