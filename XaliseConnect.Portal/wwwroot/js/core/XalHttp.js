import { XalToast } from '../components/XalToast.js';
import { XalLoaderNav } from '../loaders/XalLoaderNav.js';
import { XalLoaderOverlay } from '../loaders/XalLoaderOverlay.js';
import { XalLoaderPlaceholder } from '../loaders/XalLoaderPlaceholder.js';

const DEFAULT_NETWORK_ERROR_MESSAGE = 'Une erreur réseau est survenue. Veuillez réessayer.';

const DEFAULT_ERROR_MESSAGES = Object.freeze({
    400: 'Requête invalide.',
    401: 'Accès non autorisé à la ressource demandée.',
    403: 'Accès refusé à la ressource demandée.',
    404: 'Ressource non trouvée.',
    405: 'Méthode HTTP non autorisée pour accéder à la ressource.',
    408: 'Délai d’envoi de la requête dépassé.',
    409: 'Conflit avec l’état actuel de la ressource.',
    422: 'Données invalides.',
    429: 'Limite de requêtes atteinte.',
    500: 'Une erreur serveur est survenue.',
    502: 'Réponse invalide d’un serveur en amont.',
    503: 'Serveur temporairement indisponible.',
    504: 'Délai dépassé en attendant un serveur en amont.',
});

/**
 * Normalise les options visuelles associées à une requête HTTP.
 *
 * @param   {Object} indicators Options fournies par l'appelant.
 * @returns {Object}            Options normalisées pour les indicateurs de chargement et les erreurs.
 */
function buildIndicatorConfiguration(indicators) {
    const placeholder = indicators.placeholder
        ? typeof indicators.placeholder === 'string'
            ? { target: indicators.placeholder, mode: 'prepend' }
            : { mode: 'prepend', ...indicators.placeholder }
        : null;

    return {
        nav: indicators.nav ?? true,
        overlay: indicators.overlay ?? false,
        placeholder,
        onError: indicators.onError ?? null,
        onSuccess: indicators.onSuccess ?? null,
        errorMessages: indicators.errorMessages ?? {},
        minDuration: Number.isFinite(Number(indicators.minDuration))
            ? Math.max(0, Number(indicators.minDuration))
            : 0,
    };
}

/**
 * Extrait un message lisible du corps d'une réponse en erreur.
 *
 * @param   {Response}        response Réponse HTTP en erreur.
 * @returns {Promise<string>}          Message d'erreur exploitable par l'utilisateur.
 */
async function extractErrorMessage(response) {
    try {
        const contentType = response.headers.get('content-type') ?? '';

        if (contentType.includes('application/json')) {
            const responseBody = await response.json();
            return responseBody.message ?? responseBody.error ?? JSON.stringify(responseBody);
        }

        const responseText = (await response.text()).trim();

        return responseText.startsWith('<') ? '' : responseText;
    }
    catch {
        return '';
    }
}

/**
 * Lit une copie de la réponse selon son type de contenu.
 *
 * @param   {Response}         response Réponse à analyser.
 * @returns {Promise<unknown>}          Corps parsé en objet JSON ou en texte.
 */
async function parseResponse(response) {
    const contentType = response.headers.get('content-type') ?? '';

    if (contentType.includes('json')) {
        return await response.json();
    }

    return await response.text();
}

/**
 * Traite une erreur HTTP ou réseau et notifie l'appelant ou affiche un toast.
 *
 * @param   {Error|Response} error          Erreur HTTP ou réseau.
 * @param   {Object}         configuration  Configuration de la requête.
 * @returns {Promise<void>}                 Promesse terminée après le traitement de l'erreur.
 */
