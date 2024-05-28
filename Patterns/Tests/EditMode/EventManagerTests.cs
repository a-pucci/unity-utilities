using System;
using AP.Utilities.Patterns;
using NUnit.Framework;

public class EventManagerTests
{
    [Test]
    public void AddAction()
    {
        var evtA = new Event();
        Action<EventData> action = _ => { };
        evtA.Subscribe(action);
        Assert.IsTrue(evtA.actions.Contains(action));
    }
    
    [Test]
    public void RemoveAction()
    {
        var evtA = new Event();
        Action<EventData> action = _ => { };
        evtA.Subscribe(action);
        evtA.Unsubscribe(action);
        Assert.IsTrue(!evtA.actions.Contains(action));
    }
    
    [Test]
    public void RemoveAllActions()
    {
        var evtA = new Event();
        Action<EventData> action = _ => { };
        evtA.Subscribe(action);
        evtA.UnsubscribeAll();
        Assert.IsTrue(evtA.actions.Count == 0);
    }
    
    [Test]
    public void InvokeEvent()
    {
        bool isCalled = false;
        var evtA = new Event();
        void Action(EventData _) => isCalled = true;
        evtA.Subscribe(Action);
        evtA.Raise();
        Assert.IsTrue(isCalled);
    }
    
    [Test]
    public void InvokeEventRemoveSameAction()
    {
        bool isCalled = false;
        var evtA = new Event();

        void Action(EventData _)
        {
            isCalled = true;
            evtA.Unsubscribe(Action);
        }
        evtA.Subscribe(Action);
        evtA.Raise();
       
        Assert.IsTrue(isCalled && !evtA.actions.Contains(Action));
    }
    
    [Test]
    public void InvokeEventRemoveDifferentAction()
    {

        bool isCalledA = false;
        bool isCalledB = false;
        var evtA = new Event();
        evtA.Subscribe(ActionA);
        evtA.Subscribe(ActionB);
        evtA.Raise();

        Assert.IsTrue(isCalledA && isCalledB && evtA.actions.Contains(ActionA) && !evtA.actions.Contains(ActionB));
        return;

        void ActionA(EventData _)
        {
            isCalledA = true;
            evtA.Unsubscribe(ActionB);
        }
        void ActionB(EventData _) => isCalledB = true;
    }
}