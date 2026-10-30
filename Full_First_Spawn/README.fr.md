# Full First Spawn
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

À la **première apparition** de chaque joueur dans le monde, remplit au maximum :
vie, faim (nourriture), énergie de la combinaison, O² et H².

- Une seule fois par identité : une réapparition après une mort n'est pas remplie à nouveau.
- La liste des identités déjà servies est enregistrée dans le monde (variable de session
  `Zerma_FullFirstSpawn_Identities`), donc conservée après un redémarrage du serveur.
- Les joueurs déjà apparus avant l'ajout du mod sont traités à leur prochaine apparition.
- Le script tourne sur le serveur (ou chez l'hôte en solo) ; le jeu synchronise les valeurs.

## Contenu

- `Data/Scripts/FullFirstSpawn/FullFirstSpawn.cs` : composant de session
- Pas de `.sbc`, de modèles ni de textures
- Dépendances : aucune

## Fonctionnement

- Événement `MyVisualScriptLogicProvider.PlayerSpawned`, puis attente d'environ 2 s (120 ticks)
  pour passer après les valeurs posées par le jeu à l'apparition.
- Énergie, O² et H² : `SetPlayersEnergyLevel` / `SetPlayersOxygenLevel` / `SetPlayersHydrogenLevel`
  avec la valeur `1` (ratio 0..1 ; vérifié dans le code du jeu : l'énergie est multipliée par la
  capacité de la batterie de la combinaison).
- Vie et faim : statistiques `Health` et `Food` du `MyEntityStatComponent` du personnage, mises à leur maximum.

## Licence d'origine

Licence MIT du dépôt.

## Analyse

| # | Fichier:ligne | Problème | Gravité | Confirmé ? |
|---|---------------|----------|---------|------------|
| 1 | `FullFirstSpawn.cs` | Les types utilisés (`MyVisualScriptLogicProvider`, `MyEntityStatComponent`, `MyEntityStat`) doivent être acceptés par la whitelist ModAPI | bloquant si refusé | à vérifier en jeu |
| 2 | `FullFirstSpawn.cs` | Le délai de 2 s doit suffire pour passer après les valeurs d'apparition (salle médicale, kit de survie) | majeur | à vérifier en jeu |

Vérification statique : le script compile contre les DLL du jeu (2026-10-30), ce qui ne prouve
pas qu'il passe la whitelist.

## À tester en jeu

- [ ] Le monde se charge sans erreur de mod
- [ ] Nouveau joueur : vie, faim, énergie, O², H² à 100 % ~2 s après l'apparition
- [ ] Le log contient `Full First Spawn: stats filled for identity …`
- [ ] Mort puis réapparition : pas de nouveau remplissage
- [ ] Redémarrage du serveur : un joueur déjà servi n'est pas rempli à nouveau
- [ ] Fonctionnement en solo
- [ ] Fonctionnement en multijoueur / serveur dédié

## Historique

Voir [CHANGELOG.md](CHANGELOG.md).
