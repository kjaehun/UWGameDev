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
    private TextMeshProUGUI durationHolder;
    [SerializeField]
    private Sprite backgroundHolder;


    private CardData card;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Setup(CardData card) {
        this.card = card;
        UpdateVisuals();
    }

    public void UpdateVisuals() {
        nameHolder.text = card.getName();
        descriptionHolder.text = card.getDescription();
        durationHolder.text = card.getDuration().ToString();
    }
}
