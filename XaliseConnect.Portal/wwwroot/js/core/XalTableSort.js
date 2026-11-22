import { XalConstants } from './XalConstants.js';

/**
 * Lit le tri demandé dans le formulaire associé ou, à défaut, celui affiché par la table.
 *
 * @param   {HTMLTableElement} table Table dont le tri demandé doit être lu.
 * @returns {{key: string|null, direction: string|null}} État de tri courant.
 */
function getCurrentSortState(table) {
    const searchFormSelector = table.getAttribute(XalConstants.dataNames.xalise.tableSearchForm);
    const searchForm         = searchFormSelector ? document.querySelector(searchFormSelector) : null;

    if (searchForm instanceof HTMLFormElement) {
        const sortField         = searchForm.elements.namedItem('sort');
        const directionField    = searchForm.elements.namedItem('direction');

        if (sortField instanceof HTMLInputElement
            && directionField instanceof HTMLInputElement
            && sortField.value !== ''
            && directionField.value !== '') {
            return {
                key: sortField.value,
                direction: directionField.value,
            };
        }
    }

    const activeSortButton = [...table.querySelectorAll(XalConstants.cssQueries.table.sort.column)]
        .find((sortButton) => sortButton.closest('th')?.hasAttribute(XalConstants.ariaNames.sort));

    const activeHeader = activeSortButton?.closest('th');

    return {
        key: activeSortButton?.getAttribute(XalConstants.dataNames.xalise.tableSortColumn) ?? null,
        direction: activeHeader?.getAttribute(XalConstants.ariaNames.sort) ?? null,
    };
}

/**
 * Met à jour les icônes de tri d'une table sans modifier `aria-sort`.
 * Cette fonction sert à afficher temporairement le tri demandé ; le serveur reste responsable
 * de confirmer le tri effectif et de fournir la valeur finale de `aria-sort`.
 *
 * @param   {HTMLTableElement} table         Table dont les icônes doivent être actualisées.
 * @param   {string|null}      sortColumn    Clé de la colonne de tri souhaitée.
 * @param   {string|null}      sortDirection Sens de tri souhaité.
 * @returns {void}
 */
export function updateSortIcons(table, sortColumn, sortDirection) {
    table.querySelectorAll(XalConstants.cssQueries.table.sort.column).forEach((sortButton) => {
        const sortIcon = sortButton.querySelector(XalConstants.cssQueries.table.sort.icon);

        if (!sortIcon) {
            return;
        }

        sortIcon.classList.remove(
            XalConstants.cssClasses.bootstrapIcons.arrowUp,
            XalConstants.cssClasses.bootstrapIcons.arrowDown,
            XalConstants.cssClasses.bootstrapIcons.arrowDownUp
        );

        if (sortButton.getAttribute(XalConstants.dataNames.xalise.tableSortColumn) === sortColumn) {
            sortIcon.classList.add(
                sortDirection === XalConstants.sortDirections.ascending
                    ? XalConstants.cssClasses.bootstrapIcons.arrowUp
                    : XalConstants.cssClasses.bootstrapIcons.arrowDown
            );
        }
        else {
            sortIcon.classList.add(XalConstants.cssClasses.bootstrapIcons.arrowDownUp);
        }
    });
}

/**
 * Initialise le tri des colonnes déclarées avec `data-xal-table-sort-column`.
 * 
 * L'initialiseur met à jour provisoirement l'icône et émet `xal-table:sort` avec la clé et le sens
 * demandés. `aria-sort` reste celui des résultats visibles ; la réponse serveur confirme l'état final.
 * 
 * @param {ParentNode} root Racine dans laquelle écouter les clics de tri.
 */
export function initTableSort(root = document) {
    root.addEventListener('click', (event) => {
        if (!(event.target instanceof Element)) {
            return;
        }

        // Le clic peut provenir de l'icône ou du texte à l'intérieur du bouton
        const button = event.target.closest(XalConstants.cssQueries.table.sort.column);
        if (!(button instanceof HTMLButtonElement)) return;

        const table  = button.closest('table');

        // Ignore les déclencheurs qui ne sont pas placés dans une table
        if (!(table instanceof HTMLTableElement)) return;

        const sortColumn = button.getAttribute(XalConstants.dataNames.xalise.tableSortColumn);

        if (!sortColumn) {
            console.error('[XalTableSort] La clé de tri du bouton est absente.');
            return;
        }

        const currentSortState = getCurrentSortState(table);

        const direction = currentSortState.key === sortColumn
            && currentSortState.direction === XalConstants.sortDirections.ascending
            ? XalConstants.sortDirections.descending
            : XalConstants.sortDirections.ascending;

        updateSortIcons(table, sortColumn, direction);

        // Demande le tri ; `aria-sort` sera mis à jour par le rendu serveur après confirmation.
        table.dispatchEvent(new CustomEvent('xal-table:sort', {
            bubbles: true,
            detail: {
                key: sortColumn,
                direction,
                label: button.getAttribute(XalConstants.dataNames.xalise.tableSortLabel),
            },
        }));
    });
}
