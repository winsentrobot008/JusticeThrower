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
│   ├── Materials/          # Subway car materials (floor, walls, seats)
│   ├── Models/             # 3D models (future: NPCs, props)
│   ├── Prefabs/            # Reusable prefabs (projectile, NPCs)
│   ├── Scenes/
│   │   └── Level1_Subway.unity
│   ├── Scripts/
│   │   ├── ThrowController.cs        # Player input & throw logic
│   │   ├── ThrowableProjectile.cs    # Projectile physics & bounce
│   │   ├── ProjectilePrefab.cs       # Projectile visual setup
│   │   ├── NPC_Victim.cs             # Innocent NPC behavior
│   │   ├── NPC_Perpetrator.cs        # Guilty NPC behavior
│   │   └── Level1_Subway_Setup.cs    # Editor scene generator
│   ├── Textures/           # Texture assets
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

### Generate the Subway Car (Editor Tool)
1. Create an empty GameObject in the scene.
2. Attach `Level1_Subway_Setup` script.
3. Right-click the component → **Generate Subway Car**.

---

## 🎯 Controls

| Action | Input |
|--------|-------|
| Aim | Mouse movement (fixed crosshair) |
| Throw | Left Mouse Button / Left Ctrl |
| Quit | Escape |

---

## 🗺️ Roadmap

- [x] Project setup (Unity 2022 LTS + URP)
- [x] Subway car scene structure
- [x] Basic throw mechanic (straight + 1 bounce)
- [x] Victim & Perpetrator placeholder NPCs
- [ ] Score system & UI (TMPro)
- [ ] Skill progression system (7 levels)
- [ ] Throwable items (slipper, water balloon, knife, orb, bounce ball)
- [ ] Spin throw & arc throw physics
- [ ] NPC animations (idle, hit reactions, dodge)
- [ ] Multiple subway car levels
- [ ] AI appearance system (photo → 3D head)
- [ ] Sound effects & ambient subway audio
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
