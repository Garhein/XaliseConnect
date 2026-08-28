using XaliseConnect.Domain.Entities.Configuration;
using XaliseConnect.Domain.Entities.Reference;

namespace XaliseConnect.Domain.Entities.Workflow
{
    /// <summary>
    /// Représente une demande d'intégration.
    /// </summary>
    public sealed class IntegrationRequest : BaseEntity
    {
        /// <summary>
        /// Date et heure de réception de la demande d'intégration.
        /// </summary>
        public DateTime ReceivedAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Message brut de la demande d'intégration.
        /// </summary>
        public string RawMessage { get; private set; } = string.Empty;

        /// <summary>
        /// Référence de la source de la demande d'intégration.
        /// </summary>
        public string SourceReference { get; private set; } = string.Empty;

        /// <summary>
        /// Identifiant du type d'événement associé à la demande d'intégration.
        /// </summary>
        public int EventTypeId { get; private set; }

        /// <summary>
        /// Référence du type d'événement associé à la demande d'intégration.
        /// </summary>
        public EventType EventType { get; private set; } = null!;

        /// <summary>
        /// Identifiant de la source de la demande d'intégration.
        /// </summary>
        public int SourceId { get; private set; }

        /// <summary>
        /// Référence de la source de la demande d'intégration.
        /// </summary>
        public Source Source { get; private set; } = null!;

        /// <summary>
        /// Identifiant du statut de réception actuel de la demande d'intégration.
        /// </summary>
        public int? CurrentStatusReceptionId { get; private set; }

        /// <summary>
        /// Référence du statut de réception actuel de la demande d'intégration.
        /// </summary>
        public StatusReception CurrentStatusReception { get; private set; } = null!;

        /// <summary>
        /// Identifiant du statut de transport actuel de la demande d'intégration.
        /// </summary>
        public int? CurrentStatusTransportId { get; private set; }

        /// <summary>
        /// Référence du statut de transport actuel de la demande d'intégration.
        /// </summary>
        public StatusTransport CurrentStatusTransport { get; private set; } = null!;

        /// <summary>
        /// Identifiant du statut d'intégration actuel de la demande d'intégration.
        /// </summary>
        public int? CurrentStatusIntegrationId { get; private set; }

        /// <summary>
        /// Référence du statut d'intégration actuel de la demande d'intégration.
        /// </summary>
        public StatusIntegration CurrentStatusIntegration { get; private set; } = null!;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private IntegrationRequest() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="IntegrationRequest"/> avec les propriétés nécessaires.
        /// </summary>
        /// <param name="receivedAt">Date et heure de réception de la demande d'intégration.</param>
        /// <param name="rawMessage">Message brut de la demande d'intégration.</param>
        /// <param name="sourceReference">Référence de la source de la demande d'intégration.</param>
        /// <param name="eventType">Type d'événement associé à la demande d'intégration.</param>
        /// <param name="source">Source de la demande d'intégration.</param>
        /// <param name="currentStatusReception">Statut de réception actuel de la demande d'intégration.</param>
        /// <param name="currentStatusTransport">Statut de transport actuel de la demande d'intégration.</param>
        /// <param name="currentStatusIntegration">Statut d'intégration actuel de la demande d'intégration.</param>
        public IntegrationRequest(DateTime receivedAt,
                                  string rawMessage,
                                  string sourceReference,
                                  EventType eventType, 
                                  Source source,
                                  StatusReception currentStatusReception, 
                                  StatusTransport currentStatusTransport, 
                                  StatusIntegration currentStatusIntegration)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(rawMessage, nameof(rawMessage));
            ArgumentException.ThrowIfNullOrWhiteSpace(sourceReference, nameof(sourceReference));
            ArgumentNullException.ThrowIfNull(eventType, nameof(eventType));
            ArgumentNullException.ThrowIfNull(source, nameof(source));

            this.ReceivedAt = receivedAt;
            this.RawMessage = rawMessage;
            this.SourceReference = sourceReference;
            this.EventTypeId = eventType.Id;
            this.EventType = eventType;
            this.SourceId = source.Id;
            this.Source = source;
            this.CurrentStatusReceptionId = currentStatusReception.Id;
            this.CurrentStatusReception = currentStatusReception;
            this.CurrentStatusTransportId = currentStatusTransport.Id;
            this.CurrentStatusTransport = currentStatusTransport;
            this.CurrentStatusIntegrationId = currentStatusIntegration.Id;
            this.CurrentStatusIntegration = currentStatusIntegration;
        }
    }
}
