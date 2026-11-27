# Loader overlay

## Rôle

`XalLoaderOverlay` affiche un indicateur de chargement couvrant la fenêtre, avec un message facultatif. Le spinner reste visible lorsque le message est vide. L'overlay bloque les interactions avec la page mais ne gère pas le focus comme une boîte de dialogue modale.

Source : [`XalLoaderOverlay.js`](../XaliseConnect.Portal/wwwroot/js/loaders/XalLoaderOverlay.js).

## Initialisation et contrat DOM

Le module est initialisé par l'appel à la fonction `init()`. Celle-ci résout et vérifie les éléments suivants ; un élément manquant provoquant une erreur :

- `#xal-id-loader-overlay`: l'overlay visuel ;
- `.xal-loader-overlay__message`: l'emplacement du message ;
- `#xal-id-loader-overlay-status`: la région d'annonce accessible ;
- `#xal-main-content`: la région dont l'état occupé est signalé.

L'overlay visuel garde `aria-hidden="true"` et est masqué/affiché avec la propriété `hidden`. La région d'annonce, persistante et visuellement masquée, possède `role="status"`, `aria-live="polite"` et `aria-atomic="true"`.

## Règles de gestion des jetons et des messages

- Chaque appel à `acquire(message)` enregistre un jeton unique et le message qui lui est associé.
- L'overlay apparaît au premier jeton et reste affiché tant qu'au moins un jeton est actif.
- Le message du traitement actif le plus ancien est affiché et annoncé.
- Les traitements suivants sont conservés dans leur ordre d'acquisition. Quand le plus ancien est libéré, le message du suivant prend le relais.
- Chaque fonction de libération ne libère que son propre jeton et est idempotente.
- `#xal-main-content` reçoit `aria-busy="true"` au début du premier traitement et `aria-busy="false"` après la libération du dernier.
- Les traitements doivent libérer leur jeton dans un bloc `finally`.

## Exemple d'utilisation

```js
const releaseOverlay = XalLoaderOverlay.acquire('Enregistrement en cours...');

try {
    await saveData();
} finally {
    releaseOverlay();
}
```

Les appels concurrents affichent les messages dans leur ordre d'acquisition :

```js
const releaseFirst  = XalLoaderOverlay.acquire('Chargement des résultats...');
const releaseSecond = XalLoaderOverlay.acquire('Chargement des détails...');

releaseFirst();  // Le message des détails prend le relais.
releaseSecond(); // L'overlay est masqué et la région principale n'est plus occupée.
```

`updateMessage(message)` remplace le message du jeton actif le plus ancien. Elle ne permet pas de cibler un jeton particulier. S'il n'y a aucun jeton actif, elle modifie le texte visuel masqué et laisse la région accessible vide ; le prochain `acquire()` fournit le message qui sera affiché.

Dans `XalHttp.fetch()`, l'overlay est désactivé par défaut. `overlay: true` l'affiche sans message ; `overlay: 'Enregistrement en cours...'` associe un message à la requête. `XalHttp` libère automatiquement le jeton dans son `finally`.

## Accessibilité et limites

La région `status` annonce les changements de message de manière polie. L'overlay visuel est masqué aux technologies d'assistance afin d'éviter une seconde représentation du même message. `aria-busy` est appliqué à la zone principale, hors de la région de statut.

Cet overlay n'est pas un dialogue modal : il ne déplace pas le focus, ne le confine pas et ne rend pas le reste de la page `inert`. Le style bloque le pointeur, mais le composant ne désactive pas lui-même les interactions clavier.

## Vérification manuelle

Avec `?debug` dans l'URL, utiliser `window.xaliseDebug.XalLoaderOverlay`. 
