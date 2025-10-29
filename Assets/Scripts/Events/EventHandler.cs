using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Class that manages the lifecycle of Static Events, you don't need to explicitly call unsubscribe if you don't need to.
public abstract class EventHandler<TEvent> : MonoBehaviour where TEvent : IEvent
{
    private readonly HashSet<Action<TEvent>> subscribedActions = new();

    private void OnEnable()
    {
        subscribedActions.ToList().ForEach(EventBus.Register);
    }

    private void OnDisable()
    {
        subscribedActions.ToList().ForEach(EventBus.Unregister);
    }

    public EventHandler<TEvent> Subscribe(Action<TEvent> action)
    {
        if (subscribedActions.Add(action))
            EventBus.Register(action);
        return this;
    }

    public EventHandler<TEvent> Unsubscribe(Action<TEvent> action)
    {
        if (subscribedActions.Remove(action))
            EventBus.Unregister(action);
        return this;
    }
}