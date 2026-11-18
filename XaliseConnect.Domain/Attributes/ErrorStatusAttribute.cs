namespace XaliseConnect.Domain.Attributes;

/// <summary>
/// Indique qu'un membre d'une énumération portant cet attribut représente un statut d'erreur.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class ErrorStatusAttribute : Attribute
{
}
