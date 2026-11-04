Unity: 6.3 LTS, 6000.3.26f1, Universal 2D template (URP 2D Renderer)
Project path: Z:\LaSalle\Unity\Assignment_02\Project\Counterweight. Assets are staged in Z:\LaSalle\Unity\Assignment_02\SourceAssets (outside the project).
Git: GitHub repo, branch main, standard Unity .gitignore plus .idea/ and .vscode/
Scripts:
Assets/Scripts/Core/ResolutionEnforcer.cs: forces a 1920x1080 window in builds at startup
Scene: Assets/Scenes/Level1 (renamed from SampleScene). It is still the template default, with no gameplay objects yet.
Folders: Animations, Art, Audio, Fonts, Materials, Prefabs, Scenes, Scripts/Core, Settings, Tiles
Layers and tags: no changes yet
Physics 2D: no changes yet
Player Settings:
Product Name: Counterweight
Fullscreen Mode: Windowed, 1920x1080
Resizable Window: off
Run In Background: on
Game view: fixed 1920x1080 preset
Version control settings: Visible Meta Files, Force Text
Input: Input System package (new).
Planned approach (built in Phase 2): one Input Actions asset with two action maps, Scout (WASD, E) and Heavy (arrow keys, Right Ctrl and Enter). Each map has Move, Jump and Interact actions, and both maps are enabled at the same time.
Why not the PlayerInput component: it assigns each player its own device, and sharing one keyboard between two players needs workarounds.
Art direction: Option A, Kenney New Platformer Pack v1.1 (CC0), Default size. Plus Kenney UI Pack, Kenney Fonts, Kenney Music Jingles and Kenney Interface Sounds (all CC0), and “Happy Clappy Loop” by OwlishMedia (CC0). Impact Sounds is deferred to the end.
Credits: CREDITS.txt in the project root
Design:
Scout (yellow): fast, high jump, small collider, collects coins
Heavy (purple, 1.25x scale): slow, low jump, pushes crates, weighs down plates, collects gems
Cooperative mechanics: color-coded plate and gate pairs, Scout lever, crates, stacking
Hazards: spikes and pits. Death respawns both players at the last checkpoint.
Win: both players in the exit zone with all pickups collected
Pending decisions:
Camera clamp approach (Phase 3)
Respawn and puzzle-reset approach, and whether pickups persist after death (Phase 5)
Next: Phase 1, Step 1.1. Import the tiles, set import settings, verify the sprite pixel sizes, and set Pixels Per Unit.