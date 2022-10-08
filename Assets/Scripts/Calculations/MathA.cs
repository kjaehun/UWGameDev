using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathA : MonoBehaviour
{
    private static List<InterpolationObject> interpolationList = new List<InterpolationObject>();

    // Only use this on things which do not have a Rigidbody2D component
    public static void AddToInterpolate(GameObject thing, Vector2 goal, float time) {
        InterpolationObject interp = new InterpolationObject(thing, goal, time);
        interpolationList.Add(interp);
    }
    public static void RemoveFromInterpolate(InterpolationObject interp) {
        interpolationList.Remove(interp);
    }

    void FixedUpdate() {
        if (interpolationList.Count>0) {
            float dt = Time.deltaTime;
            foreach (InterpolationObject interp in interpolationList) {
                interp.EnactStep(dt);
            }
        }
    }
}
