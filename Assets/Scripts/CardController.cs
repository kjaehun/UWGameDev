using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameHolder;
    [SerializeField]
    private TextMeshProUGUI descriptionHolder;
    [SerializeField]
    private TextMeshProUGUI manaHolder;
    [SerializeField]
    private Sprite backgroundHolder;


    private CardData card;


    private bool followCursor;


    void OnMouseOver() {
        if (Input.GetMouseButton(0) && card.getOwner().Equals(GameLogic.instance.getCurrentController())) {
            followCursor = true;
            
        }
        
    }

    void Update() {
        if (followCursor) {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gameObject.GetComponent<Transform>().position = pos;

            GameLogic.instance.SetControlledCard(card);
        }
        if (Input.GetMouseButtonUp(0)) followCursor = false;

    }

    public void Setup(CardData card) {
        this.card = card;
        UpdateVisuals();
    }

    public void UpdateVisuals() {
        nameHolder.text = card.getName();
        descriptionHolder.text = card.getDescription();
        manaHolder.text = card.getManaCost().ToString();
    }
}
