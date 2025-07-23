using System;
using System.Collections.Generic;

public class CombatSystem : SingletonBehaviour<CombatSystem>
{
    public class Callbacks
    {
        public Action<CombatEvent> OnCombatEvent;
    }
    
    public readonly Callbacks CallbackEvents = new();

    private Queue<InGameEvent> inGameEventQueue = new();
    
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
