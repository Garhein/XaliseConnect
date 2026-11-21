namespace XaliseConnect.Application.Pagination;

/// <summary>
/// Définition des limites communes de pagination appliquées par la couche d'application.
/// </summary>
public static class PaginationPolicy
{
    private static readonly IReadOnlyList<int> _pageSizeOptions = Array.AsReadOnly(new int[] { 25, 50, 100 });

    /// <summary>
    /// Taille de page appliquée lorsqu'aucune taille autorisée n'est fournie.
    /// </summary>
    public const int CSTN_DEFAULT_PAGE_SIZE = 25;

    /// <summary>
    /// Tailles de page autorisées par les services et proposées dans les interfaces de l'application.
    /// </summary>
    public static IReadOnlyList<int> PageSizeOptions => PaginationPolicy._pageSizeOptions;

    /// <summary>
    /// Remplace une taille de page non autorisée par la taille par défaut.
    /// </summary>
    /// <param name="requestedPageSize">Taille de page demandée.</param>
    /// <returns>La taille demandée si elle est autorisée, sinon la taille par défaut.</returns>
    public static int NormalizePageSize(int requestedPageSize)
    {
        return PaginationPolicy._pageSizeOptions.Contains(requestedPageSize)
            ? requestedPageSize
            : PaginationPolicy.CSTN_DEFAULT_PAGE_SIZE;
    }
}
