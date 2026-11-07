using System.ComponentModel.DataAnnotations;

namespace XaliseConnect.Domain.Entities.Configuration;

/// <summary>
/// Représente une règle de corrélation permettant d'identifier un workflow à partir d'un message.
/// </summary>
public class CorrelationRule
{
    /// <summary>
    /// Identifiant unique de la règle de corrélation.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Libellé de la règle de corrélation.
    /// </summary>
    [MaxLength(40)]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Description de la règle, ou <see langword="null"/> si elle n'est pas renseignée.
    /// </summary>
    [MaxLength(254)]
    public string? Description { get; set; }

    /// <summary>
    /// Expression utilisée pour extraire la valeur de corrélation.
    /// </summary>
    [MaxLength(40)]
    public string Expression { get; set; } = string.Empty;
}
