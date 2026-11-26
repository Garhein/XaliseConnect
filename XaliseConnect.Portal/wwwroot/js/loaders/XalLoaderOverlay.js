import { XalConstants } from '../core/XalConstants.js';

const LOADER_OVERLAY_ELEMENT_ID         = 'xal-id-loader-overlay';
const LOADER_OVERLAY_MESSAGE_SELECTOR   = '.xal-loader-overlay__message';

/**
 * Overlay de chargement affiché lors de traitements longs.
 * 
 * Chaque traitement exécute `acquire()` et libère le jeton récupéré dans un bloc `finally`.
 * L'utilisation de jetons indépendants permet de gérer plusieurs traitements simultanés.
 * Un double appel de libération sur un même jeton n'a aucun effet, et l'overlay reste visible tant que tous les jetons n'ont pas été libérés.
 *
 * @public
 * @requires XalConstants
 * @namespace XalLoaderOverlay
 */
export const XalLoaderOverlay = (() => {
    /**
     * Jetons des traitements actifs.
     * Chaque traitement acquiert un jeton unique et le libère lorsqu'il est terminé.
     *
     * @type {Map<symbol, string>}
     */
    const _activeTokens = new Map();

    /**
     * Élément DOM de l'overlay.
     * Résolu lors de l'appel à `init()`.
     *
     * @type {HTMLElement|null}
     */
    let _overlayElement = null;

    /**
     * Élément DOM du message.
     * Résolu lors de l'appel à `init()`.
     * 
     * @type {HTMLElement|null}
     */
    let _messageElement = null;

    /**
     * Indique si l'overlay est actuellement affiché.
     * 
     * @type {boolean} `true` si l'overlay est affiché, sinon `false`.
     */
    let _isVisible = false;

    /**
     * Vérifie si les éléments DOM requis ont été résolus avant toute utilisation.
     * 
     * @returns {void}
     * @throws {Error} Si `init()` n'a pas été appelé ou si l'un des éléments DOM est manquant.
     */
    const _throwIfNotInitialized = () => {
        if (!_overlayElement || !_messageElement) {
            throw new Error(
                "[XalLoaderOverlay] Appeler `init()` avant d'utiliser `acquire()` ou de mettre à jour le message d'attente.",
            );
        }
    };

    /**
     * Synchronise l'affichage de l'overlay et gère son attribut ARIA.
     *
     * @param {boolean} isActive `true` pour afficher l'overlay, `false` pour le masquer.
     * @returns {void}
     */
    const _update = (isActive) => {
        _throwIfNotInitialized();

        _overlayElement.hidden = !isActive;
        _overlayElement.setAttribute(XalConstants.ariaNames.hidden, String(!isActive));
    };

    /**
     * Synchronise la visibilité et le message avec le premier traitement actif.
     *
     * @returns {void}
     */
    const _synchronizeActiveRequests = () => {
        const firstActiveRequest = _activeTokens.values().next();
        const hasActiveRequests  = !firstActiveRequest.done;
        const activeMessage      = hasActiveRequests ? firstActiveRequest.value : '';

        _setMessage(activeMessage);

        if (_isVisible !== hasActiveRequests) {
            _isVisible = hasActiveRequests;
            _update(hasActiveRequests);
        }
    };

    /**
     * Actualise le message affiché dans l'overlay.
     * Modifie uniquement le contenu texte sans affecter la visibilité de l'overlay.
     *
     * @param {string} [message=''] Message à afficher dans l'overlay.
     * @returns {void}
     */
    const _setMessage = (message = '') => {
        _messageElement.textContent = message;
    };

    return Object.freeze({
        /**
         * Initialise le module en résolvant les éléments DOM requis et synchronise son état initial.
         * Les appels suivants sont sans effet.
         *
         * @returns {void}
         * @throws {Error} Si l'un des éléments requis est absent du DOM.
         */
        init() {
            if (_overlayElement) return;

            const overlayElt = document.getElementById(LOADER_OVERLAY_ELEMENT_ID);
            const messageElt = overlayElt ? overlayElt.querySelector(LOADER_OVERLAY_MESSAGE_SELECTOR) : null;

            if (!overlayElt) {
                throw new Error(
                    `[XalLoaderOverlay] Le loader de page est absent du DOM (identifiant : #${LOADER_OVERLAY_ELEMENT_ID}).`
                );
            }

            if (!messageElt) {
                throw new Error(
                    `[XalLoaderOverlay] Le message associé au loader de page est absent du DOM (sélecteur : ${LOADER_OVERLAY_MESSAGE_SELECTOR}).`
                );
            }

            _overlayElement = overlayElt;
            _messageElement = messageElt;
            _isVisible      = false;

            _update(false);
        },

        /**
         * Récupère un jeton et associe un message au traitement correspondant.
         * 
         * L'overlay affiche le message du traitement actif le plus ancien.
         * Lorsqu'un traitement se termine, le message du traitement actif suivant prend sa place.
         *
         * @param {string} [message=''] Message associé au traitement.
         * @returns {() => void} Fonction idempotente qui libère le jeton de ce traitement.
         * @throws {Error} Si l'un des éléments du DOM n'a pas été initialisé.
         */
        acquire(message = '') {
            _throwIfNotInitialized();

            let isReleased      = false;
            const requestToken  = Symbol('XalLoaderOverlay token request');
            _activeTokens.set(requestToken, message);

            _synchronizeActiveRequests();

            return () => {
                if (isReleased) {
                    return;
                }

                isReleased = true;
                _activeTokens.delete(requestToken);

                _synchronizeActiveRequests();
            };
        },

        /**
         * Actualise le message du traitement actif le plus ancien sans changer l'état de visibilité de l'overlay.
         * 
         * Le message du traitement suivant s'affiche lorsque le traitement courant est libéré.
         * En l'absence de traitement actif, le texte est modifié pendant que l'overlay reste masqué.
         * 
         * @param {string} [message=''] Message à afficher dans l'overlay.
         * @returns {void}
         * @throws {Error} Si l'un des éléments du DOM n'a pas été initialisé.
         */
        updateMessage(message = '') {
            _throwIfNotInitialized();

            const firstActiveToken = _activeTokens.keys().next();

            if (!firstActiveToken.done) {
                _activeTokens.set(firstActiveToken.value, message);
                _synchronizeActiveRequests();
                return;
            }

            _setMessage(message);
        },
    });
})();
