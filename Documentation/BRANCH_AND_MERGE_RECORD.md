# Neon Rift 3D branch and merge record

## Repository and provenance

Local repository: `NeonRift3D-Survival-Week8`.

Intended GitHub owner: `yekaixin113`, verified in GitHub Desktop Accounts.

Intended private repository name: `NeonRift3D-Survival-Week8`.

Publication status: pending explicit confirmation after automatic approval review blocked the upload. No remote repository URL is claimed yet.

This history was reconstructed on 11 October 2026 from the saved original-file backup, the initial implementation script, the annotated report and the delivered fixed project. Git records actual reconstruction and merges performed today. It does not assert earlier commits or separate authorship by two people. Member A and B label the responsibilities proposed in the plan; the current Git identity authored these commits.

## Branch responsibilities

There are **5 retained branches**, including `main`, and **4 explicit merge commits**.

| Branch | Planning role | Content |
| --- | --- | --- |
| `main` | Combined deliverable | Final improved game and project documentation |
| `feature/survival-spawn` | Member A | 60-second clock, six pressure tiers, caps and safe placement; initial boundary scheduling |
| `feature/survival-hud` | Member B | Countdown/status HUD, displayed results and R-to-restart |
| `fix/terminal-state` | Member B integration/debug role | Death priority, frozen actors/projectiles and blocked post-result shots |
| `fix/enemy-persistence` | Member A spawn/debug role | Continuous spawn schedule and survivor instance/HP preservation |

## Actual merge order

1. Merge Member A spawning into `main` using `git merge --no-ff`.
2. Rebase the HUD branch onto the merged `main` before committing HUD integration in shared `Game3D.cs`; then merge it using `--no-ff`.
3. Branch from the combined game to isolate terminal-state hardening and merge it with `--no-ff`.
4. Branch from the hardened game to isolate enemy persistence and merge it with `--no-ff`.

The HUD rebase updated a branch that had no feature commits yet. It established the current spawning code as the starting point before shared-file changes. All four merges used Git's `ort` strategy, completed without conflicts and preserved both parents. No pull requests or teammate reviews were performed or claimed. Source branches are retained for inspection.

| Source branch | Feature commit | Merge commit | Target |
| --- | --- | --- | --- |
| `feature/survival-spawn` | `7bb9979` | `8f11d4a` | `main` |
| `feature/survival-hud` | `db0fcdb` | `a760b03` | `main` |
| `fix/terminal-state` | `6458a37` | `92e0389` | `main` |
| `fix/enemy-persistence` | `2862653` | `76e007d` | `main` |

## Retained version tags

| Tag | Version behaviour |
| --- | --- |
| `v0.1-original` | Original ten-kill objective; original SDK reference |
| `v0.2-survival-core` | Console-only survival clock and bounded spawning; SDK aligned to 4.7.0 |
| `v0.3-survival-hud` | Runtime countdown/status HUD and R restart |
| `v0.4-terminal-safe` | Stable death/win state and frozen post-result combat |
| `v1.0-survival-fixed` | Final smooth phase scheduling and persistent living enemies |

Earlier tags intentionally retain intermediate behaviour for comparison. Run `main` or `v1.0-survival-fixed` to use the final game. The original tag preserves the original unresolved SDK reference and may require environment alignment to build.

## Validation

All four feature/debug stages built with zero errors under Godot.NET.Sdk 4.7.0 and net8.0. The final isolated Godot 4.7 .NET run on 11 October 2026 passed **344 assertions**, including no forced boundary batches, survivor identity and health, caps, safe placement, no ten-kill victory, 60-second victory, prior-death defeat, terminal freezing, late-shot rejection and R restart. Many assertions repeat checks across entities; they are not 344 independent gameplay scenarios.

Before adding repository-only documentation, the final source/configuration/README/architecture matched the previously delivered fixed game after line-ending normalisation. Five combat classes and all scenes were byte-identical. The later README change only links collaboration documentation. Human balance/HUD playtesting remains outstanding.

## Commit graph

```text
*   76e007d (HEAD -> main, tag: v1.0-survival-fixed) Merge continuous spawning and enemy persistence fix into main
|\  
| * 2862653 (fix/enemy-persistence) Preserve spawn schedule and living enemies across phase changes
|/  
*   92e0389 (tag: v0.4-terminal-safe) Merge terminal-state hardening into main
|\  
| * 6458a37 (fix/terminal-state) Harden survival death priority and terminal state
|/  
*   a760b03 (tag: v0.3-survival-hud) Merge Member B countdown HUD and restart into main
|\  
| * db0fcdb (feature/survival-hud) Add Member B countdown HUD and round restart
|/  
*   8f11d4a (tag: v0.2-survival-core) Merge Member A survival spawning into main
|\  
| * 7bb9979 (feature/survival-spawn) Add Member A survival clock and bounded enemy spawning
|/  
* 9fd98a9 Align Godot SDK with the local 4.7.0 build environment
* ed25432 (tag: v0.1-original) Restore original Week 8 game baseline
* bcc205a Initial commit
```

This graph is a snapshot immediately after the four merges. A later documentation commit attaches this record and the translated report to `main`.
