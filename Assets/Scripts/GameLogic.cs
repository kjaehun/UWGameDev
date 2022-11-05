using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public static GameLogic instance;
    /*
    needs to manage the following:
    1. starting battles, initializing players
    2. ending turns
    3. running combat
    4. determining when the battle is over
    */

    // legacy system
    private CardData controlledCard;
    private bool controlledCardChanged;

    private int controllingPlayerIndex;

    BattleField[] battleFields = new BattleField[3];

    void Awake() {
        instance = this;
    }

    public void StartGame() {
        PlayerData player1 = new PlayerData();
        PlayerData player2 = new PlayerData();

        CardData card1 = new CardData("Basic Attack", "Deal 4 damage each turn for 2 turns.", 1, CardData.Tools.MakeAttack(4,2));
        CardData card2 = new CardData("Slow Attack", "Deal 2 damage each turn for 5 turns.", 1, CardData.Tools.MakeAttack(2, 5));
        CardData card3 = new CardData("Basic Defend", "Block 3 damage each turn for 2 turns.", 1, CardData.Tools.MakeDefend(3, 1, 2));
        CardData card4 = new CardData("Quick Defend", "Block 5 damage each turn for 1 turn.", 1, CardData.Tools.MakeDefend(5, 2, 1));

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

    public void StartBattle() {
        PlayerData[] players = new PlayerData[2] { PlayerData.GetPlayer(0), PlayerData.GetPlayer(1) };
        players[0].SetHealthBar(StatusBarController.GenerateBar(gameObject, 3.5f*Vector2.down));
        players[1].SetHealthBar(StatusBarController.GenerateBar(gameObject, 3.5f*Vector2.up));

        players[0].PrepareCards();
        players[1].PrepareCards();

        controllingPlayerIndex = 0;
    }

    public void StartTurn() {
        for (int i = 0; i < 5;i++){
            PlayerData.GetPlayer(0).DrawCard();
            PlayerData.GetPlayer(1).DrawCard();
        }

        PlayerData.GetPlayer(0).ArrangeCardsInHand();
        PlayerData.GetPlayer(1).ArrangeCardsInHand();
    }

    public void EndTurns() {
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
        StartTurn();
    }

    void Start() {
        // make battlefields
        for (int i = 0; i < 3;i++) {
            GameObject thing = GameObject.Instantiate(
                GameAssets.inst.battleFieldPrefab, new Vector3(MathA.GetSpread(i, 3, 0, 4.0f), 0, 0), Quaternion.identity
            );

            battleFields[i] = thing.GetComponent<BattleField>();
        }

        StartGame();

        StartBattle();

        StartTurn();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.G)) {
            controllingPlayerIndex = 1 - controllingPlayerIndex;
            Debug.Log("Controlling player: " + controllingPlayerIndex);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            Debug.Log("Ending Turns");
            EndTurns();
        }

        if (Input.GetMouseButtonUp(0)  && controlledCard != null) {
            foreach (BattleField battleField in battleFields) {
                if (battleField.getMouseIn(Camera.main.ScreenToWorldPoint(Input.mousePosition))) {
                    PlayerData.GetPlayer(controllingPlayerIndex).AttemptPlayCard(battleField, controlledCard);
                }
            }
        }

        if (!controlledCardChanged) controlledCard = null;

        controlledCardChanged = false;
    }

    public void SetControlledCard(CardData card) {
        this.controlledCard = card;
        this.controlledCardChanged = true;
    }

    public PlayerData getCurrentController() {
        return PlayerData.GetPlayer(controllingPlayerIndex);
    }
}
