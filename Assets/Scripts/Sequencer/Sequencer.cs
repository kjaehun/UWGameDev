using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows for events to be ran in sequence with each other.
/// Provides functionality for implementing time delays even when a chunk of code is ran in a single frame.
/// </summary>
public class Sequencer : MonoBehaviour
{
    /// <summary>
    /// The set of all events which are waiting for their time to shine.
    /// The first element of the queue is moved onto the 'enacting' list whenever the queue is advanced.
    /// </summary>
    public static List<Event> queue = new List<Event>();
    /// <summary>
    /// All events in this list are ran every frame. 
    /// Events are removed to this list when their time expires.
    /// Events are added to this list when the list is empty or when the final event induces another one.
    /// </summary>
    public static List<Event> enacting = new List<Event>();

    /// <summary>
    /// Adds an event onto the end of the queue.
    /// </summary>
    /// <param name="ev">Event object to add to end of queue</param>
    public static void Add(Event ev) {
        queue.Add(ev);
    }
    /// <summary>
    /// Adds an event onto the front of the queue, after the first event.
    /// I do not entirely remember why it is "after the first event" and not simply "at the beginning."
    /// I did it this way before for a very good reason.
    /// Adding after the first element may not be necessary with this implementation.
    /// TODO test if this is how it should work.
    /// </summary>
    /// <param name="ev">event to add to the front of the queue</param>
    public static void AddFront(Event ev) {
        if (queue.Count == 0) queue.Add(ev);
        else queue.Insert(1,ev);
    }

    /// <summary>
    /// Removes an event from 'enacting'.
    /// In other words, stops an event from running which is already running.
    /// </summary>
    /// <param name="ev">event to halt</param>
    public static void Halt(Event ev) {
        enacting.Remove(ev);
    }
    /// <summary>
    /// Removes an event from the queue.
    /// In other words, stops an event from running in the future, even though it is not currently running.
    /// </summary>
    /// <param name="ev"></param>
    public static void Remove(Event ev) {
        queue.Remove(ev);
    }

    /// <summary>
    /// Moves the first event in the queue onto the end of the 'enacting' list.
    /// </summary>
    public static void AdvanceQueue() {
        // there may be some minor redundant checks...
        if (queue.Count == 0) return;
        Event ev = queue[0];
        queue.RemoveAt(0);
        enacting.Add(ev);
    }

    /// <summary>
    /// Called by Unity.
    /// Deals with advancing the queue and enacting all events each frame.
    /// </summary>
    void Update()
    {
        float dt = Time.deltaTime;
        if (enacting.Count == 0 && queue.Count > 0) {
            AdvanceQueue();
        }
        if (enacting.Count > 0) {
            for (int i=0; i< enacting.Count; i++) {
                Event ev = enacting[i];
                ev.Progress(dt);
                if (i == enacting.Count && ev.BeyondRunSubsequentTime()) {
                    AdvanceQueue();
                }
                if (ev.BeyondMaxTime()) {
                    Halt(ev);
                    i--;
                }
            }
        }
    }

    /// <summary>
    /// Tools which allow for the easy creation of specific combinations of events.
    /// Ex: Combining an InterpolationEvent with a DelayEvent to make the code wait until an object has moved.
    /// </summary>
    public static class Tools {
        /// <summary>
        /// Creates an InterpolationEvent followed by a DelayEvent such that the 
        /// Sequencer waits until an object has finished moving.
        /// </summary>
        /// <param name="thing">GameObject to move</param>
        /// <param name="goal">goal position as a Vector2</param>
        /// <param name="travelTime">time to reach destination in seconds</param>
        public static void InterpolateAndWait(GameObject thing, Vector2 goal, float travelTime) {
            Sequencer.Add(new InterpolationEvent(thing, goal, travelTime));
            Sequencer.Add(new DelayEvent(travelTime));
        }
    }

    /// <summary>
    /// The basic form of an event.
    /// Contains all standard event fields, such as the current time, the maximum time, and the 'run subsequent time.'
    /// Also contains related methods to allow for easy interaction with Sequencer.
    /// Non-instantiable.
    /// </summary>
    public abstract class Event {

        /// <summary>
        /// Current time that this event has spent in the 'enacting' list in seconds.
        /// When this is greater than or equal to maxTime, the event is removed from the list.
        /// </summary>
        private float time;
        /// <summary>
        /// Maximum time that this event can persist within the 'enacting' list.
        /// </summary>
        private readonly float maxTime;
        /// <summary>
        /// This is a tricky one...
        /// If this is the final event in the 'enacting' list, and 'time' is greater than this number,
        /// then the queue is advanced (Sequencer.AdvanceQueue is called).
        /// This allows for multiple events to occur simultaneously at staggered times.
        /// This was a very useful addition in the past, but it may not be for this implementation.
        /// Feel free to ignore.
        /// </summary>
        private readonly float runSubsequentTime;

        /// <summary>
        /// Constructor for an Event object.
        /// Instantiates all fields and final fields.
        /// Sets maxTime and runSubsequentTime.
        /// </summary>
        /// <param name="maxTime">max lifespan of this event</param>
        /// <param name="runSubsequentTime">time until advancing the queue, if this is the final event being enacted</param>
        public Event(float maxTime, float runSubsequentTime) {
            this.time = 0.0f;
            this.maxTime = maxTime;
            this.runSubsequentTime = runSubsequentTime;
        }
        /// <summary>
        /// Overloading constructor for an Event object.
        /// Sets the maxTime.
        /// </summary>
        /// <param name="maxTime">max lifespan of this event</param>
        public Event(float maxTime) : this(maxTime,maxTime) {}

