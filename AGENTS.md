# AGENTS.md — Instructions pour les agents IA (et les humains pressés)

Ce dépôt rassemble des **mods Steam Workshop de Space Engineers** abandonnés ou cassés,
dans le but de les **analyser, les réparer et documenter chaque mise à jour**.
Ce fichier décrit comment travailler dessus. Lis-le en entier avant toute modification.

Langue de travail : **français** (commentaires de commit, README, changelogs).
Le code C# et les identifiants restent en anglais, comme dans les mods d'origine.

---

## 1. Organisation du dépôt

```
/
├── AGENTS.md                  ← ce fichier
├── CLAUDE.md                  ← renvoie vers AGENTS.md
├── README.md                  ← présentation du projet + liste des mods
├── CONTRIBUTING.md            ← règles de contribution
├── LICENSE                    ← licence MIT (contributions du projet)
├── NOTICE.md                  ← droits des auteurs originaux des mods
├── docs/
│   ├── GUIDE_MODDING_SE.md    ← mémo technique Space Engineers (pannes fréquentes, API, logs)
│   └── templates/
│       ├── MOD_README.md      ← modèle de README pour un mod
│       └── CHANGELOG.md       ← modèle de notes de mise à jour
└── <Nom du mod>/              ← un dossier par mod, à la racine
    ├── README.md              ← fiche du mod (origine, auteur, état, analyse)
    ├── CHANGELOG.md           ← notes de mise à jour du mod
    ├── metadata.mod
    ├── thumb.jpg
    ├── Data/                  ← .sbc + Data/Scripts/**/*.cs
    ├── Models/                ← .mwm
    └── Textures/              ← .dds
```

