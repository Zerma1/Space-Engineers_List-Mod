# Changelog — Airtight Force Field Hangar Doors

Format inspiré de [Keep a Changelog](https://keepachangelog.com/fr/1.1.0/).
Catégories : **Corrigé**, **Modifié**, **Supprimé**, **Ajouté**.

## [Non publié] — 2026-10-27

### Corrigé
- `ForceField.cs` : remplace les types internes du jeu `MyEntities` et `MyAirtightDoorGeneric`
  par `MyEntity` et `IMyDoor.OpenRatio`, pour éviter un refus du script par la whitelist ModAPI.
  Comportement identique attendu.
- `ForceField.cs` : ajoute une garde `null` dans `UpdateAfterSimulation10` pour éviter un plantage.

### Supprimé
- `Data/CubeBlocks - Copy.sbc` : doublon qui définissait les 22 blocs une seconde fois.
  ⚠️ La copie autorisait le montage sur toute la hauteur des faces avant/arrière ; la version
  conservée (`CubeBlocks.sbc`) ne l'autorise que sur le bloc du haut, comme pour les faces gauche/droite.
  Si des constructions existantes ne se raccordent plus, c'est ce point à revoir.
- `Data/Scripts/ForceField/obj/` : fichiers générés par Visual Studio.
- `ForceField.cs` : ligne sans effet `NeedsUpdate |= NONE` et une variable inutilisée.

### Ajouté
- `README.md` et `CHANGELOG.md` du mod.

**Testé sur :** non testé en jeu. Vérifications statiques uniquement (XML valide, aucun doublon d'ID,
chemins des `.sbc` présents).

## [1.0] — 2026-10-27

Version d'origine importée depuis le Workshop (901800144), auteur Hannobal. Aucune modification.
