namespace XaliseConnect.Application.Pagination;

/// <summary>
/// Critères génériques de tri et de pagination.
/// </summary>
public sealed class PaginationCriteria
{
    /// <summary>
    /// Numéro de la page demandée, à partir de 1.
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Nombre maximal d'éléments affichés sur une page.
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Clé de la colonne de tri, ou <see langword="null"/> si aucun tri n'est demandé.
    /// </summary>
    public string? SortColumn { get; init; }

    /// <summary>
    /// Sens du tri, ou <see langword="null"/> si aucun sens n'est demandé.
    /// </summary>
    public string? SortDirection { get; init; }
}
