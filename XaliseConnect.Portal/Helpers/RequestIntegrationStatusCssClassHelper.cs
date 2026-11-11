using XaliseConnect.Domain.Enums;
using XaliseConnect.Domain.Helpers;
using XaliseConnect.Portal.Styles;

namespace XaliseConnect.Portal.Helpers;

/// <summary>
/// Définition des méthodes utilitaires fournissant les classes CSS Bootstrap correspondants
/// aux statuts d'une demande d'intégration.
/// </summary>
public static class RequestIntegrationStatusCssClassHelper
{
    /// <summary>
    /// Récupère la classe CSS Bootstrap correspondant à un statut de réception.
    /// </summary>
    /// <param name="status">Statut de réception à examiner.</param>
    /// <returns>Classe CSS Bootstrap correspondant au statut.</returns>
    public static string GetCssClass(this EnumStatusReception status)
    {
        if (status.IsErrorStatus())
        {
            return BootstrapBackgroundCssClass.CSTS_TEXT_BG_DANGER;
        }

        return status switch
        {
            EnumStatusReception.EN_ATTENTE          => BootstrapBackgroundCssClass.CSTS_TEXT_BG_INFO,
            EnumStatusReception.RECUE               => BootstrapBackgroundCssClass.CSTS_TEXT_BG_INFO,
            EnumStatusReception.VALIDEE             => BootstrapBackgroundCssClass.CSTS_TEXT_BG_SUCCESS,
            EnumStatusReception.ECHEC_RECEPTION     => BootstrapBackgroundCssClass.CSTS_TEXT_BG_DANGER,
            EnumStatusReception.ECHEC_VALIDATION    => BootstrapBackgroundCssClass.CSTS_TEXT_BG_DANGER,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Statut de réception non pris en charge.")
        };
    }

    /// <summary>
    /// Récupère la classe CSS Bootstrap correspondant à un statut de transport.
    /// </summary>
    /// <param name="status">Statut de transport à examiner.</param>
    /// <returns>Classe CSS Bootstrap correspondant au statut.</returns>
    public static string GetCssClass(this EnumStatusTransport status)
    {
        if (status.IsErrorStatus())
        {
            return BootstrapBackgroundCssClass.CSTS_TEXT_BG_DANGER;
        }

        return status switch
        {
            EnumStatusTransport.EN_ATTENTE                      => BootstrapBackgroundCssClass.CSTS_TEXT_BG_INFO,
            EnumStatusTransport.EN_COURS                        => BootstrapBackgroundCssClass.CSTS_TEXT_BG_INFO,
            EnumStatusTransport.ENVOYEE                         => BootstrapBackgroundCssClass.CSTS_TEXT_BG_SUCCESS,
            EnumStatusTransport.ECHEC_TRANSPORT                 => BootstrapBackgroundCssClass.CSTS_TEXT_BG_DANGER,
            EnumStatusTransport.DELAI_ACCUSE_RECEPTION_DEPASSE  => BootstrapBackgroundCssClass.CSTS_TEXT_BG_DANGER,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Statut de transport non pris en charge.")
        };
    }

    /// <summary>
    /// Récupère la classe CSS Bootstrap correspondant à un statut d'intégration.
    /// </summary>
    /// <param name="status">Statut d'intégration à examiner.</param>
    /// <returns>Classe CSS Bootstrap correspondant au statut.</returns>
    public static string GetCssClass(this EnumStatusIntegration status)
    {
        if (status.IsErrorStatus())
        {
            return BootstrapBackgroundCssClass.CSTS_TEXT_BG_DANGER;
        }

        return status switch
        {
            EnumStatusIntegration.EN_ATTENTE        => BootstrapBackgroundCssClass.CSTS_TEXT_BG_INFO,
            EnumStatusIntegration.EN_COURS          => BootstrapBackgroundCssClass.CSTS_TEXT_BG_INFO,
            EnumStatusIntegration.REUSSIE           => BootstrapBackgroundCssClass.CSTS_TEXT_BG_SUCCESS,
            EnumStatusIntegration.ECHEC_INTEGRATION => BootstrapBackgroundCssClass.CSTS_TEXT_BG_DANGER,
            EnumStatusIntegration.REJETEE           => BootstrapBackgroundCssClass.CSTS_TEXT_BG_WARNING,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Statut d'intégration non pris en charge.")
        };
    }
}
