# Changelog — (DX11)Cryopods

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
- `<PCU>15</PCU>` sur les 2 capsules fonctionnelles et les 2 cockpits (valeur de la chambre
  cryo et du siège passager vanilla).
- Les 4 variantes petite grille dans la catégorie « Darth Biomech Mods » (barre G), partagée
  avec Armor Thrusters et Docking Ring.
- `README.md` et `CHANGELOG.md` du mod, avec l'analyse.

### Corrigé
- Casse des chemins dans `CubeBlocks.sbc` et `Animations.sbc` alignée sur les dossiers :
  `models\` → `Models\`, `textures\` → `Textures\`, `darth_biomech` → `Darth_Biomech`.

Aucun `TypeId`/`SubtypeId` modifié. Les points non corrigeables (textures de construction
absentes du jeu, pose du personnage de 2015) et les points cosmétiques sont documentés dans le README.

**Testé sur :** non testé en jeu. Validation statique uniquement (XML valide, pas de
`SubtypeId` en double, tous les chemins des `.sbc` présents).

## [1.0] — 2026-10-30

Version d'origine importée depuis le Workshop (294618647), auteur Darth Biomech. Aucune modification.
