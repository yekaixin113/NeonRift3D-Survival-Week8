# Neon Rift 3D Survival Mode Development and Debug Log

## Starting behavior

The original `Game3D` spawned the first enemy after 1.5 seconds and then one enemy every 2.5 seconds. Chasers appeared for the first four kills, followed by Strikers. The player won after ten kills and lost on zero health. Status was printed to the Godot output panel.

## Implemented change

The game now uses a 60-second survival objective. `SurvivalRules` owns the pure C# clock and six-tier difficulty table. `Game3D` coordinates spawning, score, victory/defeat, safe placement and finalization. `SurvivalHud` only displays state.

The six time tiers raise pressure through spawn interval, batch size and living-enemy caps. Safety boundaries limit the arena to at most 16 living enemies and 4 living Strikers, keep new enemies at least 5 units from the player and 1.4 units from living enemies, and discard blocked spawn slots instead of forcing an unfair position.

Existing `Player3D`, `Enemy3D`, `ChaserEnemy3D`, `StrikerEnemy3D`, `Bullet3D` and all `.tscn` scene files remain unchanged.

## Debug record 1 — terminal-state and boundary checks

Potential failures included ten kills still triggering victory, an enemy projectile appearing after the result, the timer spawning at 60 seconds, and death being overwritten by victory. These were addressed by:

- removing kill-count victory from `OnEnemyDestroyed`;
- checking player death before advancing the survival clock;
- ending the round before spawning when the clock reaches 60 seconds;
- routing victory and defeat through one `FinishGame` method;
- disabling `Entities` processing at the result; and
- rejecting projectile requests when the game is no longer running.

Tests confirmed that ten kills only affect score, 60 seconds produces victory for a living player, prior death remains defeat, result state cannot be overwritten, actors/projectiles freeze, and R reloads a clean scene.

## Debug record 2 — visible replacement-wave effect

Observed symptom: after one enemy was killed near a ten-second boundary, the remaining group appeared to disappear and a larger group appeared. Code inspection showed that only the killed enemy called `QueueFree`; living enemies were not explicitly cleared. The abrupt effect came from resetting the next-spawn time to the current time whenever a phase changed, which forced an immediate batch at every ten-second boundary. The Striker sequence was also reset at each boundary.

Fix:

- removed the phase-boundary assignment that forced `_nextSpawn` to the current time;
- removed `_lastPhase` and the phase-boundary reset of `_phaseSpawnCount`;
- retained the already scheduled next spawn; and
- applied the new phase interval after that natural spawn.

Result: a phase change only changes later spawn parameters. Living enemies keep the same instance, parent, health and AI state and continue moving from their current positions. Tests explicitly confirmed survivor instance and health preservation.

## Verification evidence

- Modified project build: zero errors.
- Godot 4.7 .NET headless integration suite: 344 passing assertions.
- Covered opening delay, phase boundaries, no immediate replacement wave, long-frame limits, scoring, enemy/Striker caps, safe distances, 60-second victory, early defeat, terminal freezing, restart, and surviving-enemy persistence.
- Limitation: automated checks do not replace a human playtest for win rate, aiming feel, HUD readability or final-tier balance.

## Files changed

| File | Change |
| --- | --- |
| `Scripts3D/Game3D.cs` | Replaced kill victory with survival coordination, bounded spawning, result handling, HUD integration and persistence behavior |
| `Scripts3D/SurvivalRules.cs` | Added pure C# survival clock and six-tier difficulty schedule |
| `Scripts3D/SurvivalHud.cs` | Added runtime HUD for countdown and status |
| `NeonRiftStage1.csproj` | Aligned the SDK reference with the installed/cached Godot 4.7.0 SDK while retaining `net8.0` |
| `README.md` | Updated game rules, controls, balance table and persistence semantics |
| `ARCHITECTURE.md` | Updated responsibilities and call-flow documentation |

## Git organisation on 11 October 2026

The previous source iterations were organised into five retained branches and four explicit merges. The final isolated integration test was rerun and passed 344 assertions. See `BRANCH_AND_MERGE_RECORD.md` for the reconstruction provenance, feature boundaries, actual merge IDs and version tags.
