# Deckbuilder Framework (Unity, C#)

A modular, data-driven framework for single-player roguelike deckbuilders.

## Goals
- Theme-agnostic (fantasy, sci-fi, anything)
- ScriptableObjects for cards/effects/statuses
- Clean separation of systems (Card, Effect, Status, Turn, Battle)

## Project Structure
- `Assets/_Project/Scripts` — all code
- `Assets/_Project/ScriptableObjects` — cards, effects, statuses, relics
- `Assets/_Project/Scenes` — scenes (start with Battle.unity)
- `Assets/_Project/Prefabs` — prefabs (UI, Enemies, etc.)

## Milestones
1. Core Loop (Turn, Energy, Draw/Discard)
2. Card System (CardData, EffectData, CardExecutor)
3. Status System (generic Buff/Debuff/Ailment)
4. Synergy System (data-driven rules)
5. Reward/Deck Management
6. UI polish & meta systems

## How to Build
- Unity 2022/2023+, 2D (URP optional)
- Open `Assets/_Project/Scenes/Battle.unity`

## License
MIT
