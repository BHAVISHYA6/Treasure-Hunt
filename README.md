# 🏴‍☠️ Treasure Hunt: The Quest for the Pirate King's Vault

An immersive 3D third-person adventure game built with Unity 2022.3 LTS featuring 3 distinct playable scenes, rich quest progression, interactive puzzles, procedural audio synthesis, and reward systems.

---

## 🌟 Game Highlights

- **3 Complete, Connected Scenes**:
  1. **Scene 1: The Whispering Cove (`Scene1_IslandCove.unity`)**
     - Outdoor tropical archipelago surrounded by deep ocean water, tall perimeter cliff walls, solid coastal boulders, ruined stone watchtower, palm trees, tidal stepping stones, and moving tidal platforms.
     - **Main Quest**: Read Captain Redbeard's logbook, track down the 3 hidden Map Fragments across the cove, and collect **10 Gold Coins**. When 10 coins are gathered, the ship automatically sets sail to the Forbidden Temple (or unlock the Sea Shrine for the **Bronze Skeleton Key** to pass the Ancient Sea Gate)!
     - **Water Hazard**: Falling into the sea triggers an authentic water splash VFX, splash audio, loss of 1 heart/life, and a safe teleport back to the beach campfire camp.
  2. **Scene 2: The Forbidden Temple (`Scene2_AncientTemple.unity`)**
     - Subterranean enclosed stone sanctuary enclosed by ancient perimeter dungeon walls, lined with towering stone pillars, flaming wall torches, bottomless chasm void, moving stone platform bridges, and lethal retractable spike traps with physical solid colliders.
     - **Trial of the Three Elements**: Solve the ancient tablet's riddle by dodging active spikes and traversing the chasm to awaken the **Ruby Fire Altar**, **Emerald Earth Altar**, and **Sapphire Water Altar**. Gather **10 Temple Treasures & Gems** and claim the manifested **Silver Key of Passage** on the central dais to unlock the Grand Temple Portal to the Vault!
  3. **Scene 3: The Pirate King's Vault (`Scene3_TreasureVault.unity`)**
     - Grand royal octagonal treasury surrounded by solid perimeter stone walls, 8 towering gilded columns with wall torches, sparkling amethyst crystal clusters, and piles of gold coins and giant diamonds.
     - **The Grand Climax & Ending**: Ascend the dragon dais to open the **Grand Legendary Treasure Chest**, triggering celebratory particle bursts, victorious fanfare, player victory cheer animation, and the final scoring screen awarding your Explorer Rank!

- **Realistic Physics & Solid Obstacles**:
  - Trees, pillars, cliff walls, dungeon perimeter walls, and coastal boulders now feature solid physics colliders (`CapsuleCollider` / `BoxCollider`) so the Captain can no longer walk or clip through them.
  - Spike traps now dynamically extend solid box colliders and trigger knockback + life loss upon contact.

- **Lives & Safe Respawn Mechanics**:
  - The Captain starts with **3 Hearts / Lives (`❤️❤️❤️`)**.
  - Falling into water or the bottomless abyss deducts 1 life, plays damage SFX/VFX, and safely teleports the player back to dry campfire ground with brief invulnerability.
  - If lives hit 0, a Game Over modal appears offering an immediate level retry or full game restart.

- **Automatic Level Progression & Ending**:
  - Collecting 10 coins in Level 1 automatically displays a celebratory clear banner and smoothly sets sail to Level 2.
  - Conquering the 3 altars and 10 treasures in Level 2 automatically opens the portal and leads into Level 3.
  - Collecting 20 coins in Level 3 (or unlocking the Grand Vault Chest) triggers the celebratory victory fanfare, animation, and displays the **THE END Game Complete Card** with your total score, time, rank, and a **"Restart Again (Play From Beginning)"** button!

- **100% Free & Self-Contained Resources**:
  - Uses the included RPG Tiny Hero Duo character models with full animations (idle, walk, sprint, jump, victory).
  - All environmental props (treasure chests with animated lids, spinning coins, keys, clue scrolls, altars, portals, traps, pillars, platforms) are generated cleanly without any third-party or paid store dependencies.
  - **Procedural Sound Engine (`SoundManager.cs`)**: All SFX (coins, keys, chests, jumps, footsteps, traps, portal hum, water splash, hurt, level clear, game over, victory fanfare) and background ambient music tracks are synthesized in real-time code with zero missing audio assets.

---

## 🎮 Controls

