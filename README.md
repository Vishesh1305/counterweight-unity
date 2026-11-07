<div align="center">

<!--
  ┌──────────────────────────────────────────────────────────────┐
  │  GAMEPLAY GIF                                                │
  │  1. Record a short clip (5 to 10 s, 960px wide keeps it      │
  │     under GitHub's 10 MB inline limit).                      │
  │  2. Save it as Docs/Media/gameplay.gif in the project root   │
  │     (next to this README, NOT inside Assets, so Unity        │
  │     does not import it).                                     │
  │  3. Push. The image below will appear automatically.         │
  └──────────────────────────────────────────────────────────────┘
-->
<img src="Docs/Media/gameplay.gif" alt="Counterweight gameplay" width="860" />

<br />
<br />

# C O U N T E R W E I G H T

### Two unequal heroes. One keyboard. Nobody gets left behind.

<br />

![Unity](https://img.shields.io/badge/Unity-6.3_LTS-000000?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-Scripting-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![URP](https://img.shields.io/badge/URP-2D_Renderer-2C3E50?style=for-the-badge)
![Players](https://img.shields.io/badge/Players-2_Local_Co--op-F4B400?style=for-the-badge)
![Resolution](https://img.shields.io/badge/Resolution-1920×1080-8E44AD?style=for-the-badge)
![Assets](https://img.shields.io/badge/Assets-CC0-2ECC71?style=for-the-badge)

<br />

[**About**](#about) &nbsp;·&nbsp;
[**The Duo**](#the-duo) &nbsp;·&nbsp;
[**Controls**](#controls) &nbsp;·&nbsp;
[**Co-op Mechanics**](#co-op-mechanics) &nbsp;·&nbsp;
[**Under the Hood**](#under-the-hood) &nbsp;·&nbsp;
[**Getting Started**](#getting-started) &nbsp;·&nbsp;
[**Roadmap**](#roadmap) &nbsp;·&nbsp;
[**Credits**](#credits)

</div>

<br />

---

<br />

## About

**Counterweight** is a two player, same keyboard, cooperative 2D platformer built in Unity.

One player is small and quick. The other is big and slow. Neither one can finish the level alone. The Scout slips through tunnels and flips levers. The Heavy pushes crates and holds pressure plates down with its weight. Every gate, gap and ledge is designed so that each player's strength covers the other's weakness.

There is no combat. The only thing you can do to your partner is get in their way, or give them a boost.

> [!NOTE]
> Built for **VGP236, Assignment 2** at **LaSalle College Vancouver**. This is a prototype focused on core cooperative gameplay.

<br />

## The Duo

<table>
  <tr>
    <th width="50%">
      <h3>SCOUT</h3>
      <sub>Player 1 &nbsp;·&nbsp; Yellow</sub>
    </th>
    <th width="50%">
      <h3>HEAVY</h3>
      <sub>Player 2 &nbsp;·&nbsp; Purple</sub>
    </th>
  </tr>
  <tr>
    <td>
      Small, fast and light on its feet. Fits through one tile tunnels, reaches high ledges and works the levers.
    </td>
    <td>
      Big, slow and solid. Pushes crates, weighs down pressure plates and acts as a living step for the Scout.
    </td>
  </tr>
  <tr>
    <td>

| Stat | Value |
|:--|:--|
| Run speed | `7` tiles/s |
| Jump height | `3.5` tiles |
| Mass | `1` |
| Collects | **Coins** |

</td>
<td>

| Stat | Value |
|:--|:--|
| Run speed | `4.5` tiles/s |
| Jump height | `2.2` tiles |
| Mass | `3` |
| Collects | **Gems** |

</td>
  </tr>
</table>

The two characters share one movement script. Their differences come purely from stats and collider size. There are no double jumps, wall slides, wall hangs or other special movement skills.

<br />

## Controls

<div align="center">

| Action | Scout | Heavy |
|:--|:--:|:--:|
| **Move** | <kbd>A</kbd> &nbsp; <kbd>D</kbd> | <kbd>←</kbd> &nbsp; <kbd>→</kbd> |
| **Jump** | <kbd>Space</kbd> | <kbd>↑</kbd> |
| **Interact** | <kbd>E</kbd> | <kbd>Right Ctrl</kbd> or <kbd>Enter</kbd> |

</div>

Both players share one keyboard. Each has its own action map in a single Input Actions asset, and both maps run at the same time.

<br />

## Co-op Mechanics

<table>
  <tr>
    <td width="50%" valign="top">
      <h4>Pressure Plates and Gates</h4>
      The Heavy stands on a plate to hold a gate open while the Scout runs through. Plates and gates are color coded, so a blue plate always opens a blue gate.
    </td>
    <td width="50%" valign="top">
      <h4>Scout Levers</h4>
      Only the Scout can flip a lever. Levers move platforms and drop barriers that the Heavy could never get past alone.
    </td>
  </tr>
  <tr>
    <td width="50%" valign="top">
      <h4>Crates</h4>
      Only the Heavy is strong enough to push a crate. Crates bridge spike pits, form steps and keep plates pressed down.
    </td>
    <td width="50%" valign="top">
      <h4>Stacking</h4>
      Players collide with each other. The Scout can stand on the Heavy's head to reach ledges that neither could reach alone.
    </td>
  </tr>
</table>

### Rules

| | |
|:--|:--|
| **Win** | Collect **every** coin and gem, then get **both** players inside the exit door. |
| **Lose** | If either player touches spikes or falls into a pit, **both** respawn at the last checkpoint flag. |
| **Camera** | Follows the Scout, but never leaves the Heavy behind. Invisible walls at the screen edges keep the pair together. |

<br />

## Under the Hood

<details open>
<summary><b>Tech stack</b></summary>
<br />

| Area | Choice |
|:--|:--|
| Engine | Unity **6.3 LTS** (`6000.3.26f1`) |
| Render pipeline | Universal Render Pipeline, **2D Renderer** |
| Input | **Input System** package, two action maps on one keyboard |
| Physics | Dynamic `Rigidbody2D` per player, zero friction material, `Physics2D.OverlapBox` ground check |
| Level | Tilemaps for Background, Ground, Hazards and Foreground. Ground uses a `CompositeCollider2D` |
| Display | Locked **1920×1080** windowed, enforced in Player Settings and at runtime |
| Version control | Git, standard Unity `.gitignore`, Force Text serialization |

</details>

<details>
<summary><b>Movement feel</b></summary>
<br />

- **Jump height in tiles.** Designers set jump height in tiles, and the script derives the launch velocity from gravity, so level geometry and jump height always agree.
- **Coyote time.** A short grace window after walking off a ledge where a jump still counts.
- **Jump buffer.** A jump pressed just before landing fires on landing instead of being lost.
- **Jump cut.** Releasing jump early gives a shorter hop.

</details>

<details>
<summary><b>Camera system</b></summary>
<br />

- Smoothly follows the Scout in `LateUpdate`.
- Refuses to move so far that the Heavy drops off screen, with a configurable edge margin.
- Clamped to level bounds, drawn as a gizmo in the Scene view.
- Two kinematic edge walls on their own `ScreenEdge` layer block only players, so the Scout has to wait for its partner instead of the Heavy being teleported.

</details>

<details>
<summary><b>Code structure</b></summary>
<br />

Every script lives under one root namespace:

```
Counterweight.Core           Resolution and game state
Counterweight.Player         Input reading, ground check, movement
Counterweight.CameraSystem   Camera follow and screen edge walls
Counterweight.Gameplay       Pickups, hazards, checkpoints, puzzles
Counterweight.UI             HUD and menus
```

Small single purpose components, `[SerializeField] private` fields, cached references and no per frame lookups.

</details>

<br />

## Getting Started

**Requirements:** Unity Hub with Unity **6000.3.26f1** installed.

```bash
git clone <this-repo-url>
```

1. Open **Unity Hub**, click **Add**, then **Add project from disk**, and select the cloned `Counterweight` folder.
2. Open the project with **6000.3.26f1**. The first import takes a few minutes.
3. In the **Project** window, open `Assets/Scenes/Level1`.
4. In the **Game** view, set the resolution dropdown to **1920×1080**.
5. Press **Play**.

<br />

## Project Structure

```
Counterweight/
├── Assets/
│   ├── Animations/
│   ├── Art/
│   ├── Audio/
│   ├── Fonts/
│   ├── Input/          CounterweightControls (two action maps)
│   ├── Materials/      PlayerNoFriction
│   ├── Prefabs/        Scout, Heavy
│   ├── Scenes/         Level1
│   ├── Scripts/
│   │   ├── Camera/
│   │   ├── Core/
│   │   └── Player/
│   ├── Settings/
│   └── Tiles/          Palettes and terrain tiles
├── Docs/Media/         README images and GIFs
├── CREDITS.txt
└── README.md
```

<br />

## Roadmap

- [x] **Phase 0** &nbsp; Project setup, resolution lock, art direction
- [x] **Phase 1** &nbsp; Grid, tilemaps, palette, ground and hazard colliders
- [x] **Phase 2** &nbsp; Two player input and role based movement
- [x] **Phase 3** &nbsp; Camera follow and screen edge walls
- [x] **Phase 4** &nbsp; Pickups and HUD
- [x] **Phase 5** &nbsp; Hazards, death and checkpoints
- [x] **Phase 6** &nbsp; Cooperative mechanics
- [ ] **Phase 7** &nbsp; Win condition
- [ ] **Phase 8** &nbsp; Full multi screen level
- [ ] **Phase 9** &nbsp; Sprite animation
- [ ] **Phase 10** &nbsp; Menus, pause, win, game over and credits screens
- [ ] **Phase 11** &nbsp; Polish: parallax, 2D lighting, post processing, particles, audio
- [ ] **Phase 12** &nbsp; QA and submission

<br />

## Credits

All third party assets are **CC0** (public domain). Full list in [`CREDITS.txt`](CREDITS.txt).

| Asset | Author | License |
|:--|:--|:--|
| New Platformer Pack | [Kenney](https://kenney.nl) | CC0 |
| UI Pack | [Kenney](https://kenney.nl) | CC0 |
| Kenney Fonts | [Kenney](https://kenney.nl) | CC0 |
| Music Jingles | [Kenney](https://kenney.nl) | CC0 |
| Interface Sounds | [Kenney](https://kenney.nl) | CC0 |
| Happy Clappy Loop | OwlishMedia ([OpenGameArt](https://opengameart.org/content/happy-clappy-loop)) | CC0 |

<br />

---

<div align="center">

<br />

**Counterweight** &nbsp;·&nbsp; Designed and built by **Vishesh Mistry**

[vishhz.dev](https://vishhz.dev) &nbsp;·&nbsp; LaSalle College Vancouver &nbsp;·&nbsp; BSc Game Programming

<sub>Made with Unity 6.3 LTS</sub>

<br />

</div>