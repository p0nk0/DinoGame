
# Jurassic Park Booth Game
An immersive escape room-style attraction created for CMU's 2025 Spring Carnival.

Overview
This project powered the interactive experience behind our Jurassic Park booth, synchronizing Arduino-based puzzle inputs with an animatronic dinosaur and audiovisual effects. It was developed by members of CMU's Theme Park Engineering Group and Sustainable Earth and won first place overall out of 21 student organizations.

Features
- Finite State Machine to control game state based on puzzle inputs
- Unity-controlled UI and sound effects
- Serial communication with multiple Arduino controllers
- Two puzzles (RFID "DNA Scanner" and Circuit Breaker)
- User-triggered animatronic effects

System Design
The core game logic was driven by a state machine with 14 states. Each state represents commands to visuals, sound, and hardware. Each transition represents a signal received by the Unity controller.
![FSM diagram 1]()
![FSM diagram 2]()

how to set up

my contributions

media
