using System;
using System.Collections.Generic;
public static class DelayedActions{
    private static readonly List<DelayedAction> _actions = new();
    private static readonly List<int> _completedIndices = new();
    
    /// <summary>
    /// Add a new action to the actions list with a specified wait time
    /// </summary>
    /// <param name="action"></param>
    /// <param name="waitTime"></param>
    public static void Add(Action action, double waitTime){
        _actions.Add(new DelayedAction(action, waitTime));
    }

    /// <summary>
    /// Increment the timers on all actions by delta
    /// </summary>
    /// <param name="delta"></param>
    public static void IncrementActions(double delta){
        foreach (DelayedAction d in _actions){
            d.Increment(delta);
            if (!(d.currentWaitTime >= d.waitTime)) continue;
            //Execute the method that was queued 
            d.action.Invoke();
            //Flag action for removal during cleanup process
            _completedIndices.Add(_actions.IndexOf(d));
        }
        Cleanup();
    }

    /// <summary>
    /// Remove completed actions from the action list
    /// </summary>
    private static void Cleanup(){
        foreach (int i in _completedIndices){
            _actions.RemoveAt(i);
        }
        _completedIndices.Clear();
    }
}

/// <summary>
/// Helper class for delayed actions
/// Stores all pertinent information for the action to be executed after a given delay
/// </summary>
public class DelayedAction{
    public Action action;
    public double waitTime;
    public double currentWaitTime;

    public DelayedAction(Action actn, double wait){
        currentWaitTime = 0;
        waitTime = wait;
        action = actn;
    }

    public void Increment(double delta){
        currentWaitTime += delta;
    }
}