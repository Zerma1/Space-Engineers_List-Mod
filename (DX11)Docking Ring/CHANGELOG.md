# Changelog — (DX11)Docking Ring

Format inspiré de [Keep a Changelog](https://keepachangelog.com/fr/1.1.0/).
Catégories : **Corrigé**, **Modifié**, **Supprimé**, **Ajouté**.

## [1.1] — 2026-10-30

### Modifié
- Dossier passé au format de mod moderne : `data/`, `models/`, `textures/` renommés en
  `Data/`, `Models/`, `Textures/`. Il peut ainsi être republié en objet Workshop non-legacy,
  téléchargeable par un serveur dédié (l'objet legacy d'origine fait échouer le démarrage
  du serveur avec `k_EResultTimeout`).

### Ajouté
- `metadata.mod` (`ModVersion` 1.1).
- `<PCU>115</PCU>` sur les 4 portes et `<PCU>100</PCU>` sur les 2 merge blocks (valeurs vanilla).
- La porte petite grille `DockingTubeDoorCenterS` dans la catégorie « Darth Biomech Mods »
  (barre G), partagée avec Armor Thrusters et Cryopods.
- `Data/BlockVariantGroups.sbc` : 3 groupes de variantes dans la barre G, **Docking Ring Sector**
  (secteur + coin), **Docking Ring Clamp** (pince + coin) et **Docking Ring Airlock** (4 portes).
  Amélioration demandée, hors réparation minimale.
- `README.md` et `CHANGELOG.md` du mod, avec l'analyse.

### Corrigé
- Casse des 18 chemins `<Icon>`, `<Model>` et `BuildProgressModels` de `CubeBlocks.sbc`
  alignée sur les noms réels des fichiers.

Aucun `TypeId`/`SubtypeId` modifié ; les 2 blocs cachés (`DockingTubeSector`, `DockingRing`)
sont conservés. Les points non corrigeables (textures référencées par les modèles) et les
points cosmétiques sont documentés dans le README.

**Testé sur :** Space Engineers 1.210.014 — solo local, créatif (2026-10-30). Chargement sans
erreur ni avertissement pour ce mod. Onglet et 3 groupes visibles dans la barre G, 8 blocs publics
posés, conservés après sauvegarde et rechargement. Testeur : ouverture et fermeture des 4 sas
(grande et petite grille), fusion de deux grandes grilles par les pinces, et séparation en
éteignant les pinces (une pince par grille, puis anneau complet de 4 pinces + 4 coins). Non testé :
étanchéité et sons des sas, étapes de soudure en détail, PCU affiché, serveur dédié (republication
nécessaire), migration d'un monde v1.0.

## [1.0] — 2026-10-30

Version d'origine importée depuis le Workshop (294619787), auteur Darth Biomech. Aucune modification.
