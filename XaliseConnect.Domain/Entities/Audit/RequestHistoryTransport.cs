using XaliseConnect.Domain.Entities.Reference;
using XaliseConnect.Domain.Entities.Workflow;

namespace XaliseConnect.Domain.Entities.Audit
{
    /// <summary>
    /// Historique de transport d'une demande d'intégration.
    /// </summary>
    public sealed class RequestHistoryTransport : BaseEntity
    {
        /// <summary>
        /// Date et heure de l'historique de transport.
        /// </summary>
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Détails de l'historique de transport.
        /// </summary>
        public string Details { get; private set; } = string.Empty;

        /// <summary>
        /// Identifiant de la demande d'intégration associée à l'historique de transport.
        /// </summary>
        public int IntegrationRequestId { get; private set; }

        /// <summary>
        /// Demande d'intégration associée à l'historique de transport.
        /// </summary>
        public IntegrationRequest IntegrationRequest { get; private set; } = null!;

        /// <summary>
        /// Identifiant du statut de transport associé à l'historique de transport.
        /// </summary>
        public int StatusTransportId { get; private set; }

        /// <summary>
        /// Statut de transport associé à l'historique de transport.
        /// </summary>
        public StatusTransport StatusTransport { get; private set; } = null!;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private RequestHistoryTransport() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="RequestHistoryTransport"/> avec les propriétés nécessaires.
        /// </summary>
        /// <param name="createdAt">Date et heure de l'historique de transport.</param>
        /// <param name="details">Détails de l'historique de transport.</param>
        /// <param name="integrationRequest">Demande d'intégration associée à l'historique de transport.</param>
        /// <param name="statusTransport">Statut de transport associé à l'historique de transport.</param>
        public RequestHistoryTransport(DateTime createdAt, string details, IntegrationRequest integrationRequest, StatusTransport statusTransport)
        {
            ArgumentNullException.ThrowIfNull(details, nameof(details));
            ArgumentNullException.ThrowIfNull(integrationRequest, nameof(integrationRequest));
            ArgumentNullException.ThrowIfNull(statusTransport, nameof(statusTransport));

            this.CreatedAt = createdAt;
            this.Details = details;
            this.IntegrationRequest = integrationRequest;
            this.IntegrationRequestId = integrationRequest.Id;
            this.StatusTransport = statusTransport;
            this.StatusTransportId = statusTransport.Id;
        }
    }
}
