using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{

    public GameObject cardPrefab;


    public static GameAssets instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
