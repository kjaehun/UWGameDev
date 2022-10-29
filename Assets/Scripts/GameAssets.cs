using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
   public static GameAssets inst;
   bool val = true;

   public Color waterColor;
   public Color smogColor;
   public Color sludgeColor;
   public Color radioactivityColor;
   public Color oilColor;

   public GameObject statusBarPrefab;
   public GameObject cardPrefab;

   public GameObject[] cardPrefabs;
    
    void Awake()
    {
        inst = this;
    }

}
