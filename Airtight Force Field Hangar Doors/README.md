# Airtight Force Field Hangar Doors

| | |
|---|---|
| **Auteur d'origine** | Hannobal |
| **Workshop d'origine** | https://steamcommunity.com/sharedfiles/filedetails/?id=901800144 |
| **Dernière mise à jour d'origine** | 2019-02-07 |
| **Dernière modification (ce dépôt)** | 2026-10-27 |
| **Statut** | `À tester` (correctifs faits, non validés en jeu) |
| **Version actuelle** | 1.0 (`metadata.mod`) |
| **Testé sur Space Engineers** | non testé |
| **Workshop republié** | non |

## Description

Portes de hangar étanches à champ de force (grands et petits blocs), empilables
sur 1 à 10 blocs de hauteur. La couleur du champ ouvert/fermé se règle via le nom
du bloc dans le terminal : `co <couleur>` (ouvert) et `cc <couleur>` (fermé).
Couleurs : `red orange yellow lime green spring cyan sky blue purple magenta pink`.

## Contenu

- `Data/` : `CubeBlocks.sbc` (22 blocs : 11 grands, 11 petits), `BlockCategories.sbc`, `Audio.sbc`
  (sons personnalisés commentés, le script utilise les sons vanilla `BlockProjectorOn/Off`),
  `TransparentMaterials_ForceFieldDoors.sbc` (un matériau `H_ForceField_<couleur>` par couleur)
- `Data/Scripts/ForceField/` : `ForceField.cs` (logique de couleur, sons, collisions) + `.csproj`/`.sln` (IDE uniquement)
- `Models/ForceFieldDoors/` : base, portes 1 à 10, champs de couleur (Large/Small), collisions
- `Textures/` : icônes, textures émissives, particules
- Dépendances : aucune connue

## Licence d'origine

Aucune licence précisée dans les fichiers du mod. Description Workshop à vérifier.
Contact avec l'auteur : pas encore tenté.

## Analyse

Analyse statique (lecture des fichiers et des chemins dans les `.mwm`, sans lancement du jeu :
aucune installation de Space Engineers sur le poste d'analyse).

| # | Fichier:ligne | Problème | Gravité | Confirmé ? | État |
|---|---------------|----------|---------|------------|------|
| 1 | `Data/CubeBlocks - Copy.sbc` | Copie quasi identique de `CubeBlocks.sbc` (seuls 40 `MountPoint` Front/Back diffèrent). Le jeu charge tous les `.sbc` : les 22 blocs étaient définis deux fois. | majeur | confirmé | **corrigé** (fichier supprimé, `CubeBlocks.sbc` conservé) |
| 2 | `Data/Scripts/ForceField/obj/Debug/*` | Fichiers générés par Visual Studio (`.cs` vides, `.cache`) dans le dossier compilé par le jeu. | mineur | confirmé | **corrigé** (supprimés) |
| 3 | `ForceField.cs` (ex-lignes 59, 62, 79, 86, 172) | Utilisation des types internes `MyEntities` et `MyAirtightDoorGeneric`, qui risquent d'être refusés par la whitelist ModAPI actuelle → échec de compilation du script. | bloquant potentiel | supposé | **corrigé** (remplacés par `MyEntity` et `IMyDoor.OpenRatio`) |
| 4 | `ForceField.cs` (ex-ligne 78) | `Entity.NeedsUpdate \|= MyEntityUpdateEnum.NONE;` n'a aucun effet. | cosmétique | confirmé | **corrigé** |
| 5 | `ForceField.cs` (ex-ligne 79) | Variable `door` déclarée et jamais utilisée. | cosmétique | confirmé | **corrigé** |
| 6 | `ForceField.cs:80` | `UpdateAfterSimulation10` sans test `null` : une exception dans un `Update` fait planter le jeu. | mineur | supposé | **corrigé** (garde ajoutée) |
| 7 | `ForceField.cs:210` | `GetObjectBuilder` renvoie le builder d'initialisation sans tenir compte de `copy`. | mineur | supposé | non modifié (pas de preuve de bug) |
| 8 | `ForceField.cs` | Sons et rendu gérés sans test serveur dédié (`IsDedicated`). | mineur | à vérifier en dédié | non modifié |
| 9 | `ForceField.cs:3-18` | `using` de namespaces peut-être hors whitelist (`VRageRender`, `Sandbox.Definitions`…). Le type `MyObjectBuilder_AirtightHangarDoor` et le reste n'ont pas pu être vérifiés sans les DLL du jeu. | inconnu | à vérifier dans le log du jeu | non modifié |
| 10 | `Data/CubeBlocks.sbc` | Aucune balise `<PCU>`, pas de `BlockVariantGroup`, `DamageEffectId` numérique (ancien format). Le jeu applique des valeurs par défaut. | mineur | à vérifier en jeu | non modifié |
| 11 | Modèles `ForceFieldBlock_*.mwm` (242) | Référencent `Textures\Models\H_ForceField_{add,cm,ng}.dds`, absentes à cet emplacement (elles sont dans `Textures\Models\tmp\`). Le rendu passe probablement par les matériaux transparents `H_ForceField_<couleur>` (texture présente), donc sans effet visible. | mineur | à vérifier en jeu | non modifié. Si textures roses/damier : copier `tmp\H_ForceField_*.dds` dans `Textures\Models\` |
| 12 | `ForceField.csproj` | Références à des DLL qui n'existent plus (`Sandbox.Graphics`, `VRage.Native`…) et chemin Steam en dur. N'affecte que l'IDE. | cosmétique | supposé | non modifié |

Vérifications faites : XML valide pour les 4 `.sbc`, aucun `SubtypeId` en double, tous les
chemins `Models\…` et `Textures\…` des `.sbc` existent.

## À tester en jeu

- [ ] Le monde se charge sans erreur de mod (log : `Compilation of` / `ForceField`)
- [ ] Les blocs apparaissent dans la barre G
- [ ] Construction / soudure / démontage
- [ ] Ouverture / fermeture, changement de couleur via `co` / `cc`
- [ ] Étanchéité (pressurisation) quand le champ est fermé
- [ ] Fonctionnement en multijoueur / serveur dédié

## Historique

Voir [CHANGELOG.md](CHANGELOG.md).