async function handleRequestError(error, configuration) {
    if (typeof configuration.onError === 'function') {
        try {
            await configuration.onError(error);
        }
        catch (callbackError) {
            console.error('[XalHttp] Le callback de gestion des erreurs a échoué.', callbackError);
        }

        return;
    }

    if (error instanceof Response) {
        const extractedMessage = await extractErrorMessage(error.clone());
        const message = configuration.errorMessages[error.status]
            ?? (extractedMessage || null)
            ?? DEFAULT_ERROR_MESSAGES[error.status]
            ?? `Erreur ${error.status} : ${error.statusText || 'Une erreur est survenue.'}`;

        if (error.status === 409) {
            XalToast.warning(message);
        }
        else {
            XalToast.error(message);
        }

        return;
    }

    XalToast.error(DEFAULT_NETWORK_ERROR_MESSAGE);
}

/**
 * Exécute des requêtes HTTP avec gestion commune des loaders et des erreurs.
 *
 * @namespace XalHttp
 */
export const XalHttp = Object.freeze({
    /**
     * Envoie une requête HTTP et renvoie sa réponse après vérification du statut.
     * Les loaders activés sont toujours arrêtés, y compris si la requête échoue.
     *
     * @param   {string|URL}                url                             URL de la ressource demandée.
     * @param   {RequestInit}               [fetchOptions={}]               Options natives de la requête Fetch.
     * @param   {Object}                    [indicators={}]                 Configuration des indicateurs et callbacks.
     * @param   {boolean}                   [indicators.nav=true]           Active la barre de chargement de navigation.
     * @param   {boolean|string}            [indicators.overlay=false]      Affiche l'overlay, avec un message facultatif.
     * @param   {string|Object|null}        [indicators.placeholder=null]   Cible du placeholder ou objet `{ target, mode }`.
     * @param   {Function|null}             [indicators.onError=null]       Callback personnalisé recevant une `Response` ou une `Error`.
     * @param   {Function|null}             [indicators.onSuccess=null]     Callback recevant la réponse et une copie de son corps parsé.
     * @param   {Record<number, string>}    [indicators.errorMessages={}]   Messages personnalisés indexés par statut HTTP.
     * @param   {number}                    [indicators.minDuration=0]      Durée minimale d'affichage des indicateurs, en millisecondes.
     * @returns {Promise<Response>}                                         Réponse HTTP réussie.
     * @throws  {Response}                                                  Si la réponse HTTP indique un échec.
     * @throws  {Error}                                                     Si la requête réseau ou un callback échoue.
     */
    async fetch(url, fetchOptions = {}, indicators = {}) {
        const configuration = buildIndicatorConfiguration(indicators);
        const startedAt     = Date.now();
        const loaderState   = { navStarted: false, overlayShown: false, placeholderShown: false, };

        try {
            if (configuration.nav) {
                XalLoaderNav.start();
                loaderState.navStarted = true;
            }

            if (configuration.overlay) {
                const overlayMessage = typeof configuration.overlay === 'string' ? configuration.overlay : '';
                XalLoaderOverlay.show(overlayMessage);
                loaderState.overlayShown = true;
            }

            if (configuration.placeholder) {
                XalLoaderPlaceholder.show(
                    configuration.placeholder.target,
                    {
                        mode: configuration.placeholder.mode,
                    }
                );

                loaderState.placeholderShown = true;
            }

            const response = await window.fetch(url, fetchOptions);

            if (!response.ok) {
                await handleRequestError(response, configuration);
                throw response;
            }

            if (typeof configuration.onSuccess === 'function') {
                await configuration.onSuccess(response.clone(), await parseResponse(response.clone()));
            }

            return response;
        }
        catch (error) {
            if (error instanceof Response) {
                throw error;
            }

            if (error instanceof DOMException && error.name === 'AbortError') {
                throw error;
            }

            await handleRequestError(error, configuration);
            throw error;
        }
        finally {
            const remainingDuration = configuration.minDuration - (Date.now() - startedAt);

            if (remainingDuration > 0) {
                await new Promise((resolve) => setTimeout(resolve, remainingDuration));
            }

            if (loaderState.placeholderShown) {
                XalLoaderPlaceholder.hide(configuration.placeholder.target);
            }

            if (loaderState.overlayShown) {
                XalLoaderOverlay.hide();
            }

            if (loaderState.navStarted) {
                XalLoaderNav.stop();
            }
        }
    },
});
