using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    public class Callbacks
    {
        public Action<CombatEvent> OnCombatEvent;
    }
    
    public readonly Callbacks CallbackEvents = new();

    private Queue<InGameEvent> inGameEventQueue = new();

    private void Awake()
    {
        Instance = this;
    }
    
    private void Update()
    {
        while (inGameEventQueue.Count > 0)
        {
            InGameEvent inGameEvent = inGameEventQueue.Dequeue();

            switch (inGameEvent.Type)
            {
                case InGameEvent.EventType.Combat:
                    var combatEvent = inGameEvent as CombatEvent;
                    inGameEvent.Receiver.TakeDamage(combatEvent);
                    CallbackEvents.OnCombatEvent?.Invoke(combatEvent);
                    break;
            }
        }
    }

    public void AddInGameEvent(InGameEvent callback)
    {
        inGameEventQueue.Enqueue(callback);
    }
}
