using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    public enum Type {
        ATTACK,
        DEFEND,
        SKILL
    }

    public enum Element {
        SMOG,
        SLUDGE,
        RADIOACTIVITY,
        WATER,
        OIL
    }

    GameObject physicalCard;

    private string cardName;
    private string description;

    /// private byte cardElement; // smog, sludge, radioactivity, water, oil
    /// private byte cardType; // attack, defend, skill
    private int manaCost;
    private Type type;
    private Element element;

    // if unapplicable, set to 0 (e.g. defend cards should have 0 dmg)
    private int dmg;
    private int def;
    private int duration;
    private int skill; // 0 if not a skill, some other integer if it is a skill (maybe map it to different triggers?)

    private PlayerData owner;
/*
    private static class IndexCodes {
        public static readonly int TYPE = 0;
        public static readonly int LIFESPAN = 1;
        public static readonly int PVAL = 2; // primary value. used for things like damage and defense numbers
        public static readonly int SKEY = 3; // secondary key. used for things like denoting affliction type or element
        public static readonly int SVAL = 4; // secondary value. used for things like denoting affliction amount
    }
    private int[,] vals;
*/
    public string getName() {return cardName;}
    public string getDescription() {return description;}
    public Type getCardType() {return type;}
    public int getManaCost() {return manaCost;}

    public CardData(string name, string description, Type type, Element element, int manaCost, int dmg, int def, int duration, int skill) {
        this.cardName = name;
        this.description = description;
        this.type = type;
        this.element = element;
        this.manaCost = manaCost;
        this.dmg = dmg;
        this.def = def;
        this.duration = duration;
        this.skill = skill;
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
        switch(type) {
            case Type.ATTACK:
                Ability.Attack attack = new Ability.Attack(dmg,element,duration);
                abilities.Add(attack);
                break;
            case Type.DEFEND:
                Ability.Defend defend = new Ability.Defend(def,element,duration);
                abilities.Add(defend);
                break;
            // TODO
            // case SKILL:
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

    public void setOwner(PlayerData owner) {
        this.owner = owner;
    }

    public PlayerData getOwner() { return owner; }

    public override string ToString() {
        return "Card: " + cardName;
    }

    public GameObject getPhysicalCard() { return physicalCard; }

    public CardData clone() {
        return new CardData(this.cardName, this.description, this.type, this.element, this.manaCost, this.dmg, this.def, this.duration, this.skill);
    }
}
