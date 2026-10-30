# Mod Block Tabs
n> 🇬🇧 English version (reference): [README.md](README.md).

| | |
|---|---|
| **Auteur d'origine** | Zerma1 (créé dans ce dépôt) |
| **Workshop d'origine** | aucun (mod créé dans ce dépôt) |
| **Dernière mise à jour d'origine** | 2026-10-30 (création) |
| **Dernière modification (ce dépôt)** | 2026-10-30 |
| **Statut** | `À tester` |
| **Version actuelle** | 1.0 (`metadata.mod`) |
| **Testé sur Space Engineers** | non testé |
| **Workshop republié** | non |

## Description

Ajoute dans le menu de construction (touche **G**) **un onglet par mod chargé**, nommé
`[MOD] <nom du mod>`, qui liste tous les blocs de ce mod. Plus besoin de chercher les blocs
moddés dans les catégories générales : ils restent aussi à leur place d'origine.

- Les onglets sont créés automatiquement au chargement du monde, d'après les mods de la partie.
- Le nom affiché est celui du mod dans la liste des mods (`FriendlyName`), sinon son nom de dossier.
- Les onglets sont triés par ordre alphabétique avec les autres catégories.
- Côté client uniquement : sur un serveur dédié, le script ne fait rien, mais le mod peut être
  ajouté à la liste du serveur pour que tous les joueurs l'aient.

## Contenu

- `Data/Scripts/ModBlockTabs/ModBlockTabs.cs` : composant de session (`BeforeStart` → crée les
  catégories, `UnloadData` → les retire)
- Pas de `.sbc`, de modèles ni de textures
- Dépendances : aucune

## Fonctionnement

Le menu G (`MyGuiScreenToolbarConfigBase.RecreateControls`) relit
`MyDefinitionManager.Static.GetCategories()` à chaque ouverture. Le script ajoute à ce
dictionnaire une `MyGuiBlockCategoryDefinition` par mod, dont les `ItemIds` sont au format
`TypeId/SubtypeId` (comme dans `BlockCategories.sbc`). Seuls les blocs `Public` sont listés,
et le jeu applique ensuite ses filtres habituels (survie, recherche, DLC).

## Licence d'origine

Licence MIT du dépôt.

## Analyse

| # | Fichier:ligne | Problème | Gravité | Confirmé ? |
|---|---------------|----------|---------|------------|
| 1 | `ModBlockTabs.cs` | `MyGuiBlockCategoryDefinition` et `GetCategories()` (namespace `Sandbox.Definitions`) doivent être acceptés par la whitelist ModAPI | bloquant si refusé | à vérifier en jeu |
| 2 | `ModBlockTabs.cs` | Ordre d'affichage des onglets `[MOD]` parmi les catégories vanilla (tri par nom affiché) | cosmétique | à vérifier en jeu |

Vérification statique : le script compile contre les DLL du jeu (2026-10-30), ce qui ne prouve
pas qu'il passe la whitelist.

## À tester en jeu

- [ ] Le monde se charge sans erreur de mod (pas de « Prohibited type » dans le log)
- [ ] Le log contient `Mod Block Tabs: N mod tab(s) added to the G menu`
- [ ] Les onglets `[MOD] …` apparaissent dans le menu G avec les bons blocs
- [ ] Les blocs restent aussi dans leurs catégories d'origine
- [ ] Fonctionnement en solo
- [ ] Fonctionnement en multijoueur / serveur dédié
- [ ] Recharger un autre monde (avec d'autres mods) : pas d'onglet d'un mod absent

## Historique

Voir [CHANGELOG.md](CHANGELOG.md).
