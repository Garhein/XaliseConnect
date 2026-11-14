using Microsoft.AspNetCore.Mvc;
using XaliseConnect.Application.IntegrationRequest;
using XaliseConnect.Application.IntegrationRequest.Models;
using XaliseConnect.Application.Pagination;
using XaliseConnect.Portal.Models.IntegrationRequest;
using XaliseConnect.Portal.Helpers;
using XaliseConnect.Portal.Routes;
using XaliseConnect.Domain.Enums;
using XaliseConnect.Portal.Models.Pagination;

namespace XaliseConnect.Portal.Controllers;

/// <summary>
/// Traite les demandes liées aux demandes d'intégration.
/// </summary>
[Route("demande-integration")]
public sealed class IntegrationRequestController : Controller
{
    private const int CSTN_DEFAULT_PAGE_SIZE        = 10;
    private const string CSTS_TABLE_SELECTOR        = "#xal-id-table-integration-request";
    private const string CSTS_PAGE_SIZE_SELECT_ID   = "xal-id-page-size";
    private const string CSTS_PAGE_SELECT_ID        = "xal-id-pagination-jump";
    private const string CSTS_PAGE_ITEM_LABEL       = "demande(s)";
    private const string CSTS_PAGINATION_LABEL      = "Pagination des demandes d'intégration";

    private readonly IIntegrationRequestQueryService _integrationRequestQueryService;

    /// <summary>
    /// Initialise le contrôleur des demandes d'intégration.
    /// </summary>
    /// <param name="integrationRequestQueryService">Service d'application qui traite les demandes liées aux demandes d'intégration.</param>
    /// <exception cref="ArgumentNullException">Si <paramref name="integrationRequestQueryService"/> est <see langword="null"/>.</exception>
    public IntegrationRequestController(IIntegrationRequestQueryService integrationRequestQueryService)
    {
        ArgumentNullException.ThrowIfNull(integrationRequestQueryService);

        this._integrationRequestQueryService = integrationRequestQueryService;
    }

