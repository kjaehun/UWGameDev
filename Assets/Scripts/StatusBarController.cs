using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBarController : MonoBehaviour
{
    public GameObject backBar;
    public GameObject frontBar;

    public float maxSize;
    public float width;
    [Range(0.0f,1.0f)]
    public float percent;

    // Start is called before the first frame update
    void Start()
    {
        UpdateVisuals();
    }

    void UpdateVisuals() {
        backBar.GetComponent<Transform>().localScale = new Vector2(maxSize, width);

        frontBar.GetComponent<Transform>().localScale = new Vector2(percent * maxSize,width);

        float newX = maxSize*(percent - 1)/2; 

        frontBar.GetComponent<Transform>().localPosition = new Vector2(newX, 0);
    }

    public void SetPercent(float percent) {
        this.percent = percent;
        UpdateVisuals();
    }
    // public static StatusBarController GenerateBar(GameObject parent) {
    //     return GenerateBar(parent, Vector2.down);
    // }

    // public static StatusBarController GenerateBar(GameObject parent, Vector2 offset) {
    //     GameObject physicalStatusBar = GameObject.Instantiate(GameAssets.inst.statusBarPrefab, Vector3.zero, Quaternion.identity);
    //     physicalStatusBar.GetComponent<Transform>().SetParent(parent.GetComponent<Transform>());
    //     physicalStatusBar.GetComponent<Transform>().localPosition = offset;
    //     return physicalStatusBar.GetComponent<StatusBarController>();
        
    // }

}
