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
    /// <param name="pageNumber">Numéro de la page courante borné entre 1 et le nombre total de pages.</param>
    /// <param name="pageSize">Nombre d'éléments par page, normalisé selon la politique commune de pagination.</param>
    /// <param name="totalItemCount">Nombre total d'éléments correspondant aux critères.</param>
    /// <param name="sortColumn">Colonne de tri appliquée.</param>
    /// <param name="sortDirection">Sens de tri appliqué.</param>
    /// <exception cref="ArgumentNullException">Si <paramref name="items"/> est <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Si le nombre total d'éléments est négatif.</exception>
    public PagedResult(
        IReadOnlyList<TItem> items,
        int pageNumber,
        int pageSize,
        int totalItemCount,
        string? sortColumn,
        string? sortDirection)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentOutOfRangeException.ThrowIfNegative(totalItemCount);

        int normalizedPageSize  = PaginationPolicy.NormalizePageSize(pageSize);
        int totalPageCount      = Math.Max(1, (int)Math.Ceiling(totalItemCount / (double)normalizedPageSize));

        this.Items          = items;
        this.PageNumber     = Math.Clamp(pageNumber, 1, totalPageCount);
        this.PageSize       = normalizedPageSize;
        this.TotalItemCount = totalItemCount;
        this.TotalPageCount = totalPageCount;
        this.SortColumn     = sortColumn;
        this.SortDirection  = sortDirection;
    }
}
