# Revue de code — XaliseConnect.Application

## Améliorations à considérer

- **Borner la taille de page dans Application.** Le portail autorise les tailles 10, 25 et 50, tandis que le service impose seulement une taille minimale de 1. Décider si cette limite fait partie du cas d’usage et, si oui, la faire respecter indépendamment du contrôleur.
- **Définir le comportement des plages de dates inversées.** Les critères permettent une date de début postérieure à la date de fin ; le service renvoie alors zéro résultat. Confirmer si ce cas doit être une erreur de validation ou un résultat vide.
- **Aligner les comparaisons avec le stockage cible.** Le mock compare les numéros et valeurs de corrélation par égalité exacte et sensible à la casse. Vérifier que ce comportement correspond aux règles de collation et de recherche choisies pour le SQL.
- **Ajouter des tests ciblés.** Aucun projet de tests n’a été repéré dans Application. Couvrir les filtres combinés, les statuts absents, les limites temporelles, les tris invalides, les pages hors limites et les suppressions groupées avec identifiants répétés.

## Points positifs

- Les contrats de lecture et de commande sont séparés par interfaces.
- Les services prennent un `CancellationToken`.
- Les modèles de résultat utilisent des propriétés initialisables ou en lecture seule.
- Le stockage simulé protège les accès concurrents par verrou et renvoie une copie de sa collection.

## Limites

Cette revue est statique et n’a pas validé les comportements par exécution. Les points liés au tri, à la casse et à la politique de pagination doivent être confirmés avec les règles fonctionnelles attendues et l’implémentation de stockage retenue.
