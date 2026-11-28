import { XalConstants } from '../core/XalConstants.js';

const DEFAULT_DELAY_MS  = 5000;
const TOAST_TEMPLATE_ID = 'xal-id-toast-template-feedback';
const TOAST_SELECTORS   = Object.freeze({
    container:  '.toast-container',
    root:       '.xal-toast',
    icon:       '.xal-toast__icon',
    title:      '.xal-toast__label',
    message:    '.xal-toast__message',
});

/**
 * Variantes de toast prises en charge.
 *
 * @typedef {'success'|'danger'|'warning'|'info'} ToastVariantName
 */

/**
 * Options d'affichage d'un toast personnalisé.
 *
 * @typedef {Object} ToastOptions
 * @property {string} message Texte affiché dans le toast.
 * @property {string} [title] Titre affiché dans l'en-tête.
 * @property {string} [icon]  Une ou plusieurs classes CSS de l'icône Bootstrap.
 * @property {string} [color] Une ou plusieurs classes CSS de couleur.
 */

/**
 * Configuration visuelle et textuelle des variantes prédéfinies.
 */
const TOAST_VARIANT_CONFIG = Object.freeze({
    [XalConstants.cssClasses.bootstrapVariant.success]: Object.freeze({
        title:  'Succès',
        icon:   XalConstants.cssClasses.bootstrapIcons.checkCircleFill,
        color:  XalConstants.cssClasses.bootstrapTextBg.success,
    }),
    [XalConstants.cssClasses.bootstrapVariant.danger]: Object.freeze({
        title:  'Erreur',
        icon:   XalConstants.cssClasses.bootstrapIcons.xCircleFill,
        color:  XalConstants.cssClasses.bootstrapTextBg.danger,
    }),
    [XalConstants.cssClasses.bootstrapVariant.warning]: Object.freeze({
        title:  'Avertissement',
        icon:   XalConstants.cssClasses.bootstrapIcons.exclamationTriangleFill,
        color:  XalConstants.cssClasses.bootstrapTextBg.warning,
    }),
    [XalConstants.cssClasses.bootstrapVariant.info]: Object.freeze({
        title:  'Information',
        icon:   XalConstants.cssClasses.bootstrapIcons.infoCircleFill,
        color:  XalConstants.cssClasses.bootstrapTextBg.info,
    }),
});

/**
 * Gestionnaire des notifications toast de l'application.
 *
 * Les messages sont toujours rendus comme du texte brut afin d'éviter l'interprétation de contenu HTML non fiable.
 *
 * @public
 * @requires XalConstants
 * @namespace XalToast
 */
