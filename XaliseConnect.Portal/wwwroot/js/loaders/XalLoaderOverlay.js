import { XalConstants } from '../core/XalConstants.js';

/**
 * Overlay de chargement affiché lors de traitements longs.
 * Chaque appel à `show()` reçoit un jeton de libération indépendant.
 * Le message du premier appel encore actif reste affiché ; le message suivant prend le relais lorsque ce jeton est libéré.
 *
 * @requires XalConstants
 * @namespace XalLoaderOverlay
 */
export const XalLoaderOverlay = (() => {
    /**
     * Élément DOM de l'overlay, résolu par `init()`.
     *
     * @type {HTMLElement|null}
     */
    let _overlayElement = null;

    /**
     * Élément DOM du message, résolu par `init()`.
     * 
     * @type {HTMLElement|null}
     */
    let _messageElement = null;

    /**
     * Indique si l’overlay est actuellement affiché.
     * 
     * @type {boolean} `true` si l'overlay est affiché, `false` sinon.
     */
    let _isVisible = false;

    /**
     * Messages des traitements actifs, conservés dans leur ordre d'acquisition.
     *
     * @type {Map<symbol, string>}
     */
    const _activeRequests = new Map();

    /**
     * Vérifie si les éléments DOM requis ont été initialisés avant toute utilisation.
     * 
     * @throws {Error} Si `init()` n’a pas été appelé ou si les éléments DOM sont manquants.
     */
    const _throwIfNotInitialized = () => {
        if (!_overlayElement || !_messageElement) {
            throw new Error('[XalLoaderOverlay] Veuillez exécuter `init()` avant `show()` ou `updateMessage()`.');
        }
    };

    /**
     * Synchronise l’affichage de l’overlay et gère l'attribut ARIA.
     *
     * @param {boolean} isActive `true` pour afficher l'overlay, `false` pour le masquer.
     */
    const _update = (isActive) => {
        _overlayElement.hidden = !isActive;
        _overlayElement.setAttribute(XalConstants.ariaNames.hidden, String(!isActive));
    };

    /**
     * Synchronise la visibilité et le message avec le premier traitement actif.
     *
     * @returns {void}
     */
    const _synchronizeActiveRequests = () => {
        const firstActiveRequest = _activeRequests.values().next();
        const hasActiveRequests  = !firstActiveRequest.done;
        const activeMessage      = hasActiveRequests ? firstActiveRequest.value : '';

        _setMessage(activeMessage);

        if (_isVisible !== hasActiveRequests) {
            _isVisible = hasActiveRequests;
            _update(hasActiveRequests);
        }
    };

    /**
     * Actualise le message affiché dans l’overlay de chargement.
     * Modifie uniquement le contenu texte sans affecter la visibilité de l’overlay.
     *
     * @param {string} [message=''] Message à afficher dans l’overlay.
     */
    const _setMessage = (message = '') => {
        _messageElement.textContent = message;
    };

    return Object.freeze({
        /**
         * Initialise le module en résolvant les éléments DOM requis.
         * Les appels suivants sont sans effet.
         *
         * @throws {Error} Si l’un des éléments requis est absent du DOM.
         */
        init() {
            if (_overlayElement) return;

            const overlayId         = XalConstants.elementIds.loader.overlay;
            const overlayElt        = document.getElementById(overlayId);

            const messageSelector   = XalConstants.cssQueries.loader.overlayMessage;
            const messageElt        = overlayElt ? overlayElt.querySelector(messageSelector) : null;

            if (!overlayElt) {
                throw new Error(`[XalLoaderOverlay] Élément "${overlayId}" introuvable dans le DOM.`);
            }

            if (!messageElt) {
                throw new Error(`[XalLoaderOverlay] Élément du message "${messageSelector}" introuvable dans l'overlay.`);
            }

            _overlayElement = overlayElt;
            _messageElement = messageElt;

            _isVisible = false;
            _update(false);
        },
        
        /**
         * Enregistre un traitement actif et affiche son message lorsqu'il devient le plus ancien actif.
         *
         * Le message du premier appel actif reste visible. Si cet appel se termine, le message
         * du prochain appel actif est affiché. La fonction retournée ne libère le jeton qu'une fois.
         *
         * @param   {string}   [message=''] Message associé au traitement.
         * @returns {() => void}            Fonction idempotente qui libère le jeton de ce traitement.
         * @throws  {Error}                  Si l’un des éléments du DOM n'a pas été initialisé.
         */
        show(message = '') {
            _throwIfNotInitialized();
            const requestToken = Symbol('XalLoaderOverlay request');
            _activeRequests.set(requestToken, message);
            _synchronizeActiveRequests();

            let isReleased = false;

            return () => {
                if (isReleased) {
                    return;
                }

                isReleased = true;
                _activeRequests.delete(requestToken);
                _synchronizeActiveRequests();
            };
        },

        /**
         * Actualise le message affiché dans l’overlay de chargement.
         * Modifie uniquement le contenu texte sans affecter la visibilité de l’overlay.
         * Le prochain changement du nombre d'appels actifs rétablit le message du premier appel actif.
         *
         * @param {string} [message=''] Message à afficher dans l’overlay.
         * @throws {Error} Si l’un des éléments du DOM n'a pas été initialisé.
         */
        updateMessage(message = '') {
            _throwIfNotInitialized();

            const firstActiveToken = _activeRequests.keys().next();

            if (!firstActiveToken.done) {
                _activeRequests.set(firstActiveToken.value, message);
                _synchronizeActiveRequests();
                return;
            }

            _setMessage(message);
        },
    });
})();
