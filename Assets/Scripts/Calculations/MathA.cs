using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Does basic repetitive calculations.
/// </summary>
public class MathA : MonoBehaviour
{
    /// <summary>
    /// This is written using "instance" strategy.
    /// There will only ever be one MathA instance in the scene, so it can be referred to statically.
    /// It is important, however, for this script to have non-static components since it must make use of FixedUpdate.
    /// </summary>
    private static MathA instance;
    /// <summary>
    /// List of InterpolationObjects which will be interpolated slowly each frame.
    /// </summary>
    [SerializeField]
    private List<InterpolationObject> interpolationList = new List<InterpolationObject>();

    /// <summary>
    /// Interpolates the provided GameObject to reach the end goal after the defined time has passed.
    /// Only use this on things which do not have a RigidBody2D component.
    /// There is currently no safeguard preventing you from making the same object have multiple interpolations simultaneously.
    /// The longest one will be favored, but it will look weird until they are all gone.
    /// </summary>
    /// <param name="thing">GameObject to interpolate</param>
    /// <param name="goal">goal position as a Vector2</param>
    /// <param name="time">time to reach goal</param>
    public static void Interpolate(GameObject thing, Vector2 goal, float time) {
        InterpolationObject interp = new InterpolationObject(thing, goal, time); // creates InterpolationObject
        instance.interpolationList.Add(interp); // adds it to the list of objects to interpolate
    }
    /// <summary>
    /// Stops an object from interpolating.
    /// This should be called when destroying an object mid-interpolation.
    /// Otherwise, an error will be thrown as this script will try to move a null object.
    /// TODO add a way to do this easily when providing a GameObject rather than an InterpolationObject
    /// </summary>
    /// <param name="interp">InterpolationObject to remove from the queue</param>
    public static void RemoveFromInterpolate(InterpolationObject interp) {
        instance.interpolationList.Remove(interp);
    }

    /// <summary>
    /// Called before the first frame to set up the "instance" strategy.
    /// </summary>
    void Awake() {
        instance = this;
    }


    /// <summary>
    /// Called a fixed number of times each second by Unity.
    /// Controls the movement of each GameObject in the interpolation list.
    /// Also removes InterpolationObjects from the list when they have reached their destination.
    /// </summary>
    void FixedUpdate() {
        if (interpolationList.Count>0) {
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

    /// <summary>
    /// Used for creating objects which are evenly spaced apart about some center location.
    /// Ex: For making a row of cards that are evenly spaced around the point x=3.
    /// Takes the number of objects to space, a center number, a distance between each object, and the index of an object.
    /// Provides the coordinate of the object on the assumed axis.
    /// If this is confusing let me know and I will rewrite it. Basically allows you to do this without lifting a finger:
    /// |   |   |   |   |   | 
    /// </summary>
    /// <param name="index">index of current object, where 0 starts with the left for positive values of dist</param>
    /// <param name="maxNum">number of objects to spread</param>
    /// <param name="center">center coordinate of spread (average position of all objects following spread)</param>
    /// <param name="dist">distance between each object in the spread from its neighbor(s)</param>
    /// <returns>the coordinate of this object along the associated axis within the spread</returns>
    public static float GetSpread(int index, int maxNum, float center, float dist) {
        if (maxNum < 0) maxNum = 0;
        return center + index * dist - dist * (maxNum - 1) / 2.0f;
    }


}
