# Strategy

- The word ***Yomi*** means to read your opponent's intentions and counter them (Fighting Game Glossary [(:link:)](https://glossary.infil.net/?t=Yomi)).
- This game is fundamentally about observing your opponent, knowing their motivations and patterns, and selecting the correct move to counter them.
- Effectively countering your opponent will require game knowledge of move interactions, character effects, special moves, and combo paths, and the ability to apply that knowledge rapidly within the short turn duration.

---

# Gameplay Loop

- The gameplay loop consists of the following elements:
    - Turn
        - A turn is an instance of simultaneous move selection by both players in secret, and the execution of their selected moves. 
        - Players have a limited amount of time to select their move for each turn.
        - Player values like health, special meter, combo counter, and positioning will be updated based on the results of the turn.
    - Round
        - A round consists of a series of turns.
        - A round ends when one player's health is depleted or they are knocked out of the ring. 
    - Match 
        - A match consists of a series of rounds in a best of *X* format. 
- Thus, by winning enough turns a player can win a round, and by winning enough rounds they can win a match.        

---

# Moves

## States

- The players transition between three different states throughout the game.
- The moves available to each player are different depending on what state they are in.

### Neutral

- In the neutral state, the moves are oriented around creating an opening and gaining advantage.
- The moves are:
    - Poke
    - Grab
    - Parry
- These three moves have a *rock-paper-scissors*-esque interaction with one another, wherein:
    - Poke beats Grab
    - Grab beats Parry
    - Parry beats Poke

    ![Neutral Move Interaction Diagram](/images/NeutralMoveInteractions.png)

### Advantage

- In the advantage state, the moves are oriented around building and eventually finishing a combo.
- The moves are:
    - Light
    - Heavy
    - Throw
    - Special

#### Special

- The Special move is the punctuation to the combo.
- Upon using the special move, the game will be returned to neutral state.
- The Special is only available to use when the special meter is full.
- Special meter is gained by:
    - Dealing damage (++)
    - Receiving damage (+)
- The Special beats all defensive options except Evade
- The Special move will have increased effect (damage, knockback, or both) depending on the length of the combo before it was used

### Disadvantage

- In the disadvantage state, the moves are oriented around defending against and trying to escape the opponent's combo.
- The moves are:
    - Guard
    - Sidestep
    - Tech
    - Retreat

#### Evade

- 


### Interactions between Advantage and Disadvantage

![Non-Neutral Move Interaction Diagram](/images/NonNeutralMoveInteractions.png)

---

# Combos

## General

## Combo Finisher

## Combo Breaker

## Strategy Impact

---

# Evaluating Combat

- *to be continued...*

---

# Characters

- The game will allow players to create their own fighter
- Accessories will be unlocked through achievements (ex: a crown for winning *X* games)

---

# Additional Mechanics

## Ring Out

- When getting hit a player will be knocked backwards, towards the edge of the stage.
- If a player is knocked off the stage, the opponent wins regardless of the player's health.
- While a player is on their own half of the stage, they will have a 2x multiplier applied to the knockback they transmit, allowing the player to more quickly retake neutral stage position.
    - If a player special modifies the knockback amount and is used while that player is on their own half, the modifications to the knockback will be additive not multiplicative.

    ![Knockback example](/images/KnockbackExample.png)