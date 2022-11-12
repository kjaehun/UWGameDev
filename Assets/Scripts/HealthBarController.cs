using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Allows for the creation and manipulation of player health bars as UI elements.
/// </summary>
public class HealthBarController : MonoBehaviour
{

    #region static fields
    /// <summary>
    /// Array containing two health bars: first is for first player, second is for second.
    /// </summary>
    private static HealthBarController[] healthBars = new HealthBarController[2];

    /// <summary>
    /// Invariant 2D vector which determines the offsets to place the health bar at.
    /// Feel free to change this to attain different effects.
    /// 1st num := dist from horiz sides
    /// 2nd num := dist from vert sides
    /// </summary>
    private static readonly Vector2 INTENDED_POSITION = new Vector2(100, 500);
    #endregion
    
    #region instance fields
    /// <summary>
    /// "Liquid" box stored here for easy access.
    /// </summary>
    [SerializeField]
    private Image liquid;
    /// <summary>
    /// Text box which holds health info.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI healthHolder;

    /// <summary>
    /// Holds size of the parent panel as a 2D vector: (width,height).
    /// </summary>
    private Vector2 uiSize;
    /// <summary>
    /// How far from the edges to place the liquid bar.
    /// The liquid bar is set to automatically take up 80% of the panel behind it horizontally.
    /// Incrementing the first number of this vector will decrease this percentage.
    /// </summary>
    private Vector2 barOffsets = new Vector2(0, 30);
    #endregion


    #region instance methods
    /// <summary>
    /// Sets up the uiSize
    /// </summary>
    void Start()
    {
        uiSize = gameObject.GetComponent<RectTransform>().rect.size;
    }

    /// <summary>
    /// Sets the current health and max health values.
    /// The health bar has "no memory," so these need to be changed every time the health bar is altered.
    /// </summary>
    /// <param name="currentVal">current health</param>
    /// <param name="maxVal">max health</param>
    public void SetValues(int currentVal, int maxVal)
    {
        // sets health text on health bar
        healthHolder.text = currentVal + "/" + maxVal;


        // sets filled-ness of health bar
        float percent = (float)currentVal / (float)maxVal;
        RectTransform rtrans = liquid.GetComponent<RectTransform>();

        // a bunch of math to make things scale properly
        float vertOffset = (uiSize.y - 2 * barOffsets.y) * (1 - percent);
        rtrans.offsetMax = -1 * (barOffsets + vertOffset * Vector2.up);
        rtrans.offsetMin = barOffsets;
        // trust me bro, it's just a bunch of ui stuff
    }
    #endregion


    #region makers and destroyers
    /// <summary>
    /// Destroys all health bars.
    /// </summary>
    public static void DestroyAllHealthBars() {
        foreach (HealthBarController bar in healthBars) {
            GameObject.Destroy(bar.gameObject);
        }
    }

    /// <summary>
    /// Creates a health bar at an arbitrary location with a specific initial health.
    /// The value provided will be the same for the current health and max health.
    /// </summary>
    /// <param name="playerIndex">player index determining health bar owner</param>
    /// <param name="val">amount of health and max health to provide</param>
    /// <returns>the constructed HealthBarController object</returns>
    public static HealthBarController MakeHealthBar(int playerIndex, int val) { return MakeHealthBar(playerIndex, val, val); }
    /// <summary>
    /// Creates a health bar at an arbitrary location with a specific initial current health and initial max health.
    /// </summary>
    /// <param name="playerIndex">player index determining health bar owner</param>
    /// <param name="currentVal">amount of health</param>
    /// <param name="maxVal">amount of maximum health</param>
    /// <returns>the constructed HealthBarController object</returns>
    public static HealthBarController MakeHealthBar(int playerIndex, int currentVal, int maxVal) {

        // instantiates the object
        GameObject thing = GameObject.Instantiate(GameAssets.inst.healthBar, Vector2.zero, Quaternion.identity);
        thing.name = "Health Bar (" + playerIndex + ")";    // sets the name for decorative purposes

        thing.GetComponent<Transform>().SetParent(UIScript.canvas.GetComponent<Transform>(), false); 
        // sets the parent to the canvas so it appears as a UI element

        RectTransform rtrans = thing.GetComponent<RectTransform>();
        setIntendedPosition(playerIndex, rtrans);
        // sets its intended location in the world


        HealthBarController healthBar = thing.GetComponent<HealthBarController>();
        healthBars[playerIndex] = healthBar;
        healthBar.SetValues(currentVal, maxVal);
        // completes initial setup on a HealthBarController

        return healthBar;
    }
    #endregion

    /// <summary>
    /// Taking in a player's index, sets the position of a health bar to its appropriate place.
    /// Change this code to change how the UI will appear.
    /// TODO legacy change this so it is not static. It is kinda dumb to make this static.
    /// </summary>
    /// <param name="playerIndex">player index determining health bar ownership</param>
    /// <param name="rtrans">RectTransform of health bar which should be translated</param>
    private static void setIntendedPosition(int playerIndex, RectTransform rtrans) {


        rtrans.anchorMax = new Vector2(playerIndex, 0.5f);
        rtrans.anchorMin = new Vector2(playerIndex, 0.5f);
        
        if (playerIndex == 0) {
            rtrans.anchoredPosition = new Vector2(100, 0);
        } else {
            rtrans.anchoredPosition = new Vector2(-100, 0);
        }

        rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, INTENDED_POSITION.x);
        rtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, INTENDED_POSITION.y);

    }

    
}
