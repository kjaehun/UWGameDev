using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardData.Element;

/// <summary>
/// Allows for putting abilities onto the battle field.
/// An ability could be an attack, defend, or something else.
/// </summary>
public class Ability
{   
    #region instance fields
    /// <summary>
    /// Number of turns the ability will persist for. Essentially its lifespan.
    /// </summary>
    protected int numTurns;
    /// <summary>
    /// Element of the ability.
    /// </summary>
    protected CardData.Element element;

    /// <summary>
    /// Physical representation of the ability.
    /// If no physical representation exists, then this is equal to null.
    /// </summary>
    protected AbilityRepresentation representation;
    #endregion


    #region constructors
    /// <summary>
    /// Constructor for an ability.
    /// Ideally, this is never called directly.
    /// TODO make this class "abstract" or whatever so it cannot be instantiated
    /// </summary>
    /// <param name="lifespan">lifespan of created ability</param>
    public Ability(int lifespan, CardData.Element element) {
        this.numTurns = lifespan;
        this.element = element;
    }
    /// <summary>
    /// Deconstructor.
    /// </summary>
    ~Ability() {
        // Consider putting debugging things in here.
        // Maybe check to ensure abilities are adequately disposed of after use?
        // Ensure they aren't kept around for some arbitrary reason?
        // TODO bugtests this
    }
    #endregion

    #region getters and setters
    /// <summary>
    /// Decreases the lifespan of this ability by one.
    /// </summary>
    public void DecrementTurns() {
        numTurns --;
        if (representation != null) {
            representation.UpdateVisuals();
        }
    }

    /// <summary>
    /// Getter for the element of the ability.
    /// </summary>
    /// <returns>element of ability</returns>
    public CardData.Element getElement() { return element; }

    /// <summary>
    /// Determines whether the ability has lived beyond its lifespan.
    /// </summary>
    /// <returns>true if the lifespan is less than or equal to zero, false otherwise</returns>
    public bool getExpired() {
        return (numTurns <= 0);
    }

    /// <summary>
    /// Determines whether this ability has the same element as passed.
    /// </summary>
    /// <param name="element">element to compare</param>
    /// <returns>true if the element of the ability matches the argument, false otherwise</returns>
    public bool isElement(CardData.Element element) {
        return (this.element == element);
    }

    /// <summary>
    /// Getter for the lifespan.
    /// </summary>
    /// <returns>lifespan of the ability in number of turns</returns>
    public int getLifeSpan() {
        return numTurns;
    }

    /// <summary>
    /// Getter for the value of the ability.
    /// Defaults to zero for standard abilities.
    /// Should be overriden by child classes to provide a relevant result.
    /// This value is the amount that should be displayed next to the ability on the battle field.
    /// For example, an Attack ability would return its damage.
    /// </summary>
    /// <returns>the value of the ability</returns>
    public virtual int getValue() {
        return 0;
    }
    /// <summary>
    /// Sets the physical representation of this ability.
    /// </summary>
    /// <param name="rep">physical representation to attach to this ability</param>
    public void setRepresentation(AbilityRepresentation rep) {
        this.representation = rep;
    }
    /// <summary>
    /// Destroys the physical representation.
    /// </summary>
    public void DestroyRepresentation() {
        GameObject.Destroy(representation.gameObject);
        representation = null;
    }
    #endregion


    #region gameplay manipulators


    /// <summary>
    /// Called on an ability after every turn.
    /// Override this to apply special actions, such as refreshing the defense value of a Defend ability.
    /// </summary>
    public virtual void refresh() {
        // currently does nothing on its own, but the option is left open rn
    }

    /// <summary>
    /// Sets the position of the physical representation.
    /// Precondition: a physical representation must exist.
    /// </summary>
    /// <param name="pos">LOCAL position to become the position of the representation</param>
    public void setRepresentationPosition(Vector2 pos) {
        representation.gameObject.GetComponent<Transform>().localPosition = pos;
    }

    #endregion

    /// <summary>
    /// Allows for the usage of abilities which deal attack damage each turn.
    /// </summary>
    public class Attack : Ability {

        /// <summary>
        /// Amount of damage to deal each turn.
        /// </summary>
        private int dmg;

        /* private int[] affliction; // [0] -> key; [1] -> amount

        public Attack(int dmg, int[] affliction, int lifespan) : base(lifespan) {
            this.dmg = dmg;
            this.affliction = affliction;
        }
        */

        /// <summary>
        /// Constructor for an Attack ability.
        /// </summary>
        /// <param name="dmg">damage to deal each turn</param>
        /// <param name="element">element of ability</param>
        /// <param name="lifespan">lifespan of ability</param>
        public Attack(int dmg, CardData.Element element, int lifespan) : base(lifespan,element) {
            this.dmg = dmg;
        }

        /// <summary>
        /// Runs sequenced functions which instruct this ability to deal damage to the battle field.
        /// </summary>
        /// <param name="direction">player to attack; 0-> first player, 1-> second player</param>
        /// <param name="field">BattleField to deal damage to; usually it is the same as the ability's battle field</param>
        public void DealDamage(int direction, BattleField field) {
            Sequencer.Add(new Sequencer.MethodEvent(representation.PlayActivateAnimation));
            Sequencer.Add(new Sequencer.DelayEvent(0.5f));
            Sequencer.Add(new Sequencer.FieldTakeDamageEvent(direction, dmg, field));
            Sequencer.Add(new Sequencer.DelayEvent(0.25f));
        }

        /// <summary>
        /// Defines getValue to return the damage of the ability.
        /// </summary>
        /// <returns>damage of this attack</returns>
        public override int getValue()
        {
            return dmg;
        }

    }

    /// <summary>
    /// Allows for the usage of abilities which can block an amount of damage each turn.
    /// </summary>
    public class Defend : Ability {
        /// <summary>
        /// Maximum defense of the Defend ability.
        /// </summary>
        private int maxDef;
        /// <summary>
        /// Current defense of the Defend ability.
        /// Defense is restored to its maximum after each turn.
        /// </summary>
        private int currentDef;


        /// <summary>
        /// Constructor for a Defend ability.
        /// </summary>
        /// <param name="def">initial defense and max defense</param>
        /// <param name="element">element of ability</param>
        /// <param name="lifespan">lifespan of ability in turns</param>
        public Defend(int def, CardData.Element element, int lifespan) : base(lifespan, element) {
            this.maxDef = def;
            this.currentDef = def;
        }

        /// <summary>
        /// Makes this ability take damage.
        /// Any overflow damage is returned as an integer.
        /// Ex: if the ability only had 2 defense but 3 damage was dealt, a value of 1 would be returned.
        /// Ex: if the ability had 4 defense but 3 damage was dealt, it would now have 1 defense and a 0 would be returned.
        /// </summary>
        /// <param name="dmg">damage to take</param>
        /// <returns>overflow damage</returns>
        public int TakeDamage(int dmg) {
            int diff = Mathf.Min(currentDef, dmg);

            currentDef -= diff;
            dmg -= diff;
            representation.UpdateVisuals();

            return dmg;
        }

        /// <summary>
        /// Defines getValue to return the defense amount of the Defend ability.
        /// </summary>
        /// <returns>defense of the ability</returns>
        public override int getValue()
        {
            return currentDef;
        }

        /// <summary>
        /// Defines refresh to reset the defense of the Defend ability.
        /// </summary>
        public override void refresh() {
            currentDef = maxDef;
            base.refresh();
        }
    }

    // TODO
    // implement functions for each skill categories (e.g. draw more cards, reduce enemy mana next turn, etc)
}