    /// <summary>
    /// Point d'entrée pour afficher la liste des demandes d'intégration.
    /// </summary>
    /// <param name="cancellationToken">Jeton d'annulation de la demande.</param>
    /// <returns>La vue d'affichage des demandes d'intégration.</returns>
    /// <exception cref="OperationCanceledException">Si la demande a été annulée.</exception>
    [HttpGet]
    [ActionName("Index")]
    public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken)
    {
        IntegrationRequestSearchCriteria searchCriteriaForInitialList = new IntegrationRequestSearchCriteria
        {
            Pagination = new PaginationCriteria
            {
                PageNumber      = 1,
                PageSize        = IntegrationRequestController.CSTN_DEFAULT_PAGE_SIZE,
                SortColumn      = IntegrationRequestSortColumn.CSTS_RECEIVED_AT,
                SortDirection   = PaginationSortDirection.CSTS_DESCENDING
            }
        };

        PagedResult<IntegrationRequestListItem> integrationRequests = await this._integrationRequestQueryService.GetIntegrationRequestListAsync(
            searchCriteriaForInitialList,
            cancellationToken);

        IntegrationRequestSearchCriteriaViewModel searchCriteria = new IntegrationRequestSearchCriteriaViewModel(
            SelectListHelper.CreateSelectList<EnumStatusReception>(true, "Tous les statuts", true, "Sans statut"),
            SelectListHelper.CreateSelectList<EnumStatusTransport>(true, "Tous les statuts", true, "Sans statut"),
            SelectListHelper.CreateSelectList<EnumStatusIntegration>(true, "Tous les statuts", true, "Sans statut")
        );

        IntegrationRequestListViewModel viewModel = new IntegrationRequestListViewModel(
            this._CreateResultsViewModel(integrationRequests),
            searchCriteria
        );

        return this.View("Index", viewModel);
    }

    /// <summary>
    /// Recherche les demandes d'intégration correspondant aux critères sélectionnés.
    /// </summary>
    /// <param name="requestNumber">Numéro de demande.</param>
    /// <param name="receivedFromUtc">Date et heure UTC minimales de réception.</param>
    /// <param name="receivedToUtc">Date et heure UTC maximales de réception.</param>
    /// <param name="sourceId">Identifiant de la source.</param>
    /// <param name="correlationValue">Valeur de corrélation.</param>
    /// <param name="statusReception">Sélection de statut de réception : tous, sans statut ou un statut précis.</param>
    /// <param name="statusTransport">Sélection de statut de transport : tous, sans statut ou un statut précis.</param>
    /// <param name="statusIntegration">Sélection de statut d'intégration : tous, sans statut ou un statut précis.</param>
    /// <param name="page">Numéro de page à afficher.</param>
    /// <param name="pageSize">Nombre maximal de demandes à afficher par page.</param>
    /// <param name="sort">Colonne de tri des demandes.</param>
    /// <param name="direction">Sens de tri des demandes.</param>
    /// <param name="cancellationToken">Jeton d'annulation de la demande.</param>
    /// <returns>La vue partielle contenant les résultats correspondant aux critères.</returns>
    /// <exception cref="OperationCanceledException">Si la recherche a été annulée.</exception>
    
    [HttpGet("recherche", Name = IntegrationRequestRoute.CSTS_SEARCH)]
    public async Task<IActionResult> SearchAsync(
        [FromQuery(Name = "requestNumber")] string? requestNumber,
        [FromQuery(Name = "receivedFromUtc")] DateTimeOffset? receivedFromUtc,
        [FromQuery(Name = "receivedToUtc")] DateTimeOffset? receivedToUtc,
        [FromQuery(Name = "sourceId")] int? sourceId,
        [FromQuery(Name = "correlationValue")] string? correlationValue,
        [FromQuery(Name = "statusReception")] string? statusReception,
        [FromQuery(Name = "statusTransport")] string? statusTransport,
        [FromQuery(Name = "statusIntegration")] string? statusIntegration,
        CancellationToken cancellationToken,
        [FromQuery(Name = "page")] int page = 1,
        [FromQuery(Name = "pageSize")] int pageSize = IntegrationRequestController.CSTN_DEFAULT_PAGE_SIZE,
        [FromQuery(Name = "sort")] string? sort = IntegrationRequestSortColumn.CSTS_RECEIVED_AT,
        [FromQuery(Name = "direction")] string? direction = PaginationSortDirection.CSTS_DESCENDING)
    {
        bool isReceptionStatusValid = SelectListHelper.TryParseSelectListValue(
            statusReception,
            out EnumStatusReception? receptionStatus,
            out bool searchWithoutReceptionStatus
        );

        bool isTransportStatusValid = SelectListHelper.TryParseSelectListValue(
            statusTransport,
            out EnumStatusTransport? transportStatus,
            out bool searchWithoutTransportStatus
        );

        bool isIntegrationStatusValid = SelectListHelper.TryParseSelectListValue(
            statusIntegration,
            out EnumStatusIntegration? integrationStatus,
            out bool searchWithoutIntegrationStatus
        );

        if (!isReceptionStatusValid || !isTransportStatusValid || !isIntegrationStatusValid)
        {
            return this.BadRequest();
        }

        IntegrationRequestSearchCriteria searchCriteria = new IntegrationRequestSearchCriteria
        {
            RequestNumber                   = requestNumber,
            ReceivedFromUtc                 = receivedFromUtc,
            ReceivedToUtc                   = receivedToUtc,
            SourceId                        = sourceId,
            CorrelationValue                = correlationValue,
            StatusReception                 = receptionStatus,
            SearchWithoutStatusReception    = searchWithoutReceptionStatus,
            StatusTransport                 = transportStatus,
            SearchWithoutStatusTransport    = searchWithoutTransportStatus,
            StatusIntegration               = integrationStatus,
            SearchWithoutStatusIntegration  = searchWithoutIntegrationStatus,
            Pagination                      = this._CreatePaginationCriteria(page, pageSize, sort, direction)
        };

        PagedResult<IntegrationRequestListItem> integrationRequests = await this._integrationRequestQueryService.GetIntegrationRequestListAsync(
            searchCriteria,
            cancellationToken
        );

        return this.PartialView("_IntegrationRequestResults", this._CreateResultsViewModel(integrationRequests));
    }

    /// <summary>
    /// Normalise les paramètres de tri et de pagination fournis par la requête.
    /// </summary>
    /// <param name="pageNumber">Numéro de page demandé.</param>
    /// <param name="pageSize">Nombre maximal d'éléments par page demandé.</param>
    /// <param name="sortColumn">Clé de la colonne de tri demandée.</param>
    /// <param name="sortDirection">Sens de tri demandé.</param>
    /// <returns>Critères de pagination normalisés.</returns>
    private PaginationCriteria _CreatePaginationCriteria(int pageNumber, int pageSize, string? sortColumn, string? sortDirection)
    {
        string normalizedSortColumn = sortColumn switch
        {
            IntegrationRequestSortColumn.CSTS_REQUEST_NUMBER => IntegrationRequestSortColumn.CSTS_REQUEST_NUMBER,
            IntegrationRequestSortColumn.CSTS_SOURCE => IntegrationRequestSortColumn.CSTS_SOURCE,
            _ => IntegrationRequestSortColumn.CSTS_RECEIVED_AT
        };

        string normalizedSortDirection = sortDirection?.ToLowerInvariant() switch
        {
            PaginationSortDirection.CSTS_ASCENDING => PaginationSortDirection.CSTS_ASCENDING,
            _ => PaginationSortDirection.CSTS_DESCENDING
        };

        int normalizedPageSize = pageSize is 10 or 25 or 50
            ? pageSize
            : IntegrationRequestController.CSTN_DEFAULT_PAGE_SIZE;

        return new PaginationCriteria
        {
            PageNumber      = Math.Max(1, pageNumber),
            PageSize        = normalizedPageSize,
            SortColumn      = normalizedSortColumn,
            SortDirection   = normalizedSortDirection
        };
    }

    /// <summary>
    /// Crée le modèle d'affichage combinant les résultats et les commandes de pagination partagées.
    /// </summary>
    /// <param name="page">Résultat paginé fourni par la couche Application.</param>
    /// <returns>Modèle utilisé par la vue partielle des résultats.</returns>
    /// <exception cref="ArgumentNullException">Si <paramref name="page"/> est <see langword="null"/>.</exception>
    private IntegrationRequestResultsViewModel _CreateResultsViewModel(PagedResult<IntegrationRequestListItem> page)
    {
        ArgumentNullException.ThrowIfNull(page);

        PaginationViewModel paginationViewModel = new PaginationViewModel
        {
            PageNumber          = page.PageNumber,
            PageSize            = page.PageSize,
            TotalItemCount      = page.TotalItemCount,
            TotalPageCount      = page.TotalPageCount,
            SortColumn          = page.SortColumn,
            SortDirection       = page.SortDirection,
            TableSelector       = IntegrationRequestController.CSTS_TABLE_SELECTOR,
            PageSizeSelectId    = IntegrationRequestController.CSTS_PAGE_SIZE_SELECT_ID,
            PageSelectId        = IntegrationRequestController.CSTS_PAGE_SELECT_ID,
            ItemLabel           = IntegrationRequestController.CSTS_PAGE_ITEM_LABEL,
            NavigationLabel     = IntegrationRequestController.CSTS_PAGINATION_LABEL,
            PageSizeOptions     = new int[] { 10, 25, 50 }
        };

        return new IntegrationRequestResultsViewModel(page, paginationViewModel);
    }
}
