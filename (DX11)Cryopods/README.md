# (DX11)Cryopods

| | |
|---|---|
| **Auteur d'origine** | Darth Biomech |
| **Workshop d'origine** | https://steamcommunity.com/sharedfiles/filedetails/?id=294618647 |
| **Dernière mise à jour d'origine** | 2015-12-17 |
| **Dernière modification (ce dépôt)** | 2026-10-30 |
| **Statut** | `Fonctionnel` (solo local ; serveur dédié à tester après republication ; 4 `MOD_ERROR` non bloquantes, voir point 11) |
| **Version actuelle** | 1.1 |
| **Testé sur Space Engineers** | 1.210.014 — solo local, créatif (2026-10-30) |
| **Workshop republié** | non |

## Description

Capsules cryogéniques, grands et petits blocs : capsule fonctionnelle (`CryoChamber`), capsule ouverte (cockpit), capsule décorative et pack de capsules (8 blocs).

## Contenu

- `Data/` : `CubeBlocks.sbc` (8 blocs : 2 `CryoChamberDefinition`, 2 `CockpitDefinition`, 4 blocs décoratifs `CubeBlock`), `BlockCategories.sbc`, `BlockVariantGroups.sbc` (ajouté dans ce dépôt, v1.1), `Animations.sbc` (pose du personnage `CryopodPostureSolo`), `TransparentMaterials.sbc` (`CryopodGlass`)
- `Models/Darth_Biomech/` : 8 modèles `.mwm`
- `Textures/Darth_Biomech/` : textures des modèles ; `gui/` : 4 icônes
- `metadata.mod` : ajouté dans ce dépôt (v1.1)
- `DESCRIPTION.bbcode` : description pour la page Workshop (BBCode Steam, en anglais), ajoutée dans ce dépôt (v1.1)
- `README.txt`, `thumb.jpg` : fichiers d'origine de l'auteur
- Dépendances : aucune. Les sons et la texture de vitre sont repris du jeu de base (tous présents dans le jeu actuel).

Les blocs apparaissent dans la catégorie « Darth Biomech Mods » de la barre G, partagée
avec [(DX11)Armor Thrusters](../%28DX11%29Armor%20Thrusters/) et [(DX11)Docking Ring](../%28DX11%29Docking%20Ring/)
(même `<Name>darthbiomech</Name>` dans les trois mods). Depuis la v1.1, les 8 capsules
(fonctionnelle, ouverte, décorative et pack, grande et petite grille) y forment un seul groupe
de variantes, « Cryogenic Pods ».

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
toujours nécessaire tant que le serveur utilise l'ID d'origine 294618647 :
copier le `_legacy.bin` du client Steam dans `content/244850/294618647/` du serveur et déclarer
l'objet dans `appworkshop_244850.acf` (sections `WorkshopItemsInstalled` et
`WorkshopItemDetails`, `manifest` à `-1`, mêmes `ugchandle`, `size` et `timeupdated` que
le client). Le serveur le considère alors « Up to date » et le charge.

## Analyse

Analyse statique du 2026-10-30, comparée au contenu vanilla de la version du jeu installée.
XML valide, aucun `SubtypeId` en double, tous les chemins des `.sbc` pointent vers des fichiers existants.
Les numéros de ligne renvoient à la v1.1.

