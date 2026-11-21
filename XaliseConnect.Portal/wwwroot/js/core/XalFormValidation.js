import { XalConstants } from './XalConstants.js';

const initializedForms = new WeakSet();

/**
 * Compare les bornes liées déclarées dans un formulaire et met à jour la validité de la borne de fin.
 *
 * Chaque borne de fin peut désigner sa borne de début avec `data-xal-range-start`.
 * La comparaison est effectuée uniquement lorsque les deux champs contiennent une valeur.
 *
 * @param   {HTMLFormElement} form Formulaire dont les bornes liées sont à vérifier.
 * @returns {void}
 */
function _validateRelatedRangeFields(form) {
    const rangeEndFields = form.querySelectorAll(XalConstants.cssQueries.form.rangeEndField);

    for (const rangeEndField of rangeEndFields) {
        const rangeStartSelector = rangeEndField.getAttribute(XalConstants.dataNames.xalise.rangeStart);

        const rangeStartField = rangeStartSelector
            ? form.querySelector(rangeStartSelector)
            : null;

        if (!(rangeStartField instanceof HTMLInputElement)
            || !(rangeEndField instanceof HTMLInputElement)
            || rangeStartField.value === ''
            || rangeEndField.value === '') {
            if (rangeEndField instanceof HTMLInputElement) {
                rangeEndField.setCustomValidity('');
            }

            continue;
        }

        const isRangeValid      = _areRangeBoundsValid(rangeStartField, rangeEndField);
        const validationMessage = rangeEndField.getAttribute(XalConstants.dataNames.xalise.rangeMessage) ?? 'La borne de fin doit être supérieure ou égale à la borne de début.';

        rangeEndField.setCustomValidity(isRangeValid ? '' : validationMessage);
    }
}

/**
 * Indique si la valeur de début est inférieure ou égale à la valeur de fin.
 * Les champs numériques et temporels de même type sont comparés par leur valeur numérique normalisée.
 * Les autres valeurs sont comparées selon leur représentation textuelle.
 *
 * @param   {HTMLInputElement} rangeStartField Champ de borne de début.
 * @param   {HTMLInputElement} rangeEndField   Champ de borne de fin.
 * @returns {boolean}                          `true` si la plage est valide, sinon `false`.
 */
function _areRangeBoundsValid(rangeStartField, rangeEndField) {
    const numericInputTypes = ['date', 'datetime-local', 'month', 'number', 'range', 'time', 'week'];

    if (rangeStartField.type === rangeEndField.type && numericInputTypes.includes(rangeStartField.type)) {
        return rangeStartField.valueAsNumber <= rangeEndField.valueAsNumber;
    }

    return rangeStartField.value <= rangeEndField.value;
}

/**
 * Met à jour les règles personnalisées puis valide un formulaire.
 *
 * @param   {HTMLFormElement} form Formulaire à vérifier.
 * @returns {boolean}              `true` si le formulaire est valide, sinon `false`.
 */
export function validateForm(form) {
    _validateRelatedRangeFields(form);
    form.classList.add(XalConstants.cssClasses.form.wasValidated);

    return form.checkValidity();
}

/**
 * Initialise les contraintes natives et les comparaisons entre bornes des formulaires.
 *
 * Un champ de fin peut déclarer son champ de début avec `data-xal-range-start` et personnaliser
 * son message avec `data-xal-range-message`. Les contraintes sont vérifiées à la modification
 * des champs et juste avant la soumission.
 *
 * @param   {ParentNode} [root=document] Racine DOM dans laquelle rechercher les formulaires.
 * @returns {void}
 */
export function initBootstrapFormValidation(root = document) {
    const forms = new Set(root.querySelectorAll(XalConstants.cssQueries.form.needsValidate));

    root.querySelectorAll(XalConstants.cssQueries.form.rangeEndField).forEach((rangeEndField) => {
        if (rangeEndField instanceof HTMLInputElement && rangeEndField.form) {
            forms.add(rangeEndField.form);
        }
    });

    forms.forEach((form) => {
        if (initializedForms.has(form)) {
            return;
        }

        initializedForms.add(form);

        const validateRangeFields = () => _validateRelatedRangeFields(form);

        validateRangeFields();

        form.addEventListener('input', validateRangeFields);
        form.addEventListener('change', validateRangeFields);

        form.addEventListener('submit', (event) => {
            if (!validateForm(form)) {
                event.preventDefault();
                event.stopPropagation();
            }
        });
    });
}
