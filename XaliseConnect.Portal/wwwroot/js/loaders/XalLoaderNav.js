import { XalConstants } from "../core/XalConstants.js";

const LOADER_NAV_ELEMENT_ID = "xal-id-loader-nav";

/**
 * Barre de progression, sans indication de progression, affichée dans la barre de navigation.
 *
 * Chaque traitement exécute `acquire()` et libère le jeton récupéré dans un bloc `finally`.
 * L'utilisation de jetons indépendants permet de gérer plusieurs traitements simultanés.
 * Un double appel de libération sur un même jeton n'a aucun effet, et la barre de progression reste visible tant que tous les jetons n'ont pas été libérés.
 *
 * @public
 * @requires XalConstants
 * @namespace XalLoaderNav
 */
export const XalLoaderNav = (() => {
    /**
     * Jetons des traitements actifs.
     * Chaque traitement acquiert un jeton unique et le libère lorsqu'il est terminé.
     *
     * @type {Set<symbol>}
     */
    const _activeTokens = new Set();

    /**
     * Élément DOM de la barre de progression.
     * Résolu lors de l'appel à `init()`.
     *
     * @type {HTMLElement|null}
     */
    let _progressBarElement = null;

    /**
     * Indique si au moins un traitement est en cours.
     *
     * @returns {boolean} `true` si au moins un traitement est en cours, sinon `false`.
     */
    const _isActive = () => _activeTokens.size > 0;

    /**
     * Vérifie si l'élément DOM requis a été résolu avant toute utilisation.
     *
     * @returns {void}
     * @throws {Error} Si `init()` n'a pas été appelé ou si l'élément DOM est manquant.
     */
    const _throwIfNotInitialized = () => {
        if (!_progressBarElement) {
            throw new Error(
                "[XalLoaderNav] Appelez `init()` avant d'utiliser `acquire()` ou de mettre à jour le loader.",
            );
        }
    };

    /**
     * Synchronise l'affichage de la barre de progression et gère son attribut ARIA.
     *
     * @returns {void}
     * @throws {Error} Si l'élément DOM de la barre de progression n'a pas été initialisé.
     */
    const _update = () => {
        _throwIfNotInitialized();

        const isActive = _isActive();

        _progressBarElement.hidden = !isActive;
        _progressBarElement.setAttribute(XalConstants.ariaNames.hidden, String(!isActive));
    };

    return Object.freeze({
        /**
         * Initialise le module en résolvant l'élément DOM de la barre de progression et synchronise son état initial.
         * Les appels suivants sont sans effet.
         *
         * @returns {void}
         * @throws {Error} Si l'élément de la barre de progression est absent du DOM.
         */
        init() {
            if (_progressBarElement) return;

            const element = document.getElementById(LOADER_NAV_ELEMENT_ID);

            if (!element) {
                throw new Error(
                    `[XalLoaderNav] Le loader de navigation est absent du DOM (identifiant : #${LOADER_NAV_ELEMENT_ID}).`,
                );
            }

            _progressBarElement = element;
            _update();
        },

        /**
         * Affiche la barre de progression et retourne une fonction de libération propre au jeton acquis.
         *
         * @returns {() => void} Fonction qui libère le jeton une seule fois.
         * @throws {Error} Si l'élément DOM de la barre de progression n'a pas été initialisé.
         */
        acquire() {
            _throwIfNotInitialized();

            let isReleased      = false;
            const wasActive     = _isActive();
            const requestToken  = Symbol("XalLoaderNav token request");
            _activeTokens.add(requestToken);

            if (!wasActive) _update();

            return () => {
                if (isReleased) {
                    return;
                }

                isReleased = true;
                _activeTokens.delete(requestToken);

                if (!_isActive()) _update();
            };
        },
    });
})();
