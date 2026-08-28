using XaliseConnect.Domain.Entities.Reference;
using XaliseConnect.Domain.Entities.Workflow;

namespace XaliseConnect.Domain.Entities.Audit
{
    /// <summary>
    /// Historique de réception d'une demande d'intégration.
    /// </summary>
    public sealed class RequestHistoryReception : BaseEntity
    {
        /// <summary>
        /// Date et heure de l'historique de réception.
        /// </summary>
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Détails de l'historique de réception.
        /// </summary>
        public string Details { get; private set; } = string.Empty;

        /// <summary>
        /// Identifiant de la demande d'intégration associée à l'historique de réception.
        /// </summary>
        public int IntegrationRequestId { get; private set; }

        /// <summary>
        /// Demande d'intégration associée à l'historique de réception.
        /// </summary>
        public IntegrationRequest IntegrationRequest { get; private set; } = null!;

        /// <summary>
        /// Identifiant du statut de réception associé à l'historique de réception.
        /// </summary>
        public int StatusReceptionId { get; private set; }

        /// <summary>
        /// Statut de réception associé à l'historique de réception.
        /// </summary>
        public StatusReception StatusReception { get; private set; } = null!;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private RequestHistoryReception() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="RequestHistoryReception"/> avec les propriétés nécessaires.
        /// </summary>
        /// <param name="createdAt">Date et heure de l'historique de réception.</param>
        /// <param name="details">Détails de l'historique de réception.</param>
        /// <param name="integrationRequest">Demande d'intégration associée à l'historique de réception.</param>
        /// <param name="statusReception">Statut de réception associé à l'historique de réception.</param>
        public RequestHistoryReception(DateTime createdAt, string details, IntegrationRequest integrationRequest, StatusReception statusReception)
        {
            ArgumentNullException.ThrowIfNull(details, nameof(details));
            ArgumentNullException.ThrowIfNull(integrationRequest, nameof(integrationRequest));
            ArgumentNullException.ThrowIfNull(statusReception, nameof(statusReception));

            this.CreatedAt = createdAt;
            this.Details = details;
            this.IntegrationRequest = integrationRequest;
            this.IntegrationRequestId = integrationRequest.Id;
            this.StatusReception = statusReception;
            this.StatusReceptionId = statusReception.Id;
        }
    }
}
