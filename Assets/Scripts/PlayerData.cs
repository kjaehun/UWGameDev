using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    private static PlayerData[] players = new PlayerData[2];

    private List<CardData> deck;
    
    private List<CardData> drawPile;
    private List<CardData> hand;
    private List<CardData> discard;

    private int health;
    private int maxHealth;

    private int mana;

    private StatusBarController healthBar;

    public int getHealth() {return health;}
    
    public PlayerData(int hp) {
        deck = new List<CardData>();

        drawPile = new List<CardData>();
        hand = new List<CardData>();
        discard = new List<CardData>();

        this.health = hp;
        this.maxHealth = hp;

        mana = 4;

        if (players[0] == null) players[0] = this;
        else players[1] = this;

    }
    public PlayerData() : this(30) {}


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

    public void ShuffleDiscardIntoDraw() {
        while (discard.Count > 0) {
            CardData card = discard[discard.Count-1];
            drawPile.Add(card);
            discard.RemoveAt(discard.Count-1);
        }
        CardData.Shuffle(drawPile);
    }

    // called only once to start a match
    public void PrepareCards() {
        drawPile = new List<CardData>();
        hand = new List<CardData>();
        discard = new List<CardData>();

        foreach (CardData card in deck) {
            drawPile.Add(card);
        }
        CardData.Shuffle(drawPile);
    }

    public bool isAlive() {
        return (health >= 0);
    }

    public void AttemptPlayCard(BattleField field, CardData card) {
        if (hasMana(card.getManaCost())) {

            card.PlayCard(getID(),field);

            changeMana(card.getManaCost());

            DiscardCard(card);

        }
    }

    public void TakeDamage(int dmg) {
        health -= Mathf.Min(health,dmg);

        healthBar.SetPercent((float)(health)/(float)(maxHealth));
    }

    public static PlayerData GetPlayer(int n) {
        if (n<0 || n>1) return null;
        return players[n];
    }

    public bool isInHand(CardData card) {
        return hand.Contains(card);
    }


    public bool hasMana(int val) {
        return (mana >= val);
    }
    public void changeMana(int val) {
        mana -= val;
    }
    public void setMana(int val) {
        mana = val;
    }

    public void AddToDeck(CardData card) {
        deck.Add(card.clone());
    }

    public void SetHealthBar(StatusBarController healthBar) {
        this.healthBar = healthBar;
    }

    public int getID() {
        if (this.Equals(players[0])) return 0;
        return 1;
    }

    public void DiscardCard(CardData card) {
        hand.Remove(card);
        discard.Add(card);
        card.DestroyPhysicalCard();
    }

    public void DiscardHand() {
        while (hand.Count > 0) DiscardCard(hand[0]);
    }

    public void ArrangeCardsInHand() {
        for (int i = 0; i < hand.Count; i++)
        {
            MathA.Interpolate(
                hand[i].getPhysicalCard(), 
                new Vector2(MathA.GetSpread(i, hand.Count, 0, 2.1f), MathA.GetSpread(getID(), 2, 0, 5)), 
                0.5f);
        }
    }






}
