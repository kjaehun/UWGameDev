using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    /// <summary>
    /// Holds both players.
    /// </summary>
    private static PlayerData[] players = new PlayerData[2];

    /// <summary>
    /// Contains all cards within a player's deck.
    /// </summary>
    private List<CardData> deck;

    /// <summary>
    /// Contains all cards within a player's drawpile. Worthless when outside of a match.
    /// </summary>
    private List<CardData> drawPile;
    /// <summary>
    /// Contains all cards within a player's hand. Worthless when outside of a match.
    /// </summary>
    private List<CardData> hand;
    /// <summary>
    /// Contains all cards within the discard pile. Worthless when outside of a match.
    /// </summary>
    private List<CardData> discard;

    /// <summary>
    /// Current health.
    /// </summary>
    private int health;
    /// <summary>
    /// Maximum health.
    /// </summary>
    private int maxHealth;

    /// <summary>
    /// Current mana. Worthless when outside of a match.
    /// </summary>
    private int mana;

    /// <summary>
    /// Player healthbar.
    /// </summary>
    private HealthBarController healthBar;

    /// <summary>
    /// Determines if this is the currently controlled player.
    /// </summary>
    private bool currentPlayer;

    /// <summary>
    /// Getter for health.
    /// </summary>
    /// <returns>health of player</returns>
    public int getHealth() {return health;}

    /// <summary>
    /// Constructor for a PlayerData object.
    /// Initializes all fields and stores this player in the players array.
    /// </summary>
    /// <param name="hp">starting health</param>
    public PlayerData(int hp) {
        deck = new List<CardData>();

        drawPile = new List<CardData>();
        hand = new List<CardData>();
        discard = new List<CardData>();

        this.health = hp;
        this.maxHealth = hp;

        mana = 3;

        if (players[0] == null) players[0] = this;
        else players[1] = this;

    }
    /// <summary>
    /// Overloading constructor for a PlayerData object which provides a default of 30 health.
    /// </summary>
    public PlayerData() : this(30) {}

    /// <summary>
    /// Draws a card from the draw pile and places it into the hand.
    /// Builds a physical card for the drawn card.
    /// </summary>
    public void DrawCard() {
        if (drawPile.Count == 0) {
            ShuffleDiscardIntoDraw();
        }
        if (drawPile.Count == 0) return;

        CardData card = drawPile[drawPile.Count-1];
        drawPile.RemoveAt(drawPile.Count-1);
        hand.Add(card);

        card.setOwner(this);

        card.MakePhysicalCard(new Vector2(0,-3));
    }

    /// <summary>
    /// Shuffles the discard pile into the draw pile.
    /// </summary>
    public void ShuffleDiscardIntoDraw() {
        while (discard.Count > 0) {
            CardData card = discard[discard.Count-1];
            drawPile.Add(card);
            discard.RemoveAt(discard.Count-1);
        }
        CardData.Shuffle(drawPile);
    }

    /// <summary>
    /// Initializes the draw pile, hand, and discard pile prior to a match.
    /// Should be used before starting any match.
    /// </summary>
    public void PrepareCards() {
        drawPile = new List<CardData>();
        hand = new List<CardData>();
        discard = new List<CardData>();

        foreach (CardData card in deck) {
            drawPile.Add(card);
        }
        CardData.Shuffle(drawPile);
    }

    /// <summary>
    /// Determines whether the player is alive.
    /// A player is alive iff his/her health is greater than 0.
    /// </summary>
    /// <returns></returns>
    public bool isAlive() {
        return (health > 0);
    }

    /// <summary>
    /// Allows the player to attempt to play a card.
    /// If the player has the required amount of mana, the card will be played and the mana will be drained.
    /// If the player does not have the required amount of mana, nothing happens.
    /// </summary>
    /// <param name="field">BattleField in which the card was played</param>
    /// <param name="card">card which the player is trying to play</param>
    public void AttemptPlayCard(BattleField field, CardData card) {
        if (hasMana(card.getManaCost())) {

            card.PlayCard(getID(),field);

            changeMana(card.getManaCost());

            DiscardCard(card);

        }
    }

    /// <summary>
    /// Allows for the player to take damage. Updates health bar.
    /// </summary>
    /// <param name="dmg">damage to take</param>
    public void TakeDamage(int dmg) {
        health -= Mathf.Min(health,dmg);

        healthBar.SetValues(health, maxHealth);
    }

    /// <summary>
    /// Gets a player depending on their index.
    /// </summary>
    /// <param name="n">0-> player 1, 1-> player 2</param>
    /// <returns>the PlayerData at the associated index in the players array</returns>
    public static PlayerData GetPlayer(int n) {
        if (n<0 || n>1) return null;
        return players[n];
    }

    /// <summary>
    /// Determines whether a card is in this player's hand.
    /// </summary>
    /// <param name="card">card to check if is in hand</param>
    /// <returns>true iff card is contained by the hand list</returns>
    public bool isInHand(CardData card) {
        return hand.Contains(card);
    }

    /// <summary>
    /// Determines whether the player has the amount of mana provided.
    /// </summary>
    /// <param name="val">amount to compare to</param>
    /// <returns>true iff val is less than or equal to the player's current mana</returns>
    public bool hasMana(int val) {
        return (mana >= val);
    }
    /// <summary>
    /// Changes the player's mana by the amount provided.
    /// </summary>
    /// <param name="val">amount to change mana by</param>
    public void changeMana(int val) {
        mana -= val;
        if (currentPlayer) {
            UIScript.instance.SetMana(mana);
        }
    }
    /// <summary>
    /// Sets mana to the amount provided.
    /// </summary>
    /// <param name="val">new value for mana</param>
    public void setMana(int val) {
        mana = val;
        if (currentPlayer) {
            UIScript.instance.SetMana(mana);
        }
    }

    /// <summary>
    /// Adds a CLONE of the card to the deck.
    /// </summary>
    /// <param name="card">card to add to deck</param>
    public void AddToDeck(CardData card) {
        deck.Add(card.clone());
    }

    /// <summary>
    /// Sets the healh bar field to a provided health bar.
    /// </summary>
    /// <param name="healthBar">health bar to store</param>
    public void SetHealthBar(HealthBarController healthBar) {
        this.healthBar = healthBar;
    }

    /// <summary>
    /// Gets this player's ID.
    /// </summary>
    /// <returns>0 if this is the first player, 1 if this is the second player</returns>
    public int getID() {
        if (this.Equals(players[0])) return 0;
        return 1;
    }

    /// <summary>
    /// Moves a card from the hand to the discard pile.
    /// Destroys the attached physical card.
    /// Does not check if the card is actually in the hand.
    /// </summary>
    /// <param name="card"></param>
    public void DiscardCard(CardData card) {
        hand.Remove(card);
        discard.Add(card);
        card.DestroyPhysicalCard();
    }

    /// <summary>
    /// Discards the player's entire hand.
    /// Uses DiscardCard.
    /// </summary>
    public void DiscardHand() {
        while (hand.Count > 0) DiscardCard(hand[0]);
    }

    /// <summary>
    /// Physically arranges all cards in hand to make them visually appealing.
    /// </summary>
    public void ArrangeCardsInHand() {
        for (int i = 0; i < hand.Count; i++)
        {
            MathA.Interpolate(
                hand[i].getPhysicalCard(), 
                new Vector2(MathA.GetSpread(i, hand.Count, 0, 2.1f), MathA.GetSpread(getID(), 2, 0, 10)), 
                0.5f);
        }
    }

    public static void SetControllingPlayer(int index) {
        for (int i = 0; i < players.Length;i++) {
            if (i == index)
            {
                players[i].currentPlayer = true;
                UIScript.instance.SetMana(players[i].mana);
            }
            else players[i].currentPlayer = false;
        }
    }
}
