using Microsoft.AspNetCore.Mvc;
using XaliseConnect.Application.IntegrationRequest;
using XaliseConnect.Application.IntegrationRequest.Models;
using XaliseConnect.Portal.Models.IntegrationRequest;
using XaliseConnect.Portal.Helpers;
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
    /// Affiche la liste des demandes d'intégration.
    /// </summary>
    /// <param name="cancellationToken">Jeton d'annulation de la demande.</param>
    /// <returns>La vue contenant le modèle d'affichage des demandes d'intégration.</returns>
    /// <exception cref="OperationCanceledException">Si la consultation a été annulée.</exception>
    [HttpGet]
    [ActionName("Index")]
    public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<IntegrationRequestListItem> integrationRequests = await this._integrationRequestQueryService.GetIntegrationRequestListAsync(cancellationToken);

        IntegrationRequestSearchCriteriaViewModel searchCriteria = new IntegrationRequestSearchCriteriaViewModel(
            SelectListHelper.CreateSelectList<EnumStatusReception>(includeUnspecifiedOption: true, unspecifiedOptionText: "Tous les statuts"),
            SelectListHelper.CreateSelectList<EnumStatusTransport>(includeUnspecifiedOption: true, unspecifiedOptionText: "Tous les statuts"),
            SelectListHelper.CreateSelectList<EnumStatusIntegration>(includeUnspecifiedOption: true, unspecifiedOptionText: "Tous les statuts")
        );

        IntegrationRequestListViewModel viewModel = new IntegrationRequestListViewModel(integrationRequests, searchCriteria);

        return this.View("Index", viewModel);
    }
}
