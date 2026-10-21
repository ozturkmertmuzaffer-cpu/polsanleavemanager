# Git

Uzak repo: **GitHub**. Ekip: 5 geliştirici. Varsayılan branch: `main`.

## Branching
- `feature/<spec-no>-<short-name>` — **no branch without a spec.** Also for CHANGE-lane mini-specs.
- Refactors: `refactor/<spec-no>-<short-name>` (mini-spec with Changed behavior = none).
- Fixes: `fix/<report-id>-<short-name>` (bug fixes carry a report, not a spec number);
  incidents: `incident/<date>-<short-name>`; trivial changes: `trivial/<short-name>` (see below).

## Commits
- Conventional Commits, with a plan reference: `feat(catalog): paging endpoint [plan 0001/3]`.
- Agent commits follow the same standard: the agent writes the message, the human approves.

## Forbidden
- Direct commits to the default branch.
- Force push, history rewriting on shared branches. Undo = `git revert` (see recovery R-11).

## Trivial changes
Yalnızca change-request triage rubric'inin (`workflows/change-request.md`) TRIVIAL olarak
sınıfladığı işler için: kabul kriteri değişmez, paylaşılan kod etkilenmez.
- Policy: **(1) PR + one reviewer, no spec** — bootstrap'ta seçildi (2026-10-21). Branch:
  `trivial/<short-name>`. Doğrudan commit (2) ve zorunlu mini-spec (3) seçilmedi.
- Always: one narrow commit, `scripts/check` green. A second file or a new test means re-triage.

## Pull requests
- PR template checklist completed; `scripts/check` green in CI; squash-merge.
- Her PR en az **1 onay** ister; yazar kendi PR'ını onaylayamaz.
- `main` branch protection (GitHub ayarı, repo yöneticisi açar): PR zorunlu, `check` ve `doctor`
  job'ları zorunlu status check, 1 onay, force push ve silme kapalı.
