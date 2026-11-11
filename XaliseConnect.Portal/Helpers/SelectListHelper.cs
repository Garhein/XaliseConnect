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
    /// Valeur de formulaire représentant la sélection d'une valeur <see langword="null"/>.
    /// </summary>
    public const string CSTS_NULL_VALUE = "__null__";

    /// <summary>
    /// Crée une liste déroulante à partir d'une énumération, en utilisant l'attribut <see cref="DescriptionAttribute"/> positionné
    /// sur chaque membre pour définir le libellé affiché dans la liste.
    /// </summary>
    /// <typeparam name="TEnum">Type de l'énumération.</typeparam>
    /// <param name="includeUnspecifiedOption"><see langword="true"/> pour inclure une option qui ne restreint pas la sélection ; <see langword="false"/> pour ne pas l'inclure.</param>
    /// <param name="unspecifiedOptionText">Libellé de l'option qui ne restreint pas la sélection.</param>
    /// <param name="includeNullValueOption"><see langword="true"/> pour inclure une option représentant une valeur <see langword="null"/> ; <see langword="false"/> pour ne pas l'inclure.</param>
    /// <param name="nullValueOptionText">Libellé de l'option représentant une valeur <see langword="null"/>.</param>
    /// <returns>Liste déroulante contenant les valeurs numériques et les libellés de l'énumération.</returns>
    /// <exception cref="ArgumentException">Si une option incluse est associée à un libellé vide ou composé d'espaces.</exception>
    /// <exception cref="InvalidOperationException">Si un membre de l'énumération ne possède pas d'attribut <see cref="DescriptionAttribute"/>.</exception>
    public static SelectList CreateSelectList<TEnum>(
        bool includeUnspecifiedOption = false,
        string unspecifiedOptionText = "Non renseigné",
        bool includeNullValueOption = false,
        string nullValueOptionText = "Sans valeur") where TEnum : struct, Enum
    {
        List<SelectListItem> items = [];

        if (includeUnspecifiedOption)
        {
            if (string.IsNullOrWhiteSpace(unspecifiedOptionText))
            {
                throw new ArgumentException("Le libellé de l'option de sélection par défaut ne peut pas être vide.", nameof(unspecifiedOptionText));
            }

            items.Add(new SelectListItem
            {
                Value = string.Empty,
                Text  = unspecifiedOptionText
            });
        }

        if (includeNullValueOption)
        {
            if (string.IsNullOrWhiteSpace(nullValueOptionText))
            {
                throw new ArgumentException("Le libellé de l'option correspondant à une valeur non renseignée ne peut pas être vide.", nameof(nullValueOptionText));
            }

            items.Add(new SelectListItem
            {
                Value = SelectListHelper.CSTS_NULL_VALUE,
                Text  = nullValueOptionText
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

    /// <summary>
    /// Convertit la valeur d'une liste déroulante d'énumération en valeur d'énumération ou en sélection d'une valeur <see langword="null"/>.
    /// </summary>
    /// <typeparam name="TEnum">Type de l'énumération.</typeparam>
    /// <param name="selectedValue">Valeur sélectionnée.</param>
    /// <param name="enumValue">Valeur d'énumération sélectionnée, ou <see langword="null"/> si aucune valeur précise n'est demandée.</param>
    /// <param name="isNullValueSelected"><see langword="true"/> si l'option représentant une valeur <see langword="null"/> est sélectionnée ; sinon, <see langword="false"/>.</param>
    /// <returns><see langword="true"/> si la sélection est reconnue ; sinon, <see langword="false"/>.</returns>
    public static bool TryParseSelectListValue<TEnum>(string? selectedValue, out TEnum? enumValue, out bool isNullValueSelected) where TEnum : struct, Enum
    {
        enumValue           = null;
        isNullValueSelected = false;

        if (string.IsNullOrWhiteSpace(selectedValue))
        {
            return true;
        }

        if (selectedValue == SelectListHelper.CSTS_NULL_VALUE)
        {
            isNullValueSelected = true;
            return true;
        }

        if (Enum.TryParse<TEnum>(selectedValue, out TEnum parsedValue) && Enum.IsDefined(parsedValue))
        {
            enumValue = parsedValue;
            return true;
        }

        return false;
    }
}
