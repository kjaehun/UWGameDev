using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardData.Element;

public class Ability
{   
    protected int numTurns;
    protected CardData.Element element;
    protected AbilityRepresentation representation;

    public Ability(int lifespan) {
        this.numTurns = lifespan;
    }

    public void DecrementTurns() {
        numTurns --;
        if (representation != null) {
            representation.UpdateVisuals();
        }
    }

    public CardData.Element getElement() { return element; }

    public bool getExpired() {
        return (numTurns <= 0);
    }

    public int getLifeSpan() {
        return numTurns;
    }

    public virtual int getValue() {
        return 0;
    }

    public virtual void refresh() {
        // currently does nothing on its own, but the option is left open rn
    }

    public void setRepresentation(AbilityRepresentation rep) {
        this.representation = rep;
    }

    public void setRepresentationPosition(Vector2 pos) {
        representation.gameObject.GetComponent<Transform>().localPosition = pos;
    }

    public void DestroyRepresentation() {
        GameObject.Destroy(representation.gameObject);
        representation = null;
    }

    public class Attack : Ability {
        private int dmg;

        /* private int[] affliction; // [0] -> key; [1] -> amount

        public Attack(int dmg, int[] affliction, int lifespan) : base(lifespan) {
            this.dmg = dmg;
            this.affliction = affliction;
        }
        */

        public Attack(int dmg, CardData.Element element, int lifespan) : base(lifespan) {
            this.dmg = dmg;
            this.element = element;
        }

        public void DealDamage(int direction, BattleField field) {
            Sequencer.Add(new Sequencer.MethodEvent(representation.PlayActivateAnimation));
            Sequencer.Add(new Sequencer.DelayEvent(0.5f));
            Sequencer.Add(new Sequencer.FieldTakeDamageEvent(direction, dmg, field));
            Sequencer.Add(new Sequencer.DelayEvent(0.25f));
        }

        public override int getValue()
        {
            return dmg;
        }

        public override void refresh()
        {
            base.refresh();
        }

    }

    public class Defend : Ability {
        private int maxDef;
        private int currentDef;

        // private byte element;

        // public Defend(int def, byte element, int lifespan) : base(lifespan) {
        //     this.maxDef = def;
        //     this.currentDef = def;

        //     this.element = element;
        // }


        public Defend(int def, CardData.Element element, int lifespan) : base(lifespan) {
            this.maxDef = def;
            this.currentDef = def;

            this.element = element;
        }

        public int TakeDamage(int dmg) {
            int diff = Mathf.Min(currentDef, dmg);

            currentDef -= diff;
            dmg -= diff;
            representation.UpdateVisuals();

            return dmg;
        }

        public bool isElement(CardData.Element element) {
            return (this.element == element);
        }

        public override int getValue()
        {
            return currentDef;
        }

        public override void refresh() {
            currentDef = maxDef;
            base.refresh();
        }
    }

    // TODO
    // implement functions for each skill categories (e.g. draw more cards, reduce enemy mana next turn, etc)
}
