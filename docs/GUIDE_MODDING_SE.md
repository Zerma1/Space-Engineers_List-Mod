# Guide technique — Réparer un mod Space Engineers

Mémo pour analyser et réparer les mods de ce dépôt. À enrichir à chaque nouveau
type de panne rencontré.

## 1. Anatomie d'un mod

| Élément              | Rôle                                                                 |
|----------------------|----------------------------------------------------------------------|
| `metadata.mod`       | Version du mod (`<ModVersion>`)                                      |
| `thumb.jpg`          | Vignette Workshop                                                    |
| `Data/*.sbc`         | Définitions XML (blocs, composants, sons, matériaux, catégories…). **Tous** les `.sbc` de `Data/` sont chargés, quel que soit leur nom. |
| `Data/Scripts/<Dossier>/**/*.cs` | Scripts ModAPI, **compilés par le jeu** au chargement. Tous les `.cs` du dossier sont compilés, y compris ceux de `obj/`. |
| `Models/*.mwm`       | Modèles 3D (binaire, exportés via SE Mod SDK / Blender SEUT)         |
| `Textures/*.dds`     | Textures et icônes                                                   |
| `Audio/*.wav/.xwm`   | Sons                                                                 |

## 2. Où lire les erreurs

- Client : `%AppData%\SpaceEngineers\SpaceEngineers.log` (chercher `Compilation of`, `Error`, `Exception`, le nom du mod).
- Serveur dédié : `%AppData%\SpaceEngineersDedicated\` ou le dossier d'instance.
- En jeu : les erreurs de compilation de scripts sont listées dans le menu
  « Mods » / message au chargement du monde.
- Pour tester localement : copier le dossier du mod dans `%AppData%\SpaceEngineers\Mods\`,
  créer un monde **créatif hors-ligne**, ajouter le mod local.

## 3. Pannes fréquentes (checklist d'analyse)

### Définitions `.sbc`
- [ ] **Définitions en double** : un même `TypeId`/`SubtypeId` défini dans deux fichiers
      (souvent un `... - Copy.sbc` oublié). Le dernier chargé écrase l'autre → comportement imprévisible.
- [ ] **Chemins invalides** : `<Model>`, `<Icon>`, `BuildProgressModels`, textures pointant vers un fichier absent.
- [ ] **XML invalide** ou balise obsolète supprimée par Keen (le log l'indique).
- [ ] **Champs devenus obligatoires** dans les versions récentes : `<PCU>`, `<BlockVariants>`/`<BlockVariantGroups>` (catégories), `<DLC>` le cas échéant.
- [ ] **Sons / effets / matériaux** référencés mais qui n'existent plus dans le jeu de base (renommés par Keen).
- [ ] **Catégories** (`BlockCategories.sbc`) : depuis la refonte de la barre G, les blocs non
      listés dans une `BlockVariantGroup` peuvent être difficiles à trouver.
- [ ] `xsi:type` ne correspondant pas au `TypeId` (ex. `MyObjectBuilder_AirtightHangarDoorDefinition` ↔ `AirtightHangarDoor`).

### Scripts C# (ModAPI)
- [ ] **Whitelist** : seuls certains namespaces/types sont autorisés (`Sandbox.ModAPI`, `VRage.Game.ModAPI`,
      `VRageMath`, etc.). Une classe interne (`Sandbox.Game.Entities.*`, `MyEntities`…) utilisée
      autrefois peut avoir été retirée de la whitelist → erreur de compilation « Prohibited type ».
- [ ] **Version C#** : le compilateur du jeu accepte C# 6 (pas de fonctionnalités plus récentes
      comme les `switch` par motif avancés, `record`, etc.). À vérifier selon la version du jeu.
- [ ] **API renommées / supprimées** : signatures d'événements, propriétés déplacées
      (`IMyTerminalBlock`, `IMyFunctionalBlock`, `IMyDoor`…).
- [ ] **`GetObjectBuilder`** : retourner `null` ou un builder copié incorrectement peut corrompre la sauvegarde.
- [ ] **`NeedsUpdate`** : `|= NONE` n'a aucun effet ; pour désactiver une mise à jour il faut `&= ~flag` ou `=`.
- [ ] **Multijoueur / serveur dédié** : code client (sons, rendu, notifications) exécuté côté serveur
      sans vérifier `MyAPIGateway.Utilities.IsDedicated` ou `MyAPIGateway.Session.Player`.
- [ ] **Abonnements d'événements** non désabonnés dans `Close()` → fuites / exceptions.
- [ ] **Exceptions non gérées** dans `Update*` : font crasher le jeu → entourer de `try/catch` avec log
      via `MyLog.Default.WriteLineAndConsole`.
- [ ] **Fichiers parasites** : `obj/`, `bin/`, `TemporaryGeneratedFile_*.cs` dans `Data/Scripts` — compilés par le jeu, à supprimer.

### Fichiers projet (.csproj)
Les `.csproj` d'origine référencent souvent des DLL disparues (`Sandbox.Graphics.dll`,
`VRage.Native.dll`…) et un chemin Steam en dur. Ils ne servent qu'à l'IDE.
Si on les met à jour : utiliser une variable (ex. `$(SpaceEngineersBin)`) plutôt qu'un chemin absolu.

## 4. Ressources

- Documentation ModAPI officielle : https://keensoftwarehouse.github.io/SpaceEngineersModAPI/
- Wiki modding Keen : https://spaceengineers.wiki.gg/wiki/Modding
- Code source de référence (définitions vanilla) : dossier `Content/Data` de l'installation du jeu.
- Outil de modèles : SEUT (Space Engineers Utilities pour Blender).
- Communauté : serveur Discord « Keen Software House », canal modding.

> Les liens ci-dessus peuvent évoluer : en cas de lien mort, le mettre à jour dans une PR.
