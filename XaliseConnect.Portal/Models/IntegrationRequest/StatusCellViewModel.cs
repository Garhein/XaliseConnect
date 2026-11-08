namespace XaliseConnect.Portal.Models.IntegrationRequest;

/// <summary>
/// Modèle de données pour l'affichage d'une cellule de statut.
/// </summary>
public sealed class StatusCellViewModel
{
    /// <summary>
    /// Numéro de la demande d'intégration.
    /// </summary>
    public required string RequestNumber { get; init; }

    /// <summary>
    /// Libellé du statut à afficher.
    /// </summary>
    public required string Label { get; init; }

    /// <summary>
    /// Classe CSS à appliquer.
    /// </summary>
    public required string CssClass { get; init; }

    /// <summary>
    /// <see langword="true"/> s'il s'agit d'une erreur, sino <see langword="false"/>.
    /// </summary>
    public bool IsError { get; init; }

    /// <summary>
    /// Libellé de l'étape en erreur.
    /// </summary>
    public required string FailureStage { get; init; }

    /// <summary>
    /// Libellé à appliquer à la cellule si aucun statut n'est définit.
    /// </summary>
    public required string EmptyAriaLabel { get; init; }
}
