# Cursed

A top-down shooter game made with Unity for the course "Einführung in Digitale Spiele" at university.

## How to play

### Controls

- WS to move forwards and backwards
- AD to rotate
- Spacebar (or try Enter) to shoot
- Escape to pause
- C to Cheat (skip level)

### Objective

There is a key on each level. You need to find the key and then go to the exit to proceed to the next level.
There are 3 + 1 large levels. When you finish the last level, you win the game.

### Curses

In each level you get a curse. These will make the game a little more difficult.

### Enemies

There are 3 types of enemies:

- Regular (green)
- Sniper (yellow): Shoots from a distance, moves slowly and has few health points
- Runner (red): Moves fast and has few health points

Enemies will split up when you pick up a key. A few of them will follow you and the rest will go to the exit.

## Credits

- Noah Apelt
- Marlon Hörner
- Tobias Fricke

## Quick side note

... because they might be unnoticed otherwise:

- we are using behavior trees for the enemies (picture included in files)
- A* for pathfinding was implemented by us
- the loading screen actually prepares: enemies, bullets and A*
