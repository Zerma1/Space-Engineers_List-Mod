# Airtight Force Field Hangar Doors

| | |
|---|---|
| **Auteur d'origine** | Hannobal |
| **Workshop d'origine** | https://steamcommunity.com/sharedfiles/filedetails/?id=901800144 |
| **Dernière mise à jour d'origine** | 2019-02-07 |
| **Dernière modification (ce dépôt)** | 2026-10-27 |
| **Statut** | `Analysé` (analyse préliminaire, à compléter) |
| **Version actuelle** | 1.0 (`metadata.mod`) |
| **Testé sur Space Engineers** | non testé |
| **Workshop republié** | non |

## Description

Portes de hangar étanches à champ de force (grands et petits blocs), empilables
sur 1 à 10 blocs de hauteur. La couleur du champ ouvert/fermé se règle via le nom
du bloc dans le terminal : `co <couleur>` (ouvert) et `cc <couleur>` (fermé).
Couleurs : `red orange yellow lime green spring cyan sky blue purple magenta pink`.

## Contenu

- `Data/` : `CubeBlocks.sbc`, `CubeBlocks - Copy.sbc`, `BlockCategories.sbc`, `Audio.sbc`,
  `TransparentMaterials_ForceFieldDoors.sbc`
- `Data/Scripts/ForceField/` : `ForceField.cs` (logique de couleur, sons, collisions) + `.csproj`/`.sln` + `obj/`
- `Models/ForceFieldDoors/` : base, portes 1 à 10, champs de couleur (Large/Small), collisions
- `Textures/` : icônes, textures émissives, particules
- Dépendances : aucune connue

## Licence d'origine

Aucune licence précisée dans les fichiers du mod. Description Workshop à vérifier.
Contact avec l'auteur : pas encore tenté.

## Analyse

Analyse statique préliminaire (lecture des fichiers, sans lancement du jeu).

| # | Fichier:ligne | Problème | Gravité | Confirmé ? |
|---|---------------|----------|---------|------------|
| 1 | `Data/CubeBlocks - Copy.sbc` | Copie quasi identique de `CubeBlocks.sbc` (seuls les `MountPoint` Front/Back diffèrent). Le jeu charge tous les `.sbc` : chaque bloc est défini deux fois et l'une des versions écrase l'autre. | majeur | confirmé (fichiers comparés) |
| 2 | `Data/Scripts/ForceField/obj/Debug/*.cs` | Fichiers générés par Visual Studio (vides) dans le dossier compilé par le jeu, plus des `.cache`. Inutiles. | mineur | confirmé |
| 3 | `Data/Scripts/ForceField/ForceField.cs:5,59,62,79,86,172` | Utilisation de types internes (`Sandbox.Game.Entities.MyEntities`, `MyAirtightDoorGeneric`, `MyEntity3DSoundEmitter`). Ils peuvent être refusés par la whitelist ModAPI actuelle → échec de compilation du script. | bloquant potentiel | à vérifier dans le log du jeu |
| 4 | `Data/Scripts/ForceField/ForceField.cs:78` | `Entity.NeedsUpdate \|= MyEntityUpdateEnum.NONE;` n'a aucun effet. | cosmétique | confirmé |
| 5 | `Data/Scripts/ForceField/ForceField.cs:79` | Variable `door` déclarée et jamais utilisée. | cosmétique | confirmé |
| 6 | `Data/Scripts/ForceField/ForceField.cs:211` | `GetObjectBuilder` renvoie le builder d'initialisation sans tenir compte de `copy`. | mineur | supposé |
| 7 | `Data/Scripts/ForceField/ForceField.cs` | Sons et rendu gérés sans test serveur dédié (`IsDedicated`). | mineur | à vérifier en dédié |
| 8 | `Data/CubeBlocks.sbc` | Aucune balise `<PCU>` ; pas de `BlockVariantGroup`. Le jeu applique des valeurs par défaut, mais les blocs peuvent être mal rangés dans la barre G. | mineur | à vérifier en jeu |
| 9 | `ForceField.csproj` | Références à des DLL qui n'existent plus (`Sandbox.Graphics`, `VRage.Native`…) et chemin Steam en dur. N'affecte que l'IDE. | cosmétique | supposé |
| 10 | `Textures/Models/tmp/` | Dossier `tmp` : vérifier si ces textures sont réellement référencées. | mineur | à vérifier |

## À tester en jeu

- [ ] Le monde se charge sans erreur de mod (log : `Compilation of` / `ForceField`)
- [ ] Les blocs apparaissent dans la barre G
- [ ] Construction / soudure / démontage
- [ ] Ouverture / fermeture, changement de couleur via `co` / `cc`
- [ ] Étanchéité (pressurisation) quand le champ est fermé
- [ ] Fonctionnement en multijoueur / serveur dédié

## Historique

Voir [CHANGELOG.md](CHANGELOG.md).
