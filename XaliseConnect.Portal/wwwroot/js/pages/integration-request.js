import { XalConstants } from '../core/XalConstants.js';
import { getClosestAttributeContext, getTemplateById } from '../shared/dom.js';
import { initPopovers } from '../shared/initPopovers.js';
import { initTables } from '../shared/initTables.js';
import { XalDialog } from '../components/XalDialog.js';
import { XalToast } from '../components/XalToast.js';
import { XalLoaderOverlay } from '../loaders/XalLoaderOverlay.js';
import { XalHttp } from '../core/XalHttp.js';

const SELECTORS = Object.freeze({
    delete:         '.xal-table__row-actions--delete',
    history:        '.xal-table__row-actions--history',
    content:        '.xal-table__row-actions--content',
    failure:        '.xal-table__failure-trigger',
    deleteSelected: `[${XalConstants.dataNames.xalise.deleteSelected}]`,
    searchResults:  '#xal-id-integration-request-results',
});

const TEMPLATE_IDS = Object.freeze({
    history:    'xal-id-static-integration-history',
    rawContent: 'xal-id-static-integration-raw-content',
    error:      'xal-id-static-integration-error',
});

// =-=-=-
// Actions de ligne
// =-=-=-

/**
 * Demande confirmation avant de supprimer une demande d'intégration.
 *
 * La fonction récupère l'identifiant et l'URL de suppression depuis les attributs
 * du déclencheur, puis exécute la suppression uniquement après confirmation.
 *
 * @param   {HTMLElement} trigger Bouton ayant déclenché la demande de suppression.
 * @returns {Promise<void>}       Promesse terminée après la fermeture de la modale.
 * @throws  {Error}               Si la modale ne peut pas être initialisée ou affichée.
 */
async function confirmDelete(trigger) {
    const context = getClosestAttributeContext(
        trigger,
        XalConstants.cssQueries.xalise.record,
        XalConstants.dataNames.xalise.recordId
    );

    if (!context) return;

    const deleteUrl = trigger.getAttribute(XalConstants.dataNames.xalise.deleteUrl);

    if (!deleteUrl) {
        XalToast.error('Impossible de déterminer l’adresse de suppression de la demande.');
        return;
    }

    const recordLabel = trigger.getAttribute(XalConstants.dataNames.xalise.recordLabel) ?? context.value;
    const content     = `Êtes-vous sûr de vouloir supprimer la demande d'intégration ${recordLabel} ?`;

    await XalDialog.show({
        title: 'Supprimer la demande',
        content,
        dismissible: false,
        showCloseButton: false,
        preset: 'deleteConfirmation',
        variant: XalConstants.cssClasses.modal.variant.danger,
        buttonOverrides: {
            delete: {
                action: async () => deleteIntegrationRequest(deleteUrl, context, recordLabel),
            },
        },
    });
}

/**
 * Supprime une demande puis recharge les résultats avec les critères et l'état de table actuels.
 *
 * @param   {string} deleteUrl      URL de suppression générée par le serveur.
 * @param   {Object} recordContext  Contexte DOM de la demande à supprimer.
 * @param   {string} recordLabel    Numéro lisible de la demande.
 * @returns {Promise<void>}
 */
async function deleteIntegrationRequest(deleteUrl, recordContext, recordLabel) {
    try {
        await XalHttp.fetch(
            deleteUrl,
            {
                method: 'DELETE',
                headers: {
                    Accept: 'application/json',
                },
            },
            {
                nav: false,
                overlay: 'Suppression de la demande en cours...',
                onError: async (error) => showDeleteRequestError(error, recordLabel),
            }
        );

        XalToast.success(`La demande ${recordLabel} a été supprimée avec succès.`);

        const table = recordContext.element.closest('table');
        refreshIntegrationRequestSearch(table);
    }
    catch {
        // Le callback onError a déjà affiché un message adapté et aucune actualisation n'est demandée.
    }
}

/**
 * Demande confirmation avant de supprimer les demandes sélectionnées.
 *
 * @param   {HTMLElement} trigger Bouton de suppression groupée.
 * @returns {Promise<void>}       Promesse terminée après la fermeture de la modale.
 * @throws  {Error}               Si la modale ne peut pas être initialisée ou affichée.
 */
