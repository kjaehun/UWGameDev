using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The purpose of this class is to allow for easy reference to many different prefab assets.
/// To reference a prefab, one could type: "GameAssets.inst.[prefab name]" in any class.
/// </summary>
public class GameAssets : MonoBehaviour
{
    /// <summary>
    /// Instance holder used for "instance" strategy.
    /// </summary>
    public static GameAssets inst;

    /// <summary>
    /// Color for water cards.
    /// </summary>
    public Color waterColor;
    /// <summary>
    /// Color for smog cards.
    /// </summary>
    public Color smogColor;
    /// <summary>
    /// Color for sludge cards.
    /// </summary>
    public Color sludgeColor;
    /// <summary>
    /// Color for radioactive cards.
    /// </summary>
    public Color radioactivityColor;
    /// <summary>
    /// Color for oil cards.
    /// </summary>
    public Color oilColor;

    /// <summary>
    /// Prefab for a health bar.
    /// </summary>
    public GameObject healthBar;
    /// <summary>
    /// Prefab for a physical card.
    /// </summary>
    public GameObject cardPrefab;
    /// <summary>
    /// Prefab for a battle field.
    /// </summary>
    public GameObject battleFieldPrefab;

    public GameObject[] abilityRepresentationPrefabs;

    public GameObject cardBackPrefab;

    /// <summary>
    /// Called by Unity prior to the first frame.
    /// Sets up "instance" strategy.
    /// </summary>
    void Awake()
    {
        inst = this;
    }

}
