using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScript : MonoBehaviour
{
    public static UIScript instance;

    [SerializeField] 
    private TextMeshProUGUI manaAmountHolder;
    [SerializeField]
    private TextMeshProUGUI maxManaHolder;

    void Awake() {
        instance = this;
    }




}
