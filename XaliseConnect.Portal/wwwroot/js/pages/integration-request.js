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

    const content = document.createElement('p');
    content.classList.add(XalConstants.cssClasses.text.justify);
    content.append('Êtes-vous sûr de vouloir supprimer la demande d\'intégration ');

    const identifier        = document.createElement('b');
    identifier.textContent  = recordLabel;
    content.append(identifier, ' ?');

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

        const table                 = recordContext.element.closest('table');
        const searchFormSelector    = table?.getAttribute(XalConstants.dataNames.xalise.tableSearchForm);
        const searchForm            = searchFormSelector ? document.querySelector(searchFormSelector) : null;

        if (!(searchForm instanceof HTMLFormElement)) {
            console.error('[IntegrationRequest] Le formulaire de recherche est introuvable après la suppression de la demande.');
            return;
        }

        searchForm.dispatchEvent(new Event('xal-search:state-change'));
    }
    catch {
        // Le callback onError a déjà affiché un message adapté et aucune actualisation n'est demandée.
    }
}

/**
 * Affiche un message utilisateur adapté à l'échec d'une suppression.
 *
 * @param   {Error|Response} error      Erreur réseau ou réponse HTTP en échec.
 * @param   {string}          recordLabel Numéro lisible de la demande.
 * @returns {Promise<void>}        Promesse terminée après l'affichage du message.
 */
async function showDeleteRequestError(error, recordLabel) {
    if (!(error instanceof Response)) {
        XalToast.error('La communication avec le serveur a échoué. La demande n’a pas été supprimée. Vérifiez votre connexion puis réessayez.');
        return;
    }

    if (error.status === 404) {
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
    XalLoaderOverlay.show('Enregistrement en cours...');

    try {
        await new Promise((resolve) => setTimeout(resolve, 5000));
    }
    finally {
        XalLoaderOverlay.hide();
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
            `${SELECTORS.delete}, ${SELECTORS.history}, ${SELECTORS.content}, ${SELECTORS.failure}`
        );
        
        if (!trigger || !root.contains(trigger)) return;

        if (trigger.matches(SELECTORS.delete)) {
            void confirmDelete(trigger);
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
