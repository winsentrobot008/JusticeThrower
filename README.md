# JusticeThrower 🎯

> *"In a world full of creeps, be the justice."*

**JusticeThrower** is a first-person fixed-perspective **realistic physics throwing puzzle game** built with **Unity 2022 LTS + URP**. You play as a "Justice Thrower" — a silent vigilante riding the subway, bus, and other tight spaces. Your mission: use **bounce shots, spin throws, and curved trajectories** to punish the "perpetrators" hiding behind innocent victims.

You cannot move. You must rely on **physics mastery + skill progression** to clear each level.

---

## 📖 Story

Every day, you take the same subway line. Every day, you see them — the harassers, the pickpockets, the creeps who make everyone else's ride a nightmare. The police don't see. The cameras don't catch them. But you do.

You are **JusticeThrower**. No mask. No cape. Just a steady hand and a moral compass.

---

## 🎮 Core Gameplay

### Fixed View, No Movement
- You stand at one end of the subway car — **you cannot move**.
- Victory depends entirely on:
  - **Throwing angle**
  - **Throwing force**
  - **Physics technique** (bounce, spin, arc)

### Realistic FPS Graphics (Unity URP)
- Real-time lighting & shadows
- Realistic materials & textures
- Immersive first-person subway car environment

### Real Physics (Billiard-Style Bounce)
Projectiles follow real physics:
- **Angle of incidence = Angle of reflection**
- **Spin affects bounce angle**
- **Arc throws** can clear obstacles
- **Multi-bounce** to hit tricky targets

---

## 📈 Skill Progression System

The protagonist learns new skills in each level:

| Level | New Skill | Description |
|-------|-----------|-------------|
| 1 | Basic Bounce | Single bounce to hit target |
| 2 | Spin Throw | Change bounce angle with spin |
| 3 | Arc Throw | Over/under throw to clear obstacles |
| 4 | Sharp Angle Bounce | Throwing knife with sharper rebounds |
| 5 | Splash Attack | Water balloon with AoE splash damage |
| 6 | Predictive Throw | Lead moving targets |
| 7 | AI Dodge Counter | Enemies dodge projectiles |

---

## 🧰 Throwable Items

| Item | Trait | Use Case |
|------|-------|----------|
| Slipper | Light, good bounce | Early levels |
| Water Balloon | Splash AoE | Mid levels |
| Throwing Knife | Fast, strong spin | Skill levels |
| Energy Orb | Strong arc, chargeable | Advanced levels |
| Bounce Ball | Multi-bounce | Ultimate levels |

---

## 🚇 Level Design

### Level 1: Subway Car (Level1_Subway)
- **Environment**: A single subway car carriage (~20m × 3.5m × 2.8m)
- **Seating**: Bench seats along both walls
- **Lighting**: Fluorescent overhead lights (URP real-time)
- **NPCs**: 3 Victims (green) + 2 Perpetrators (red) — placeholder capsules
- **Player spawn**: At one end of the car, fixed camera facing down the aisle
- **Skill taught**: Basic Bounce

---

## 🤖 AI Appearance System (Future Version)

Players can upload photos (front + side) of friends. AI will automatically:
- Generate 3D head models
- Generate textures (skin, hair, eyes)
- Auto-rig bones
- Replace NPC appearances in-game

---

## 🧱 Project Structure

```
JusticeThrower/
├── Assets/
│   ├── Materials/              # Subway car materials (floor, walls, seats)
│   ├── Models/                 # 3D models (future: NPCs, props)
│   ├── PhysicsMaterials/       # BouncySurface, SlipperBounce
│   ├── Prefabs/                # Reusable prefabs (projectile, NPCs)
│   ├── Scenes/
│   │   └── Level1_Subway.unity
│   ├── Scripts/
│   │   ├── Stage 1 (MVP):
│   │   │   ├── MVP_SceneSetup.cs       # Procedural subway car generator (Stage 3)
│   │   │   ├── PlayerThrow.cs          # Player input & throw logic (Stage 2 enhanced)
│   │   │   ├── BouncePhysics.cs        # Projectile bounce physics + LevelManager notify
│   │   │   ├── SlipperPrefab.cs        # Slipper visual setup (legacy)
│   │   │   ├── VictimNPC.cs            # Innocent NPC + animation controller
│   │   │   ├── NaughtyNPC.cs           # Guilty NPC + dodge behavior + animation
│   │   │   ├── INPCHitReaction.cs      # NPC hit reaction interface
│   │   │   ├── LevelManager.cs         # Level completion + victim tracking (3 = game over)
│   │   │   └── UIManager.cs            # Crosshair, cooldown, victim count, level/game over
│   │   ├── Stage 2 (Spin Throw):
│   │   │   ├── ThrowableItem.cs        # Base class for all throwable items
│   │   │   ├── SlipperItem.cs          # Slipper (light, good bounce)
│   │   │   ├── ThrowingKnifeItem.cs    # Knife (fast, strong spin)
│   │   │   ├── EnergyOrbItem.cs        # Orb (arc, chargeable, glow)
│   │   │   ├── SkillManager.cs         # Skill & item unlock system (7 levels)
│   │   │   ├── SpinThrow.cs            # Spin throw skill (mouse X → spin)
│   │   │   └── ArcThrow.cs             # Arc throw skill (hold → charge → arc)
│   │   ├── Stage 3 (NPC + Items):
│   │   │   ├── WaterBalloonItem.cs     # Water Balloon (AoE splash, Level 5)
│   │   │   ├── NPCAnimationController.cs # Idle sway, hit reaction, dodge animation
│   │   │   ├── NPCDodgeBehavior.cs     # AI dodge detection (Level 7)
│   │   │   ├── HitFeedbackManager.cs   # Screen shake, flash, hit marker
│   │   │   └── BounceSoundFX.cs        # Bounce & hit sound effects
│   │   └── (Legacy / WIP):
│   │       ├── ThrowController.cs      # (legacy)
│   │       ├── ThrowableProjectile.cs  # (legacy)
│   │       ├── ProjectilePrefab.cs     # (legacy)
│   │       ├── NPC_Victim.cs           # (legacy)
│   │       ├── NPC_Perpetrator.cs      # (legacy)
│   │       └── Level1_Subway_Setup.cs  # (legacy)
│   ├── Textures/               # Texture assets
│   └── URP/
│       └── URP-HighFidelity.asset    # URP Render Pipeline asset
├── Packages/
│   ├── manifest.json
│   └── packages-lock.json
├── ProjectSettings/
│   ├── ProjectSettings.asset
│   ├── ProjectVersion.txt
│   ├── GraphicsSettings.asset
│   ├── QualitySettings.asset
│   ├── TagManager.asset
│   └── InputManager.asset
├── .gitignore
└── README.md
```

