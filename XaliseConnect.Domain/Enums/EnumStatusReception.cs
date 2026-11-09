using System.ComponentModel;
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
    [Description("En attente")]
    EN_ATTENTE = 1,
    
    /// <summary>
    /// Demande d'intégration reçue.
    /// </summary>
    [Description("Reçue")]
    RECUE = 2,
    
    /// <summary>
    /// Demande d'intégration validée.
    /// </summary>
    [Description("Validée")]
    VALIDEE = 3,

    /// <summary>
    /// Échec de la réception de la demande d'intégration.
    /// </summary>
    [ErrorStatus]
    [Description("Échec de la réception")]
    ECHEC_RECEPTION = 4,
    
    /// <summary>
    /// Échec de la validation de la demande d'intégration.
    /// </summary>
    [ErrorStatus]
    [Description("Échec de la validation")]
    ECHEC_VALIDATION = 5
}
