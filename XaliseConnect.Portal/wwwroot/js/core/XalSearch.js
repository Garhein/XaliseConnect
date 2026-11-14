import { XalHttp } from './XalHttp.js';

const SEARCH_FORM_SELECTOR              = 'form.xal-form-search[data-xal-search-results]';
const CONVERTER_ATTRIBUTE_SELECTOR      = '[data-xal-search-convert][name]';
const initializedForms                  = new WeakSet();
const searchNumbersByResultsContainer   = new WeakMap();

const VALUE_CONVERTERS = Object.freeze({
    'datetime-local-to-utc': (value) => {
        const localDate = new Date(value);

        return Number.isNaN(localDate.getTime())
            ? null
            : localDate.toISOString();
    },
});

/**
 * Construit l'URL et les options HTTP à partir d'un formulaire de recherche.
 *
 * @param   {HTMLFormElement}                       form Formulaire à sérialiser.
 * @returns {{ url: URL, options: RequestInit }}         URL et options de la requête.
 * @throws  {Error}                                      Si un convertisseur déclaré sur un champ n'est pas disponible.
 */
function createSearchRequest(form) {
    const searchUrl             = new URL(form.action, window.location.origin);
    const searchParameters      = new URLSearchParams();
    const converterNamesByField = new Map(
        Array.from(form.querySelectorAll(CONVERTER_ATTRIBUTE_SELECTOR), (field) => [
            field.name,
            field.dataset.xalSearchConvert,
        ])
    );

    for (const [fieldName, fieldValue] of new FormData(form).entries()) {
        if (typeof fieldValue !== 'string' || fieldValue.length === 0) {
            continue;
        }

        const converterName = converterNamesByField.get(fieldName);
        let searchValue     = fieldValue;

        if (converterName) {
            const converter = VALUE_CONVERTERS[converterName];

            if (typeof converter !== 'function') {
                throw new Error(`Le convertisseur de recherche '${converterName}' n'est pas défini.`);
            }

            searchValue = converter(fieldValue);

            if (searchValue === null) {
                continue;
            }
        }

        searchParameters.append(fieldName, searchValue);
    }

    const method    = form.method.toUpperCase();
    const headers   = {
        Accept: 'text/html',
        'X-Requested-With': 'XMLHttpRequest',
    };

    const options = { method, headers };

    if (method === 'GET') {
        searchUrl.search = searchParameters.toString();
    }
    else {
        headers['Content-Type'] = 'application/x-www-form-urlencoded;charset=UTF-8';
        options.body            = searchParameters;
    }

    return { url: searchUrl, options };
}

/**
 * Construit la configuration des indicateurs de chargement d'un formulaire.
 *
 * @param   {HTMLFormElement} form  Formulaire contenant la configuration.
 * @returns {Object}                Configuration compatible avec XalHttp.fetch.
 * @throws  {Error}                 Si le type de loader déclaré n'est pas pris en charge.
 */
function createLoadingIndicators(form) {
    const loaderType = (form.dataset.xalSearchLoader ?? 'none').toLowerCase();

    if (!['none', 'nav', 'overlay'].includes(loaderType)) {
        throw new Error(`Le type de loader de recherche ${loaderType} n'est pas pris en charge.`);
    }

    const overlayMessage = form.dataset.xalSearchLoaderMessage?.trim();
    const indicators = {
        nav: loaderType === 'nav',
        overlay: loaderType === 'overlay'
            ? overlayMessage || true
            : false,
    };

    const placeholderTarget = form.dataset.xalSearchPlaceholder;
    const configuredMinDuration = form.dataset.xalSearchLoaderMinDuration;

    if (placeholderTarget) {
        indicators.placeholder = {
            target: placeholderTarget,
            mode: form.dataset.xalSearchPlaceholderMode || 'prepend',
        };
    }

    if (configuredMinDuration !== undefined) {
        const minDuration = Number(configuredMinDuration);

        indicators.minDuration = Number.isFinite(minDuration)
            ? Math.max(0, minDuration)
            : 0;
    }
    else if (placeholderTarget) {
        indicators.minDuration = 500;
    }

    return indicators;
}

/**
 * Soumet un formulaire de recherche et remplace son conteneur de résultats.
 *
 * @param   {HTMLFormElement}   form                Formulaire soumis.
 * @param   {HTMLElement}       resultsContainer    Conteneur à actualiser.
 * @returns {Promise<void>}                         Promesse terminée après le traitement de la réponse.
 */
async function submitSearchForm(form, resultsContainer) {
    const searchNumber = (searchNumbersByResultsContainer.get(resultsContainer) ?? 0) + 1;
    searchNumbersByResultsContainer.set(resultsContainer, searchNumber);

    try {
        const searchRequest     = createSearchRequest(form);
        const loadingIndicators = createLoadingIndicators(form);

        const response = await XalHttp.fetch(
            searchRequest.url,
            searchRequest.options,
            loadingIndicators
        );

        const responseHtml  = await response.text();

        if (searchNumber !== searchNumbersByResultsContainer.get(resultsContainer)) {
            return;
        }

        resultsContainer.innerHTML = responseHtml;

        resultsContainer.dispatchEvent(new CustomEvent('xal-search:results-updated', {
            bubbles: true,
            detail: {
                form,
                resultsContainer,
            },
        }));
    }
    catch (error) {
        if (!(error instanceof Response)) {
            console.error('[XalSearch] La recherche a échoué.', error);
        }
    }
}

/**
 * Initialise les formulaires de recherche Ajax déclarés dans la racine.
 *
 * Chaque formulaire doit préciser le sélecteur de son conteneur avec `data-xal-search-results`.
 * Les champs nécessitant une conversion précisent leur convertisseur avec `data-xal-search-convert`.
 * Le loader se configure avec `data-xal-search-loader` (`none`, `nav` ou `overlay`).
 * Pour personnaliser le message de l'overlay, ajouter `data-xal-search-loader-message` au formulaire.
 *
 * Exemple : `data-xal-search-loader="overlay" data-xal-search-loader-message="Recherche en cours..."`.
 *
 * @param   {ParentNode} [root=document] Racine contenant les formulaires.
 * @returns {void}
 */
export function initSearchForms(root = document) {
    const forms = root.querySelectorAll(SEARCH_FORM_SELECTOR);

    for (const form of forms) {
        if (initializedForms.has(form)) {
            continue;
        }

        const resultsSelector  = form.dataset.xalSearchResults;
        const resultsContainer = resultsSelector
            ? root.querySelector(resultsSelector)
            : null;

        if (!(resultsContainer instanceof HTMLElement)) {
            console.error(`[XalSearch] Conteneur de résultats introuvable pour le formulaire '${form.id}'.`);
            continue;
        }

        initializedForms.add(form);

        form.addEventListener('submit', (event) => {
            event.preventDefault();
            const pageField = form.elements.namedItem('page');

            if (pageField instanceof HTMLInputElement) {
                pageField.value = '1';
            }

            void submitSearchForm(form, resultsContainer);
        });

        form.addEventListener('xal-search:state-change', () => {
            void submitSearchForm(form, resultsContainer);
        });
    }
}