| # | Fichier:ligne | Problème | Gravité | Confirmé ? | État |
|---|---------------|----------|---------|------------|------|
| 1 | (structure du mod) | Objet Workshop legacy (`_legacy.bin`) : son téléchargement échoue sur un serveur dédié (`k_EResultTimeout`) et le serveur ne démarre pas | bloquant (serveur dédié) | confirmé (serveur Pterodactyl, 2026-10-29) | dossier passé au format moderne en v1.1 ; republication Workshop nécessaire pour régler le problème |
| 2 | (structure du mod) | Dossiers `data/`, `models/`, `textures/` en minuscules, pas de `metadata.mod` | mineur | confirmé | corrigé en v1.1 |
| 3 | `Data/CubeBlocks.sbc` (tous les `<Icon>`, `<Model>`, `<OverlayTexture>`), `Data/Animations.sbc:15` | Casse des chemins différente des dossiers (`models\`, `textures\`, `darth_biomech`) | mineur | supposé (aucun effet sous Windows) | corrigé en v1.1. Les `.mwm` référencent eux-mêmes `textures\darth_biomech\…` en minuscules : non modifiable sans les sources des modèles |
| 4 | `Data/CubeBlocks.sbc:60,118,177,232` | Pas de `<PCU>` (champ ajouté au jeu après 2015) | mineur | supposé (valeur par défaut non vérifiée) | corrigé en v1.1 (`15` sur les capsules fonctionnelles et les cockpits, comme la chambre cryo et le siège passager vanilla) ; blocs décoratifs laissés à la valeur par défaut |
| 5 | `Data/BlockCategories.sbc:43-54` | La catégorie « Darth Biomech Mods » ne listait que les 4 blocs grande grille | mineur | supposé (les variantes petite grille restent peut-être accessibles via le changement de taille, `BlockPairName`) | corrigé en v1.1 (8 blocs listés) |
| 6 | `Models/Darth_Biomech/*_progress_01.mwm` | Les modèles de construction utilisent `Textures\Models\Cubes\armor\armor_construction_*.dds`, qui n'existent plus dans le jeu : texture manquante pendant la construction | cosmétique | confirmé pour `cryopod_progress_01.mwm` (log de rendu du 2026-10-30 : « Missing textures for part(s) 03 - Default ») ; `cryopod_solo_progress_01.mwm` pas encore affiché | non modifiable (binaire) |
| 7 | `Data/Animations.sbc:15`, `Data/CubeBlocks.sbc:25,83,143,200` | Pose du personnage (`character_cryopod_solo_pose.mwm`) exportée en 2015 : le squelette du personnage a changé depuis, la pose peut être incorrecte | mineur | supposé, à vérifier en jeu | non modifié |
| 8 | `Data/BlockCategories.sbc:10,26` | Les catégories `LargeBlocks` et `SmallBlocks` n'existent plus dans le jeu : de nouvelles catégories portant ces noms peuvent apparaître dans la barre G | cosmétique | supposé, à vérifier en jeu | non modifié |
| 9 | `Data/CubeBlocks.sbc:53` (et 7 autres) | `<DamageEffectId>` remplacé par `<DamageEffectName>` dans le jeu actuel ; encore utilisé par quelques fichiers vanilla, donc a priori accepté | cosmétique | supposé | non modifié |
| 10 | `Models/Darth_Biomech/cryopod.mwm`, `Textures/Darth_Biomech/*_ns.dds`, `*_glass_clear_*.dds` | Fichiers non référencés par les `.sbc` (ou seulement par ce modèle inutilisé) : restes de la version DX9 | cosmétique | confirmé (aucune référence dans les `.sbc`) | conservés (binaires d'origine) |
| 11 | `Data/CubeBlocks.sbc:240,334` (`DB_Cryopod`, `DB_Cryopod_pack`) et `:286,380` (versions petite grille) | Blocs décoratifs (`CubeBlock`) dont la recette contient un composant `Computer` : le jeu affiche 4 `MOD_ERROR` au chargement (« contient un ordinateur, mais le TypeId du bloc n'est pas celui d'un terminal » : le bloc a un propriétaire qui ne peut pas être changé via le terminal). Les blocs sont quand même créés. Présent dès la version d'origine | mineur | confirmé (`SpaceEngineers.log` du 2026-10-30) | non modifié : changer le `TypeId` est exclu (suppression des blocs des mondes) ; retirer le `Computer` de la recette changerait le coût de construction, en attente de décision |

### Résultat du chargement en jeu (2026-10-30, solo local)

Monde créatif local avec les 3 mods (DX11) en v1.1 (`SpaceEngineers.log` du 2026-10-30, 09:38 et 09:58) :
les 8 définitions sont créées. Seul message : les 4 `MOD_ERROR` non bloquantes du point 11.
Les modèles, textures et l'animation ne sont chargés qu'à l'affichage des blocs : pas encore vérifiés.
Troisième chargement à 10:05, avec le groupe de variantes : `BlockVariantGroups.sbc` est chargé et
post-traité sans erreur ni avertissement.

Constaté en jeu par le testeur (2026-10-30) : l'onglet « Darth Biomech Mods » est bien séparé du
reste de la barre G et le groupe « Cryogenic Pods » s'affiche. Les 8 blocs (grande et petite grille)
ont été posés, puis le monde a été sauvegardé et rechargé (10:09) : les 8 sont présents dans la sauvegarde.
Log de rendu : seul `cryopod_progress_01.mwm` signale des textures manquantes (point 6) ; aucun
message pour les autres modèles. Aucun message sur l'animation dans le log du jeu.

## À tester en jeu

- [x] Le monde se charge (log du 2026-10-30, définitions seulement ; 4 `MOD_ERROR` non bloquantes, point 11)
- [x] Les 8 blocs apparaissent dans la catégorie « Darth Biomech Mods », avec ceux d'Armor Thrusters et Docking Ring — testeur, 2026-10-30
- [x] Le groupe « Cryogenic Pods » s'affiche — testeur, 2026-10-30
- [x] Les blocs posés restent après sauvegarde et rechargement (8/8 dans la sauvegarde du 2026-10-30)
- [x] Icônes et modèles s'affichent — testeur, 2026-10-30 (seul le modèle de construction du pack signale des textures manquantes dans le log de rendu, point 6)
- [ ] Étapes de soudure / démontage (pas vérifiées en détail)
- [x] Capsules fonctionnelles (grande et petite) : entrer, capsule pressurisée — testeur, 2026-10-30
- [ ] Capsule fonctionnelle : respawn
- [x] Capsules ouvertes (cockpit, grande et petite) : s'asseoir, pose du personnage correcte, vue 1re personne — testeur, 2026-10-30
- [ ] PCU affiché à 15 sur les blocs fonctionnels (non vérifié explicitement)
- [ ] Les blocs d'un monde existant (v1.0) sont toujours là après passage à la v1.1
- [x] Fonctionnement en solo — 1.210.014, 2026-10-30
- [ ] Fonctionnement en multijoueur / serveur dédié (après republication)

## Historique

Voir [CHANGELOG.md](CHANGELOG.md).
