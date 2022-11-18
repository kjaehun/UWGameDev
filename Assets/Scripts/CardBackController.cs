using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardBackController : MonoBehaviour
{
    /// <summary>
    /// A list containing all the card backs.
    /// This is referenced purely to allow the code to easily destroy all of them.
    /// </summary>
    private static LinkedList<GameObject> cardBacks = new LinkedList<GameObject>();

    /// <summary>
    /// The type of this card.
    /// TODO make this not necessary.
    /// </summary>
    private CardData.Type type;

    /// <summary>
    /// The background sprite of the card back.
    /// </summary>
    [SerializeField]
    private SpriteRenderer background;
    /// <summary>
    /// legacy
    /// The text on the card back. 
    /// Not really necessary later on.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        // adds this card to the list for storage
        cardBacks.AddLast(gameObject);
    }

    /// <summary>
    /// Sets the type and the visuals.
    /// TODO setting the 'type' is not necessary, only really need to store visuals
    /// </summary>
    /// <param name="type">type of card; use CardBackController.DisplayType</param>
    void Setup(CardData.Type type) {
        this.type = type;
        UpdateVisuals();
    }

    /// <summary>
    /// Updates the card visuals depending on the type.
    /// </summary>
    void UpdateVisuals() {
        if (type == CardData.Type.ATTACK) {
            background.color = Color.red;
            text.SetText("Attack");

        } else if (type == CardData.Type.DEFEND) {
            background.color = Color.blue;
            text.SetText("Defend");

        } else if (type == CardData.Type.SKILL) {
            background.color = Color.yellow;
            text.SetText("Skill");

        }
    }

    /// <summary>
    /// Makes a flipped over card of the specified type.
    /// Returns the created GameObject.
    /// </summary>
    /// <param name="type">type of card; use CardBackController.DisplayType</param>
    /// <returns>GameObject of newly created card back</returns>
    public static GameObject MakeCardBack(CardData.Type type) {
        GameObject cardBack = GameObject.Instantiate(GameAssets.inst.cardBackPrefab, Vector2.zero, Quaternion.identity);

        cardBack.GetComponent<CardBackController>().Setup(type);
        return cardBack;
    }

    /// <summary>
    /// Destroys all card backs.
    /// </summary>
    public static void DestroyAllCardBacks() {
        foreach (GameObject thing in cardBacks) {
            GameObject.Destroy(thing);
        }
        cardBacks = new LinkedList<GameObject>();
    }

}
