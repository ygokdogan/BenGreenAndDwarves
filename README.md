<div align="center">

<img src="Assets/Sprites/GameIcon.png" alt="Ben Green & 960 Dwarfs icon" width="140">

# Ben Green & 960 Dwarfs

**A swipe-to-decide survival game about 960 pushy dwarf salesmen, one ruined vacation, and seven very long days.**

*Made for the **Brackeys Game Jam 2026.2***

[![Unity](https://img.shields.io/badge/Unity-6000.3-000000?style=for-the-badge&logo=unity&logoColor=white)](https://unity.com/)
[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![DOTween](https://img.shields.io/badge/DOTween-animation-8A2BE2?style=for-the-badge)](https://dotween.demigiant.com/)
[![Brackeys Game Jam](https://img.shields.io/badge/Brackeys%20Game%20Jam-2026.2-ff6f3c?style=for-the-badge)](https://itch.io/jams)

<img src="Assets/Sprites/Background/outside.png" alt="The front door, where the dwarves keep knocking" width="640">

> ***"Just when you finally took a week off to clear your head, your entire neighborhood got invaded..."***

[**Lore**](#lore--premise) · [**Gameplay**](#core-gameplay--mechanics) · [**Game overs**](#game-over-scenarios) · [**Code**](#architecture--codebase-structure) · [**Run it**](#getting-started)

</div>

---

*Ben Green & 960 Dwarfs* is a 2D narrative survival and resource-management game. It combines **Reigns**-style swipe-based decision-making, dark humor, procedural character variations, and a core deceptive mechanic: **Truth vs. Lie** encounters.

## Lore & Premise

Just when you finally took a week off from work to clear your head, your entire neighborhood got invaded. You wake up one morning to discover all outside movement blocked: exactly **960 bizarre dwarves** have set up camp, turning your quiet street into pure chaos.

This isn't just any regular military invasion force. They are an aggressive syndicate of pushy door-to-door salesmen roaming non-stop. One tries to shove a rusty socket wrench through your peephole, while another frantically rings the doorbell trying to sell you a single, mismatched sock.

With local authorities completely ignoring the crisis, your vacation is officially ruined. For the next **7 days**, trapped inside your home, you must survive solely by striking absurd trades with these 960 relentless pushers—because if you don't answer the door, they start breaking your windows.

---

## Core Gameplay & Mechanics

### 1. Daily & Hourly Time Loop
- Each day runs from **09:00 AM** to **09:00 PM** (12 hours / 12 customer encounters per day).
- Every decision (Accept or Decline) advances the clock by 1 hour.
- Surviving all **7 days** without hitting any fatal thresholds wins the game.

### 2. Four-Resource Balance (Reigns-Style Dual-Threshold Risk)
You must balance four essential attributes:
- **Happiness**
- **Cash**
- **Storage**
- **Health**

> [!WARNING]
> **CRITICAL RULE:** If any stat drops to **0** OR reaches its **maximum capacity (default: 130)**, the game ends immediately! Both total deprivation and extreme excess lead to hilarious, dark-comedy game overs.

#### Game Over Scenarios:
| Stat | Minimum Threshold (0) | Maximum Threshold (130) |
|---|---|---|
| **Cash** | **BANKRUPT:** You went so broke that your wallet physically rejected you. You starved to death on the floor. | **GREED TARGET:** Your bank account triggered a government glitch. You were arrested for aggressive money laundering before buying a yacht. |
| **Health** | **COLLAPSED:** Your biology gave up. You caught a mild sniffle from a dwarf and expired three minutes later. | **OVEREXERTED:** Peak human perfection achieved; your body realized it had no more challenges and initiated self-destruct. |
| **Happiness** | **BURNOUT:** Terminal depression. You locked the door and slowly dissolved into your couch cushions until society forgot you. | **DELUSION:** Dopamine receptors fried. You stared at a blank wall, laughed into a joy-induced seizure, and died smiling. |
| **Storage** | **EMPTY WAREHOUSE:** You sold every single possession. The house emptied, and so did your will to live. | **WAREHOUSE EXPLOSION:** Hoarding reached critical mass. You stepped on a rogue Lego, fell, and were crushed by four tons of junk. |

<details>
<summary><b>See all eight death screens</b></summary>

<br>

<table>
  <tr>
    <td align="center"><img src="Assets/Sprites/UI/Death%20Screens/money_min.png" width="128" alt="Bankrupt"><br><sub>Bankrupt</sub></td>
    <td align="center"><img src="Assets/Sprites/UI/Death%20Screens/money_max.png" width="128" alt="Greed target"><br><sub>Greed Target</sub></td>
    <td align="center"><img src="Assets/Sprites/UI/Death%20Screens/health_min.png" width="128" alt="Collapsed"><br><sub>Collapsed</sub></td>
    <td align="center"><img src="Assets/Sprites/UI/Death%20Screens/health_max.png" width="128" alt="Overexerted"><br><sub>Overexerted</sub></td>
  </tr>
  <tr>
    <td align="center"><img src="Assets/Sprites/UI/Death%20Screens/happiness_min.png" width="128" alt="Burnout"><br><sub>Burnout</sub></td>
    <td align="center"><img src="Assets/Sprites/UI/Death%20Screens/happiness_max.png" width="128" alt="Delusion"><br><sub>Delusion</sub></td>
    <td align="center"><img src="Assets/Sprites/UI/Death%20Screens/storage_min.png" width="128" alt="Empty warehouse"><br><sub>Empty Warehouse</sub></td>
    <td align="center"><img src="Assets/Sprites/UI/Death%20Screens/storage_max.png" width="128" alt="Warehouse explosion"><br><sub>Warehouse Explosion</sub></td>
  </tr>
</table>

</details>

---

### 3. "Trust No One": Truths, Lies & Delayed Consequences

Not every traveling salesman is honest:
- **True Encounters:** What the dwarf promises on the card is exactly what you get (`claimedEffects == actualEffects`).
- **Lie Encounters:** The vendor presents a seemingly beneficial deal on the card (`claimedEffects`), but accepting it applies completely different hidden penalties (`actualEffects`).
- **Pending Effects (Delayed Consequences):** Certain deals have consequences that don't manifest immediately. After a set number of hours (e.g., storing a server in your closet that overheats and catches fire 3 hours later), a popup notification triggers, applying delayed stat damages.

---

### 4. End-of-Day Upkeep & Player Profiling

At 21:00, the day concludes and the game calculates your daily Accept ratio:
$$\text{Accept Ratio} = \frac{\text{Daily Accepts}}{\text{Total Encounters}}$$

Based on this ratio, you are categorized into an behavioral archetype that dictates your daily upkeep penalty or bonus:

| Accept Ratio | Archetype Title | Profile Description & Upkeep Effects |
|---|---|---|
| **0.80 – 1.00** | **The Mark (The Perfect Victim)** | Accepted almost everything. Prey to scammers; suffers severe stat penalties. |
| **0.60 – 0.79** | **The Aggressive Investor** | High-risk taker who embraces most trade opportunities. |
| **0.40 – 0.59** | **The Shrewd Tycoon** | Balanced, calculating, and discerning negotiator. |
| **0.20 – 0.39** | **The Frugal Miser** | Stingy and conservative; hoards cash and rejects risky ventures. |
| **0.00 – 0.19** | **The Paranoid Recluse** | Rejects almost everyone; locks the door against the outside world. |

---

### 5. Challenge & Modifier System

At the start of the game (and managed by `ChallengeDealer`), players are dealt 3 weighted random Challenge cards.
- **Objectives:**
  - Survive until the end (`SurviveUntilEnd`)
  - Keep a stat within a specific range (`KeepStatInRangeAlways`, `KeepStatInRangeAtEnd`)
  - Keep a stat above or below a threshold (`KeepStatAboveAlways`, `KeepStatBelowAtEnd`)
  - Accept or reject a targeted quota of offers per day or in total (`AcceptOffers`, `RejectOffers`)
- **Rewards:**
  - `DoubleMaxCapacity` / `HalfMaxCapacity`: Alters the stat ceiling.
  - `CheatDeath`: Saves you once from a game over by resetting the fatal stat to 50.
  - `CenterAllStats`: Resets all stats to half their maximum capacity.
  - `IgnoreUpkeep`: Disables daily upkeep penalties.

---

### 6. Procedural Dwarf Generation (960 Combinations)

To fulfill the lore of 960 distinct salesmen, the game proceduralizes vendor visuals using modular parts:
- **Sprite Categories:** Bodies, Faces, Hairs, Hats, Mustaches, and Bags.
- **Duplicate Prevention:** A hash set tracks generated combinations to ensure variety.
- **Dynamic Presentation:** Animated into view via DOTween easing curves.

---

## Architecture & Codebase Structure

<details>
<summary><b>Show the full script tree</b></summary>

```text
Assets/
├── Scripts/
│   ├── GameManager.cs              # Central game coordinator, day resolution, game over / victory
│   ├── GameFlowManager.cs          # State machine (ChallengeSelection, PlayingEncounter, DaySummary, etc.)
│   ├── StatManager.cs              # Core stats dictionary, limits, and dual game-over checks
│   ├── TimeManager.cs              # 09:00 - 21:00 clock progression and day tracking
│   ├── EncounterManager.cs         # Loads encounters from Resources, binds procedural vendor, handles choices
│   ├── AudioManager.cs             # Persistent background music crossfades and UI / event sound effects
│   ├── Challenges/
│   │   ├── ChallengeData.cs        # ScriptableObject definition for goals, constraints, and rewards
│   │   ├── ChallengeManager.cs     # Tracks progress, verifies completion, and applies rewards
│   │   ├── ChallengeDealer.cs      # Instantiates and animates 3 challenge cards on the table
│   │   ├── ChallengeCardUI.cs      # Visual card component for challenge display
│   │   └── ChallengeTrackerHUD.cs  # In-game HUD element tracking active objective progress
│   ├── Effects/
│   │   ├── StatEffect.cs           # Effect struct (StatType, amount, revealDelay)
│   │   ├── PendingEffects.cs       # Manages scheduled / delayed consequence queue
│   │   └── UpkeepEffects.cs        # ScriptableObject for end-of-day upkeep thresholds
│   ├── ScriptableObjects/
│   │   ├── EncounterData.cs        # Abstract base class for encounters
│   │   ├── TrueEncounterData.cs    # Honest vendors (Claimed == Actual)
│   │   ├── LieEncounterData.cs     # Deceptive vendors (Claimed != Actual)
│   │   └── VendorData.cs           # Part index struct for dwarf appearance
│   ├── VendorAppearance/
│   │   ├── VendorGenerator.cs      # Procedural generation logic for dwarf parts
│   │   └── VendorController.cs     # Sprite renderers and DOTween entrance/exit tweens
│   ├── Tutorial/
│   │   ├── LoreScreenManager.cs    # Typewriter presentation of the prologue story
│   │   └── TutorialManager.cs      # Step-by-step interactive onboarding
│   └── UI/
│       ├── HUDManager.cs           # Stat bars and clock display
│       ├── EncounterUIManager.cs   # Vendor dialogue box and offer presentation
│       ├── DayEndUIManager.cs      # End-of-day summary breakdown
│       ├── GameOverUIManager.cs    # Death screen with custom failure artwork and descriptions
│       ├── GameWonUIManager.cs     # Day 7 victory celebration panel
│       ├── PendingEffectPopup.cs   # Consequence popup for delayed effects
│       └── Utilities/
│           ├── SwipeableCard.cs    # Drag-and-swipe upward decision card with physics tilt
│           └── TypewriterEffect.cs # Smooth text typing animation
```

</details>

---

## Scenes

1. **MenuScene:** Main title screen, sound settings, extras (replay lore and tutorial).
2. **LoreScene:** Introduction explaining the 960 dwarf salesmen invasion.
3. **TutorialScene:** Guided walkthrough of the HUD, stat bars, swipe cards, and challenge selection.
4. **GameplayScene:** Core gameplay loop (Challenges $\rightarrow$ Encounters $\rightarrow$ Day End $\rightarrow$ Victory/Defeat).

---

## Technologies & Libraries

- **Engine:** Unity 6 (6000.3.15f1), 2D
- **Animation & Tweening:** Demigiant DOTween (smooth UI scaling, card tilt physics, vendor transitions)
- **Text & UI:** TextMesh Pro & Unity uGUI
- **Input:** Unity EventSystem & Input System (pointer drag, hover states, thresholds)

---

## Getting Started

1. Clone the repo and open it in **Unity 6000.3.15f1** (Unity 6) via Unity Hub → *Add project from disk*.

   ```bash
   git clone https://github.com/ygokdogan/BenGreenAndDwarves.git
   ```

2. In `File -> Build Settings`, verify the scene build order:
   - `Assets/Scenes/MenuScene.unity` (0)
   - `Assets/Scenes/LoreScene.unity` (1)
   - `Assets/Scenes/TutorialScene.unity` (2)
   - `Assets/Scenes/GameplayScene.unity` (3)
3. Open `MenuScene` and press **Play**.

---

## License & Credits

Created as part of the **Brackeys Game Jam 2026.2** game jam. All code, design, audio, and visual assets are proprietary to the original creators.

---

<div align="center">

Made with too many doorbells by [**Yusuf Emir Gökdoğan**](https://github.com/ygokdogan)

</div>
