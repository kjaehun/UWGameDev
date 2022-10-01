using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardController : MonoBehaviour
{

    public TextMeshProUGUI costHolder;
    public TextMeshProUGUI actionHolder;

    CardData card;

    public void Setup(CardData card) {
        this.card = card;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
