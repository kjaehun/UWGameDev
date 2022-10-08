using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolationObject
{
    private GameObject thing;
    private Vector2 goal;
    private Vector2 velocity;
    private float time;

    public InterpolationObject(GameObject thing, Vector2 goal, float time) {
        this.thing = thing;
        this.goal = goal;
        this.time = time;
        this.velocity = (goal-(Vector2)thing.transform.position)/time;
    }

    public void EnactStep(float dt) {
        time -= dt;
        if (time <= 0) {
            thing.transform.position = goal;
            MathA.RemoveFromInterpolate(this);
        } else {
            thing.transform.position += (Vector3)(velocity * dt);
        }
    }


}
