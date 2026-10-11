# Neon Rift 3D — Week 8 Collaboration Demo

A Godot 4 **.NET/C#** teaching project to demonstrate OOP, game rules,
Godot APIs, and GitHub collaboration. All source-code comments are in English.

## Running

1. Open this directory as a project in a compatible Godot 4 .NET editor.
2. Build the C# project in Godot, then run `Scenes3D/Main3D.tscn`.
3. Use **W/A/S/D** to move; **move the mouse** to aim; hold **left mouse button** to shoot.
4. Read the on-screen survival timer, health, pressure tier, enemies, kills and score. Press **R** after victory or defeat to restart.

The project targets `Godot.NET.Sdk/4.7.0` and `net8.0`. The original 4.7.2 SDK reference
was adjusted to match the installed Godot 4.7 .NET editor and cached SDK. The assembly is named `NeonRiftStage1` for compatibility; this
legacy assembly name is **not** the current gameplay stage or dimension.

## Game rules (values defined by our C# code)

| Rule | Value | File |
| --- | --- | --- |
| Victory | Stay alive for 60 simulation seconds | `SurvivalRules.cs`, `Game3D.cs` |
| Initial enemy spawn | 1.5 s after start | `Game3D.cs` |
| Subsequent spawns | Six 10-second tiers; bounded batches and living-enemy caps | `SurvivalRules.cs` |
| Enemy progression | Time-based mixed batches; at most 4 living Strikers | `SurvivalRules.cs`, `Game3D.cs` |
| Player max health | 100 HP | `Player3D.cs` |
| Player movement | 7 world units/s | `Player3D.cs` |
| Player firing rate | 5 shots/s (0.2 s apart) | `Player3D.cs` |
| Player damage immunity | 0.6 s after taking damage | `Player3D.cs` |
| Player bullet | 14 units/s, 24 damage, 2 s lifetime | `Game3D.cs`, `Bullet3D.cs` |
| Enemy bullet | 8 units/s, 11 damage, 4 s lifetime | `Game3D.cs`, `Bullet3D.cs` |
| Chaser | 38 HP, 2.0 units/s, 14 contact damage, 100 score | `ChaserEnemy3D.cs` |
| Chaser stopping distance | 1.0 world unit | `ChaserEnemy3D.cs` |
| Striker | 72 HP, 3.2 units/s, 22 contact damage, 180 score | `StrikerEnemy3D.cs` |
| Striker preferred distance | 4.5–6.5 world units | `StrikerEnemy3D.cs` |
| Striker fire | First at 1.0 s, then every 1.6 s if within 12 units | `StrikerEnemy3D.cs` |
| Enemy contact | Up to 1 hit per 0.6 s within 1.1 units | `Enemy3D.cs` |

**Engine vs user code:** `Node3D`, `CharacterBody3D`, `Vector3`,
`_PhysicsProcess`, `MoveAndSlide`, `MoveAndCollide`, `GD.Load`, etc. are
**Godot-provided**. Game parameters, behaviours, class responsibilities,
and win/lose conditions are **our own design**.

## Week 8 GitHub collaboration

**Student A**: edit `Scripts3D/StrikerEnemy3D.cs` — change the shooting
interval from `1.6f` to `1.2f`; also update the corresponding comment.

**Student B**: edit `Scripts3D/Player3D.cs` — change the player speed
from `7f` to `8.5f`; also update the corresponding comment.

These independent edits are normally **mergeable** because they modify
different files. To intentionally demonstrate a **merge conflict**, let
both students branch from the same commit and change
`SurvivalRules.cs`'s `DurationSeconds = 60.0` line to different values. Merge one
branch into `main` first, then merge the other **without first reconciling
those conflicting changes**; Git may require manual resolution. The
existence of two pull requests alone does not necessarily create a conflict.

For detailed call flow, signals, and collision layers, see `ARCHITECTURE.md`.

## Survival balance

| Seconds | Batch interval | Batch size | Living cap | Striker cap | Striker slot |
| --- | ---: | ---: | ---: | ---: | --- |
| 0–10 | 2.5 s | 1 | 6 | 0 | None |
| 10–20 | 2.0 s | 1 | 8 | 1 | Every fifth successful spawn |
| 20–30 | 2.0 s | 2 | 10 | 2 | Every fourth |
| 30–40 | 1.5 s | 2 | 12 | 3 | Every fourth |
| 40–50 | 1.25 s | 3 | 14 | 4 | Every third |
| 50–60 | 1.0 s | 3 | 16 | 4 | Every third |

First spawn: 1.5 seconds. A new tier does not force an immediate wave: enemies
already alive keep their instance, current health and behaviour, and the new
tier's interval starts after the next naturally scheduled spawn. There are no
catch-up bursts.
Slots blocked by caps or safe placement are discarded. Striker slots fall back to
Chasers when the ranged cap is reached. Spawn points stay at least 5 units from
the player and 1.4 units from living enemies, with 16 attempts per slot.
At 60 seconds, a living player wins before further spawning or later actor updates
in that tick. A death already received ends the run as defeat. Results are final;
enemies/projectiles freeze and R reloads a fresh scene. Paused simulation does not
advance the timer. Balance is a bounded first-pass tuning, not a measured win rate.

## Collaboration and iteration record

This repository organises the completed survival changes into five retained branches and four explicit merges reconstructed on 11 October 2026. See [the project plan](Documentation/PROJECT_PLAN.md), [the branch and merge record](Documentation/BRANCH_AND_MERGE_RECORD.md) and [the history provenance](Documentation/HISTORY_PROVENANCE.md).

The [English change report](Documentation/Survival_Mode_Change_Report_EN.docx) includes the balance rationale, annotated initial code and final persistence fix. [Verification](Documentation/FINAL_VERIFICATION.txt) records the final 344-assertion Godot run. Use `main` or `v1.0-survival-fixed` for the final gameplay.
