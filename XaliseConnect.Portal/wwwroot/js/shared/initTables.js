import { XalConstants } from '../core/XalConstants.js';
import { initTableSelection } from '../core/XalTableSelection.js';
import { initTableSort } from '../core/XalTableSort.js';
import { initTablePagination } from '../core/XalTablePagination.js';

/**
 * Met à jour l'état de recherche caché et demande une nouvelle recherche.
 *
 * @param   {HTMLTableElement}              table           Table dont les données changent.
 * @param   {Object<string, string|number>} stateValues     Valeurs d'état à modifier.
 * @returns {void}
 */
function submitTableStateChange(table, stateValues) {
    const searchFormSelector = table.getAttribute(XalConstants.dataNames.xalise.tableSearchForm);
    const searchForm         = searchFormSelector ? document.querySelector(searchFormSelector) : null;

    if (!(searchForm instanceof HTMLFormElement)) {
        console.error('[XalTable] Le formulaire de recherche de la table est introuvable.');
        return;
    }

    for (const [fieldName, fieldValue] of Object.entries(stateValues)) {
        const field = searchForm.elements.namedItem(fieldName);

        if (!(field instanceof HTMLInputElement)) {
            console.error(`[XalTable] Le champ d'état '${fieldName}' est introuvable dans le formulaire de recherche.`);
            return;
        }

        field.value = String(fieldValue);
    }

    searchForm.dispatchEvent(new Event('xal-search:state-change'));
}

/**
 * Initialise les interactions génériques d'une table dans la racine fournie.
 * Les changements de tri et de pagination actualisent le formulaire de recherche associé.
 *
 * @param   {ParentNode} [root=document] Racine contenant la table.
 * @returns {void}
 */
export function initTables(root = document) {
    initTableSelection(root);
    initTableSort(root);
    initTablePagination(root);

    root.addEventListener('xal-table:sort', (event) => {
        const table = event.target;

        if (!(table instanceof HTMLTableElement)) {
            return;
        }

        const { key, direction } = event.detail;

        submitTableStateChange(table, {
            sort: key,
            direction,
            page: 1,
        });
    });

    root.addEventListener('xal-table:page-size-change', (event) => {
        const table = event.target;

        if (!(table instanceof HTMLTableElement)) {
            return;
        }

        submitTableStateChange(table, {
            pageSize: event.detail.pageSize,
            page: 1,
        });
    });

    root.addEventListener('xal-table:page-change', (event) => {
        const table = event.target;

        if (!(table instanceof HTMLTableElement)) {
            return;
        }

        submitTableStateChange(table, {
            page: event.detail.page,
        });
    });
}
