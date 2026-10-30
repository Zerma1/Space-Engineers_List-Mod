# Changelog — (DX11)Armor Thrusters

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
- `<ThrusterType>Ion</ThrusterType>` et `<PCU>12</PCU>` sur les 4 blocs (valeurs du petit
  propulseur ionique vanilla).
- `README.md` et `CHANGELOG.md` du mod, avec l'analyse.

### Corrigé
- Casse des chemins dans `CubeBlocks.sbc` alignée sur les fichiers :
  `Textures\darth_biomech\…` → `Textures\Darth_Biomech\…`,
  `small_armor_thruster.mwm` → `Small_armor_thruster.mwm`.

Aucun `TypeId`/`SubtypeId` modifié. Les points cosmétiques (anciennes catégories, balises
obsolètes, nom « Small » du grand bloc) sont documentés dans le README mais pas modifiés.

**Testé sur :** non testé en jeu. Validation statique uniquement (XML valide, pas de
`SubtypeId` en double, chemins `<Icon>`/`<Model>`/`BuildProgressModels` présents).

## [1.0] — 2026-10-30

Version d'origine importée depuis le Workshop (295393216), auteur Darth Biomech. Aucune modification.