async function confirmDeleteSelected(trigger) {
    const deleteUrl = trigger.getAttribute(XalConstants.dataNames.xalise.deleteSelectedUrl);

    if (!deleteUrl) {
        XalToast.error('Impossible de déterminer l’adresse de suppression des demandes sélectionnées.');
        return;
    }

    const actionsContainer  = trigger.closest(`[${XalConstants.dataNames.xalise.tableContainerActions}]`);
    const tableSelector     = actionsContainer?.getAttribute(XalConstants.dataNames.xalise.tableContainerActions);
    const table             = tableSelector ? document.querySelector(tableSelector) : null;

    if (!(table instanceof HTMLTableElement)) {
        XalToast.error('Le tableau des demandes sélectionnées est introuvable.');
        return;
    }

    const selectedRequests = [...table.querySelectorAll(XalConstants.cssQueries.table.selection.checkRow)]
        .filter((checkbox) => checkbox instanceof HTMLInputElement && checkbox.checked)
        .map((checkbox) => {
            const row           = checkbox.closest(XalConstants.cssQueries.xalise.record);
            const deleteButton  = row?.querySelector(SELECTORS.delete);

            return {
                recordId: Number(row?.getAttribute(XalConstants.dataNames.xalise.recordId)),
                requestNumber: deleteButton?.getAttribute(XalConstants.dataNames.xalise.recordLabel) ?? '',
            };
        })
        .filter((request) => Number.isInteger(request.recordId) && request.recordId > 0);

    if (selectedRequests.length === 0) {
        XalToast.warning('Sélectionnez au moins une demande à supprimer.');
        return;
    }

    const content = selectedRequests.length === 1
        ? 'Confirmez-vous la suppression de la demande sélectionnée ?'
        : `Confirmez-vous la suppression des ${selectedRequests.length} demandes sélectionnées ?`;

    await XalDialog.show({
        title: 'Supprimer les demandes sélectionnées',
        content,
        dismissible: false,
        showCloseButton: false,
        preset: 'deleteConfirmation',
        variant: XalConstants.cssClasses.modal.variant.danger,
        buttonOverrides: {
            delete: {
                action: async () => deleteSelectedIntegrationRequests(deleteUrl, table, selectedRequests),
            },
        },
    });
}

/**
 * Envoie les identifiants sélectionnés, affiche le bilan et actualise la liste si au moins une suppression réussit.
 *
 * @param   {string}                deleteUrl        URL de suppression groupée générée par le serveur.
 * @param   {HTMLTableElement}      table            Tableau contenant les demandes sélectionnées.
 * @param   {Array<{recordId:number, requestNumber:string}>} selectedRequests Demandes confirmées par l'utilisateur.
 * @returns {Promise<void>}
 */