        /// <summary>
        /// This method is to be implemented by child classes.
        /// It is executed every frame that the event persists within 'enacting'.
        /// </summary>
        protected abstract void DoAction();

        /// <summary>
        /// Called every frame by the Sequencer.
        /// Does this Event object's action while also advancing its time.
        /// </summary>
        /// <param name="dt">time to add to this event</param>
        public void Progress(float dt) {
            DoAction();
            time += dt;
        }

        /// <summary>
        /// Determines whether this event has lived beyond its lifespan.
        /// </summary>
        /// <returns>true if this Event object's time alive is >= its max lifespan, false otherwise</returns>
        public bool BeyondMaxTime() {
            return time >= maxTime;
        }
        /// <summary>
        /// Determines whether this event has existed beyond the run subsequent time.
        /// </summary>
        /// <returns>true if this Event object's time alive is >= its run subsequent time, false otherwise</returns>
        public bool BeyondRunSubsequentTime() {
            return time >= runSubsequentTime;
        }
    }
    
    /// <summary>
    /// An imperative, basic type of event.
    /// A DelayEvent does nothing but insert a delay between events in the Sequencer.
    /// This easily allows for one thing to happen strictly after a certain amount of seconds.
    /// </summary>
    public class DelayEvent : Event {

        /// <summary>
        /// Constructs a DelayEvent given a delay in seconds.
        /// </summary>
        /// <param name="delay">delay in seconds</param>
        public DelayEvent(float delay) : base(delay) {

        }

        /// <summary>
        /// Defines DoAction to do nothing.
        /// </summary>
        protected override void DoAction()
        {
            // does literally nothing since it is for delay
        }
    }


    /// <summary>
    /// Abstract extension of Event for all events which occur in only a single moment.
    /// Good for methods which should only run once.
    /// </summary>
    public abstract class InstantEvent : Event {

        /// <summary>
        /// Constructor for InstantEvent.
        /// </summary>
        public InstantEvent() : base(0) {

        }
    }

    /// <summary>
    /// A MessageEvent will print a message to the console whenever it is called.
    /// Used solely for debugging.
    /// </summary>
    public class MessageEvent : InstantEvent {
        /// <summary>
        /// String to print to console.
        /// </summary>
        private string message;

        /// <summary>
        /// Constructor for a MessageEvent.
        /// </summary>
        /// <param name="message">message to print later</param>
        public MessageEvent(string message) : base() {
            this.message = message;
        }

        /// <summary>
        /// Defines DoAction to print the stored message.
        /// </summary>
        protected override void DoAction() {
            Debug.Log(message);
        }
    }

    /// <summary>
    /// An AnimationEvent is a type of InstantEvent which will run an animation on an Animator.
    /// This will be very useful later down the line.
    /// </summary>
    public class AnimationEvent : InstantEvent {
        /// <summary>
        /// Animator to trigger.
        /// </summary>
        private Animator anim;

        /// <summary>
        /// Trigger name.
        /// </summary>
        private string trigger;

        /// <summary>
        /// Constructor for an AnimationEvent.
        /// Takes in an Animator and the associated trigger name, and stores them for execution later.
        /// </summary>
        /// <param name="animator">Animator object to store</param>
        /// <param name="trigger">trigger in the form of a string to executes</param>
        public AnimationEvent(Animator animator, string trigger) : base() {
            this.anim = animator;
            this.trigger = trigger;
        }

        /// <summary>
        /// Defines DoAction to trigger the specific animation of the Animator.
        /// </summary>
        protected override void DoAction()
        {
            anim.SetTrigger(trigger);
        }
    }
    /// <summary>
    /// An InterpolationEvent is a type of InstantEvent which will run a linear interpolation on a GameObject.
    /// </summary>
    public class InterpolationEvent : InstantEvent {
        /// <summary>
        /// GameObject to interpolate.
        /// </summary>
        private GameObject thing;
        /// <summary>
        /// End/goal position of GameObject.
        /// </summary>
        private Vector2 endPos;
        /// <summary>
        /// Maximum time in seconds for GameObject to reach the goal.
        /// </summary>
        private float travelTime;

        /// <summary>
        /// Constructor for an InterpolationEvent.
        /// </summary>
        /// <param name="thing">GameObject to interpolate</param>
        /// <param name="endPos">goal position</param>
        /// <param name="travelTime">time ot get to goal in seconds</param>
        public InterpolationEvent(GameObject thing, Vector2 endPos, float travelTime) : base() {
            this.thing = thing;
            this.endPos = endPos;
            this.travelTime = travelTime;
        }

        /// <summary>
        /// Defines DoAction to call the interpolation function on the stored object.
        /// </summary>
        protected override void DoAction()
        {
            MathA.Interpolate(thing,endPos,travelTime);
        }
    }

    /// <summary>
    /// A MethodEvent allows for ANY method with no parameter to be stored and ran in the Sequencer.
    /// Note that this is a type of InstantEvent.
    /// </summary>
    public class MethodEvent : InstantEvent {
        /// <summary>
        /// Stored method "outline."
        /// Do not ask me what a "delegate" is because I looked it up, wrote this code, and promptly forgot.
        /// </summary>
        public delegate void MethodDelegate();

        /// <summary>
        /// Stored method.
        /// </summary>
        MethodDelegate method;

        /// <summary>
        /// Constructor for a MethodEvent. Stores an element with no parameters. Ex: GameLogic.StartBattle
        /// </summary>
        /// <param name="method">method to store and run in Sequencer</param>
        public MethodEvent(MethodDelegate method) : base() {
            this.method = method;
        }

        /// <summary>
        /// Defines DoAction to run the stored method.
        /// </summary>
        protected override void DoAction()
        {
            method();
        }
    }
}
