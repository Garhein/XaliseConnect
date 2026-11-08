using System.Reflection;
using XaliseConnect.Domain.Attributes;

namespace XaliseConnect.Domain.Helpers;

/// <summary>
/// Définition des méthodes utilitaires applicables aux énumérations.
/// </summary>
public static class EnumHelper
{
    /// <summary>
    /// Indique si le membre d'énumération porte l'attribut <see cref="ErrorStatusAttribute"/>.
    /// </summary>
    /// <typeparam name="TEnum">Type de l'énumération.</typeparam>
    /// <param name="status">Valeur à examiner</param>
    /// <returns><see langword="true"/> si le membre porte l'attribut, sinon <see langword="false"/>.</returns>
    public static bool IsErrorStatus<TEnum>(this TEnum status) where TEnum : struct, Enum
    {
        FieldInfo? enumMember = typeof(TEnum).GetField(status.ToString());

        return enumMember?.IsDefined(typeof(ErrorStatusAttribute), inherit: false) == true;
    }
}
