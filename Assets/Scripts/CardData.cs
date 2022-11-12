using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    #region enums
    /// <summary>
    /// Data type to determine the type of a card.
    /// </summary>
    public enum Type {
        ATTACK,
        DEFEND,
        SKILL
    }
    /// <summary>
    /// Data type to determine the element of a card.
    /// </summary>
    public enum Element {
        SMOG,
        SLUDGE,
        RADIOACTIVITY,
        WATER,
        OIL
    }
    #endregion

    #region instance fields

    /// <summary>
    /// Associated physical card of this specific data. If null, then no physical manifestation exists.
    /// </summary>
    private GameObject physicalCard;

    /// <summary>
    /// Name of the card.
    /// </summary>
    private string cardName;
    /// <summary>
    /// Description of the card's effects.
    /// </summary>
    private string description;

    /// <summary>
    /// Mana cost to play card.
    /// </summary>
    private int manaCost;
    /// <summary>
    /// Type of card (attack, defend, skill).
    /// </summary>
    private Type type;
    /// <summary>
    /// Element of card.
    /// </summary>
    private Element element;

    /// <summary>
    /// Damage dealt by this card.
    /// If unapplicable, set to 0 (e.g. defend cards should have 0 dmg).
    /// </summary>
    private int dmg;
    /// <summary>
    /// Defense stat of this card.
    /// If unapplicable, set to 0 (e.g. attack cards should have 0 def).
    /// </summary>
    private int def;
    /// <summary>
    /// Number of turns spent on field.
    /// </summary>
    private int duration;
    private int skill; // 0 if not a skill, some other integer if it is a skill (maybe map it to different triggers?)

    /// <summary>
    /// Reference to the owner of this card.
    /// </summary>
    private PlayerData owner;

    #endregion

    #region getters and setters
    /// <summary>
    /// Getter for the name.
    /// </summary>
    /// <returns>card name</returns>
    public string getName() {return cardName;}
    /// <summary>
    /// Getter for the description.
    /// </summary>
    /// <returns>card description</returns>
    public string getDescription() {return description;}
    /// <summary>
    /// Getter for the type.
    /// </summary>
    /// <returns>card type</returns>
    public Type getCardType() {return type;}
    /// <summary>
    /// Getter for the mana cost.
    /// </summary>
    /// <returns>mana cost of card</returns>
    public int getManaCost() {return manaCost;}

    /// <summary>
    /// Sets the owner of this card.
    /// </summary>
    /// <param name="owner">PlayerData to make owner</param>
    public void setOwner(PlayerData owner) {
        this.owner = owner;
    }

    /// <summary>
    /// Getter for card owner.
    /// </summary>
    /// <returns>owner of card as PlayerData</returns>
    public PlayerData getOwner() { return owner; }

    /// <summary>
    /// Getter for physical card as a GameObject. If null, no such object exists.
    /// </summary>
    /// <returns>physical card of this card</returns>
    public GameObject getPhysicalCard() { return physicalCard; }

    /// <summary>
    /// String representation of card. Used for debugging.
    /// </summary>
    /// <returns>string version of card</returns>
    public override string ToString() {
        return "Card: " + cardName;
    }

    #endregion

    #region constructors
    /// <summary>
    /// Constructor for a CardData instance.
    /// </summary>
    /// <param name="name">name of card</param>
    /// <param name="description">description of card's actions</param>
    /// <param name="type">attack, defend, or skill</param>
    /// <param name="element">element of card</param>
    /// <param name="manaCost">mana cost of card</param>
    /// <param name="dmg">damage dealt by card each turn</param>
    /// <param name="def">damage blocked by card each turn</param>
    /// <param name="duration">number of turns spent on the field before expiration</param>
    /// <param name="skill">integer to determine potential skill effects</param>
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
    /// <summary>
    /// Creates an identical copy of this data. 
    /// This allows for many different physical cards to exist which have the same data.
    /// This, however, does mean that all these cards have distinct CardData instances.
    /// </summary>
    /// <returns>new CardData following clone</returns>
    public CardData clone() {
        return new CardData(this.cardName, this.description, this.type, this.element, this.manaCost, this.dmg, this.def, this.duration, this.skill);
    }
    #endregion

    #region physical card methods
    /// <summary>
    /// Constructs a physical manifestation of this card at a location in the world.
    /// </summary>
    /// <param name="pos">location to place card</param>
    public void MakePhysicalCard(Vector2 pos) {
        if (physicalCard != null) GameObject.Destroy(physicalCard);
        
        physicalCard = GameObject.Instantiate(GameAssets.inst.cardPrefab, pos, Quaternion.identity);
        physicalCard.GetComponent<CardController>().Setup(this);
    }
    /// <summary>
    /// Constructs a physical manifestation of this card at an arbitrary locataion in the world.
    /// </summary>
    public void MakePhysicalCard() {
        MakePhysicalCard(Vector2.zero);
    }
    /// <summary>
    /// Destroys the physical card associated with this card.
    /// </summary>
    public void DestroyPhysicalCard() {
        GameObject.Destroy(physicalCard);
        physicalCard = null;
    }
    #endregion

    #region methods for playing cards
    /// <summary>
    /// Plays a card from a certain player onto the battle field.
    /// </summary>
    /// <param name="player">player which plays the card</param>
    /// <param name="field">BattleField onto which the card is played</param>
    public void PlayCard(int player, BattleField field) {
        List<Ability> abilities = FormatAbilities();

        foreach (Ability ability in abilities) {
            field.AddAbility(player,ability);
        }

        DestroyPhysicalCard();
    }

    /// <summary>
    /// If dmg, def, or skill are positive, returns an ability associated with that value
    /// </summary>
    /// <returns></returns>
    public List<Ability> FormatAbilities() {
        List<Ability> abilities = new List<Ability>();
        if (this.dmg > 0) {
            Ability.Attack attack = new Ability.Attack(dmg,element,duration);
            abilities.Add(attack);
        }
        if (this.def > 0) {
            Ability.Defend defend = new Ability.Defend(def,element,duration);
            abilities.Add(defend);
        }
        if (this.skill > 0) {
            ; //TODO
        }
        return abilities;
    }
    #endregion
    
}