using XaliseConnect.Domain.Entities.Audit;
using XaliseConnect.Domain.Entities.Configuration;
using XaliseConnect.Domain.Entities.Instance;
using XaliseConnect.Domain.Entities.Reference;

namespace XaliseConnect.Domain.Entities.Workflow
{
    /// <summary>
    /// Représente une demande d'intégration.
    /// </summary>
    public sealed class IntegrationRequest : BaseEntity
    {
        /// <summary>
        /// Détail enregistré lors de la création d'une demande d'intégration.
        /// </summary>
        private const string InitialReceptionHistoryDetails = "Demande d'intégration réceptionnée.";

        /// <summary>
        /// Historique des statuts de réception de la demande d'intégration.
        /// </summary>
        private readonly List<RequestHistoryReception> _historyReception = [];

        /// <summary>
        /// Historique des statuts de transport de la demande d'intégration.
        /// </summary>
        private readonly List<RequestHistoryTransport> _historyTransport = [];

        /// <summary>
        /// Historique des statuts d'intégration de la demande d'intégration.
        /// </summary>
        private readonly List<RequestHistoryIntegration> _historyIntegration = [];

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
        public int CurrentStatusReceptionId { get; private set; }

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
        public StatusTransport? CurrentStatusTransport { get; private set; }

        /// <summary>
        /// Identifiant du statut d'intégration actuel de la demande d'intégration.
        /// </summary>
        public int? CurrentStatusIntegrationId { get; private set; }

        /// <summary>
        /// Référence du statut d'intégration actuel de la demande d'intégration.
        /// </summary>
        public StatusIntegration? CurrentStatusIntegration { get; private set; }

        /// <summary>
        /// Identifiant de l'instance du flux associée à la demande d'intégration.
        /// </summary>
        public int? WorkflowInstanceId { get; private set; }

        /// <summary>
        /// Référence de l'instance du flux associée à la demande d'intégration.
        /// </summary>
        public WorkflowInstance? WorkflowInstance { get; private set; }

        /// <summary>
        /// Collection en lecture seule des historiques de réception associés à la demande d'intégration.
        /// </summary>
        public IReadOnlyCollection<RequestHistoryReception> HistoryReception => this._historyReception.AsReadOnly();

        /// <summary>
        /// Collection en lecture seule des historiques de transport associés à la demande d'intégration.
        /// </summary>
        public IReadOnlyCollection<RequestHistoryTransport> HistoryTransport => this._historyTransport.AsReadOnly();

        /// <summary>
        /// Collection en lecture seule des historiques d'intégration associés à la demande d'intégration.
        /// </summary>
        public IReadOnlyCollection<RequestHistoryIntegration> HistoryIntegration => this._historyIntegration.AsReadOnly();

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
        public IntegrationRequest(DateTime receivedAt,
                                  string rawMessage,
                                  string sourceReference,
                                  EventType eventType,
                                  Source source,
                                  StatusReception currentStatusReception)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(rawMessage, nameof(rawMessage));
            ArgumentException.ThrowIfNullOrWhiteSpace(sourceReference, nameof(sourceReference));
            ArgumentNullException.ThrowIfNull(currentStatusReception, nameof(currentStatusReception));
            ArgumentNullException.ThrowIfNull(eventType, nameof(eventType));
            ArgumentNullException.ThrowIfNull(source, nameof(source));

            this.ReceivedAt = receivedAt;
            this.RawMessage = rawMessage;
            this.SourceReference = sourceReference;
            this.EventTypeId = eventType.Id;
            this.EventType = eventType;
            this.SourceId = source.Id;
            this.Source = source;

            this.ChangeReceptionStatus(currentStatusReception, IntegrationRequest.InitialReceptionHistoryDetails, receivedAt);
        }

        /// <summary>
        /// Change le statut de réception de la demande d'intégration et ajoute un nouvel historique de réception.
        /// </summary>
        /// <param name="status">Statut de réception.</param>
        /// <param name="details">Détails du changement de statut.</param>
        /// <param name="occurredAt">Date et heure du changement de statut.</param>
        public void ChangeReceptionStatus(StatusReception status, string details, DateTime occurredAt)
        {
            ArgumentNullException.ThrowIfNull(status);
            ArgumentException.ThrowIfNullOrWhiteSpace(details);

            this.CurrentStatusReception = status;
            this.CurrentStatusReceptionId = status.Id;
            this._historyReception.Add(new RequestHistoryReception(occurredAt, details, this, status));
        }

        /// <summary>
        /// Change le statut de transport de la demande d'intégration et ajoute un nouvel historique de transport.
        /// </summary>
        /// <param name="status">Statut de transport.</param>
        /// <param name="details">Détails du changement de statut.</param>
        /// <param name="occurredAt">Date et heure du changement de statut.</param>
        public void ChangeTransportStatus(StatusTransport status, string details, DateTime occurredAt)
        {
            ArgumentNullException.ThrowIfNull(status);
            ArgumentException.ThrowIfNullOrWhiteSpace(details);

            this.CurrentStatusTransport = status;
            this.CurrentStatusTransportId = status.Id;
            this._historyTransport.Add(new RequestHistoryTransport(occurredAt, details, this, status));
        }

        /// <summary>
        /// Change le statut d'intégration de la demande d'intégration et ajoute un nouvel historique d'intégration.
        /// </summary>
        /// <param name="status">Statut d'intégration.</param>
        /// <param name="details">Détails du changement de statut.</param>
        /// <param name="occurredAt">Date et heure du changement de statut.</param>
        public void ChangeIntegrationStatus(StatusIntegration status, string details, DateTime occurredAt)
        {
            ArgumentNullException.ThrowIfNull(status);
            ArgumentException.ThrowIfNullOrWhiteSpace(details);

            this.CurrentStatusIntegration = status;
            this.CurrentStatusIntegrationId = status.Id;
            this._historyIntegration.Add(new RequestHistoryIntegration(occurredAt, details, this, status));
        }

        /// <summary>
        /// Assigne une instance de flux à la demande d'intégration.
        /// </summary>
        /// <param name="workflowInstance">Instance de flux à assigner.</param>
        public void AssignWorkflowInstance(WorkflowInstance workflowInstance)
        {
            ArgumentNullException.ThrowIfNull(workflowInstance);

            this.WorkflowInstance = workflowInstance;
            this.WorkflowInstanceId = workflowInstance.Id;
        }
    }
}
