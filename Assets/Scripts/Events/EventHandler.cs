using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventHandler<TEvent> : MonoBehaviour where TEvent : IEvent
{
    private readonly List<Action<TEvent>> subscribedActions = new();

    private void OnEnable()
    {
        subscribedActions.ForEach(EventBus.Register);
    }

    private void OnDisable()
    {
        subscribedActions.ForEach(EventBus.Unregister);
    }

    public EventHandler<TEvent> Subscribe(Action<TEvent> action)
    {
        subscribedActions.Add(action);
        EventBus.Register(action); 
        return this;
    }

    public EventHandler<TEvent> UnSubscribe(Action<TEvent> action)
    {
        subscribedActions.Remove(action);
        EventBus.Unregister(action); 
        return this;
    }
}