| Action | Primary Key | Alternate |
| :--- | :--- | :--- |
| **Move** | `W`, `A`, `S`, `D` | Arrow Keys |
| **Sprint** | `Left Shift` | `Right Shift` |
| **Jump** | `Spacebar` | Controller South |
| **Look / Orbit Camera** | `Mouse Movement` | - |
| **Camera Zoom** | `Mouse Scroll Wheel` | - |
| **Interact (Read / Collect / Open)** | `E` | `Enter` |
| **Pause / Close Clue Parchment** | `Escape` | - |

---

## 🗺️ Scene Walkthrough & Objectives

### Level 1: The Whispering Cove
1. Approach the campfire dock and press `[E]` on **Captain's Logbook** to read the quest clues.
2. Platform up the ruined stone watchtower to collect **Map Fragment #1**.
3. Hop across the tidal stepping stones to the eastern islet to collect **Map Fragment #2**.
4. Time your crossing on the moving northern platform to reach the cache for **Map Fragment #3**.
5. Once all 3 fragments are collected, head to the Sea Shrine where the **Bronze Skeleton Key** hovers.
6. Take the key, unlock the **Ancient Sea Gate**, and step into the swirling portal!

### Level 2: The Forbidden Temple
1. Inspect the **Tablet of the Ancients** at the temple entrance.
2. Time your steps past the periodic **Spike Traps** (watch for the red warning glow!).
3. Ignite the **Ruby Fire Altar** in the western wing and the **Emerald Earth Altar** in the eastern wing.
4. Ride the moving stone platforms across the misty chasm to ignite the **Sapphire Water Altar**.
5. With all three altars awakened, return to the central dais to claim the **Silver Key of Passage**.
6. Unlock the **Grand Temple Portal** to enter the Pirate King's Vault!

### Level 3: The Pirate King's Vault
1. Walk down the golden hall and read the **Vault Monument Inscription**.
2. Collect the surrounding bonus coins and giant diamonds.
3. Climb the stepped dais and press `[E]` to unlock the **Grand Legendary Treasure Chest**!
4. Enjoy the triumphant victory fanfare, victory animation, and view your **Total Score**, **Coins Collected**, **Elapsed Time**, and **Explorer Rank**!

---

## 🛠️ Project Structure & Architecture

```
Assets/
├── Inputs/
│   ├── InputSystem.inputsettings.asset
│   └── PlayerActions.inputactions
├── Models/
│   └── RPG Tiny Hero Duo/ (Character models, animations, and materials)
├── Scenes/
│   ├── MainScene.unity          (Direct launch into Level 1)
│   ├── Scene1_IslandCove.unity   (Level 1)
│   ├── Scene2_AncientTemple.unity(Level 2)
│   └── Scene3_TreasureVault.unity(Level 3)
└── Scripts/
    ├── IInteractable.cs         (Interaction interface)
    ├── PlayerController.cs      (Dual-input movement, jumping, sprint, animations)
    ├── ThirdPersonMovement.cs   (Backwards-compatible wrapper extending PlayerController)
    ├── ThirdPersonCamera.cs     (Smooth orbital follow, collision avoidance, zoom)
    ├── GameManager.cs           (Singleton: scores, coins, keys, quest state, scene flow)
    ├── SoundManager.cs          (Real-time procedural audio synthesis for SFX & BGM)
    ├── GameHUD.cs               (Dynamic canvas: objective tracker, coin counter, pause menu, modals)
    ├── ProceduralProps.cs       (3D procedural mesh & material builder for all props)
    ├── ClueScroll.cs            (Interactive story scrolls & tablets)
    ├── CollectibleCoin.cs       (Floating, rotating coins with magnetic pull)
    ├── KeyPickup.cs             (Bronze, Silver, Gold keys with requirement logic)
    ├── TreasureChest.cs         (Animated hinged chest with key check & particle bursts)
    ├── ElementalAltar.cs        (Fire, Earth, Water puzzle altars with colored beacons)
    ├── TemplePuzzleController.cs(Trial of elements controller)
    ├── SpikeTrap.cs             (Retractable timed hazard)
    ├── MovingPlatform.cs        (Smooth waypoint platform physics)
    ├── ScenePortal.cs           (Inter-scene teleportation archway)
    ├── SceneDirector.cs         (Runtime level bootstrap & in-editor scene builder)
    └── Editor/
        └── TreasureHuntSceneBuilder.cs (One-click menu tool: "Treasure Hunt -> Build All 3 Scenes")
```

---

## 🚀 How to Play

1. Open the project in **Unity 2022.3 LTS**.
2. In the `Project` window, double-click **`Assets/Scenes/MainScene.unity`** (or **`Scene1_IslandCove.unity`**).
3. Click the **Play (▶)** button at the top of the Unity Editor.
4. Embark on the adventure!
