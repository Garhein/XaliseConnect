using XaliseConnect.Domain.Entities.Reference;
using XaliseConnect.Domain.Entities.Workflow;

namespace XaliseConnect.Domain.Entities.Audit
{
    /// <summary>
    /// Historique d'intégration d'une demande d'intégration.
    /// </summary>
    public sealed class RequestHistoryIntegration : BaseEntity
    {
        /// <summary>
        /// Date et heure de l'historique d'intégration.
        /// </summary>
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Détails de l'historique d'intégration.
        /// </summary>
        public string Details { get; private set; } = string.Empty;

        /// <summary>
        /// Identifiant de la demande d'intégration associée à l'historique d'intégration.
        /// </summary>
        public int IntegrationRequestId { get; private set; }

        /// <summary>
        /// Demande d'intégration associée à l'historique d'intégration.
        /// </summary>
        public IntegrationRequest IntegrationRequest { get; private set; } = null!;

        /// <summary>
        /// Identifiant du statut d'intégration associé à l'historique d'intégration.
        /// </summary>
        public int StatusIntegrationId { get; private set; }

        /// <summary>
        /// Statut d'intégration associé à l'historique d'intégration.
        /// </summary>
        public StatusIntegration StatusIntegration { get; private set; } = null!;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private RequestHistoryIntegration() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="RequestHistoryIntegration"/> avec les propriétés nécessaires.
        /// </summary>
        /// <param name="createdAt">Date et heure de l'historique d'intégration.</param>
        /// <param name="details">Détails de l'historique d'intégration.</param>
        /// <param name="integrationRequest">Demande d'intégration associée à l'historique d'intégration.</param>
        /// <param name="statusIntegration">Statut d'intégration associé à l'historique d'intégration.</param>
        public RequestHistoryIntegration(DateTime createdAt, string details, IntegrationRequest integrationRequest, StatusIntegration statusIntegration)
        {
            ArgumentNullException.ThrowIfNull(details, nameof(details));
            ArgumentNullException.ThrowIfNull(integrationRequest, nameof(integrationRequest));
            ArgumentNullException.ThrowIfNull(statusIntegration, nameof(statusIntegration));

            this.CreatedAt = createdAt;
            this.Details = details;
            this.IntegrationRequest = integrationRequest;
            this.IntegrationRequestId = integrationRequest.Id;
            this.StatusIntegration = statusIntegration;
            this.StatusIntegrationId = statusIntegration.Id;
        }
    }
}
