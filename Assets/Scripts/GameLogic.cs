using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardData.Element;
using static CardData.Type;

/// <summary>
/// Controls the basic logical flow of a match.
/// Manages the following:
/// 1. starting battles, initializing players
/// 2. ending turns
/// 3. running combat
/// 4. determining when the battle is over
/// </summary>
public class GameLogic : MonoBehaviour
{
    /// <summary>
    /// Used for "instance" strategy.
    /// </summary>
    public static GameLogic instance;

    // legacy system -- change these
    /// <summary>
    /// Current card which the player is manipulating.
    /// </summary>
    private CardData controlledCard;
    /// <summary>
    /// Used to (attempt) to ensure that only one card may be dragged at once.
    /// </summary>
    private bool controlledCardChanged;

    /// <summary>
    /// Index of player who currently has control over the game.
    /// Ex: If controllingPlayerIndex = 0, then the user can move player 1's cards, play those cards, use his mana, etc..
    /// </summary>
    private int controllingPlayerIndex;

    /// <summary>
    /// Array of all three battle fields present in the game.
    /// Begins empty, but populated when a match begins.
    /// </summary>
    private BattleField[] battleFields = new BattleField[3];

    /// <summary>
    /// This is a little complicated but here is the idea:
    /// When a turn begins, immediately after the cards are drawn, this array takes a "snapshot" of the cards.
    /// Think of this "snapshot" as a picture, which is used to show the opponent the proper things.
    /// The snapshot only records the types of each card (ATTACK, DEFEND, SKILL).
    /// Thus, the first list maps cards of the first player, and the second list maps cards of the second player.
    /// Depending on which player is active, the opposite player's card backs are drawn onto the screen using the card backs.
    /// </summary>
    private List<CardData.Type>[] cardBacksSnapshot = new List<CardData.Type>[2];


    void Awake() {
        instance = this;
    }

    // TODO
    // create a lot of separate functions just for initializing specific cards?
    // like public CardData smogDefense(arguments) {return smog defense card}

    //CardData(string name, string description, Type type, Element element, int manaCost, int dmg, int def, int duration, int skill)

    /// <summary>
    /// Starts a game. 
    /// Basically what should happen when someone runs the .exe file.
    /// Creates two players, initializes all the cards, adds cards to decks...
    /// </summary>
    public void StartGame() {
        PlayerData player1 = new PlayerData();
        PlayerData player2 = new PlayerData();

        CardData card1 = new CardData("Basic Attack", "Deal 4 damage each turn for 2 turns.", CardData.Type.ATTACK, CardData.Element.SLUDGE, 1, 4, 0, 2, 0);
        CardData card2 = new CardData("Slow Attack", "Deal 2 damage each turn for 5 turns.", CardData.Type.ATTACK, CardData.Element.SLUDGE, 1, 2, 0, 5, 0);
        CardData card3 = new CardData("Basic Defend", "Block 3 damage each turn for 2 turns.", CardData.Type.DEFEND, CardData.Element.SLUDGE, 1, 0, 3, 5, 0);
        CardData card4 = new CardData("Quick Defend", "Block 5 damage each turn for 1 turn.", CardData.Type.DEFEND, CardData.Element.SLUDGE, 1, 0, 5, 1, 0);

        // CardData card1 = new CardData("Basic Attack", "Deal 4 damage each turn for 2 turns.", 1, CardData.Tools.MakeAttack(4,2));
        // CardData card2 = new CardData("Slow Attack", "Deal 2 damage each turn for 5 turns.", 1, CardData.Tools.MakeAttack(2, 5));
        // CardData card3 = new CardData("Basic Defend", "Block 3 damage each turn for 2 turns.", 1, CardData.Tools.MakeDefend(3, 1, 2));
        // CardData card4 = new CardData("Quick Defend", "Block 5 damage each turn for 1 turn.", 1, CardData.Tools.MakeDefend(5, 2, 1));

        for (int i = 0; i < 3;i++) {
            player1.AddToDeck(card1);
            player2.AddToDeck(card1);
        }
        for (int i = 0; i < 3;i++) {
            player1.AddToDeck(card2);
            player2.AddToDeck(card2);
        }
        for (int i = 0; i < 3;i++) {
            player1.AddToDeck(card3);
            player2.AddToDeck(card3);
        }
        for (int i = 0; i < 3;i++) {
            player1.AddToDeck(card4);
            player2.AddToDeck(card4);
        }
            
    }
    /// <summary>
    /// Begins a battle between two players.
    /// Generates the player health bars, prepares their cards, and sets up one player to take control.
    /// </summary>
    public void StartBattle() {
        PlayerData[] players = new PlayerData[2] { PlayerData.GetPlayer(0), PlayerData.GetPlayer(1) };
        players[0].setHealthBar(HealthBarController.MakeHealthBar(0,30));
        players[1].setHealthBar(HealthBarController.MakeHealthBar(1,30));

        players[0].PrepareCards();
        players[1].PrepareCards();

        controllingPlayerIndex = 0;
        PlayerData.SetControllingPlayer(controllingPlayerIndex);
    }
    /// <summary>
    /// Starts a turn.
    /// Lets players draw all their cards and arranges them in the hand.
    /// Resets the mana of both players.
    /// </summary>
    public void StartTurn() {
        PlayerData[] players = new PlayerData[] { PlayerData.GetPlayer(0),PlayerData.GetPlayer(1)};

        for (int pl = 0; pl < 2;pl++) {
            // TODO change from literal '5' to allow players to draw a varied amount of cards
            // depending on circumstance
            for (int i = 0; i < 5; i++)
            {
                players[pl].DrawCard();
            }
            // TODO change from literal '3' to something else to make alterable
            players[pl].setMana(3);
        }

        TakeSnapshot();

        // build cards depending on active player

        RenderCards();
    }

