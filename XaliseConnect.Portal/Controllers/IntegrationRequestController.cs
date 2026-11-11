using Microsoft.AspNetCore.Mvc;
using XaliseConnect.Application.IntegrationRequest;
using XaliseConnect.Application.IntegrationRequest.Models;
using XaliseConnect.Portal.Models.IntegrationRequest;
using XaliseConnect.Portal.Helpers;
using XaliseConnect.Portal.Routes;
using XaliseConnect.Domain.Enums;

namespace XaliseConnect.Portal.Controllers;

/// <summary>
/// Traite les demandes liées aux demandes d'intégration.
/// </summary>
[Route("demande-integration")]
public sealed class IntegrationRequestController : Controller
{
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
        IReadOnlyList<IntegrationRequestListItem> integrationRequests = await this._integrationRequestQueryService.GetIntegrationRequestListAsync(
            new IntegrationRequestSearchCriteria(), 
            cancellationToken
        );

        IntegrationRequestSearchCriteriaViewModel searchCriteria = new IntegrationRequestSearchCriteriaViewModel(
            SelectListHelper.CreateSelectList<EnumStatusReception>(true, "Tous les statuts", true, "Sans statut"),
            SelectListHelper.CreateSelectList<EnumStatusTransport>(true, "Tous les statuts", true, "Sans statut"),
            SelectListHelper.CreateSelectList<EnumStatusIntegration>(true, "Tous les statuts", true, "Sans statut")
        );

        IntegrationRequestListViewModel viewModel = new IntegrationRequestListViewModel(integrationRequests, searchCriteria);

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
        CancellationToken cancellationToken)
    {
        // TODO: obtenir les critères via une instance de IntegrationRequestSearchCriteriaViewModel et mapper vers IntegrationRequestSearchCriteria
        // TODO: revoir tout le JavaScripts (XalHttp, événement sur la page, ...)

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
            SearchWithoutStatusIntegration  = searchWithoutIntegrationStatus
        };

        IReadOnlyList<IntegrationRequestListItem> integrationRequests = await this._integrationRequestQueryService.GetIntegrationRequestListAsync(
            searchCriteria,
            cancellationToken
        );

        return this.PartialView("_IntegrationRequestResults", integrationRequests);
    }
}
