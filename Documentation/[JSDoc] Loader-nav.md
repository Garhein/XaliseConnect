# Loader de navigation

## Rôle

`XalLoaderNav` affiche une barre de chargement indéterminée sous la barre de navigation. Elle signale une activité sans afficher de pourcentage et ne bloque pas les interactions avec la page.

Source : [`XalLoaderNav.js`](../XaliseConnect.Portal/wwwroot/js/loaders/XalLoaderNav.js).

## Initialisation et contrat DOM

Le module est initialisé par l'appel à la fonction `init()`. Celle-ci peut être appelée plusieurs fois : seule la première résolution du loader est effectuée.

L'élément requis est `#xal-id-loader-nav`. Le layout lui donne le rôle `progressbar`, un nom accessible et l'état initial `hidden` / `aria-hidden="true"`. Si l'élément est absent, `init()` lève une erreur.

Les méthodes `acquire()` et la synchronisation interne ne doivent être utilisées qu'après l'initialisation.

## Règles de gestion des jetons

- Chaque appel à `acquire()` crée un jeton indépendant et retourne sa fonction de libération.
- La barre apparaît à l'acquisition du premier jeton.
- Elle reste affichée tant qu'au moins un jeton est actif.
- La libération d'un jeton ne masque pas la barre si d'autres traitements sont en cours.
- La fonction de libération est idempotente : ses appels suivants n'ont aucun effet.
- Chaque traitement doit libérer son jeton dans un bloc `finally`.

Les jetons sont des `Symbol` privés au module. Les appelants n'ont pas à les conserver ni à les manipuler directement ; ils conservent uniquement la fonction de libération.

## Exemple d'utilisation

```js
const releaseLoader = XalLoaderNav.acquire();

try {
    await loadData();
} finally {
    releaseLoader();
}
```

Deux traitements simultanés acquièrent chacun leur propre jeton :

```js
const releaseFirst  = XalLoaderNav.acquire();
const releaseSecond = XalLoaderNav.acquire();

releaseFirst();  // La barre reste visible.
releaseSecond(); // La barre est masquée.
```

Dans l'application, `XalHttp.fetch()` active ce loader par défaut (`nav: true`) et libère son jeton dans son `finally`. Pour le désactiver sur une requête, passer `{ nav: false }` dans la configuration des indicateurs.

## Accessibilité et limites

L'attribut `aria-hidden` suit l'état de la barre. Le loader est indéterminé : il n'indique ni progression chiffrée ni durée restante. Il n'est pas modal et ne bloque pas le clavier ou les interactions de page.

## Vérification manuelle

Avec `?debug` dans l'URL, le module est accessible dans `window.xaliseDebug`. 