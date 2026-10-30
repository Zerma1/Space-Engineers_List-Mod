# (DX11)Cryopods

| | |
|---|---|
| **Auteur d'origine** | Darth Biomech |
| **Workshop d'origine** | https://steamcommunity.com/sharedfiles/filedetails/?id=294618647 |
| **Dernière mise à jour d'origine** | 2015-12-17 |
| **Dernière modification (ce dépôt)** | 2026-10-30 |
| **Statut** | `À analyser` |
| **Version actuelle** | 1.0 (pas de `metadata.mod` : mod au format Workshop 2015) |
| **Testé sur Space Engineers** | non testé en jeu (chargé par un serveur dédié le 2026-10-29, blocs non vérifiés) |
| **Workshop republié** | non |

## Description

Capsules cryogéniques, grands et petits blocs : capsule fonctionnelle (`CryoChamber`), capsule ouverte (cockpit), capsule décorative et pack de capsules (8 blocs).

## Contenu

- `data/` : `CubeBlocks.sbc` (8 blocs, dont 2 `CryoChamberDefinition` et 2 `CockpitDefinition`), `BlockCategories.sbc`, `Animations.sbc` (pose du personnage), `TransparentMaterials.sbc`
- `models/Darth_Biomech/` : 8 modèles `.mwm`
- `textures/Darth_Biomech/` : textures et icônes
- `README.txt`, `thumb.jpg` : fichiers d'origine de l'auteur
- Dépendances : aucune connue

## Licence d'origine

Aucune licence précisée par l'auteur. Le `README.txt` d'origine indique seulement
« Model made by Darth Biomech » (DeviantArt : http://darth-biomech.deviantart.com).
Contact avec l'auteur : pas encore tenté.

## Note : mod Workshop « legacy » (2015)

Le Workshop distribue ce mod sous forme d'une archive unique `<ugchandle>_legacy.bin`
(un zip dont les chemins utilisent des `\`). Ce dossier contient cette archive extraite
telle quelle (dossiers `data/`, `models/`, `textures/` en minuscules).

Sur un **serveur dédié**, le téléchargement de ces anciens objets échoue
(`k_EResultTimeout`) et le serveur ne démarre pas. Contournement constaté le 2026-10-29 :
copier le `_legacy.bin` du client Steam dans `content/244850/294618647/` du serveur et déclarer
l'objet dans `appworkshop_244850.acf` (sections `WorkshopItemsInstalled` et
`WorkshopItemDetails`, `manifest` à `-1`, mêmes `ugchandle`, `size` et `timeupdated` que
le client). Le serveur le considère alors « Up to date » et le charge.

## Analyse

| # | Fichier:ligne | Problème | Gravité | Confirmé ? |
|---|---------------|----------|---------|------------|
| 1 | — | Analyse pas encore faite | — | — |

## À tester en jeu

- [ ] Le monde se charge sans erreur de mod
- [ ] Les blocs apparaissent dans la barre G
- [ ] Construction / soudure / démontage
- [ ] Fonctionnement en solo
- [ ] Fonctionnement en multijoueur / serveur dédié

## Historique

Voir [CHANGELOG.md](CHANGELOG.md).