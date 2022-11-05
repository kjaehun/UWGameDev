using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityRepresentation : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI valueHolder;
    [SerializeField]
    private TextMeshProUGUI turnHolder;

    Ability ability;

    // Start is called before the first frame update
    void Start()
    {
        UpdateVisuals();
    }

    private void UpdateVisuals() {
        turnHolder.text = ability.getLifeSpan().ToString();
        valueHolder.text = ability.getValue().ToString();
    }
}
