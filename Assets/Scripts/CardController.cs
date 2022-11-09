using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls code elements of a physical manifestation of a card.
/// </summary>
public class CardController : MonoBehaviour
{
    /// <summary>
    /// Holds the name.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI nameHolder;
    /// <summary>
    /// Holds the description.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI descriptionHolder;
    /// <summary>
    /// Holds the mana cost.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI manaHolder;
    /// <summary>
    /// Holds the background.
    /// </summary>
    [SerializeField]
    private Sprite backgroundHolder;

    /// <summary>
    /// Referenced card.
    /// </summary>
    private CardData card;

    // TODO make only one card follow cursor at once
    /// <summary>
    /// Determines whether the card should follow the cursor.
    /// If true, card will follow.
    /// Legacy system. Destroy this later.
    /// </summary>
    private bool followCursor;

    /// <summary>
    /// Called by Unity whenever the mouse enters this card's collider.
    /// Allows for players to interact with cards.
    /// </summary>
    void OnMouseOver() {
        if (Input.GetMouseButton(0) && card.getOwner().Equals(GameLogic.instance.getCurrentController())) {
            followCursor = true;
            
        }
        
    }
    /// <summary>
    /// Called by Unity each frame.
    /// Used to make the card follow the cursor.
    /// </summary>
    void Update() {
        if (followCursor) {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gameObject.GetComponent<Transform>().position = pos;

            GameLogic.instance.SetControlledCard(card);
        }
        if (Input.GetMouseButtonUp(0)) followCursor = false;

    }

    /// <summary>
    /// Stores card information on this physical card, and updates the visuals of the card.
    /// </summary>
    /// <param name="card">CardData to store</param>
    public void Setup(CardData card) {
        this.card = card;
        UpdateVisuals();
    }

    /// <summary>
    /// Sets the information present on the physical card to the data stored in the associated CardData.
    /// </summary>
    public void UpdateVisuals() {
        nameHolder.text = card.getName();
        descriptionHolder.text = card.getDescription();
        manaHolder.text = card.getManaCost().ToString();
    }
}
