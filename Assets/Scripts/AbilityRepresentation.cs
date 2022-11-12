using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Allows for abilities to have a physical representation on the board.
/// This is what lets players see that their actions have effects.
/// </summary>
public class AbilityRepresentation : MonoBehaviour
{
    #region storage fields
    [SerializeField]
    private TextMeshProUGUI valueHolder; // holds either def or dmg
    [SerializeField]
    private TextMeshProUGUI turnHolder; // holds turn number
    [SerializeField]
    private SpriteRenderer spriteRenderer; // holds sprite renderer for color changes
    [SerializeField]
    private Animator animator; // holds attached animator
    #endregion

    /// <summary>
    /// Associated ability.
    /// In theory, a number of representations could point to one ability, but only one ability can point to a representation.
    /// Perhaps I could alter the implementation such that one ability can point to many representations?
    /// TODO change this system as described above.
    /// </summary>
    private Ability ability;

    /// <summary>
    /// Merely calls UpdateVisuals.
    /// </summary>
    void Start()
    {
        UpdateVisuals();
    }

    /// <summary>
    /// Updates the representation's value number, lifespan, and color based on its attributes.
    /// </summary>
    public void UpdateVisuals() {
        turnHolder.text = ability.getLifeSpan().ToString();
        valueHolder.text = ability.getValue().ToString();

        CardData.Element element = ability.getElement();

        Color color = Color.white;

        if (element== CardData.Element.OIL) color = GameAssets.inst.oilColor;
        else if (element == CardData.Element.RADIOACTIVITY) color = GameAssets.inst.radioactivityColor;
        else if (element == CardData.Element.SLUDGE) color = GameAssets.inst.sludgeColor;
        else if (element == CardData.Element.SMOG) color = GameAssets.inst.smogColor;
        else if (element == CardData.Element.WATER) color = GameAssets.inst.waterColor;

        spriteRenderer.color = color;
    }

    #region constructors
    /// <summary>
    /// Constructs a physical ability representation.
    /// </summary>
    /// <param name="parent">BattleField to attach this representation to; allows for easy manipulation of local coords</param>
    /// <param name="ability">ability to attach to this representation</param>
    /// <returns>the newly created AbilityRepresentation</returns>
    public static AbilityRepresentation MakeAbilityRepresentation(GameObject parent, Ability ability) {
        GameObject thing;
        if (ability is Ability.Attack) {
            thing = GameObject.Instantiate(GameAssets.inst.abilityRepresentationPrefabs[0], Vector2.zero, Quaternion.identity);
        } else {
            thing = GameObject.Instantiate(GameAssets.inst.abilityRepresentationPrefabs[1], Vector2.zero, Quaternion.identity);
        }

        thing.GetComponent<Transform>().SetParent(parent.GetComponent<Transform>(), false);

        AbilityRepresentation rep = thing.GetComponent<AbilityRepresentation>();
        rep.ability = ability;
        ability.setRepresentation(rep);

        return rep;
    }
    #endregion

    #region animations
    /// <summary>
    /// Plays the Activate animation, which consists of a little turn, much like tapping a card.
    /// </summary>
    public void PlayActivateAnimation() {
        // legacy, do not check for non-null-ness
        if (animator != null) animator.SetTrigger("Activate");
    }
    #endregion

}
