using Microsoft.AspNetCore.Mvc.Rendering;

namespace XaliseConnect.Portal.Models.IntegrationRequest;

/// <summary>
/// Modèle des données nécessaires à l'affichage des critères de recherche des demandes d'intégration.
/// </summary>
public sealed class IntegrationRequestSearchCriteriaViewModel
{
    /// <summary>
    /// Statut de réception.
    /// </summary>
    public SelectList StatusReceptionOptions { get; }

    /// <summary>
    /// Statut de transport.
    /// </summary>
    public SelectList StatusTransportOptions { get; }

    /// <summary>
    /// Statut d'intégration.
    /// </summary>
    public SelectList StatusIntegrationOptions { get; }

    /// <summary>
    /// Initialise les options des critères de recherche.
    /// </summary>
    /// <param name="statusReceptionOptions">Options du statut de réception.</param>
    /// <param name="statusTransportOptions">Options du statut de transport.</param>
    /// <param name="statusIntegrationOptions">Options du statut d'intégration.</param>
    /// <exception cref="ArgumentNullException">Si une liste d'options est <see langword="null"/>.</exception>
    public IntegrationRequestSearchCriteriaViewModel(SelectList statusReceptionOptions, SelectList statusTransportOptions, SelectList statusIntegrationOptions)
    {
        ArgumentNullException.ThrowIfNull(statusReceptionOptions);
        ArgumentNullException.ThrowIfNull(statusTransportOptions);
        ArgumentNullException.ThrowIfNull(statusIntegrationOptions);

        this.StatusReceptionOptions     = statusReceptionOptions;
        this.StatusTransportOptions     = statusTransportOptions;
        this.StatusIntegrationOptions   = statusIntegrationOptions;
    }
}
