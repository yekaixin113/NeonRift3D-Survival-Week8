# Neon Rift 3D Survival Mode Project Plan

Status: as-built technical plan. Enter both member names and the GitHub repository URL before submission.

## 1. Game idea

Convert the original kill-target game into a 60-second survival round. Enemies continue spawning through six ten-second pressure tiers. The player wins by remaining alive until the HUD timer reaches zero and loses if health reaches zero first. Kills continue to add score but do not end the round.

Visible changes include an on-screen countdown, health, pressure tier, living-enemy count, kills, score, final result, and R-to-restart instruction. Enemy spawning becomes time-based and increasingly intense while remaining bounded by living-enemy and ranged-enemy caps.

## 2. Pair responsibilities

| Team member | Responsibility | Main files | Retained feature branch |
| --- | --- | --- | --- |
| Member A — enter name | Enemy spawning, six-tier difficulty, safety caps and enemy-persistence fix | `Scripts3D/SurvivalRules.cs`, spawn-related sections of `Scripts3D/Game3D.cs` | `feature/survival-spawn` |
| Member B — enter name | Countdown, HUD, end-state/restart behavior and verification | `Scripts3D/SurvivalHud.cs`, integration sections of `Scripts3D/Game3D.cs` | `feature/survival-hud` |

Both members may need to edit `Game3D.cs`, `README.md`, and `ARCHITECTURE.md`. To reduce conflict, Member A should merge the spawn/rules branch first. Member B should then rebase the HUD branch, integrate `Game3D.cs`, run the combined tests, and request review before merging.

## 3. Combined behavior

The final game must:

- start a 60-second survival timer;
- continuously attempt enemy spawns without clearing living enemies;
- increase pressure every ten seconds through bounded intervals, batch sizes and caps;
- keep existing enemy combat classes and scene files unchanged;
- display timer, HP, pressure, enemies, kills and score in the HUD;
- award victory only when a living player reaches 60 seconds;
- award defeat when the player dies before the timer ends;
- freeze actors and projectiles after a result; and
- reload a clean round when R is pressed after victory or defeat.

## 4. Test plan

| Test | Expected result | Owner |
| --- | --- | --- |
| Timer and victory boundary | Timer starts at 60.0 and reaches 0.0; a living player wins; no spawn occurs at or after the boundary | Member B; Member A reviews |
| Early defeat | Health reaching zero before the timer ends produces defeat and cannot later become victory | Member B |
| Kill-rule regression | Ten kills add score but do not end Survival Mode | Member B |
| Pressure progression | Six time tiers use the documented intervals, batch sizes, total caps and Striker caps | Member A |
| Spawn fairness | New enemies are at least 5 units from the player and 1.4 units from living enemies; failed positions are skipped | Member A |
| Enemy persistence | Killing one enemy or crossing a phase boundary does not recreate or delete surviving enemies; their instance and health remain | Member A; Member B reviews |
| End-state stability | Existing actors/projectiles stop processing and no late projectile is created after the result | Member B |
| Restart | R reloads the game with 100 HP, zero kills/score and a fresh 60-second timer | Member B |
| Build/regression | Project builds with zero errors; movement, aiming, shooting, contact damage and both enemy types still work | Both members |

## 5. GitHub plan

Repository URL: pending publication approval for `yekaixin113/NeonRift3D-Survival-Week8` (private).

Actual branch count: **5**, including `main`.

Actual merge method: four `git merge --no-ff` merges into `main`, preserving two parents. The HUD branch was rebased onto merged spawning work before shared-file integration. All merges completed without conflicts. See `BRANCH_AND_MERGE_RECORD.md` for commit IDs and version tags.

The Git history was reconstructed on 11 October 2026 from the earlier code and backup. Member labels describe planned responsibilities, not evidence of separate authors. No pull requests or teammate reviews were performed. Team names and acceptance of the proposed division still require confirmation by the group.
