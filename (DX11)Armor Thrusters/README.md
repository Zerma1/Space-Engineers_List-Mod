# (DX11)Armor Thrusters

| | |
|---|---|
| **Auteur d'origine** | Darth Biomech |
| **Workshop d'origine** | https://steamcommunity.com/sharedfiles/filedetails/?id=295393216 |
| **Dernière mise à jour d'origine** | 2015-12-10 |
| **Dernière modification (ce dépôt)** | 2026-10-30 |
| **Statut** | `À tester` |
| **Version actuelle** | 1.1 |
| **Testé sur Space Engineers** | non testé en jeu (v1.0 chargée par un serveur dédié le 2026-10-29, blocs non vérifiés) |
| **Workshop republié** | non |

## Description

Propulseurs intégrés dans un bloc d'armure, grands et petits blocs, en version droite et inclinée (4 blocs).

## Contenu

- `Data/` : `CubeBlocks.sbc` (4 `ThrustDefinition` : `LargeBlockArmorThrust`, `SmallBlockArmorThrust`, `LargeBlockArmorSloppedThrust`, `SmallBlockArmorSloppedThrust`), `BlockCategories.sbc`, `BlockVariantGroups.sbc` (ajouté dans ce dépôt, v1.1)
- `Models/Darth_Biomech/` : 4 modèles `.mwm`
- `Textures/Darth_Biomech/GUI/` : 2 icônes `.dds`
- `metadata.mod` : ajouté dans ce dépôt (v1.1)
- `README.txt`, `thumb.jpg` : fichiers d'origine de l'auteur
- Dépendances : aucune. Modèles de construction, sons et matériaux de flamme repris du jeu de base (tous présents dans le jeu actuel).

Les blocs apparaissent dans la catégorie « Darth Biomech Mods » de la barre G, partagée
avec [(DX11)Cryopods](../%28DX11%29Cryopods/) et [(DX11)Docking Ring](../%28DX11%29Docking%20Ring/)
(même `<Name>darthbiomech</Name>` dans les trois mods). Depuis la v1.1, les 4 propulseurs
(droit et incliné, grande et petite grille) y forment un seul groupe de variantes, « Armor Thrusters ».

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
toujours nécessaire tant que le serveur utilise l'ID d'origine 295393216 :
copier le `_legacy.bin` du client Steam dans `content/244850/295393216/` du serveur et déclarer
l'objet dans `appworkshop_244850.acf` (sections `WorkshopItemsInstalled` et
`WorkshopItemDetails`, `manifest` à `-1`, mêmes `ugchandle`, `size` et `timeupdated` que
le client). Le serveur le considère alors « Up to date » et le charge.

## Analyse

Analyse statique du 2026-10-30, comparée au contenu vanilla de la version du jeu installée.
XML valide, aucun `SubtypeId` en double, les 4 blocs sont bien listés dans les catégories.
Les numéros de ligne renvoient à la v1.1.

| # | Fichier:ligne | Problème | Gravité | Confirmé ? | État |
|---|---------------|----------|---------|------------|------|
| 1 | (structure du mod) | Objet Workshop legacy (`_legacy.bin`) : son téléchargement échoue sur un serveur dédié (`k_EResultTimeout`) et le serveur ne démarre pas | bloquant (serveur dédié) | confirmé (serveur Pterodactyl, 2026-10-29) | dossier passé au format moderne en v1.1 ; republication Workshop nécessaire pour régler le problème |
| 2 | (structure du mod) | Dossiers `data/`, `models/`, `textures/` en minuscules, pas de `metadata.mod` | mineur | confirmé | corrigé en v1.1 |
| 3 | `Data/CubeBlocks.sbc:11,95,169,254,98` | Casse des chemins différente des fichiers (`darth_biomech`, `small_armor_thruster.mwm`) : aucun effet sous Windows, fichier introuvable possible sur un système sensible à la casse | mineur | supposé (aucun effet sous Windows) | corrigé en v1.1 |
| 4 | `Data/CubeBlocks.sbc:84-85` (et équivalents) | Pas de `<ThrusterType>` ni de `<PCU>` (champs ajoutés au jeu après 2015) : type de propulseur et PCU laissés aux valeurs par défaut | mineur | supposé (valeurs par défaut non vérifiées) | corrigé en v1.1 (`Ion`, `12`, comme le petit propulseur ionique vanilla) |
| 5 | `Data/BlockCategories.sbc:10,24,38` | Les catégories `LargeBlocks`, `SmallBlocks` et `ShipThrusters` n'existent plus dans le jeu : de nouvelles catégories portant ces noms peuvent apparaître dans la barre G. Les blocs restent accessibles via la catégorie « Darth Biomech Mods » (ligne 56) | cosmétique | supposé, à vérifier en jeu | non modifié |
| 6 | `Data/CubeBlocks.sbc:70,75,33` (et équivalents) | Balises anciennes : `<FlameGlareMaterial>` (remplacée par `<FlameFlare>`), `<DamageEffectId>` (remplacée par `<DamageEffectName>`), `<Variants>`. Elles figurent encore dans quelques fichiers vanilla, donc a priori acceptées ; au pire, pas de reflet de flamme ni d'effet de dégât | cosmétique | supposé, à vérifier en jeu | non modifié |
| 7 | `Data/CubeBlocks.sbc:10` | Le grand bloc s'affiche « Small Armor Thrust » (idem pour la version inclinée, ligne 168) : même nom que la version petite grille | cosmétique | confirmé | non modifié (nom d'origine) |

### Résultat du chargement en jeu (2026-10-30, solo local)

Monde créatif local avec les 3 mods (DX11) en v1.1 (`SpaceEngineers.log` du 2026-10-30, 09:38 et 09:58) :
les 4 définitions sont créées sans aucune erreur ni aucun avertissement. Les balises anciennes
(point 6) sont donc acceptées par le jeu. Les modèles et textures ne sont chargés qu'à l'affichage
des blocs : pas encore vérifiés. Troisième chargement à 10:05, avec le groupe de variantes :
`BlockVariantGroups.sbc` est chargé et post-traité sans erreur ni avertissement.

Constaté en jeu par le testeur (2026-10-30) : l'onglet « Darth Biomech Mods » est bien séparé du
reste de la barre G et le groupe « Armor Thrusters » s'affiche. Blocs posés puis monde sauvegardé
et rechargé (10:09) : les 2 propulseurs petite grille sont présents dans la sauvegarde, et le log
de rendu ne signale aucune texture manquante pour les modèles de ce mod. Les 2 propulseurs grande
grille n'ont pas encore été posés (oubli confirmé par le testeur) : à tester.

## À tester en jeu

- [x] Le monde se charge sans erreur de mod (log du 2026-10-30, définitions seulement)
- [x] Les blocs apparaissent dans la barre G (catégorie « Darth Biomech Mods ») — testeur, 2026-10-30
- [x] Le groupe « Armor Thrusters » s'affiche — testeur, 2026-10-30
- [ ] Icônes, modèles et étapes de construction s'affichent (petite grille : aucune texture manquante dans le log de rendu ; grande grille à refaire)
- [ ] Construction / soudure / démontage
- [ ] Poussée, flamme bleue, PCU affiché à 12
- [ ] Les blocs d'un monde existant (v1.0) sont toujours là après passage à la v1.1
- [ ] Fonctionnement en solo
- [ ] Fonctionnement en multijoueur / serveur dédié (après republication)

## Historique

Voir [CHANGELOG.md](CHANGELOG.md).