---

## 🛠️ Tech Stack

| Component | Technology |
|-----------|-----------|
| Engine | Unity 2022.3.52f1 (LTS) |
| Render Pipeline | Universal Render Pipeline (URP) 14.0.11 |
| Language | C# 9.0 |
| Physics | Unity Physics (3D) — billiard-style |
| Platform | Android (Phase 1) |
| AI Factory | Cline (local) + Fleet (cloud) |
| Version Control | Git + GitHub |

---

## 🚀 Getting Started

### Prerequisites
- Unity Hub + Unity 2022.3.52f1
- Git

### Setup
```bash
git clone https://github.com/winsentrobot008/JusticeThrower.git
cd JusticeThrower
# Open the project in Unity Hub with Unity 2022.3.52f1
```

### First Launch
1. Open the project in Unity.
2. Navigate to `Assets/Scenes/Level1_Subway.unity`.
3. Press **Play** to test the throw mechanic.
4. Use **Left Click** to throw a projectile.
5. Use **Q/E** to cycle through unlocked items.
6. Use **1/2/3** to toggle skills (Basic Bounce / Spin Throw / Arc Throw).

### Generate the Subway Car (Editor Tool)
1. Create an empty GameObject in the scene.
2. Attach `MVP_SceneSetup` script.
3. Right-click the component → **Generate MVP Scene**.

---

## 🎯 Controls

| Action | Input |
|--------|-------|
| Aim | Mouse movement (fixed crosshair) |
| Throw | Left Mouse Button |
| Cycle Item (prev/next) | Q / E |
| Skill: Basic Bounce | 1 |
| Skill: Spin Throw | 2 (mouse X controls spin) |
| Skill: Arc Throw | 3 (hold to charge, release to throw) |
| Quit | Escape |

---

## 🗺️ Roadmap

- [x] Project setup (Unity 2022 LTS + URP)
- [x] Subway car scene structure
- [x] Basic throw mechanic (straight + 1 bounce)
- [x] Victim & Perpetrator placeholder NPCs
- [x] **Stage 2: Spin Throw system**
- [x] **Throwable items: Slipper, Throwing Knife, Energy Orb**
- [x] **Spin Throw physics (mouse X → spin → curved bounce)**
- [x] **Arc Throw physics (hold → charge → arc trajectory)**
- [x] **SkillManager with 7-level progression**
- [x] **Stage 3: NPC Animation + Water Balloon + Hit Feedback**
- [x] **Water Balloon (Splash Attack, Level 5) — AoE splash damage**
- [x] **NPCAnimationController — idle sway, hit reaction, dodge animation**
- [x] **NPCDodgeBehavior — AI dodge detection (Level 7)**
- [x] **HitFeedbackManager — screen shake, flash, hit marker**
- [x] **BounceSoundFX — bounce & hit sound effects**
- [x] **LevelManager — victim hit tracking (3 = game over)**
- [x] **UIManager — victim hit count, game over panel**
- [ ] Score system & UI (TMPro)
- [ ] Bounce Ball (multi-bounce, Level 7)
- [ ] Multiple subway car levels
- [ ] AI appearance system (photo → 3D head)
- [ ] Sound effects & ambient subway audio (real audio clips)
- [ ] Visual polish (VFX, lighting, post-processing)
- [ ] Android build & deployment

---

## 🏗️ AI Factory Architecture

- **Cline (Local)**: Generates Unity project, scripts, scenes, builds APK
- **Fleet (Cloud)**: Auto-build, auto-test, auto-deploy

---

## 📄 License

This project is for educational and portfolio purposes.

---

*Made with ❤️ and a strong sense of subway justice.*
