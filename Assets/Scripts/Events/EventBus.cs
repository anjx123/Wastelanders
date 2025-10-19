using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EventBus
{
    private static class Listeners<T> where T : IEvent
    {
        public static readonly HashSet<Action<T>> bindings = new();
    }

    public static void Register<T>(Action<T> action) where T : IEvent
    {
        Listeners<T>.bindings.Add(action);
    }

    public static void Unregister<T>(Action<T> action) where T : IEvent
    {
        Listeners<T>.bindings.Remove(action);
    }

    public static void Raise<T>(T anEvent) where T : IEvent
    {
        Listeners<T>.bindings.ToList().ForEach(action => action(anEvent));
    }
}