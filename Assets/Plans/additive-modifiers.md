# Project Overview
- Game Title: RangeGrid (FPS Roguelike)
- High-Level Concept: An FPS with roguelike upgrade systems (Tank/Agile modes).
- Players: Single player.
- Input System: New Input System.
- Active Rendering Pipeline: Default (Standard).

# Game Mechanics
## Core Gameplay Loop
The player collects upgrades that modify stats like speed, jump height, health, damage, ammo, and fire rate.

## Modifier System
Currently uses a percentage-based multiplier system. This plan converts it to an additive value system for easier tuning and clarity.

# UI
The Upgrade UI shows the titles of the upgrades. These will be updated to remove percentage mentions (e.g., "Thick Plating (+25% Health)" -> "Thick Plating (Increase Health)").

# Key Asset & Context
- `PlayerStats.cs`: Manages player stats and applies bonuses.
- `UpgradeManager.cs`: Syncs hierarchy modifier objects with the upgrade system.
- Modifier scripts (`HealthModifier.cs`, `SpeedModifier.cs`, etc.): Store the bonus values in the hierarchy.
- `ProjectileStandard.cs`, `PlayerGrenade.cs`, `TankKick.cs`: Apply the damage bonus to damage instances.

# Implementation Steps

## Step 1: Update Modifier Scripts
Rename the multiplier fields to addition fields in the 6 modifier scripts and update default values to flat amounts.
- `HealthModifier.cs`: `HealthMultiplier` -> `HealthAddition` (default 50)
- `SpeedModifier.cs`: `SpeedMultiplier` -> `SpeedAddition` (default 3)
- `JumpModifier.cs`: `JumpMultiplier` -> `JumpAddition` (default 2)
- `DamageModifier.cs`: `DamageMultiplier` -> `DamageAddition` (default 10)
- `AmmoModifier.cs`: `AmmoMultiplier` -> `AmmoAddition` (default 5)
- `FireRateModifier.cs`: `FireRateMultiplier` -> `FireRateAddition` (default 2)
**Assigned role**: developer
**Dependencies**: None
**Parallelizable**: Yes

## Step 2: Update PlayerStats.cs
Refactor the stats to be additive.
- Rename `XMult` fields to `XBonus` (initialize to 0).
- Add `m_BaseDelays` and `m_BaseMaxAmmo` dictionaries to track base weapon stats.
- Change `ApplyUpgrade` to use addition: `m_Controller.MaxSpeedOnGround = m_BaseMaxSpeedOnGround + SpeedBonus`.
- Update `ApplyFireRate` to use frequency addition: `1 / ( (1/BaseDelay) + FireRateBonus )`.
**Assigned role**: developer
**Dependencies**: None
**Parallelizable**: Yes

## Step 3: Update UpgradeManager.cs
Update the synchronization logic.
- Update `UpdateUpgradeValue` to read the new `XAddition` field names.
- Remove the `- 1f` logic that was converting multipliers to percentage decimals.
**Assigned role**: developer
**Dependencies**: Step 1
**Parallelizable**: No

## Step 4: Update Damage Application Callers
Update damage sources to use additive bonuses.
- `ProjectileStandard.cs`: `finalDamage += PlayerStats.Instance.DamageBonus`
- `PlayerGrenade.cs`: `damage + PlayerStats.Instance.DamageBonus`
- `TankKick.cs`: `KickDamage + PlayerStats.Instance.DamageBonus`
**Assigned role**: developer
**Dependencies**: Step 2
**Parallelizable**: No

## Step 5: Hierarchy and Asset Cleanup
Rename objects and update titles to remove percentage mentions.
- Use a script to rename hierarchy objects under `======= perks modifiers =======`.
- Use a script to update the `Title` and `Value` of `UpgradeData` ScriptableObjects in `Assets/FPS/Data/Upgrades/`.
**Assigned role**: developer
**Dependencies**: Step 3
**Parallelizable**: No

# Verification & Testing
1. **Manual Check**: Verify that changing a modifier value in the hierarchy (e.g., setting Jump Addition to 10) results in a noticeable flat increase in game.
2. **Console Check**: Verify that "Synced X from hierarchy" logs show the flat values.
3. **UI Check**: Verify that the upgrade titles in the selection menu no longer contain percentages.