async function deleteSelectedIntegrationRequests(deleteUrl, table, selectedRequests) {
    let hasReceivedSuccessfulResponse = false;

    try {
        const response = await XalHttp.fetch(
            deleteUrl,
            {
                method: 'POST',
                headers: {
                    Accept: 'text/html',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(selectedRequests),
            },
            {
                nav: false,
                overlay: 'Suppression des demandes sélectionnées en cours...',
                onError: async (error) => showDeleteRequestError(error, null, true),
            }
        );

        hasReceivedSuccessfulResponse = true;

        const responseHtml      = await response.text();
        const responseDocument  = new DOMParser().parseFromString(responseHtml, 'text/html');
        const deletionSummary   = responseDocument.body.querySelector(`[${XalConstants.dataNames.xalise.deletionSummary}]`);

        if (!(deletionSummary instanceof HTMLElement)) {
            refreshAfterUnreadableBulkDeletionSummary(table);
            return;
        }

        const deletedCount   = Number(deletionSummary.getAttribute(XalConstants.dataNames.xalise.deletionSuccessCount));
        const failedCount    = Number(deletionSummary.getAttribute(XalConstants.dataNames.xalise.deletionFailureCount));
        const requestedCount = Number(deletionSummary.getAttribute(XalConstants.dataNames.xalise.deletionTotalCount));

        const hasValidCounts = Number.isInteger(deletedCount)
            && deletedCount >= 0
            && Number.isInteger(failedCount)
            && failedCount >= 0
            && Number.isInteger(requestedCount)
            && requestedCount > 0
            && requestedCount === selectedRequests.length
            && deletedCount + failedCount === requestedCount;

        if (!hasValidCounts) {
            refreshAfterUnreadableBulkDeletionSummary(table);
            return;
        }

        const hasAllFailed = deletedCount === 0 && failedCount === requestedCount;

        if (hasAllFailed) {
            XalToast.error(`Aucune des ${requestedCount} demandes sélectionnées n’a pu être supprimée. La liste n’a pas été modifiée.`);
            return;
        }

        const hasAllSucceeded = deletedCount === requestedCount && failedCount === 0;

        if (hasAllSucceeded) {
            const deletedMessage = deletedCount === 1
                ? '1 demande a été supprimée avec succès.'
                : `${deletedCount} demandes ont été supprimées avec succès.`;

            XalToast.success(deletedMessage);
        }
        else {
            await showBulkDeletionSummary(deletionSummary);
        }

        refreshIntegrationRequestSearch(table);
    }
    catch {
        if (hasReceivedSuccessfulResponse) {
            refreshAfterUnreadableBulkDeletionSummary(table);
        }
        // Sinon, le callback onError a déjà signalé l'échec HTTP ou réseau.
    }
}

/**
 * Signale que le bilan d'une suppression groupée est inexploitable et relance la recherche
 * pour réconcilier les résultats avec l'état courant du serveur.
 *
 * @param   {HTMLTableElement} table Tableau dont les résultats doivent être actualisés.
 * @returns {void}
 */
function refreshAfterUnreadableBulkDeletionSummary(table) {
    XalToast.warning('Le résultat des suppressions n’a pas pu être confirmé. La liste va être rechargée.');
    refreshIntegrationRequestSearch(table);
}

/**
 * Affiche le détail du résultat rendu par une vue Razor, puis attend la fermeture de la modale.
 *
 * @param   {HTMLElement} summaryView   Récapitulatif HTML renvoyé par le serveur.
 * @returns {Promise<void>}             Promesse terminée après la fermeture de la modale.
 */
async function showBulkDeletionSummary(summaryView) {
    const dialogElement = await XalDialog.show({
        title: 'Résultat de la suppression',
        content: summaryView,
        size: 'lg',
        variant: 'warning',
        dismissible: false,
        showCloseButton: false,
        preset: 'information',
    });

    await new Promise((resolve) => {
        dialogElement.addEventListener('hidden.bs.modal', resolve, { once: true });
    });
}

/**
 * Recharge les résultats du tableau en conservant les critères de recherche et l'état de pagination/tri.
 *
 * @param   {HTMLTableElement|null} table Tableau dont les résultats doivent être rechargés.
 * @returns {void}
 */
function refreshIntegrationRequestSearch(table) {
    const searchFormSelector = table?.getAttribute(XalConstants.dataNames.xalise.tableSearchForm);
    const searchForm         = searchFormSelector ? document.querySelector(searchFormSelector) : null;

    if (!(searchForm instanceof HTMLFormElement)) {
        console.error('[IntegrationRequest] Le formulaire de recherche est introuvable après la suppression des demandes.');
        return;
    }

    searchForm.dispatchEvent(new Event('xal-search:state-change'));
}

/**
 * Affiche un message utilisateur adapté à l'échec d'une suppression.
 *
 * @param   {Error|Response} error      Erreur réseau ou réponse HTTP en échec.
 * @param   {string}          recordLabel Numéro lisible de la demande.
 * @returns {Promise<void>}        Promesse terminée après l'affichage du message.
 */
async function showDeleteRequestError(error, recordLabel, isBulkOperation = false) {
    if (!(error instanceof Response)) {
        const targetLabel = isBulkOperation ? 'Les demandes sélectionnées n’ont pas été supprimées.' : 'La demande n’a pas été supprimée.';
        XalToast.error(`La communication avec le serveur a échoué. ${targetLabel} Vérifiez votre connexion puis réessayez.`);
        return;
    }

    if (error.status === 404 && recordLabel) {
        XalToast.error(`La demande ${recordLabel} n’existe plus ou a déjà été supprimée.`);
        return;
    }

    let message = '';

    try {
        const responseBody = await error.clone().json();
        message = responseBody.message ?? responseBody.error ?? '';
    }
    catch {
        // La réponse peut ne pas contenir de corps JSON exploitable.
    }

    if (!message) {
        message = {
            403: 'Vous n’êtes pas autorisé à supprimer cette demande.',
            500: 'Une erreur du serveur ou du stockage a empêché la suppression. La liste n’a pas été modifiée.',
            503: 'Le service de suppression est temporairement indisponible. Réessayez ultérieurement.',
        }[error.status] ?? 'La suppression de la demande a échoué. La liste n’a pas été modifiée.';
    }

    XalToast.error(message);
}

async function showHistory(trigger) {
    const context = getClosestAttributeContext(
        trigger,
        XalConstants.cssQueries.xalise.record,
        XalConstants.dataNames.xalise.recordId
    );

    if (!context) return;

    const details = getTemplateById(TEMPLATE_IDS.history);
    if (!details) return;

    await XalDialog.show({
        title: `Historique de la demande ${context.value}`,
        content: details.content,
        contentIsHtml: true,
        size: 'lg',
    });
}

async function showRawContent(trigger) {
    const context = getClosestAttributeContext(
        trigger,
        XalConstants.cssQueries.xalise.record,
        XalConstants.dataNames.xalise.recordId
    );

    if (!context) return;

    const details = getTemplateById(TEMPLATE_IDS.rawContent);
    if (!details) return;

    await XalDialog.show({
        title: `Contenu brut de la demande ${context.value}`,
        content: details.content,
        size: 'xl',
    });
}

async function showIntegrationFailure(trigger) {
    const context = getClosestAttributeContext(
        trigger,
        XalConstants.cssQueries.xalise.record,
        XalConstants.dataNames.xalise.recordId
    );
    
    if (!context) return;

    const details = getTemplateById(TEMPLATE_IDS.error);
    if (!details) return;

    await XalDialog.show({
        title: `Échec du transport - Demande ${context.value}`,
        content: details.content,
        variant: 'danger',
        dismissible: false,
        buttons: [
            {
                title: 'Fermer',
                classes: [
                    XalConstants.cssClasses.bootstrapBtn.size.sm,
                    XalConstants.cssClasses.bootstrapBtn.outlineSecondary,
                ],
                icon: XalConstants.cssClasses.bootstrapIcons.xLg,
            },
            {
                title: 'Rejouer l\'intégration',
                classes: [
                    XalConstants.cssClasses.bootstrapBtn.size.sm,
                    XalConstants.cssClasses.bootstrapBtn.xalise,
                ],
                icon: XalConstants.cssClasses.bootstrapIcons.arrowRepeat,
                action: () => replayIntegrationRequest(context.value),
            },
        ],
    });
}

async function replayIntegrationRequest(recordId) {
    // TODO : appeler le serveur, mettre à jour la demande et signaler le résultat.
    const releaseOverlay = XalLoaderOverlay.show('Enregistrement en cours...');

    try {
        await new Promise((resolve) => setTimeout(resolve, 5000));
    }
    finally {
        releaseOverlay();
    }

    XalToast.custom({
        title: 'Rejeu demandé',
        message: `Demande ${recordId} transmise pour rejeu.`,
        icon: XalConstants.cssClasses.bootstrapIcons.arrowRepeat,
        color: XalConstants.cssClasses.bootstrapTextBg.xalise,
    });
}

// =-=-=-=-=-=-=-=-=-=-=-=-
// Fonctions utilitaires
// =-=-=-=-=-=-=-=-=-=-=-=-

function exposeDialogDebugModule() {
    if (!new URLSearchParams(window.location.search).has('debug')) return;

    window.xaliseDebug = Object.freeze({
        ...window.xaliseDebug,
        XalDialog,
    });
}

// =-=-=-=-=-=-=-=-=-=-=-=-
// Initialisation déléguée
// =-=-=-=-=-=-=-=-=-=-=-=-

/**
 * Initialise les actions de ligne des demandes d'intégration par délégation de clic.
 * L'écouteur unique prend également en charge les lignes ajoutées après l'initialisation.
 *
 * @param {ParentNode} [root=document] Racine contenant le tableau des demandes.
 */
export function initIntegrationRequestActions(root = document) {
    root.addEventListener('click', (event) => {
        if (!(event.target instanceof Element)) return;

        const trigger = event.target.closest(
            `${SELECTORS.delete}, ${SELECTORS.deleteSelected}, ${SELECTORS.history}, ${SELECTORS.content}, ${SELECTORS.failure}`
        );
        
        if (!trigger || !root.contains(trigger)) return;

        if (trigger.matches(SELECTORS.delete)) {
            void confirmDelete(trigger);
        }
        else if (trigger.matches(SELECTORS.deleteSelected)) {
            void confirmDeleteSelected(trigger);
        }
        else if (trigger.matches(SELECTORS.history)) {
            void showHistory(trigger);
        }
        else if (trigger.matches(SELECTORS.content)) {
            void showRawContent(trigger);
        }
        else if (trigger.matches(SELECTORS.failure)) {
            void showIntegrationFailure(trigger);
        }
    });
}

/**
 * Réinitialise les composants des résultats après une recherche Ajax.
 *
 * @param   {CustomEvent} event Événement contenant le conteneur actualisé.
 * @returns {void}
 */
function handleSearchResultsUpdated(event) {
    const resultsContainer = event.detail?.resultsContainer;

    if (!(resultsContainer instanceof HTMLElement) || !resultsContainer.matches(SELECTORS.searchResults)) {
        return;
    }

    const resultsContent = resultsContainer.firstElementChild;

    if (resultsContent) {
        initTables(resultsContent);
        initPopovers(resultsContent);
    }
}

/**
 * Initialise la page des demandes d'intégration.
 *
 * @returns {void}
 */
function initIntegrationRequestPage() {
    const initialResults = document.querySelector(`${SELECTORS.searchResults} .xal-search-results-content`);

    if (initialResults) {
        initTables(initialResults);
        initPopovers(initialResults);
    }

    initIntegrationRequestActions();
    exposeDialogDebugModule();

    document.addEventListener('xal-search:results-updated', handleSearchResultsUpdated);

    document.addEventListener('xal-dialog:error', (event) => {
        const { error, button } = event.detail;

        console.log('Événement reçu : xal-dialog:error');
        console.log('Bouton concerné : ', button.title);
        console.error(error);

        XalToast.error('Le rejeu a échoué.');
    });
}

initIntegrationRequestPage();
