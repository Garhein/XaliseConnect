const LOADER_PLACEHOLDER_TEMPLATE_ID = 'xal-id-loader-placeholder-template';
const LOADER_PLACEHOLDER_SELECTOR    = '.xal-loader-placeholder';

/**
 * Placeholder visuel affiché dans une zone cible pendant le chargement de données asynchrones.
 *
 * Chaque traitement exécute `acquire()` et libère le jeton récupéré dans un bloc `finally`.
 * Un double appel de libération sur un même jeton n'a aucun effet.
 * Le placeholder reste présent tant que tous les jetons n'ont pas été libérés.
 *
 * @public
 * @namespace XalLoaderPlaceholder
 */
export const XalLoaderPlaceholder = (() => {
    /**
     * États des cibles qui possèdent au moins un placeholder actif.
     *
     * @type {WeakMap<HTMLElement, Object>}
     */
    const _targetStates = new WeakMap();

    /**
     * Modes d'insertion du placeholder supportés.
     *
     * @type {Readonly<{ PREPEND: 'prepend', REPLACE: 'replace', APPEND: 'append' }>}
     */
    const INSERTION_MODES = Object.freeze({
        PREPEND: 'prepend',
        REPLACE: 'replace',
        APPEND: 'append',
    });

    /**
     * Ensemble des modes d'insertion valides.
     *
     * @type {Set<'prepend'|'replace'|'append'>}
     */
    const _validModes = new Set(Object.values(INSERTION_MODES));

    /**
     * Normalise le mode d'insertion ou retourne le mode par défaut lorsqu'il est inconnu.
     *
     * @param {unknown} mode Valeur du mode à normaliser.
     * @returns {'prepend'|'replace'|'append'} Mode d'insertion pris en charge.
     */
    const _normalizeInsertionMode = (mode) => {
        const normalizedMode = typeof mode === 'string'
            ? mode.toLowerCase()
            : mode;

        if (_validModes.has(normalizedMode)) {
            return normalizedMode;
        }

        console.warn(
            `[XalLoaderPlaceholder] Mode d'insertion "${normalizedMode}" non valide ; le mode "${INSERTION_MODES.PREPEND}" sera utilisé.`,
        );

        return INSERTION_MODES.PREPEND;
    };

    /**
     * Template HTML des placeholders.
     * Résolu lors de l'appel à `init()`.
     *
     * @type {HTMLTemplateElement|null}
     */
    let _templateElement = null;

    /**
     * Retourne l'élément cible fourni ou recherche le premier élément correspondant au sélecteur CSS.
     *
     * @param {string|HTMLElement} target Sélecteur CSS recherché dans le document ou élément HTML à utiliser directement.
     * @returns {HTMLElement|null} Élément HTML correspondant, ou `null` si la cible est absente ou n'est pas un élément HTML.
     * @throws {DOMException} Si le sélecteur CSS est invalide (nom de l'exception : `SyntaxError`).
     */
    const _resolveTargetElement = (target) => {
        if (target instanceof HTMLElement) {
            return target;
        }

        if (typeof target === 'string') {
            const targetElement = document.querySelector(target);
            return targetElement instanceof HTMLElement ? targetElement : null;
        }

        return null;
    };

    /**
     * Clone l'élément racine du template sans modifier le template d'origine.
     *
     * @returns {HTMLElement} Clone prêt à être inséré dans une cible.
     * @throws {Error} Si le template ne contient plus l'élément racine attendu.
     */
    const _createPlaceholderElement = () => {
        const templateRoot = _templateElement.content.firstElementChild;

        if (!(templateRoot instanceof HTMLElement) || !templateRoot.matches(LOADER_PLACEHOLDER_SELECTOR)) {
            throw new Error(
                `[XalLoaderPlaceholder] L'élément racine du template est absent ou ne correspond pas au sélecteur attendu (${LOADER_PLACEHOLDER_SELECTOR}).`,
            );
        }

        const placeholderElement = document.importNode(templateRoot, true);

        if (!(placeholderElement instanceof HTMLElement)) {
            throw new Error(
                "[XalLoaderPlaceholder] Le clone du template n'est pas un élément HTML.",
            );
        }

        return placeholderElement;
    };

    /**
     * Insère le placeholder dans la cible selon la stratégie demandée.
     * En mode `replace`, les noeuds initiaux sont déplacés dans un fragment afin de pouvoir, si besoin, les restaurer à la libération.
     *
     * @param {HTMLElement} target Élément dans lequel insérer le placeholder.
     * @param {'prepend'|'replace'|'append'} mode Stratégie d'insertion du placeholder.
     * @param {HTMLElement} placeholderElement Placeholder à insérer dans la cible.
     * @returns {DocumentFragment|null} Noeuds initiaux déplacés en mode `replace`, ou `null` pour les autres modes.
     * @throws {Error} Si `mode` ne correspond à aucune stratégie d'insertion prise en charge.
     */
    const _insertPlaceholder = (target, mode, placeholderElement) => {
        switch (mode) {
            case INSERTION_MODES.PREPEND: {
                target.prepend(placeholderElement);
                return null;
            }
            case INSERTION_MODES.REPLACE: {
                const originalContent = document.createDocumentFragment();

                while (target.firstChild) {
                    originalContent.appendChild(target.firstChild);
                }

                target.replaceChildren(placeholderElement);

                return originalContent;
            }
            case INSERTION_MODES.APPEND: {
                target.append(placeholderElement);
                return null;
            }
            default: {
                throw new Error(
                    `[XalLoaderPlaceholder] Le mode d'insertion "${mode}" n'est pas pris en charge.`,
                );
            }
        }
    };

    /**
     * Vérifie si l'élément DOM requis a été résolu avant son utilisation.
     *
     * @returns {void}
     * @throws {Error} Si `init()` n'a pas été appelé ou si l'élément DOM est manquant.
     */
    const _throwIfNotInitialized = () => {
        if (!_templateElement) {
            throw new Error(
                "[XalLoaderPlaceholder] Appelez `init()` avant d'utiliser `acquire()`.",
            );
        }
    };

    /**
     * État partagé par les traitements qui utilisent la même cible.
     *
     * @typedef  {Object} PlaceholderTargetState
     * @property {Set<symbol>}                  activeTokens        Jetons des traitements actifs.
     * @property {'prepend'|'replace'|'append'} mode                Mode utilisé pour insérer le placeholder.
     * @property {DocumentFragment|null}        originalContent     Contenu initial conservé en mode `replace`.
     * @property {HTMLElement}                  placeholderElement  Élément placeholder associé à la cible.
     */

    /**
     * Crée la fonction idempotente qui libère le jeton d'un traitement.
     *
     * Le placeholder reste en place tant que d'autres jetons sont actifs pour la cible.
     * À la dernière libération, le contenu initial est restauré en mode `replace` si le placeholder est le seul noeud de la cible ; sinon, seul ce placeholder est retiré.
     *
     * @param {HTMLElement} targetElement Élément associé au traitement.
     * @param {PlaceholderTargetState} targetState État partagé des traitements actifs sur l'élément.
     * @param {symbol} requestToken Jeton du traitement à libérer.
     * @returns {() => void} Fonction qui libère le jeton une seule fois.
     */
    const _createReleaseFunction = (targetElement, targetState, requestToken) => {
        let isReleased = false;

        return () => {
            if (isReleased) {
                return;
            }

            isReleased = true;
            targetState.activeTokens.delete(requestToken);

            if (targetState.activeTokens.size > 0) {
                return;
            }

            _targetStates.delete(targetElement);

            const shouldRestoreOriginalContent =
                targetState.mode === INSERTION_MODES.REPLACE
                && targetState.originalContent !== null
                && targetState.placeholderElement.parentElement === targetElement
                && targetElement.childNodes.length === 1
                && targetElement.firstChild === targetState.placeholderElement;

            if (shouldRestoreOriginalContent) {
                targetElement.replaceChildren(targetState.originalContent);
                return;
            }

            targetState.placeholderElement.remove();
        };
    };

    return Object.freeze({
        INSERTION_MODES,

        /**
         * Recherche et valide le template HTML requis par le module.
         * Conserve sa référence pour les appels suivants ; une initialisation répétée est sans effet.
         *
         * @returns {void}
         * @throws {Error} Si le template est absent, n'est pas un élément `<template>`, ou ne contient pas un unique élément racine HTML attendu.
         */
        init() {
            if (_templateElement) {
                return;
            }

            const templateElement = document.getElementById(LOADER_PLACEHOLDER_TEMPLATE_ID);

            if (!(templateElement instanceof HTMLTemplateElement)) {
                throw new Error(
                    `[XalLoaderPlaceholder] Le template du loader est absent du DOM ou n'est pas un élément <template> (identifiant : #${LOADER_PLACEHOLDER_TEMPLATE_ID}).`,
                );
            }

            const templateRoot = templateElement.content.firstElementChild;

            if (templateElement.content.children.length !== 1 || !(templateRoot instanceof HTMLElement) || !templateRoot.matches(LOADER_PLACEHOLDER_SELECTOR)) {
                throw new Error(
                    `[XalLoaderPlaceholder] Le template du loader doit contenir un unique élément racine HTML correspondant au sélecteur "${LOADER_PLACEHOLDER_SELECTOR}".`,
                );
            }

            _templateElement = templateElement;
        },

        /**
         * Acquiert un jeton de chargement et affiche le placeholder dans la cible.
         *
         * Le premier appel actif insère le placeholder et les appels suivants sur la même cible le partagent.
         * Le placeholder reste affiché jusqu'à la libération du dernier jeton, puis il est retiré.
         * En mode `replace`, le contenu initial est restauré si le placeholder est encore seul dans la cible.
         *
         * @param {string|HTMLElement} target Sélecteur CSS de la cible ou élément HTML.
         * @param {Object} [options={}] Options d'insertion.
         * @param {'prepend'|'replace'|'append'} [options.mode='prepend'] Position du placeholder. Une valeur non prise en charge utilise `prepend`.
         * @returns {() => void} Fonction idempotente qui libère le jeton ; sans effet si la cible est introuvable.
         * @throws {Error} Si `init()` n'a pas été appelé.
         * @throws {DOMException} Si `target` est un sélecteur CSS invalide (nom : `SyntaxError`).
         */
        acquire(target, { mode = INSERTION_MODES.PREPEND } = {}) {
            _throwIfNotInitialized();

            const targetElement = _resolveTargetElement(target);

            if (!targetElement) {
                console.warn('[XalLoaderPlaceholder] Cible introuvable :', target);
                return () => {};
            }

            const normalizedMode = _normalizeInsertionMode(mode);
            let targetState      = _targetStates.get(targetElement);

            if (!targetState) {
                const placeholderElement = _createPlaceholderElement();
                const originalContent    = _insertPlaceholder(targetElement, normalizedMode, placeholderElement);

                targetState = {
                    activeTokens: new Set(),
                    mode: normalizedMode,
                    originalContent,
                    placeholderElement,
                };

                _targetStates.set(targetElement, targetState);
            }
            else if (targetState.mode !== normalizedMode) {
                console.warn(
                    `[XalLoaderPlaceholder] La cible possède déjà un placeholder en mode "${targetState.mode}" ; le mode "${normalizedMode}" sera ignoré jusqu'à sa libération.`,
                );
            }

            const requestToken = Symbol('XalLoaderPlaceholder token request');

            targetState.activeTokens.add(requestToken);

            return _createReleaseFunction(targetElement, targetState, requestToken);
        },
    });
})();
