using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScript : MonoBehaviour
{
    public static UIScript instance;
    public static Canvas canvas;

    [SerializeField] 
    private TextMeshProUGUI manaAmountHolder;
    [SerializeField]
    private TextMeshProUGUI maxManaHolder;

    void Awake() {
        instance = this;
        canvas = gameObject.GetComponent<Canvas>();
    }

    public void SetMana(int val) {
        manaAmountHolder.text = "" + val;
    }
    public void SetMaxMana(int val) {
        maxManaHolder.text = ""+val;
    }




}
