using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField : MonoBehaviour
{
    /// <summary>
    /// Constant values used for referral in abilities array.
    /// </summary>
    private static class ABILITIES {
        public static readonly int ALL = 0;
        public static readonly int ATTACKS = 1;
        public static readonly int DEFENDS = 2;
        public static readonly int OTHER = 3;    
    }

    List<Ability>[,] abilities = new List<Ability>[2,4];
    // [0,x] = first player; [1,x] = second player
    // [x,0] = all cards; [x,1] = attacks; [x,2] = defends; [x,3] = skills

    /// <summary>
    /// Constant values used for referral in afflictions array.
    /// </summary>
    public static class AFFLICTIONS {
        public static readonly byte CHOKED = 0;
        public static readonly byte SLOW = 1;
        public static readonly byte IRRADIATED = 2;
        public static readonly byte FLOODED = 3;
        public static readonly byte COVERED = 4;
    }

    int[,] afflictions = new int[2,5];
    // [0,x] = first player; [1,x] = second player
    // [x,0] = choked; [x,1] = slow; [x,2] = irradiated; [x,3] = flooded; [x,4] = covered

    /// <summary>
    /// Determines which player should go first in this battle field.
    /// </summary>
    /// <returns>0 if first player should go first, 1 if second player should go first</returns>
    public int CalculatePlayerPriority() {
        return 0;
    }

    /// <summary>
    /// Adds a provided ability onto the battle field for a specific player's side. 
    /// Attacks should be placed on the side that they are attacking from. 
    /// Defends should be placed on the side they are defending.
    /// </summary>
    /// <param name="player">side to place ability; 0-> first player, 1-> second player</param>
    /// <param name="ability">ability to add</param>
    public void AddAbility(int player, Ability ability) {
        abilities[player,ABILITIES.ALL].Add(ability);
        if (ability is Ability.Attack) abilities[player,ABILITIES.ATTACKS].Add(ability);
        if (ability is Ability.Defend) abilities[player,ABILITIES.DEFENDS].Add(ability);
        else abilities[player,ABILITIES.OTHER].Add(ability);
    }

    /// <summary>
    /// Removes provided ability from the player's side.
    /// </summary>
    /// <param name="player">side to place ability; 0-> first player, 1-> second player</param>
    /// <param name="ability">ability to remove</param>
    public void RemoveAbility(int player, Ability ability) {
        for (int i=0;i<4;i++) {
            abilities[player,i].Remove(ability);
        }
    }

    /// <summary>
    /// Enacts the battle at this battle field.
    /// </summary>
    public void EnactBattle() {
        int playerPriority = CalculatePlayerPriority();

        EnactAfflictions(playerPriority);

        EnactAbilities(playerPriority);

        for (int plyr=0; plyr < 2; plyr++) {
            for (int i = 0; i < abilities[plyr, 0].Count;i++) {
                Ability ability = abilities[plyr, 0][i];
                ability.DecrementTurns();
                if (ability.getExpired())
                {
                    RemoveAbility(plyr, ability);
                    i--;
                }
            }
        }
        
    }

    /// <summary>
    /// Enacts all afflictions which should occur to this player within a battle.
    /// </summary>
    /// <param name="player">side to enact afflictions first; 0-> first player, 1-> second player</param>
    private void EnactAfflictions(int player) {
        int scndPlayer = 1-player;

        TakeDamage(player, afflictions[player,AFFLICTIONS.CHOKED]);
        ApplyAffliction(player,new int[] {AFFLICTIONS.CHOKED, -1});

        TakeDamage(scndPlayer, afflictions[scndPlayer,AFFLICTIONS.CHOKED]);
        ApplyAffliction(scndPlayer, new int[] {AFFLICTIONS.CHOKED, -1});

        if (afflictions[player,AFFLICTIONS.IRRADIATED] == 4) TakeDamage(player, (PlayerData.GetPlayer(0).getHealth()+1) / 2);

        if (afflictions[scndPlayer,AFFLICTIONS.IRRADIATED] == 4) TakeDamage(scndPlayer, (PlayerData.GetPlayer(1).getHealth()+1) / 2);

    }

    /// <summary>
    /// Enacts all abilities in sequence, switching between one player and the other. 
    /// First does the first player's first ability, then the second player's first ability, then first player's second ability, etc..
    /// Once someone runs out of abilities, all abilities of the other player are immediately ran in sequence. 
    /// </summary>
    /// <param name="player">side to enact abilities first; 0-> first player, 1-> second player</param>
    private void EnactAbilities(int player) {
        int scndPlayer = 1-player;

        // H_EnactAllCardsInCategory(player,CARDS.SKILLS);

        H_EnactAllAbilitiesInCategory(player,ABILITIES.ATTACKS);


        


    }

    /// <summary>
    /// Enacts all abilities in their proper order in a specific category. Categories include things like attacks or skill abilities.
    /// </summary>
    /// <param name="player">side given priority; 0-> first player, 1-> second player</param>
    /// <param name="category">category to enact ability of</param>
    private void H_EnactAllAbilitiesInCategory(int player, int category) {
        int i = player;
        int j = 0;

        while (i < abilities[0,category].Count || j < abilities[1,category].Count) {


            if (i <= j) {
                if (i < abilities[0,category].Count) {
                    // do first player card at index i
                    Ability ability = abilities[0,category][i];

                    H_EnactAbility(ability,0);
                }
                
                i++;
            } else {
                if (j < abilities[1,category].Count) {
                    // do second player card at index j
                    Ability ability = abilities[1, category][j];

                    H_EnactAbility(ability,1);
                }

                j++;
            }
        }
    }

    /// <summary>
    /// Helper for H_EnactAllAbilitiesInCategory. Calls different methods depending on if the ability was an attack, skill, etc..
    /// </summary>
    /// <param name="ability">Ability to enact</param>
    /// <param name="side">side enacting ability; 0-> first player, 1-> second player</param>
    private void H_EnactAbility(Ability ability, int side) {
        if (ability is Ability.Attack) (ability as Ability.Attack).DealDamage(1-side,this);
    }

    /// <summary>
    /// Deals directional damage to a BattleField object, which it manages and administers to the defends and player adequately.
    /// </summary>
    /// <param name="direction">player to deal damage to; 0-> first player, 1-> second player</param>
    /// <param name="dmg">amount of damage to deal</param>
    public void TakeDamage(int direction, int dmg) {
        foreach (Ability.Defend defense in abilities[direction,ABILITIES.DEFENDS]) {
            dmg -= defense.TakeDamage(dmg);
            if (dmg == 0) return;
        }
        PlayerData.GetPlayer(direction).TakeDamage(dmg);
    }
    /// <summary>
    /// Deals directional damage to a BattleField object, along with an affliction. Calls BattleField.TakeDamage.
    /// </summary>
    /// <param name="direction">player to deal damage to; 0-> first player, 1-> second player</param>
    /// <param name="dmg">amount of damage to deal</param>
    /// <param name="affliction">2-element affliction array</param>
    public void TakeAfflictionDamage(int direction, int dmg, int[] affliction, CardData.Element element) {
        if (affliction[0] != -1) {
            foreach (Ability.Defend defense in abilities[direction,ABILITIES.DEFENDS]) {
                if (defense.isElement(element)) {
                    // this one takes damage, do not deal affliction
                    dmg -= defense.TakeDamage(dmg);
                    if (dmg > 0) {
                        TakeDamage(direction, dmg);
                    }
                    return;
                }
            }
            ApplyAffliction(direction,affliction);
        }

        TakeDamage(direction, dmg);
    }

    /// <summary>
    /// Applies an affliction to a player.
    /// </summary>
    /// <param name="player">player to take affliction; 0-> first player, 1-> second player</param>
    /// <param name="affl">2-element affliction array to apply</param>
    /// <exception cref="System.Exception">thrown if player value is invalid or key value of affliction array is invalid</exception>
    public void ApplyAffliction(int player, int[] affl) {

        int key = affl[0];
        int val = affl[1];

        // Methods for testing. Delete on launch for slightly reduced runtime.
        #region legacy
        if (player < 0 || player > 1) throw new System.Exception(
            "ApplyAffliction failed: 'player' field passed value: " + player
        );
        if (key < 0 || key > AFFLICTIONS.COVERED) throw new System.Exception(
            "ApplyAffliction failed: 'key' field passed value: " + key
        );
        #endregion

        if (key == AFFLICTIONS.CHOKED) {
            afflictions[player,key] = Mathf.Max(0,afflictions[player,key] + val);
        }
        else if (key == AFFLICTIONS.SLOW || key == AFFLICTIONS.FLOODED || key == AFFLICTIONS.COVERED) {
            if (val > 0)  afflictions[player,key] = 1;
            else afflictions[player,key] = 0;
        }
        else if (key == AFFLICTIONS.IRRADIATED) {
            afflictions[player,key] = Mathf.Max(0,Mathf.Min(4,afflictions[player,key] + val));
        }
    }

    public bool getMouseIn(Vector3 mousePos) {
        Vector2 pos = gameObject.GetComponent<Transform>().position;
        return (Mathf.Abs(pos.x - mousePos.x) <= 1.6f && Mathf.Abs(pos.y - mousePos.y) <= 2.85f);
    }

    void Start() {
        for (int i = 0; i < abilities.GetLength(0);i++) {
            for (int j = 0; j < abilities.GetLength(1);j++) {
                abilities[i, j] = new List<Ability>();
            }
        }
    }
}