export const XalToast = (() => {
    /**
     * Template HTML des toasts.
     * Résolu lors de l'appel à `init()` et cloné à chaque affichage.
     * 
     * @type {HTMLTemplateElement|null}
     */
    let _toastTemplateElement = null;

    /**
     * Vérifie si l'élément DOM requis a été résolu avant son utilisation.
     *
     * @returns {void}
     * @throws {Error} Si `init()` n'a pas été appelé ou si l'élément DOM est manquant.
     */
    const _throwIfNotInitialized = () => {
        if (!_toastTemplateElement) {
            throw new Error(
                "[XalToast] Initialisez le module avec `init()` avant d'afficher une notification."
            );
        }
    };

    /**
     * Vérifie les options communes aux notifications prédéfinies et personnalisées.
     *
     * @param {ToastOptions} options Contenu et apparence du toast à afficher.
     * @returns {void}
     * @throws {TypeError} Si le message ou une option facultative n'est pas une chaîne de caractères.
     */
    const _validateToastOptions = (options) => {
        if (!options || typeof options !== 'object' || Array.isArray(options)) {
            throw new TypeError(
                '[XalToast] Les options de la notification doivent être un objet.'
            );
        }

        if (typeof options.message !== 'string') {
            throw new TypeError(
                '[XalToast] Le message de la notification doit être une chaîne de caractères.'
            );
        }

        for (const optionName of ['title', 'icon', 'color']) {
            if (options[optionName] !== undefined && typeof options[optionName] !== 'string') {
                throw new TypeError(
                    `[XalToast] L'option "${optionName}" de la notification doit être une chaîne de caractères.`,
                );
            }
        }
    };

    /**
     * Récupère et valide le délai fourni aux variantes prédéfinies.
     *
     * @param {Object} options Options facultatives de la variante.
     * @returns {number} Délai en millisecondes avant fermeture automatique de la notification.
     * @throws {TypeError} Si les options ne sont pas un objet ou si le délai n'est pas un nombre.
     * @throws {RangeError} Si le délai n'est pas fini ou est négatif.
     */
    const _getVariantDelay = (options = {}) => {
        if (!options || typeof options !== 'object' || Array.isArray(options)) {
            throw new TypeError(
                "[XalToast] Les options d'affichage de la notification doivent être un objet."
            );
        }

        return _normalizeDelay(options.delay ?? DEFAULT_DELAY_MS);
    };

    /**
     * Vérifie et normalise le délai de fermeture automatique de la notification.
     *
     * @param {number} delay Délai en millisecondes.
     * @returns {number} Délai validé.
     * @throws {TypeError} Si le délai n'est pas un nombre.
     * @throws {RangeError} Si le délai n'est pas fini ou est négatif.
     */
    const _normalizeDelay = (delay) => {
        if (typeof delay !== 'number') {
            throw new TypeError(
                '[XalToast] Le délai de fermeture de la notification doit être un nombre.'
            );
        }

        if (!Number.isFinite(delay) || delay < 0) {
            throw new RangeError(
                '[XalToast] Le délai de fermeture de la notification doit être un nombre fini positif ou nul.'
            );
        }

        return delay;
    };

    /**
     * Ajoute les classes CSS fournies à un élément.
     *
     * @param {HTMLElement} element Élément auquel ajouter les classes.
     * @param {string} classNames Liste de classes séparées par des espaces.
     * @returns {void}
     */
    const _addCssClasses = (element, classNames) => {
        const cssClasses = classNames.trim().split(/\s+/).filter(Boolean);

        if (cssClasses.length > 0) {
            element.classList.add(...cssClasses);
        }
    };

    /**
     * Crée les options d'affichage d'une variante prédéfinie.
     *
     * @param {ToastVariantName} variant Variante à afficher.
     * @param {string} message Texte à afficher.
     * @returns {ToastOptions} Options complètes de la variante.
     */
    const _createVariantOptions = (variant, message) => {
        return Object.freeze({
            ...TOAST_VARIANT_CONFIG[variant],
            message: message,
        });
    };

    /**
     * Clone et configure l'élément toast à partir du template.
     * Le message est toujours inséré comme texte brut.
     *
     * @param {ToastOptions} options Options d'affichage.
     * @returns {HTMLElement} Élément toast configuré.
     * @throws {Error} Si le module n'est pas initialisé ou si le template ne contient plus les éléments requis.
     * @throws {TypeError} Si les options de la notification ne sont pas valides.
     */
    const _createToastElement = (options) => {
        _throwIfNotInitialized();
        _validateToastOptions(options);

        const fragment      = document.importNode(_toastTemplateElement.content, true);
        const toastElement  = fragment.firstElementChild;

        if (!(toastElement instanceof HTMLElement)) {
            throw new Error(
                '[XalToast] Le template de la notification ne contient plus son élément racine.'
            );
        }

        const iconElement       = toastElement.querySelector(TOAST_SELECTORS.icon);
        const titleElement      = toastElement.querySelector(TOAST_SELECTORS.title);
        const messageElement    = toastElement.querySelector(TOAST_SELECTORS.message);

        if (!iconElement || !titleElement || !messageElement) {
            throw new Error(
                '[XalToast] Le template ne contient pas tous les éléments requis pour afficher une notification.',
            );
        }

        if (options.color) {
            _addCssClasses(toastElement, options.color);
        }

        titleElement.textContent    = options.title ?? '';
        messageElement.textContent  = options.message;

        const iconClasses  = options.icon ?? '';
        iconElement.hidden = iconClasses.trim().length === 0;

        if (!iconElement.hidden) {
            _addCssClasses(iconElement, iconClasses);
        }

        return toastElement;
    };

    /**
     * Crée, insère et affiche un toast Bootstrap.
     * Le toast et son instance Bootstrap sont détruits après la fermeture.
     *
     * @param {ToastOptions} options Options d'affichage.
     * @param {number} delay Délai en millisecondes avant fermeture automatique.
     * @returns {void}
     * @throws {Error} Si le module n'est pas initialisé ou si Bootstrap Toast est indisponible.
     * @throws {RangeError|TypeError} Si le délai ou les options sont invalides.
     */
    const _displayToast = (options, delay = DEFAULT_DELAY_MS) => {
        const normalizedDelay   = _normalizeDelay(delay);
        const toastElement      = _createToastElement(options);
        const BootstrapToast    = globalThis.bootstrap?.Toast;

        if (!BootstrapToast) {
            throw new Error('[XalToast] Le composant Bootstrap Toast est indisponible.');
        }

        const toastInstance = new BootstrapToast(
            toastElement,
            {
                autohide: true,
                delay: normalizedDelay,
            }
        );

        toastElement.addEventListener(
            'hidden.bs.toast',
            () => {
                toastInstance.dispose();
                toastElement.remove();
            },
            { once: true },
        );

        try {
            const container     = document.querySelector(TOAST_SELECTORS.container);
            const parentElement = container ?? document.body;

            parentElement.appendChild(toastElement);
            toastInstance.show();
        } catch (error) {
            toastInstance.dispose();
            toastElement.remove();
            throw error;
        }
    };

    return Object.freeze({
        /**
         * Initialise le module et valide le template utilisé par les toasts.
         * Les appels suivants sont sans effet.
         *
         * @returns {void}
         * @throws {Error} Si le template est absent, mal formé ou incomplet.
         */
        init() {
            if (_toastTemplateElement) {
                return;
            }

            const templateElement = document.getElementById(TOAST_TEMPLATE_ID);

            if (!(templateElement instanceof HTMLTemplateElement)) {
                throw new Error(
                    `[XalToast] Le template est absent du DOM ou n'est pas un élément <template> (identifiant : #${TOAST_TEMPLATE_ID}).`,
                );
            }

            const templateRoot = templateElement.content.firstElementChild;

            if (templateElement.content.children.length !== 1
                || !(templateRoot instanceof HTMLElement)
                || !templateRoot.matches(TOAST_SELECTORS.root)) {
                throw new Error(
                    `[XalToast] Le template doit contenir un unique élément racine HTML correspondant au sélecteur "${TOAST_SELECTORS.root}".`,
                );
            }

            const requiredSelectors = [
                TOAST_SELECTORS.icon,
                TOAST_SELECTORS.title,
                TOAST_SELECTORS.message,
            ];

            const missingSelector = requiredSelectors.find(
                (selector) => !templateRoot.querySelector(selector),
            );

            if (missingSelector) {
                throw new Error(
                    `[XalToast] Le template ne contient pas l'élément requis correspondant au sélecteur "${missingSelector}".`,
                );
            }

            _toastTemplateElement = templateElement;
        },

        /**
         * Affiche une notification de succès.
         *
         * @param {string} message Texte à afficher.
         * @param {Object} [options={}] Options d'affichage.
         * @param {number} [options.delay=5000] Délai en millisecondes avant fermeture automatique de la notification.
         * @returns {void}
         * @throws {Error} Si le module n'est pas initialisé ou si Bootstrap Toast est indisponible.
         * @throws {RangeError|TypeError} Si le message ou le délai est invalide.
         */
        success(message, options = {}) {
            _displayToast(
                _createVariantOptions(XalConstants.cssClasses.bootstrapVariant.success, message),
                _getVariantDelay(options),
            );
        },

        /**
         * Affiche une notification d'erreur.
         *
         * @param {string} message Texte à afficher.
         * @param {Object} [options={}] Options d'affichage.
         * @param {number} [options.delay=5000] Délai en millisecondes avant fermeture automatique de la notification.
         * @returns {void}
         * @throws {Error} Si le module n'est pas initialisé ou si Bootstrap Toast est indisponible.
         * @throws {RangeError|TypeError} Si le message ou le délai est invalide.
         */
        error(message, options = {}) {
            _displayToast(
                _createVariantOptions(XalConstants.cssClasses.bootstrapVariant.danger, message),
                _getVariantDelay(options),
            );
        },

        /**
         * Affiche une notification d'avertissement.
         *
         * @param {string} message Texte à afficher.
         * @param {Object} [options={}] Options d'affichage.
         * @param {number} [options.delay=5000] Délai en millisecondes avant fermeture automatique de la notification.
         * @returns {void}
         * @throws {Error} Si le module n'est pas initialisé ou si Bootstrap Toast est indisponible.
         * @throws {RangeError|TypeError} Si le message ou le délai est invalide.
         */
        warning(message, options = {}) {
            _displayToast(
                _createVariantOptions(XalConstants.cssClasses.bootstrapVariant.warning, message),
                _getVariantDelay(options),
            );
        },

        /**
         * Affiche une notification d'information.
         *
         * @param {string} message Texte à afficher.
         * @param {Object} [options={}] Options d'affichage.
         * @param {number} [options.delay=5000] Délai en millisecondes avant fermeture automatique de la notification.
         * @returns {void}
         * @throws {Error} Si le module n'est pas initialisé ou si Bootstrap Toast est indisponible.
         * @throws {RangeError|TypeError} Si le message ou le délai est invalide.
         */
        info(message, options = {}) {
            _displayToast(
                _createVariantOptions(XalConstants.cssClasses.bootstrapVariant.info, message),
                _getVariantDelay(options),
            );
        },

        /**
         * Affiche une notification avec un titre, une icône et des classes CSS personnalisés.
         * Le message reste du texte brut.
         *
         * @param {ToastOptions} options Options définissant le message et l'apparence du toast.
         * @param {number} [delay=5000] Délai en millisecondes avant fermeture automatique de la notification.
         * @returns {void}
         * @throws {Error} Si le module n'est pas initialisé ou si Bootstrap Toast est indisponible.
         * @throws {RangeError|TypeError} Si les options ou le délai sont invalides.
         */
        custom(options, delay = DEFAULT_DELAY_MS) {
            _displayToast(options, delay);
        },
    });
})();
