using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Reflection;

namespace XaliseConnect.Portal.Helpers;

/// <summary>
/// Définition des méthodes utilitaires applicables aux listes déroulantes (<see cref="SelectList"/>).
/// </summary>
public static class SelectListHelper
{
    /// <summary>
    /// Crée une liste déroulante à partir d'une énumération, en utilisant l'attribut <see cref="DescriptionAttribute"/> positionné
    /// sur chaque membre pour définir le libellé affiché dans la liste.
    /// </summary>
    /// <typeparam name="TEnum">Type de l'énumération.</typeparam>
    /// <param name="includeUnspecifiedOption"><see langword="true"/> pour inclure une option non spécifiée ; <see langword="false"/> pour ne pas l'inclure.</param>
    /// <param name="unspecifiedOptionText">Libellé de l'option non spécifiée.</param>
    /// <returns>Liste déroulante contenant les valeurs numériques et les libellés de l'énumération.</returns>
    /// <exception cref="ArgumentException">Si l'option non spécifiée est demandée avec un libellé vide ou composé d'espaces.</exception>
    /// <exception cref="InvalidOperationException">Si un membre de l'énumération ne possède pas d'attribut <see cref="DescriptionAttribute"/>.</exception>
    public static SelectList CreateSelectList<TEnum>(bool includeUnspecifiedOption = false, string unspecifiedOptionText = "Non renseigné") where TEnum : struct, Enum
    {
        List<SelectListItem> items = [];

        if (includeUnspecifiedOption)
        {
            if (string.IsNullOrWhiteSpace(unspecifiedOptionText))
            {
                throw new ArgumentException("Le libellé de l'option sans valeur ne peut pas être vide");
            }

            items.Add(new SelectListItem
            {
                Value = string.Empty,
                Text  = unspecifiedOptionText
            });
        }

        foreach (string memberName in Enum.GetNames<TEnum>())
        {
            FieldInfo? enumField                = typeof(TEnum).GetField(memberName, BindingFlags.Public | BindingFlags.Static);
            DescriptionAttribute? description   = enumField?.GetCustomAttribute<DescriptionAttribute>();

            if (description is null)
            {
                throw new InvalidOperationException($"Le membre '{memberName}' de l'énumération '{typeof(TEnum).Name}' doit avoir un attribut 'Description'.");
            }

            TEnum enumValue = Enum.Parse<TEnum>(memberName);

            items.Add(new SelectListItem
            {
                Value = Enum.Format(typeof(TEnum), enumValue, "D"),
                Text  = description.Description
            });
        }

        return new SelectList(items, nameof(SelectListItem.Value), nameof(SelectListItem.Text));
    }
}
