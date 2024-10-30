# Fight the Fiend

**Fight the Fiend** is a roguelike turn-based dungeon crawler built in Unity. Traverse procedurally generated dungeons, battle monstrous fiends, and acquire powerful loot as you journey through the depths of a dark and mysterious world. Beware: each level is unique, and every choice could be your last!

## Table of Contents
- [About the Game](#about-the-game)
- [Features](#features)
- [Installation](#installation)
- [Gameplay](#gameplay)
- [Code Structure](#code-structure)
- [Assets](#assets)
- [Contributing](#contributing)
- [License](#license)

## About the Game
Fight the Fiend is designed with a classic roguelike gameplay style, offering endless replayability through randomly generated dungeon floors and turn-based combat. The goal is simple: survive, gather treasures, defeat enemies, and make it as far as possible without dying.

## Features
- **Procedurally Generated Levels**: Each dungeon is randomly generated, ensuring no two playthroughs are the same.
- **Turn-Based Combat**: Strategic, turn-based mechanics give players control to make careful moves.
- **Character Progression**: Gather items, upgrade your abilities, and become a more formidable fighter.
- **Immersive Atmosphere**: Dark, eerie sound design and a haunting pixel art style to enhance the dungeon experience.
- **Save and Load System**: Continue your progress from where you left off.

## Installation

### Prerequisites
- Unity version 2021.3.5 or higher
- .NET Framework 4.7.1 or higher

### Setup
Clone the repository:

```bash
git clone https://github.com/username/fight-the-fiend.git
```
## Open the Project in Unity

1. Launch **Unity Hub**.
2. Click on **Open Project** and navigate to the directory where you cloned the repository.
3. Open the project to start developing or testing in Unity.
4. Press **Play** in the Unity editor to begin testing the game.

## Build

To build a standalone version:

1. In Unity, navigate to **File > Build Settings**.
2. Choose your platform (PC, Mac, Linux).
3. Select **Build** to generate the standalone executable.

## Gameplay

The player controls a character navigating through randomly generated dungeon floors filled with enemies, treasures, and traps. Movement and combat are turn-based, allowing for a strategic approach to each encounter.

- **Movement**: Use arrow keys or WASD to navigate the dungeon.
- **Combat**: Approach enemies and use various abilities to fight them in turn-based combat.
- **Inventory**: Collect items and manage your inventory to enhance abilities and survive longer.
- **Progression**: Defeat enemies and bosses to move deeper into the dungeon, unlocking more challenging enemies and powerful loot.

## Code Structure

The project is structured as follows:

- **Assets/**: Contains all game assets, including sprites, sound effects, and prefabs.
- **Scripts/**: All C# scripts for gameplay mechanics, player controls, AI, and utilities.
- **Scenes/**: Scene files used in the game, including the main game level and UI.
- **Prefabs/**: Prefab files for enemies, items, and player character setups.

### Key Scripts

- **DungeonGenerator.cs**: Responsible for procedural dungeon generation.
- **PlayerController.cs**: Manages player input and character actions.
- **EnemyAI.cs**: Handles enemy behaviors and attack patterns.
- **TurnManager.cs**: Controls the turn-based system for players and enemies.

## Assets

All assets, including sprites, sound effects, and animations, are located in the **Assets/** folder. If using or modifying these assets, please ensure they meet the licensing requirements specified in this repository.

## Contributing

Contributions to *Fight the Fiend* are welcome! Please follow these steps:

1. Fork the repository.
2. Create a new branch (`feature/YourFeature`).
3. Commit your changes and push them to your branch.
4. Open a pull request with a description of the changes you’ve made.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

Enjoy playing *Fight the Fiend*!
