namespace XaliseConnect.Domain.Entities
{
    /// <summary>
    /// Classe de base pour toutes les entités du domaine, fournissant un identifiant unique.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Identifiant unique de l'entité.
        /// </summary>
        public int Id { get; protected set; }
    }
}