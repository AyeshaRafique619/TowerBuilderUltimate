This Unity project is a mobile-compatible physics-based tower building game. The player taps to place blocks on a platform, creating a swaying tower that can collapse if it becomes unstable.

Gameplay Overview: 
⚫Tap/click to place blocks on the ground or top of the tower using raycasting.
⚫Each block is procedurally generated with random color .
⚫Simple scoring system based on number of placed blocks.
⚫Physics forces make the tower sway more as it grows taller.
⚫If the tower tilts beyond 30o angle, it collapses, ending the game.

Features 
Touch Input & Raycasting:
-Uses Input.GetTouch(0) for mobile input or Input.GetMouseButtonDown(0) for editor testing.
-Raycasts detect valid placement surfaces (ground or other blocks).
Serialized Fields:
Configurable fields via Unity Inspector
-Block prefab
-Spawn offset
-Sway force
-Max tilt angle
-Block colors
-UI references
Procedural Content Generation:
-Random block color using a predefined array.
-Subtle Perlin noise-based rotation for variety.
Physics-Based Sway (Delta Time):
-Sway force increases with tower height using Time.deltaTime.
-Blocks gently sway to simulate instability.
-Collapse triggers a slow-motion effect.
Game Over Condition:
-If the top block's tilt angle exceeds a 30° limit , the tower collapses.
-Slow-motion effect is triggered during collapse.
-A game over panel shows the final height/score.
Restart Feature:
-Clears all blocks and resets the game to initial state with one click.

Components
TowerBuilder.cs - Main game controller:
-Handles input and block placement
-Manages tower physics and stability checks
-Controls game state and scoring
BlockBehavior.cs - Block physics controller:
-Applies stabilization forces
-Manages individual block physics

Scene Setup:
 Ground: Flat surface tagged 'Ground'.
 Block: Cube prefab with Rigidbody and BlockBehavior.
 UI: Score display, Game Over panel with restart button.

How to Play: 
1.Tap the screen to place blocks on the tower
2.Each successful placement increases your score
3.Keep the tower balanced - if it tilts too far, it collapses!
4.After collapse, tap "Restart" to try again