- **Un mod = un dossier à la racine**, nommé comme le mod d'origine (espaces autorisés).
- Le dossier d'un mod doit rester **directement copiable** dans
  `%AppData%\SpaceEngineers\Mods\` et publiable sur le Workshop tel quel.
- Tout ce qui n'est pas le mod lui-même (outils, docs générales) va dans `docs/` ou à la racine,
  jamais dans le dossier d'un mod (hors `README.md` et `CHANGELOG.md`).

## 2. Flux de travail pour un mod

Suis ces étapes dans l'ordre. Ne saute pas l'analyse.

### Étape 0 — Import
1. Le mod est ajouté dans son dossier, **tel que téléchargé** (premier commit = état d'origine, intact).
   Message : `import(<mod>): version Workshop <ID> de <auteur>`.
2. Créer `README.md` et `CHANGELOG.md` à partir de `docs/templates/`, en renseignant
   la **date de dernière mise à jour d'origine** (celle affichée sur la page Workshop).
3. Ajouter le mod au tableau de `README.md` à la racine (statut `À analyser`).

### Étape 1 — Analyse (aucune modification de fichier du mod)
Remplir la section **Analyse** du README du mod :
- Inventaire : `.sbc` présents, scripts, modèles, textures, dépendances à d'autres mods.
- Lecture de **tous** les `.sbc` et `.cs`. Vérifier en priorité les points listés dans
  `docs/GUIDE_MODDING_SE.md` § « Pannes fréquentes ».
- Chaque problème est noté avec : fichier:ligne, symptôme attendu en jeu, gravité
  (`bloquant` / `majeur` / `mineur` / `cosmétique`), et **si c'est confirmé ou supposé**.
- Ne jamais présenter une hypothèse comme un fait. Si un test en jeu est nécessaire
  pour confirmer, l'écrire explicitement (« à vérifier en jeu »).

### Étape 2 — Réparation
- **Corriger le minimum nécessaire** pour que le mod fonctionne sur la version actuelle du jeu.
  Pas de refonte, pas de nouvelles fonctionnalités, pas de rééquilibrage, sauf demande explicite.
- **Ne jamais changer un `TypeId`/`SubtypeId` existant** : cela supprime les blocs des mondes
  des joueurs. Si un renommage est indispensable, le signaler et demander avant.
- Conserver le style du code d'origine (nommage, indentation, commentaires).
- Les fichiers morts (doublons `- Copy.sbc`, `obj/`, `bin/`, `.cs` vides) sont supprimés
  **dans un commit séparé** et listés dans le changelog.
- Un correctif = un commit, préfixé par le nom court du mod :
  `fix(ForceField): retire la définition dupliquée CubeBlocks - Copy.sbc`.

### Étape 3 — Notes de mise à jour
- Ajouter une entrée dans le `CHANGELOG.md` **du mod** (format décrit dans le modèle,
  inspiré de *Keep a Changelog*). Chaque entrée indique : version, date (AAAA-MM-JJ),
  version de Space Engineers testée, liste des changements, et ce qui a été testé ou non.
- Incrémenter `<ModVersion>` dans `metadata.mod` à chaque version publiée.
- Mettre à jour le statut du mod dans le README du mod **et** dans le tableau du README racine.
- **Obligatoire** : mettre à jour le champ **« Dernière modification (ce dépôt) »** du README
  du mod **et** la colonne « Dernière modif. » du README racine, à la date du jour (AAAA-MM-JJ),
  à chaque modification du mod — même si la date figure déjà dans l'historique git.
  Les joueurs lisent les README sans consulter les commits.

### Étape 4 — Validation
Un agent ne peut généralement pas lancer le jeu. Il doit donc :
- vérifier que les `.sbc` sont du XML valide (ex. PowerShell : `[xml](Get-Content -Raw fichier.sbc)`) ;
- vérifier qu'aucun `SubtypeId` n'est défini deux fois dans le mod ;
- vérifier que chaque chemin (`<Model>`, `<Icon>`, `<File>`, textures) pointe vers un fichier existant
  (attention à la casse et aux `\`) ;
- vérifier que les scripts n'utilisent que des API de la whitelist ModAPI ;
- puis marquer le changelog `Non testé en jeu` tant qu'un humain n'a pas confirmé.

Le statut `Fonctionnel` ne doit être mis **que** après un test en jeu par un humain.

## 3. Statuts possibles d'un mod

| Statut           | Signification                                                    |
|------------------|------------------------------------------------------------------|
| `À analyser`     | Importé, rien n'a été regardé                                    |
| `Analysé`        | Problèmes identifiés dans le README du mod, aucun correctif      |
| `En réparation`  | Correctifs en cours                                              |
| `À tester`       | Correctifs faits, pas encore validés en jeu                      |
| `Fonctionnel`    | Testé en jeu sur la version indiquée                             |
| `Bloqué`         | Ne peut pas être réparé sans ressource manquante (modèle source, etc.) |

## 4. Règles importantes

- **Dates obligatoires** dans chaque README de mod : « Dernière mise à jour d'origine »
  (Workshop) et « Dernière modification (ce dépôt) », au format `AAAA-MM-JJ`. Ne jamais les
  laisser vides (écrire « inconnue » si l'information n'existe pas).
- **Droits d'auteur** : les mods appartiennent à leurs auteurs. Toujours créditer l'auteur
  d'origine et le lien Workshop dans le README du mod. Ne pas supprimer les crédits
  existants. Voir `NOTICE.md`.
- **Fichiers binaires** (`.mwm`, `.dds`, `.jpg`) : ne pas les modifier ni les régénérer
  sans demande explicite. Un agent ne peut pas les inspecter : signaler seulement
  si un fichier référencé est absent ou inutilisé.
- **Pas de secrets** : pas de chemins personnels, d'identifiants Steam, etc. dans les fichiers.
- **Pas de push / publication Workshop** sans demande explicite de l'utilisateur.
- Les fichiers `.csproj`/`.sln` servent uniquement à l'autocomplétion dans l'IDE :
  le jeu compile lui-même les `.cs` de `Data/Scripts`. Ne pas considérer une compilation
  `msbuild` réussie comme une preuve que le mod fonctionne.

## 5. Fin de tâche

Avant de rendre la main, résumer :
1. les problèmes trouvés (confirmés / supposés),
2. les fichiers modifiés,
3. ce qui reste à tester en jeu.
