using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows for easy interpolation of objects.
/// Used exclusively by MathA.
/// </summary>
[System.Serializable]
public class InterpolationObject
{
    /// <summary>
    /// GameObject to interpolate.
    /// Serializable for easy debugging.
    /// </summary>
    [SerializeField]
    private GameObject thing;
    /// <summary>
    /// Goal/end position of interpolation.
    /// </summary>
    private Vector2 goal;
    /// <summary>
    /// Amount to move by per second. Essentially: (goal-start)/time.
    /// If one wants to make non-linear interpolation, they could do so by slightly rewriting this class to make velocity
    /// a function, then extend this class to give specific implementations of the velocity function.
    /// </summary>
    private Vector2 velocity;
    /// <summary>
    /// Time until goal is reached. Ticks down towards 0.
    /// Serializable for easy debugging.
    /// </summary>
    [SerializeField]
    private float time;

    /// <summary>
    /// Constructor for an InterpolationObject.
    /// Initializes all fields.
    /// </summary>
    /// <param name="thing">GameObject to interpolate</param>
    /// <param name="goal">goal position as a Vector2</param>
    /// <param name="time">time given to reach goal</param>
    public InterpolationObject(GameObject thing, Vector2 goal, float time) {
        this.thing = thing;
        this.goal = goal;
        this.time = time;
        this.velocity = (goal-(Vector2)thing.transform.position)/time;
    }

    /// <summary>
    /// Does one step of an InterpolationObject. 
    /// A "step" includes moving the object a small amount towards the goal along with decrementing the time.
    /// Callibrates for the possibility of overshooting. Goal will never be overshot.
    /// </summary>
    /// <param name="dt">step size in seconds</param>
    public void EnactStep(float dt) {
        time -= dt; // decrement time
        if (time <= 0) { // case for preventing overshooting of goal. 
            //              admittedly, not the most efficient implementation, but i dont mind if you dont.
            thing.transform.position = goal;
        } else {
            thing.transform.position += (Vector3)(velocity * dt);
        }
    }

    /// <summary>
    /// Determines whether the InterpolationObject has lived too long.
    /// An InterpolationObject is considered to have "lived too long" iff its time provided to reach the goal has passed,
    /// which is to say that it has reached the goal.
    /// </summary>
    /// <returns>true if the InterpolationObject has made it to the goal, false otherwise</returns>
    public bool isBeyondTime() {
        return (time <= 0);
    }

}
