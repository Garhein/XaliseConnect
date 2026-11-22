import { XalConstants } from '../core/XalConstants.js';
import { initTableSelection } from '../core/XalTableSelection.js';
import { initTableSort, updateSortIcons } from '../core/XalTableSort.js';
import { initTablePagination } from '../core/XalTablePagination.js';

const PAGE_SELECT_SELECTOR = '[data-xal-table-page-select]';

/**
 * Récupère le tri et la page actuellement affichés, qui représentent le dernier état appliqué.
 *
 * @param   {ParentNode}       root  Racine contenant la table et sa pagination.
 * @param   {HTMLTableElement} table Table dont l'état affiché doit être lu.
 * @returns {Object<string, string>} Valeurs à restaurer si le tri demandé échoue.
 */
function getAppliedTableState(root, table) {
    const activeSortButton = [...table.querySelectorAll(XalConstants.cssQueries.table.sort.column)]
        .find((sortButton) => sortButton.closest('th')?.hasAttribute(XalConstants.ariaNames.sort));

    const activeSortHeader  = activeSortButton?.closest('th');
    const pageSelect        = root.querySelector(PAGE_SELECT_SELECTOR);

    return {
        sort: activeSortButton?.getAttribute(XalConstants.dataNames.xalise.tableSortColumn) ?? '',
        direction: activeSortHeader?.getAttribute(XalConstants.ariaNames.sort) ?? '',
        page: pageSelect instanceof HTMLSelectElement ? pageSelect.value : '1',
    };
}

/**
 * Met à jour l'état de recherche caché et demande une nouvelle recherche.
 *
 * @param   {HTMLTableElement}              table            Table dont les données changent.
 * @param   {Object<string, string|number>} stateValues      Valeurs d'état à modifier.
 * @param   {Object<string, string>}        [rollbackValues] État affiché à restaurer en cas d'échec.
 * @returns {void}
 */
function submitTableStateChange(table, stateValues, rollbackValues = null) {
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

    searchForm.dispatchEvent(new CustomEvent('xal-search:state-change', {
        detail: {
            rollbackValues,
            tableId: rollbackValues ? table.id : null,
        },
    }));
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

        submitTableStateChange(
            table,
            {
                sort: key,
                direction,
                page: 1,
            },
            getAppliedTableState(root, table)
        );
    });

    root.addEventListener('xal-table:page-size-change', (event) => {
        const table = event.target;

        if (!(table instanceof HTMLTableElement)) {
            return;
        }

        submitTableStateChange(
            table,
            {
                pageSize: event.detail.pageSize,
                page: 1,
            }
        );
    });

    root.addEventListener('xal-table:page-change', (event) => {
        const table = event.target;

        if (!(table instanceof HTMLTableElement)) {
            return;
        }

        submitTableStateChange(
            table,
            {
                page: event.detail.page,
            }
        );
    });

    root.addEventListener('xal-search:state-change-failed', (event) => {
        const table = event.target;
        const rollbackValues = event.detail?.rollbackValues;

        if (!(table instanceof HTMLTableElement) || !rollbackValues) {
            return;
        }

        updateSortIcons(table, rollbackValues.sort, rollbackValues.direction);
    });
}
