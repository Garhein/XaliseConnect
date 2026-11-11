using XaliseConnect.Application.IntegrationRequest.Models;
using XaliseConnect.Domain.Helpers;
using XaliseConnect.Portal.Models.IntegrationRequest;

namespace XaliseConnect.Portal.Helpers;

/// <summary>
/// Définition des méthodes de création des modèles de données utilisés pour l'affichage
/// d'un statut d'une demande d'intégration.
/// </summary>
public static class RequestIntegrationStatusPresentationHelper
{
    /// <summary>
    /// Création du modèle de données pour le statut de réception.
    /// </summary>
    /// <param name="req">Informations de la demande d'intégration.</param>
    /// <returns>Les données à afficher.</returns>
    public static StatusCellViewModel CreateReceptionCell(IntegrationRequestListItem req)
    {
        bool isError    = false;
        string label    = string.Empty;
        string cssClass = string.Empty;

        if (req.StatusReception.HasValue)
        {
            isError  = req.StatusReception.Value.IsErrorStatus();
            label    = string.IsNullOrWhiteSpace(req.StatusReceptionLabel) ? string.Empty : req.StatusReceptionLabel;
            cssClass = req.StatusReception.Value.GetCssClass();
        }

        return new StatusCellViewModel()
        {
            RequestNumber   = req.RequestNumber,
            Label           = label,
            CssClass        = cssClass,
            IsError         = isError,
            FailureStage    = "reception",
            EmptyAriaLabel  = "Aucun statut de réception"
        };
    }

    /// <summary>
    /// Création du modèle de données pour le statut de transport.
    /// </summary>
    /// <param name="req">Informations de la demande d'intégration.</param>
    /// <returns>Les données à afficher.</returns>
    public static StatusCellViewModel CreateTransportCell(IntegrationRequestListItem req)
    {
        bool isError    = false;
        string label    = string.Empty;
        string cssClass = string.Empty;

        if (req.StatusTransport.HasValue)
        {
            isError  = req.StatusTransport.Value.IsErrorStatus();
            label    = string.IsNullOrWhiteSpace(req.StatusTransportLabel) ? string.Empty : req.StatusTransportLabel;
            cssClass = req.StatusTransport.Value.GetCssClass();
        }

        return new StatusCellViewModel()
        {
            RequestNumber   = req.RequestNumber,
            Label           = label,
            CssClass        = cssClass,
            IsError         = isError,
            FailureStage    = "transport",
            EmptyAriaLabel  = "Aucun statut de transport"
        };
    }

    /// <summary>
    /// Création du modèle de données pour le statut d'intégration.
    /// </summary>
    /// <param name="req">Informations de la demande d'intégration.</param>
    /// <returns>Les données à afficher.</returns>
    public static StatusCellViewModel CreateIntegrationCell(IntegrationRequestListItem req)
    {
        bool isError    = false;
        string label    = string.Empty;
        string cssClass = string.Empty;

        if (req.StatusIntegration.HasValue)
        {
            isError  = req.StatusIntegration.Value.IsErrorStatus();
            label    = string.IsNullOrWhiteSpace(req.StatusIntegrationLabel) ? string.Empty : req.StatusIntegrationLabel;
            cssClass = req.StatusIntegration.Value.GetCssClass();
        }

        return new StatusCellViewModel()
        {
            RequestNumber   = req.RequestNumber,
            Label           = label,
            CssClass        = cssClass,
            IsError         = isError,
            FailureStage    = "integration",
            EmptyAriaLabel  = "Aucun statut d'intégration"
        };
    }
}
