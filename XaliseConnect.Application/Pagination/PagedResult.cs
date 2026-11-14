namespace XaliseConnect.Application.Pagination;

/// <summary>
/// Résultat générique contenant une page d'éléments et les métadonnées de pagination.
/// </summary>
/// <typeparam name="TItem">Type des éléments de la page.</typeparam>
public sealed class PagedResult<TItem>
{
    /// <summary>
    /// Éléments de la page courante.
    /// </summary>
    public IReadOnlyList<TItem> Items { get; }

    /// <summary>
    /// Numéro de la page courante, à partir de 1.
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Nombre maximal d'éléments par page.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Nombre total d'éléments correspondant aux critères, avant pagination.
    /// </summary>
    public int TotalItemCount { get; }

    /// <summary>
    /// Nombre total de pages, avec une page minimale lorsque le résultat est vide.
    /// </summary>
    public int TotalPageCount { get; }

    /// <summary>
    /// Colonne de tri appliquée.
    /// </summary>
    public string? SortColumn { get; }

    /// <summary>
    /// Sens de tri appliqué.
    /// </summary>
    public string? SortDirection { get; }

    /// <summary>
    /// Initialise un résultat paginé.
    /// </summary>
    /// <param name="items">Éléments de la page courante.</param>
    /// <param name="pageNumber">Numéro de la page courante.</param>
    /// <param name="pageSize">Nombre maximal d'éléments par page.</param>
    /// <param name="totalItemCount">Nombre total d'éléments correspondant aux critères.</param>
    /// <param name="sortColumn">Colonne de tri appliquée.</param>
    /// <param name="sortDirection">Sens de tri appliqué.</param>
    /// <exception cref="ArgumentNullException">Si <paramref name="items"/> est <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Si le numéro de page ou la taille de page est inférieur à 1, ou si le nombre total d'éléments est négatif.</exception>
    public PagedResult(
        IReadOnlyList<TItem> items,
        int pageNumber,
        int pageSize,
        int totalItemCount,
        string? sortColumn,
        string? sortDirection)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageNumber, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(totalItemCount);

        this.Items          = items;
        this.PageNumber     = pageNumber;
        this.PageSize       = pageSize;
        this.TotalItemCount = totalItemCount;
        this.TotalPageCount = Math.Max(1, (int)Math.Ceiling(totalItemCount / (double)pageSize));
        this.SortColumn     = sortColumn;
        this.SortDirection  = sortDirection;
    }
}
