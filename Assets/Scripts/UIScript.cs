using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages some UI elements, broadly.
/// </summary>
public class UIScript : MonoBehaviour
{
    /// <summary>
    /// Holds itself for "instance" strategy.
    /// </summary>
    public static UIScript instance;
    /// <summary>
    /// Holds the canvas instance for easy access by other scripts.
    /// </summary>
    public static Canvas canvas;

    /// <summary>
    /// Text holder for the current amount of mana.
    /// </summary>
    [SerializeField] 
    private TextMeshProUGUI manaAmountHolder;
    /// <summary>
    /// Text holder for the maximum amount of mana.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI maxManaHolder;

    /// <summary>
    /// Sets up the "instance" strategy.
    /// </summary>
    void Awake() {
        instance = this;
        canvas = gameObject.GetComponent<Canvas>();
    }

    /// <summary>
    /// Sets the shown mana on the UI.
    /// </summary>
    /// <param name="val">mana to show on UI</param>
    public void SetMana(int val) {
        manaAmountHolder.text = "" + val;
    }
    /// <summary>
    /// Sets the shown max mana on the UI.
    /// </summary>
    /// <param name="val">max mana to show on UI</param>
    public void SetMaxMana(int val) {
        maxManaHolder.text = ""+val;
    }
}
