# Contribuer

> 🇬🇧 Version anglaise : [CONTRIBUTING.md](CONTRIBUTING.md). C'est la version de référence ; celle-ci peut être en retard.

Merci de vouloir aider à garder ces mods en vie ! Le processus complet est décrit
dans [AGENTS.md](AGENTS.md) ; il s'applique aussi bien aux humains qu'aux agents IA.

## En bref

1. **Forkez** le dépôt et créez une branche : `fix/<nom-du-mod>-<sujet>` ou `import/<nom-du-mod>`.
2. **Importer un nouveau mod** : premier commit = le mod tel que téléchargé, sans aucune
   modification. Ajoutez ensuite `README.md` et `CHANGELOG.md` depuis
   [`docs/templates/`](docs/templates/) et une ligne dans le tableau du [README](README.fr.md).
3. **Réparer un mod** : commencez par remplir la section « Analyse » du README du mod,
   puis faites des correctifs **minimaux**, un sujet par commit.
4. **Documentez** chaque changement dans le `CHANGELOG.md` du mod, et mettez à jour
   **la date « Dernière modification (ce dépôt) »** dans le README du mod et dans le
   tableau du README racine. C'est obligatoire, même si git garde déjà la date.
5. Ouvrez une **Pull Request** en indiquant :
   - la version de Space Engineers utilisée,
   - ce qui a été testé en jeu (solo, serveur dédié, multijoueur),
   - ce qui ne l'a pas été.

## Signaler un mod cassé

Ouvrez une *issue* avec : nom du mod, lien Workshop, symptôme, extrait du log
`%AppData%\SpaceEngineers\SpaceEngineers.log` (ou du log du serveur dédié).

## Règles

- Ne jamais modifier un `TypeId` / `SubtypeId` existant (casse les mondes des joueurs).
- Toujours conserver les crédits de l'auteur d'origine (voir [NOTICE.md](NOTICE.md)).
- En contribuant, vous acceptez que votre contribution soit publiée sous [licence MIT](LICENSE).

## Format des commits

```
<type>(<mod>): <description courte en français>
```

Types : `import`, `fix`, `chore` (nettoyage), `docs`, `feat` (seulement si demandé).
Exemple : `chore(ForceField): supprime le doublon CubeBlocks - Copy.sbc`.
