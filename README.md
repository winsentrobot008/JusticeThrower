# JusticeThrower 🎯

> *"In a world full of creeps, be the justice."*

**JusticeThrower** is a first-person fixed-perspective physics-based game built with **Unity 2022 LTS + URP**. You are a silent vigilante riding the subway, armed with nothing but your aim and a pocket full of projectiles. Your mission: hit the shady characters, spare the innocent — and make every throw count.

---

## 📖 Story

Every day, you take the same subway line. Every day, you see them — the harassers, the pickpockets, the creeps who make everyone else's ride a nightmare. The police don't see. The cameras don't catch them. But you do.

You are **JusticeThrower**. No mask. No cape. Just a steady hand and a moral compass.

---

## 🎮 Gameplay

### Core Mechanic
- **First-person fixed view** — you sit at one end of the subway car and look down the aisle.
- **Aim and throw** projectiles at NPCs using the mouse.
- **Straight-line trajectory** with **one bounce** off walls/seats for trick shots.
- Each level is a subway car with a mix of **Victims** (green — innocent) and **Perpetrators** (red — guilty).

### Scoring
| Action | Score |
|--------|-------|
| Hit a Perpetrator | +100 |
| Hit a Victim | -200 |
| Miss (projectile expires) | -10 |
| Bounce shot (hit after bounce) | +50 bonus |

### Game Over Conditions
- Hitting **3 Victims** → Game Over (you've become the menace)
- Running out of projectiles → Game Over
- Time runs out per car → Move to next car (or fail)

---

## 🚇 Level Design

### Level 1: Subway Car (Level1_Subway)
- **Environment**: A single subway car carriage (~20m × 3.5m × 2.8m)
- **Seating**: Bench seats along both walls
- **Lighting**: Fluorescent overhead lights (URP real-time)
- **NPCs**: 3 Victims + 2 Perpetrators (placeholder capsules)
- **Player spawn**: At one end of the car, fixed camera facing down the aisle

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
| Physics | Unity Physics (3D) |
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
- [ ] NPC animations (idle, hit reactions)
- [ ] Multiple subway car levels
- [ ] Power-ups (piercing shot, multi-bounce, slow-mo)
- [ ] Sound effects & ambient subway audio
- [ ] Visual polish (VFX, lighting, post-processing)

---

## 📄 License

This project is for educational and portfolio purposes.

---

*Made with ❤️ and a strong sense of subway justice.*