    /// <summary>
    /// Called by GameLogic.StartTurn.
    /// Takes a "snapshot" of both player's hands.
    /// See GameLogic.cardBacksSnapshot for more info.
    /// </summary>
    public void TakeSnapshot() {
        for (int pl = 0; pl < 2;pl++) {
            cardBacksSnapshot[pl] = new List<CardData.Type>();

            List<CardData> hand = PlayerData.GetPlayer(pl).getHand();

            foreach (CardData card in hand) {
                cardBacksSnapshot[pl].Add(card.getCardType());
            }
        }

    }
    /// <summary>
    /// Ends both players turns.
    /// Plays all cards from each player.
    /// Enacts each battle field to determine what occurs.
    /// </summary>
    public void EndTurns() {

        // play cards from each player
        PlayerData[] players = PlayerData.GetPlayers();
        foreach (PlayerData player in players) {
            List<CardData> hand = player.getHand();
            while (hand.Count > 0) {
                hand[0].PlayCard();
                player.DiscardCard(hand[0]);
            }
        }


        for (int i = 0; i < 2;i++) {
            PlayerData.GetPlayer(i).DiscardHand();
        }
            foreach (BattleField battleField in battleFields)
            {
                battleField.EnactBattle();
            }
        if (!PlayerData.GetPlayer(0).isAlive()) {
            Debug.Log("Player 2 wins!");
        } else if (!PlayerData.GetPlayer(1).isAlive()) {
            Debug.Log("Player 1 wins!");
        }
        Sequencer.Add(new Sequencer.MethodEvent(StartTurn));
    }
    /// <summary>
    /// Called by Unity on start.
    /// Generates three battle fields.
    /// TODO this is bad! Do NOT do it this way! 
    /// The battle fields should be generated only when a match begins, and ought to be removed after.
    /// </summary>
    void Start() {
        // make battlefields
        for (int i = 0; i < 3;i++) {
            GameObject thing = GameObject.Instantiate(
                GameAssets.inst.battleFieldPrefab, new Vector2(MathA.GetSpread(i, 3, 0, 4.1f), 0), Quaternion.identity
            );

            battleFields[i] = thing.GetComponent<BattleField>();
        }

        StartGame();

        StartBattle();

        StartTurn();
    }

