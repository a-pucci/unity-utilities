using System;
using System.Collections.Generic;
using AP.Utilities.Patterns;
using NUnit.Framework;

public class EventManagerTests
{
    [Test]
    public void AddAction()
    {
        var evtA = new Event();
        Action<EventData> action = _ => { };
        EventsManager.AddAction(evtA, action);
        Dictionary<Event, List<Action<EventData>>> map = EventsManager.EventActionsMap;
        Assert.IsTrue(map.ContainsKey(evtA) && map[evtA].Contains(action));
    }
    
    [Test]
    public void RemoveAction()
    {
        var evtA = new Event();
        Action<EventData> action = _ => { };
        EventsManager.AddAction(evtA, action);
        EventsManager.RemoveAction(action);
        Dictionary<Event, List<Action<EventData>>> map = EventsManager.EventActionsMap;
        Assert.IsTrue(map.ContainsKey(evtA) && !map[evtA].Contains(action));
    }
    
    [Test]
    public void RemoveMultipleAction()
    {
        var evtA = new Event();
        var evtB = new Event();
        Action<EventData> action = _ => { };
        EventsManager.AddAction(evtA, action);
        EventsManager.AddAction(evtB, action);
        EventsManager.RemoveAction(action);
        Dictionary<Event, List<Action<EventData>>> map = EventsManager.EventActionsMap;
        Assert.IsTrue(map.ContainsKey(evtA) && !map[evtA].Contains(action) &&
                      map.ContainsKey(evtB) && !map[evtB].Contains(action));
    }
    
    [Test]
    public void RemoveActionWithEvent()
    {
        var evtA = new Event();
        Action<EventData> action = _ => { };
        EventsManager.AddAction(evtA, action);
        EventsManager.RemoveAction(evtA, action);
        Dictionary<Event, List<Action<EventData>>> map = EventsManager.EventActionsMap;
        Assert.IsTrue(map.ContainsKey(evtA) && !map[evtA].Contains(action));
    }
    
    [Test]
    public void RemoveMultipleActionWithEvent()
    {
        var evtA = new Event();
        var evtB = new Event();
        Action<EventData> action = _ => { };
        EventsManager.AddAction(evtA, action);
        EventsManager.AddAction(evtB, action);
        EventsManager.RemoveAction(evtA, action);
        Dictionary<Event, List<Action<EventData>>> map = EventsManager.EventActionsMap;
        Assert.IsTrue(map.ContainsKey(evtA) && !map[evtA].Contains(action) &&
                      map.ContainsKey(evtB) && map[evtB].Contains(action));
    }
    
    [Test]
    public void RemoveAllActions()
    {
        var evtA = new Event();
        Action<EventData> action = _ => { };
        EventsManager.AddAction(evtA, action);
        EventsManager.RemoveAllActions(evtA);
        Dictionary<Event, List<Action<EventData>>> map = EventsManager.EventActionsMap;
        Assert.IsTrue(map.ContainsKey(evtA) && !map[evtA].Contains(action));
    }
    
    [Test]
    public void InvokeEvent()
    {
        bool isCalled = false;
        var evtA = new Event();
        void Action(EventData _) => isCalled = true;
        EventsManager.AddAction(evtA, Action);
        EventsManager.Invoke(evtA);
        Assert.IsTrue(isCalled);
    }
    
    [Test]
    public void InvokeEventRemoveSameAction()
    {
        bool isCalled = false;
        var evtA = new Event();

        EventsManager.AddAction(evtA, Action);
        EventsManager.Invoke(evtA);
        Dictionary<Event, List<Action<EventData>>> map = EventsManager.EventActionsMap;
        Assert.IsTrue(isCalled && map.ContainsKey(evtA) && !map[evtA].Contains(Action));
        return;

        void Action(EventData _)
        {
            isCalled = true;
            EventsManager.RemoveAction(evtA, Action);
        }
    }
    
    [Test]
    public void InvokeEventRemoveDifferentAction()
    {
        bool isCalledA = false;
        bool isCalledB = false;
        var evtA = new Event();

        EventsManager.AddAction(evtA, ActionA);
        EventsManager.AddAction(evtA, ActionB);
        EventsManager.Invoke(evtA);
        Dictionary<Event, List<Action<EventData>>> map = EventsManager.EventActionsMap;
        Assert.IsTrue(isCalledA && isCalledB && map.ContainsKey(evtA) && map[evtA].Contains(ActionA) && !map[evtA].Contains(ActionB));
        return;

        void ActionA(EventData _)
        {
            isCalledA = true;
            EventsManager.RemoveAction(evtA, ActionB);
        }
        void ActionB(EventData _) => isCalledB = true;
    }
}