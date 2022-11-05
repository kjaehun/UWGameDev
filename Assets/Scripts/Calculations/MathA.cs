using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathA : MonoBehaviour
{
    private static MathA instance;
    [SerializeField]
    private List<InterpolationObject> interpolationList = new List<InterpolationObject>();

    // Only use this on things which do not have a Rigidbody2D component
    public static void Interpolate(GameObject thing, Vector2 goal, float time) {
        InterpolationObject interp = new InterpolationObject(thing, goal, time);
        instance.interpolationList.Add(interp);
    }
    public static void RemoveFromInterpolate(InterpolationObject interp) {
        instance.interpolationList.Remove(interp);
    }

    void Awake() {
        instance = this;
    }

    void FixedUpdate() {
        if (interpolationList.Count>0) {
            Debug.Log("hey");
            float dt = Time.deltaTime;
            for (int i = 0; i < interpolationList.Count;i++) {
                interpolationList[i].EnactStep(dt);
                if (interpolationList[i].isBeyondTime()) {
                    interpolationList.RemoveAt(i);
                    i--;
                    continue;
                }
            }
        }
    }

    public static float GetSpread(int index, int maxNum, float center, float dist) {
        return center + index * dist - dist * (maxNum - 1) / 2.0f;
    }


}
