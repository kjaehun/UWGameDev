using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : MonoBehaviour
{
    public static List<Event> queue = new List<Event>();
    public static List<Event> enacting = new List<Event>();
    
    public static void Add(Event ev) {
        queue.Add(ev);
    }
    public static void AddFront(Event ev) {
        if (queue.Count == 0) queue.Add(ev);
        else queue.Insert(1,ev);
    }


    public static void Halt(Event ev) {
        enacting.Remove(ev);
    }
    public static void Remove(Event ev) {
        queue.Remove(ev);
    }

    public static void AdvanceQueue() {
        // there may be some minor redundant checks...
        if (queue.Count == 0) return;
        Event ev = queue[0];
        queue.Remove(ev);
        enacting.Add(ev);
    }

    // Update is called once per frame
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

    public static class Tools {
        public static void InterpolateAndWait(GameObject thing, Vector2 goal, float travelTime) {
            Sequencer.Add(new InterpolationEvent(thing, goal, travelTime));
            Sequencer.Add(new DelayEvent(travelTime));
        }
    }

    public abstract class Event {
        private float time;
        private readonly float maxTime;
        private readonly float runSubsequentTime;


        public Event(float time, float maxTime, float runSubsequentTime) {
            this.time = time;
            this.maxTime = maxTime;
            this.runSubsequentTime = runSubsequentTime;
        }
        public Event(float maxTime, float runSubsequentTime) : this(0, maxTime, runSubsequentTime) {}
        public Event(float maxTime) : this(0,maxTime,maxTime) {}

        // executed every frame it is run
        protected abstract void DoAction();
        

        // need to make it so that it only advances the queue if 1. there are no items in enacting
            // or 2. the last event has time > runsubsequenttime
        public void Progress(float dt) {
            DoAction();
            time += dt;
        }

        public bool BeyondMaxTime() {
            return time >= maxTime;
        }
        public bool BeyondRunSubsequentTime() {
            return time >= runSubsequentTime;
        }
    }

    public class DelayEvent : Event {
        public DelayEvent(float delay) : base(delay) {

        }

        protected override void DoAction()
        {
            // does literally nothing since it is for delay
        }
    }

    public abstract class InstantEvent : Event {
        public InstantEvent() : base(0) {

        }
    }

    public class MessageEvent : InstantEvent {
        string message;
        public MessageEvent(string message) : base() {
            this.message = message;
        }

        protected override void DoAction() {
            Debug.Log(message);
        }
    }

    public class AnimationEvent : InstantEvent {
        Animator anim;
        string trigger;
        public AnimationEvent(Animator animator, string trigger) : base() {
            this.anim = animator;
            this.trigger = trigger;
        }

        protected override void DoAction()
        {
            anim.SetTrigger(trigger);
        }
    }

    public class InterpolationEvent : InstantEvent {
        GameObject thing;
        Vector2 endPos;
        float travelTime;

        public InterpolationEvent(GameObject thing, Vector2 endPos, float travelTime) : base() {
            this.thing = thing;
            this.endPos = endPos;
            this.travelTime = travelTime;
        }

        protected override void DoAction()
        {
            MathA.Interpolate(thing,endPos,travelTime);
        }
    }

    public class MethodEvent : InstantEvent {
        public delegate void MethodDelegate();

        MethodDelegate method;

        public MethodEvent(MethodDelegate method) : base() {
            this.method = method;
        }

        protected override void DoAction()
        {
            method();
        }
    }
}
