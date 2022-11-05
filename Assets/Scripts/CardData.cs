using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    GameObject physicalCard;

    private string cardName;
    private string description;

    private byte cardElement; // smog, sludge, radioactivity, water, oil
    private byte cardType; // attack, defend, skill
    private int manaCost;

    private PlayerData owner;

    private static class IndexCodes {
        public static readonly int TYPE = 0;
        public static readonly int LIFESPAN = 1;
        public static readonly int PVAL = 2; // primary value. used for things like damage and defense numbers
        public static readonly int SKEY = 3; // secondary key. used for things like denoting affliction type or element
        public static readonly int SVAL = 4; // secondary value. used for things like denoting affliction amount
    }
    private int[,] vals;

    public string getName() {return cardName;}
    public string getDescription() {return description;}
    public byte getCardType() {return cardType;}
    public int getManaCost() {return manaCost;}

    public CardData(string name, string description, int manaCost, int[,] vals) {
        this.cardName = name;
        this.description = description;
        this.manaCost = manaCost;
        this.vals = vals;
    }

    public void PlayCard(int player, BattleField field) {
        List<Ability> abilities = FormatAbilities();

        foreach (Ability ability in abilities) {
            field.AddAbility(player,ability);
        }

        DestroyPhysicalCard();
    }

    public void DestroyPhysicalCard() {
        GameObject.Destroy(physicalCard);
        physicalCard = null;
    }

    
    public List<Ability> FormatAbilities() {
        List<Ability> abilities = new List<Ability>();
        for (int i=0;i<vals.GetLength(0);i++) {
            if (vals[i,IndexCodes.TYPE] == Ability.Codes.ATTACK) {

                int dmg = vals[i,IndexCodes.PVAL];
                int[] affliction = new int[2] {vals[i,IndexCodes.SKEY],vals[i,IndexCodes.SVAL]};
                int duration = vals[i,IndexCodes.LIFESPAN];


                Ability.Attack attack = new Ability.Attack(dmg,affliction,duration);
                abilities.Add(attack);

            } else if (vals[i,IndexCodes.TYPE] == Ability.Codes.DEFEND) {

                int def = vals[i,IndexCodes.PVAL];
                int duration = vals[i,IndexCodes.LIFESPAN];
                byte element = (byte) vals[i,IndexCodes.SKEY];


                Ability.Defend defend = new Ability.Defend(def, element, duration);
                abilities.Add(defend);

            } else if (vals[i,IndexCodes.TYPE] == Ability.Codes.OTHER) {
                // Do nothing just yet... TODO when other types of abilities are implemented
            }
        }

        return abilities;
    }


    public void MakePhysicalCard(Vector2 pos) {
        if (physicalCard != null) GameObject.Destroy(physicalCard);
        physicalCard = GameObject.Instantiate(GameAssets.inst.cardPrefab, pos, Quaternion.identity);
        physicalCard.GetComponent<CardController>().Setup(this);
    }

    public void MakePhysicalCard() {
        MakePhysicalCard(Vector2.zero);
    }


    public static void Shuffle(List<CardData> list) {
        // option 1: make a random, injective transformation T: [0, list.length-1] -> [0, list.length-1]
        // option 2: fuckshit method
        //      option 2 is much easier
        int thoroughness = 3;

        for (int z=0;z<thoroughness;z++) {
            for (int i=0; i<list.Count;i++) {
                int removeIndex = Random.Range(0,list.Count);
                int addIndex = Random.Range(0, list.Count);

                CardData removedCard = list[removeIndex];
                list.RemoveAt(removeIndex);
                list.Insert(addIndex, removedCard);
            }
        }

    }



    public static class Tools {
        public static int[,] MakeAttack(int dmg, int afflictionKey, int afflictionAmount, int duration) {
            return new int[1, 5] {
                {Ability.Codes.ATTACK,duration,dmg,afflictionKey,afflictionAmount}
            };
        }
        public static int[,] MakeAttack(int dmg, int duration) {
            return MakeAttack(dmg, -1, 0, duration);
        }
        public static int[,] MakeDefend(int def, int element, int duration) {
            return new int[1, 5] {
                {Ability.Codes.DEFEND,duration,def,element,0}
            };
        }
    }

    public CardData clone() {
        return new CardData(this.cardName, this.description, this.manaCost, this.vals);
    }

    public void setOwner(PlayerData owner) {
        this.owner = owner;
    }

    public PlayerData getOwner() { return owner; }

    public override string ToString() {
        return "Card: " + cardName;
    }

    public GameObject getPhysicalCard() { return physicalCard; }


}
