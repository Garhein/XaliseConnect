using System.Reflection;
using XaliseConnect.Domain.Attributes;

using System.ComponentModel;

namespace XaliseConnect.Domain.Helpers;

/// <summary>
/// Définition des méthodes utilitaires applicables aux énumérations.
/// </summary>
public static class EnumHelper
{
    /// <summary>
    /// Récupère le libellé défini par l'attribut <see cref="DescriptionAttribute"/> du membre d'énumération.
    /// </summary>
    /// <typeparam name="TEnum">Type de l'énumération.</typeparam>
    /// <param name="enumValue">Membre dont le libellé doit être récupéré.</param>
    /// <returns>Texte défini dans l'attribut <see cref="DescriptionAttribute"/>.</returns>
    /// <exception cref="InvalidOperationException">Si le membre ne possède pas d'attribut <see cref="DescriptionAttribute"/>.</exception>
    public static string GetDescription<TEnum>(this TEnum enumValue) where TEnum : struct, Enum
    {
        FieldInfo? enumMember               = typeof(TEnum).GetField(enumValue.ToString());
        DescriptionAttribute? description   = enumMember?.GetCustomAttribute<DescriptionAttribute>();

        if (description is null)
        {
            throw new InvalidOperationException($"Le membre '{enumValue}' de l'énumération '{typeof(TEnum).Name}' doit avoir un attribut 'Description'.");
        }

        return description.Description;
    }

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
