using XaliseConnect.Domain.Attributes;

namespace XaliseConnect.Domain.Enums;

/// <summary>
/// Statuts de réception d'une demande d'intégration.
/// </summary>
public enum EnumStatusReception : int
{
    /// <summary>
    /// Demande d'intégration en attente de traitement.
    /// </summary>
    EN_ATTENTE = 1,
    
    /// <summary>
    /// Demande d'intégration reçue.
    /// </summary>
    RECUE = 2,
    
    /// <summary>
    /// Demande d'intégration validée.
    /// </summary>
    VALIDEE = 3,

    /// <summary>
    /// Échec de la réception de la demande d'intégration.
    /// </summary>
    [ErrorStatus]
    ECHEC_RECEPTION = 4,
    
    /// <summary>
    /// Échec de la validation de la demande d'intégration.
    /// </summary>
    [ErrorStatus]
    ECHEC_VALIDATION = 5
}
