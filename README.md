# Space Engineers — Mods abandonnés, réparés par la communauté

Ce dépôt rassemble des mods du **Steam Workshop de Space Engineers** qui ne
fonctionnent plus ou ne sont plus mis à jour par leur auteur. Le but :

1. **Analyser** chaque mod et documenter pourquoi il est cassé ;
2. **Le réparer** avec des correctifs minimaux, sans casser les mondes existants ;
3. **Tracer chaque mise à jour** dans les notes de version du mod ;
4. Permettre à **la communauté de continuer** à les entretenir.

## Mods

| Mod | Auteur d'origine | Workshop | MAJ d'origine | Dernière modif. | Statut | Testé sur |
|-----|------------------|----------|---------------|-----------------|--------|-----------|
| [Airtight Force Field Hangar Doors](Airtight%20Force%20Field%20Hangar%20Doors/) | Hannobal | [901800144](https://steamcommunity.com/sharedfiles/filedetails/?id=901800144) | 2019-02-07 | 2026-10-27 | `À tester` | — |
| [(DX11)Armor Thrusters](%28DX11%29Armor%20Thrusters/) | Darth Biomech | [295393216](https://steamcommunity.com/sharedfiles/filedetails/?id=295393216) | 2015-12-10 | 2026-10-30 | `Fonctionnel` | 1.210.014 (solo) |
| [(DX11)Cryopods](%28DX11%29Cryopods/) | Darth Biomech | [294618647](https://steamcommunity.com/sharedfiles/filedetails/?id=294618647) | 2015-12-17 | 2026-10-30 | `À analyser` | — |
| [(DX11)Docking Ring](%28DX11%29Docking%20Ring/) | Darth Biomech | [294619787](https://steamcommunity.com/sharedfiles/filedetails/?id=294619787) | 2015-10-02 | 2026-10-30 | `À analyser` | — |

Statuts : `À analyser` → `Analysé` → `En réparation` → `À tester` → `Fonctionnel` (ou `Bloqué`).
Le détail de chaque mod (analyse, problèmes, tests) est dans son `README.md`,
l'historique dans son `CHANGELOG.md`.

## Installer un mod depuis ce dépôt

1. Téléchargez le dépôt (bouton **Code → Download ZIP**) ou clonez-le.
2. Copiez le dossier du mod voulu dans `%AppData%\SpaceEngineers\Mods\`.
3. Dans le jeu : **Charger une partie → Modifier les paramètres → Mods**, puis
   ajoutez le mod (onglet local).

> ⚠️ Faites une sauvegarde de votre monde avant de remplacer la version Workshop
> d'un mod par une version de ce dépôt.

## Contribuer

Toute aide est bienvenue : signaler un mod cassé, proposer un correctif, tester en jeu.

- [CONTRIBUTING.md](CONTRIBUTING.md) — comment proposer une modification
- [AGENTS.md](AGENTS.md) — processus détaillé (analyse → réparation → changelog), valable aussi pour les agents IA
- [docs/GUIDE_MODDING_SE.md](docs/GUIDE_MODDING_SE.md) — pannes fréquentes et où lire les erreurs
- [docs/templates/](docs/templates/) — modèles de README et de CHANGELOG pour un nouveau mod

## Licence

- Le travail réalisé dans ce dépôt (correctifs, documentation, outils) est sous
  **[licence MIT](LICENSE)** : chacun peut le réutiliser, le modifier et le republier
  librement en gardant la mention de copyright.
- Les **mods d'origine restent la propriété de leurs auteurs**, crédités dans
  chaque dossier. Voir [NOTICE.md](NOTICE.md) avant toute republication sur le Workshop.

Space Engineers est une marque de Keen Software House. Projet non affilié à Keen Software House.