    /// <summary>
    /// Gets input from the player.
    /// Currently only used for dev tools and moving cards around.
    /// </summary>
    void Update() {
        if (Input.GetKeyDown(KeyCode.G)) {
            controllingPlayerIndex = 1 - controllingPlayerIndex;
            Debug.Log("Controlling player: " + controllingPlayerIndex);

            PlayerData.SetControllingPlayer(controllingPlayerIndex);
            RenderCards();
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            Debug.Log("Ending Turns");
            EndTurns();
        }

        if (Input.GetMouseButtonUp(0)  && controlledCard != null) {
            bool cardDesignated = false;
            foreach (BattleField battleField in battleFields) {
                if (battleField.getMouseIn(Camera.main.ScreenToWorldPoint(Input.mousePosition))) {
                    PlayerData.GetPlayer(controllingPlayerIndex).AttemptPlayCard(battleField, controlledCard);
                    cardDesignated = true;
                    break;
                }
            }
            if (!cardDesignated) PlayerData.GetPlayer(controllingPlayerIndex).AttemptPlayCard(null, controlledCard);
        }

        if (!controlledCardChanged) controlledCard = null;

        controlledCardChanged = false;
    }

    /// <summary>
    /// Sets the card which the user is currently moving. legacy
    /// </summary>
    /// <param name="card">card being moved</param>
    public void SetControlledCard(CardData card) {
        this.controlledCard = card;
        this.controlledCardChanged = true;
    }
    /// <summary>
    /// Gets the player who is currently in control.
    /// </summary>
    /// <returns>PlayerData of controlling player</returns>
    public PlayerData getCurrentController() {
        return PlayerData.GetPlayer(controllingPlayerIndex);
    }

    /// <summary>
    /// Renders cards onto the screen.
    /// Places opponent card backs onto the top of the screen, along with all player cards in their respective locations.
    /// TODO optomize
    /// </summary>
    public void RenderCards() {
        // step 0: delete all cards
        CardBackController.DestroyAllCardBacks();
        for (int pl = 0; pl < 2;pl++) {
            foreach (CardData card in PlayerData.GetPlayer(pl).getHand()) {
                card.DestroyPhysicalCard();
            }
        }

        // step 1: make card backs of opponent & put them at their intended positions
        List<CardData.Type> opponentTypes = cardBacksSnapshot[1 - controllingPlayerIndex];

        for (int i = 0; i < opponentTypes.Count; i++)
        {
            // creates new card back
            GameObject newBack = CardBackController.MakeCardBack(opponentTypes[i]);
            // moves new card back to its intended location
            newBack.GetComponent<Transform>().position = new Vector2(MathA.GetSpread(i, opponentTypes.Count, 0, 2.1f), 5f);
        }

        // step 2: make cards of controlling player & put them at their intended locations
        {
            // set up all the lists for where cards could exist
            // this is necessary because getting their indices will be important later for spreading
            List<GameObject> heldCards = new List<GameObject>();
            List<GameObject>[] battleFieldCards = new List<GameObject>[battleFields.Length];
            for (int i = 0; i < battleFieldCards.Length;i++) {
                battleFieldCards[i] = new List<GameObject>();
            }


            List<CardData> playerHand = PlayerData.GetPlayer(controllingPlayerIndex).getHand();
            // goes through each card in the hand and puts it into its proper category
            foreach (CardData card in playerHand) {
                GameObject physicalCard = card.MakePhysicalCard();

                BattleField playLocation = card.getPlayLocation();
                if (playLocation == null) heldCards.Add(physicalCard);
                else {
                    for (int i = 0; i < battleFields.Length;i++) {
                        if (playLocation.Equals(battleFields[i])) battleFieldCards[i].Add(physicalCard);
                    }
                }
            }

            // place all cards at their intended locations
            for (int i = 0; i < heldCards.Count;i++) {
                heldCards[i].GetComponent<Transform>().position = new Vector2(MathA.GetSpread(i, heldCards.Count, 0, 2.1f), -5);
            }
            for (int field = 0; field < battleFieldCards.Length; field++) {
                for (int i = 0; i < battleFieldCards[field].Count;i++) {
                    battleFieldCards[field][i].GetComponent<Transform>().position =
                    new Vector2(MathA.GetSpread(i, battleFieldCards[field].Count, 0, 2.1f), 0)
                    + (Vector2)battleFields[field].gameObject.GetComponent<Transform>().position;
                }
            }
        }




    }
}
