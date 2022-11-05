using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    public static class Codes {
        public static readonly int ATTACK = 1;
        public static readonly int DEFEND = 2;
        public static readonly int OTHER = 3;
    }
    
    protected int numTurns;

    public Ability(int lifespan) {
        this.numTurns = lifespan;
    }

    public void DecrementTurns() {
        numTurns --;
    }

    public bool getExpired() {
        return (numTurns <= 0);
    }

    public int getLifeSpan() {
        return numTurns;
    }

    public virtual int getValue() {
        return 0;
    }

    public class Attack : Ability {
        private int dmg;

        private int[] affliction; // [0] -> key; [1] -> amount

        public Attack(int dmg, int[] affliction, int lifespan) : base(lifespan) {
            this.dmg = dmg;
            this.affliction = affliction;
        }

        public void DealDamage(int direction, BattleField field) {
            field.TakeDamage(direction, dmg);
        }

        public override int getValue()
        {
            return dmg;
        }

    }

    public class Defend : Ability {
        private int maxDef;
        private int currentDef;

        private byte element;

        public Defend(int def, byte element, int lifespan) : base(lifespan) {
            this.maxDef = def;
            this.currentDef = def;

            this.element = element;
        }

        public int TakeDamage(int dmg) {
            int diff = Mathf.Min(currentDef, dmg);

            currentDef -= diff;
            dmg -= diff;
            return dmg;
        }

        public bool isElement(byte element) {
            return (this.element == element);
        }

        public override int getValue()
        {
            return currentDef;
        }
    }
}
