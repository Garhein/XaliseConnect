# Loader placeholder

## Rôle

`XalLoaderPlaceholder` clone un squelette visuel depuis un template HTML et l'insère dans une cible pendant le chargement de données. La cible peut être un élément HTML ou un sélecteur CSS.

Source : [`XalLoaderPlaceholder.js`](../XaliseConnect.Portal/wwwroot/js/loaders/XalLoaderPlaceholder.js).

## Initialisation et contrat DOM

Le module est initialisé par l'appel à la fonction `init()`. Cette méthode valide le template `#xal-id-loader-placeholder-template` : il doit être un élément `<template>` contenant un unique élément racine HTML correspondant à `.xal-loader-placeholder`.

À chaque acquisition, le module clone l'élément racine avec `document.importNode(..., true)`. Le `true` copie également les descendants, ici les blocs du squelette. Le template original reste inchangé.

## Modes d'insertion

Le module expose `XalLoaderPlaceholder.INSERTION_MODES` :

| Mode | Effet |
| --- | --- |
| `prepend` | Insère le placeholder au début de la cible sans remplacer son contenu. Mode par défaut. |
| `append` | Ajoute le placeholder à la fin de la cible. |
| `replace` | Déplace les nœuds initiaux dans un `DocumentFragment`, puis remplace le contenu de la cible par le placeholder. |

Le mode est normalisé en minuscules. Une valeur non prise en charge génère un avertissement dans la console et utilise `prepend`.

## Règles de gestion des jetons

- Chaque appel à `acquire(target, options)` retourne une fonction de libération idempotente propre à cet appel.
- Les appels actifs sur le même élément partagent un placeholder et un état commun.
- Le mode du premier appel actif sur cette cible est conservé. Si un appel suivant demande un autre mode, celui-ci est ignoré et un avertissement est écrit dans la console.
- La libération d'un jeton ne retire pas le placeholder tant que d'autres jetons restent actifs sur la même cible.
- À la dernière libération, le module retire uniquement le placeholder qu'il a inséré.
- En mode `replace`, le contenu d'origine est restauré si le placeholder est encore le seul nœud de la cible. Si la cible a été remplacée par du contenu réel, ce nouveau contenu est conservé.
- Il est recommandé de libérer chaque jeton dans un bloc `finally`.

## Exemple d'utilisation directe

En supposant qu'une page contienne une cible `#results` :

```js
const releasePlaceholder = XalLoaderPlaceholder.acquire('#results', {
    mode: XalLoaderPlaceholder.INSERTION_MODES.PREPEND,
});

try {
    await loadResults();
} finally {
    releasePlaceholder();
}
```

Pour un chargement qui remplace temporairement la zone :

```js
const releasePlaceholder = XalLoaderPlaceholder.acquire('#results', {
    mode: XalLoaderPlaceholder.INSERTION_MODES.REPLACE,
});

try {
    await loadAndRenderResults();
} finally {
    releasePlaceholder();
}
```

Si le chargement échoue sans avoir remplacé le placeholder, le contenu déplacé est restauré à la dernière libération.

## Intégration avec `XalHttp`

`XalHttp.fetch()` accepte `placeholder` sous forme de sélecteur ou d'objet de configuration. Il acquiert le placeholder avant la requête et appelle sa fonction de libération dans `finally`, y compris si la requête échoue.

```js
await XalHttp.fetch(
    '/integration-request/search',
    { method: 'POST' },
    {
        placeholder: {
            target: '#results',
            mode: 'replace',
        },
        onSuccess: async (response, data) => {
            renderResults(data);
        },
    },
);
```

Une chaîne seule utilise le mode `prepend` :

```js
await XalHttp.fetch('/integration-request/search', {}, {
    placeholder: '#results',
});
```

## Comportements particuliers et limites

- Si aucun élément ne correspond au sélecteur, le module émet un avertissement et retourne une fonction de libération sans effet.
- Un sélecteur CSS mal formé provoque une `DOMException` dont le nom est `SyntaxError`.
- Le module ne fournit pas de méthode `hide()` : la fonction retournée par `acquire()` constitue le contrat de libération.
- Le placeholder est purement visuel ; ce module ne marque pas la cible avec `aria-busy` et n'annonce pas son état. Pour une mise à jour asynchrone accessible, l'appelant doit signaler l'état occupé de la région et fournir une annonce adaptée, ou activer aussi `XalLoaderOverlay`.
- Comme le placeholder est un enfant de la cible, un rendu qui remplace entièrement cette cible peut également retirer le placeholder avant la libération du dernier jeton. Les requêtes simultanées visant la même zone doivent donc coordonner le rendu de leurs résultats.

## Vérification manuelle

Avec `?debug` dans l'URL, le module est accessible dans `window.xaliseDebug`.
