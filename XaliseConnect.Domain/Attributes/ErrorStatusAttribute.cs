namespace XaliseConnect.Domain.Attributes;

/// <summary>
/// Indique qu'un membre d'une énumération représente un statut d'erreur.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class ErrorStatusAttribute : Attribute
{
}
