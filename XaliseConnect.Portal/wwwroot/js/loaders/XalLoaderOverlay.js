import { XalConstants } from '../core/XalConstants.js';

const MAIN_CONTENT_ELEMENT_ID           = 'xal-main-content';
const LOADER_OVERLAY_ELEMENT_ID         = 'xal-id-loader-overlay';
const LOADER_OVERLAY_STATUS_ELEMENT_ID  = 'xal-id-loader-overlay-status';
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
     * Région de statut annoncée aux technologies d'assistance.
     * Résolue lors de l'appel à `init()`.
     *
     * @type {HTMLElement|null}
     */
    let _statusElement = null;

    /**
     * Région principale de l'application, marquée comme occupée pendant les traitements.
     * Résolue lors de l'appel à `init()`.
     *
     * @type {HTMLElement|null}
     */
    let _mainContentElement = null;

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
        if (!_overlayElement
            || !_messageElement
            || !_statusElement
            || !_mainContentElement
        ) {
            throw new Error(
                "[XalLoaderOverlay] Appeler `init()` avant d'utiliser `acquire()` ou de mettre à jour le message d'attente.",
            );
        }
    };

    /**
     * Synchronise la visibilité de l'overlay.
     *
     * @param {boolean} isActive `true` pour afficher l'overlay, `false` pour le masquer.
     * @returns {void}
     */
    const _updateVisibility = (isActive) => {
        _throwIfNotInitialized();
        _overlayElement.hidden = !isActive;
    };

    /**
     * Synchronise l'état occupé de la région principale.
     *
     * @param {boolean} isBusy `true` si la région principale est occupée, sinon `false`.
     * @returns {void}
     */
    const _updateBusyState = (isBusy) => {
        _throwIfNotInitialized();
        _mainContentElement.setAttribute(XalConstants.ariaNames.busy, String(isBusy));
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
        _setStatusMessage(activeMessage);

        if (_isVisible !== hasActiveRequests) {
            _isVisible = hasActiveRequests;
            _updateVisibility(hasActiveRequests);
            _updateBusyState(hasActiveRequests);
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

    /**
     * Actualise le message de la région de statut accessible.
     *
     * @param {string} [message=''] Message à annoncer aux technologies d'assistance.
     * @returns {void}
     */
    const _setStatusMessage = (message = '') => {
        if (_statusElement.textContent !== message) {
            _statusElement.textContent = message;
        }
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

            const overlayElt     = document.getElementById(LOADER_OVERLAY_ELEMENT_ID);
            const messageElt     = overlayElt ? overlayElt.querySelector(LOADER_OVERLAY_MESSAGE_SELECTOR) : null;
            const statusElt      = document.getElementById(LOADER_OVERLAY_STATUS_ELEMENT_ID);
            const mainContentElt = document.getElementById(MAIN_CONTENT_ELEMENT_ID);

            if (!overlayElt) {
                throw new Error(
                    `[XalLoaderOverlay] Le loader de page est absent du DOM (identifiant : #${LOADER_OVERLAY_ELEMENT_ID}).`,
                );
            }

            if (!messageElt) {
                throw new Error(
                    `[XalLoaderOverlay] Le message associé au loader de page est absent du DOM (sélecteur : ${LOADER_OVERLAY_MESSAGE_SELECTOR}).`,
                );
            }

            if (!statusElt) {
                throw new Error(
                    `[XalLoaderOverlay] La région d'annonce du loader de page est absente du DOM (identifiant : #${LOADER_OVERLAY_STATUS_ELEMENT_ID}).`,
                );
            }

            if (!mainContentElt) {
                throw new Error(
                    `[XalLoaderOverlay] La région principale de l'application est absente du DOM (identifiant : #${MAIN_CONTENT_ELEMENT_ID}).`,
                );
            }

            _overlayElement     = overlayElt;
            _messageElement     = messageElt;
            _statusElement      = statusElt;
            _mainContentElement = mainContentElt;
            _isVisible          = false;

            _updateVisibility(false);
            _updateBusyState(false);
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

            let isReleased = false;
            const requestToken = Symbol('XalLoaderOverlay token request');
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
            _setStatusMessage('');
        },
    });
})();
