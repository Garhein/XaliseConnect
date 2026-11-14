import { XalConstants } from './XalConstants.js';

const TABLE_PAGE_SIZE_ATTRIBUTE     = XalConstants.dataNames.xalise.tablePageSize;
const TABLE_PAGE_ATTRIBUTE          = XalConstants.dataNames.xalise.tablePage;
const TABLE_PAGE_SELECT_ATTRIBUTE   = XalConstants.dataNames.xalise.tablePageSelect;
const TABLE_PAGINATION_ATTRIBUTE    = XalConstants.dataNames.xalise.tablePagination;

/**
 * Récupère la table ciblée par un contrôle de pagination.
 *
 * @param   {HTMLElement}           control         Contrôle contenant le sélecteur de table.
 * @param   {string}                attributeName   Nom de l'attribut qui contient le sélecteur.
 * @returns {HTMLTableElement|null}                 Table ciblée ou null si elle est introuvable.
 */
function getTableFromControl(control, attributeName) {
    const tableSelector = control.getAttribute(attributeName);
    const table         = tableSelector ? document.querySelector(tableSelector) : null;

    return table instanceof HTMLTableElement ? table : null;
}

/**
 * Transmet un changement de page à la table concernée.
 *
 * @param   {HTMLTableElement}  table       Table ciblée.
 * @param   {number}            pageNumber  Numéro de page demandé.
 * @returns {void}
 */
function dispatchPageChange(table, pageNumber) {
    if (!(table instanceof HTMLTableElement) || !Number.isInteger(pageNumber) || pageNumber < 1) {
        return;
    }

    table.dispatchEvent(new CustomEvent('xal-table:page-change', {
        bubbles: true,
        detail: { page: pageNumber },
    }));
}

/**
 * Initialise les contrôles génériques de taille et de numéro de page.
 *
 * @param   {ParentNode} [root=document] Racine contenant les contrôles.
 * @returns {void}
 */
export function initTablePagination(root = document) {
    root.addEventListener('change', (event) => {
        if (!(event.target instanceof HTMLSelectElement)) {
            return;
        }

        const control = event.target;

        if (control.hasAttribute(TABLE_PAGE_SIZE_ATTRIBUTE)) {
            const table     = getTableFromControl(control, TABLE_PAGE_SIZE_ATTRIBUTE);
            const pageSize  = Number(control.value);

            if (table && Number.isInteger(pageSize) && pageSize > 0) {
                table.dispatchEvent(new CustomEvent('xal-table:page-size-change', {
                    bubbles: true,
                    detail: {
                        pageSize,
                        page: 1,
                    },
                }));
            }

            return;
        }

        if (control.hasAttribute(TABLE_PAGE_SELECT_ATTRIBUTE)) {
            const table = getTableFromControl(control, TABLE_PAGE_SELECT_ATTRIBUTE);
            dispatchPageChange(table, Number(control.value));
        }
    });

    root.addEventListener('click', (event) => {
        if (!(event.target instanceof Element)) {
            return;
        }

        const control = event.target.closest(`[${TABLE_PAGE_ATTRIBUTE}]`);

        if (!(control instanceof HTMLButtonElement) || control.disabled) {
            return;
        }

        const pagination = control.closest(`[${TABLE_PAGINATION_ATTRIBUTE}]`);
        const table      = pagination instanceof HTMLElement
            ? getTableFromControl(pagination, TABLE_PAGINATION_ATTRIBUTE)
            : null;

        if (!table || !pagination) {
            return;
        }

        const currentPage   = Number(pagination.dataset.xalTableCurrentPage);
        const pageCount     = Number(pagination.dataset.xalTablePageCount);
        const pageCommand   = control.getAttribute(TABLE_PAGE_ATTRIBUTE);

        const pageNumber = {
            first: 1,
            previous: currentPage - 1,
            next: currentPage + 1,
            last: pageCount,
        }[pageCommand];

        dispatchPageChange(table, pageNumber);
    });
}
