# (DX11)Docking Ring

| | |
|---|---|
| **Auteur d'origine** | Darth Biomech |
| **Workshop d'origine** | https://steamcommunity.com/sharedfiles/filedetails/?id=294619787 |
| **Dernière mise à jour d'origine** | 2015-10-02 |
| **Dernière modification (ce dépôt)** | 2026-10-30 |
| **Statut** | `À tester` |
| **Version actuelle** | 1.1 |
| **Testé sur Space Engineers** | non testé en jeu (v1.0 chargée par un serveur dédié le 2026-10-29, blocs non vérifiés) |
| **Workshop republié** | non |

## Description

Anneau d'amarrage modulaire : secteurs, coins, pinces d'amarrage (blocs de fusion) et sas (portes), surtout en grands blocs (10 blocs, dont 2 cachés).

## Contenu

- `Data/` : `CubeBlocks.sbc` (10 blocs : 4 `DoorDefinition`, 2 `MergeBlockDefinition`, 4 `CubeBlock` ; `DockingTubeSector` et `DockingRing` sont cachés, `<Public>false</Public>`), `BlockCategories.sbc`, `BlockVariantGroups.sbc` (ajouté dans ce dépôt, v1.1)
- `Models/Darth_Biomech/` : 16 modèles `.mwm` (dont les sous-parties de porte `Docking_ring_door_left/right.mwm`)
- `Textures/Darth_Biomech/` : textures des modèles ; `gui/` : 4 icônes
- `metadata.mod` : ajouté dans ce dépôt (v1.1)
- `README.txt`, `thumb.jpg` : fichiers d'origine de l'auteur
- Dépendances : aucune. Sons de porte et textures de construction repris du jeu de base.

Les blocs apparaissent dans la catégorie « Darth Biomech Mods » de la barre G, partagée
avec [(DX11)Armor Thrusters](../%28DX11%29Armor%20Thrusters/) et [(DX11)Cryopods](../%28DX11%29Cryopods/)
(même `<Name>darthbiomech</Name>` dans les trois mods). Depuis la v1.1, les 8 blocs publics y sont
rangés en 3 groupes de variantes (une icône par groupe) :

| Groupe | Blocs |
|--------|-------|
| Docking Ring Sector | `DockingRingSectorPart`, `DockingRingSectorCorner` |
| Docking Ring Clamp | `DockingRingPart` (merge block), `DockingRingCorner` |
| Docking Ring Airlock | `DockingTubeDoor`, `DockingTubeDoorReverse`, `DockingTubeDoorCenter`, `DockingTubeDoorCenterS` (petite grille) |

## Licence d'origine

Aucune licence précisée par l'auteur. Le `README.txt` d'origine indique seulement
« Model made by Darth Biomech » (DeviantArt : http://darth-biomech.deviantart.com).
Contact avec l'auteur : pas encore tenté.

## Note : mod Workshop « legacy » (2015)

Le Workshop distribue la version d'origine sous forme d'une archive unique `<ugchandle>_legacy.bin`
(un zip dont les chemins utilisent des `\`). La v1.0 de ce dépôt est cette archive extraite
telle quelle (dossiers `data/`, `models/`, `textures/` en minuscules). Depuis la v1.1, le dossier
est au format moderne (`Data/`, `Models/`, `Textures/`, `metadata.mod`) : une fois **republié**,
il devient un objet Workshop normal, que les serveurs dédiés téléchargent sans problème.

Sur un **serveur dédié**, le téléchargement de l'objet legacy d'origine échoue
(`k_EResultTimeout`) et le serveur ne démarre pas. Contournement constaté le 2026-10-29,
toujours nécessaire tant que le serveur utilise l'ID d'origine 294619787 :
copier le `_legacy.bin` du client Steam dans `content/244850/294619787/` du serveur et déclarer
l'objet dans `appworkshop_244850.acf` (sections `WorkshopItemsInstalled` et
`WorkshopItemDetails`, `manifest` à `-1`, mêmes `ugchandle`, `size` et `timeupdated` que
le client). Le serveur le considère alors « Up to date » et le charge.

## Analyse

Analyse statique du 2026-10-30, comparée au contenu vanilla de la version du jeu installée.
XML valide, aucun `SubtypeId` en double, les 18 chemins des `.sbc` pointent vers des fichiers
existants, sons de porte et axes de symétrie (`ZMinusHalfX`, `MinusHalfX`, `HalfX`) encore utilisés par le jeu.
Les numéros de ligne renvoient à la v1.1.

| # | Fichier:ligne | Problème | Gravité | Confirmé ? | État |
|---|---------------|----------|---------|------------|------|
| 1 | (structure du mod) | Objet Workshop legacy (`_legacy.bin`) : son téléchargement échoue sur un serveur dédié (`k_EResultTimeout`) et le serveur ne démarre pas | bloquant (serveur dédié) | confirmé (serveur Pterodactyl, 2026-10-29) | dossier passé au format moderne en v1.1 ; republication Workshop nécessaire pour régler le problème |
| 2 | (structure du mod) | Dossiers `data/`, `models/`, `textures/` en minuscules, pas de `metadata.mod` | mineur | confirmé | corrigé en v1.1 |
| 3 | `Data/CubeBlocks.sbc` (18 chemins `<Icon>`, `<Model>`, `BuildProgressModels`) | Casse des chemins différente des fichiers (`darth_biomech`, `docking_ring_Part.mwm` pour `Docking_Ring_part.mwm`, `docking_ring_Sector.dds`…) | mineur | supposé (aucun effet sous Windows) | corrigé en v1.1. Les `.mwm` référencent eux-mêmes `Textures\darth_biomech\…` en minuscules : non modifiable sans les sources des modèles |
| 4 | `Data/CubeBlocks.sbc:128,218,268,319,369,439` | Pas de `<PCU>` (champ ajouté au jeu après 2015) | mineur | supposé (valeur par défaut non vérifiée) | corrigé en v1.1 (`115` sur les portes, `100` sur les merge blocks, comme en vanilla) ; blocs `CubeBlock` laissés à la valeur par défaut |
| 5 | `Data/BlockCategories.sbc:43-54` | La porte petite grille `DockingTubeDoorCenterS` manquait dans la catégorie « Darth Biomech Mods » | mineur | confirmé (absente de la liste) | corrigée en v1.1 |
| 6 | `Models/Darth_Biomech/Docking_ring_door*.mwm`, `*_progress_*.mwm` | Les modèles référencent des textures `_de` absentes (`docking_ring_door_de.dds` du mod, `default_grey_construction_de.dds` du jeu). En DX11 le jeu utilise les `_cm`/`_ng`/`_add`, qui sont présentes | cosmétique | supposé, à vérifier en jeu | non modifiable (binaire) |
| 7 | `Data/CubeBlocks.sbc:376-439` | Blocs cachés `DockingTubeSector` et `DockingRing` (3×3, anciens modèles) : leurs modèles n'utilisent que des textures DX9 (`_de`, `_metal_ns`), donc un rendu probablement incorrect. Conservés pour ne pas supprimer ces blocs des mondes existants | cosmétique | supposé, à vérifier en jeu | non modifié |
| 8 | `Data/BlockCategories.sbc:10,29` | Les catégories `LargeBlocks` et `SmallBlocks` n'existent plus dans le jeu : de nouvelles catégories portant ces noms peuvent apparaître dans la barre G | cosmétique | supposé, à vérifier en jeu | non modifié |
| 9 | `Data/CubeBlocks.sbc:127,169` | `<DamageEffectId>` remplacé par `<DamageEffectName>` dans le jeu actuel ; encore utilisé par quelques fichiers vanilla, donc a priori accepté | cosmétique | supposé | non modifié |
| 10 | `Textures/Darth_Biomech/*_ns.dds`, `*_me.dds` | Textures DX9 non référencées par les modèles DX11 | cosmétique | supposé (recherche de chaînes dans les `.mwm`) | conservées (binaires d'origine) |

### Résultat du chargement en jeu (2026-10-30, solo local)

Monde créatif local avec les 3 mods (DX11) en v1.1 (`SpaceEngineers.log` du 2026-10-30, 09:38) :
les 10 définitions de blocs sont créées, sans aucune erreur ni aucun avertissement pour ce mod.
Les modèles et textures ne sont chargés qu'à l'affichage des blocs : pas encore vérifiés.
Second chargement à 09:58, avec les groupes de variantes : `BlockVariantGroups.sbc` est chargé et
post-traité sans erreur ni avertissement. Affichage des groupes dans la barre G pas encore vérifié.

## À tester en jeu

- [x] Le monde se charge sans erreur de mod (log du 2026-10-30, définitions seulement)
- [ ] Les 8 blocs publics apparaissent dans la catégorie « Darth Biomech Mods », avec ceux d'Armor Thrusters et Cryopods
- [ ] Les 3 groupes (Sector, Clamp, Airlock) s'affichent avec leur icône, et on passe d'une variante à l'autre
- [ ] Icônes, modèles et étapes de construction s'affichent
- [ ] Construction / soudure / démontage
- [ ] Portes (sas) : ouverture/fermeture, sous-parties animées, sons, étanchéité
- [ ] Pince d'amarrage (merge block) : fusion entre deux vaisseaux
- [ ] PCU affiché (115 portes, 100 pinces)
- [ ] Les blocs d'un monde existant (v1.0) sont toujours là après passage à la v1.1
- [ ] Fonctionnement en solo
- [ ] Fonctionnement en multijoueur / serveur dédié (après republication)

## Historique

Voir [CHANGELOG.md](CHANGELOG.md).
