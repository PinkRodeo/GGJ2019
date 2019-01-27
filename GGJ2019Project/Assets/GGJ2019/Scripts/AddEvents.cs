using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ClawTarget))]
public class AddEvents : MonoBehaviour
{
    public enum E_EventTrigger
    {
        OnClawConnect,
        OnShipDocked,
        OnShipUnDocked,
    }

    public E_EventTrigger eventTrigger = E_EventTrigger.OnShipDocked;

    [SerializeField] DialogScriptableObject EventChain;
    [SerializeField] bool DoOnce = true;

    bool DoneOnce = false;

    [ContextMenu("AddItems")]
    public void Add()
    {
        if (DoOnce == true && DoOnce == DoneOnce)
            return;


        if (DoOnce == true)
        {
            JunkerGameMode.FinishedStoryPart(EventChain.name);

            DoneOnce = true;
        }

        if (EventChain != null)
            foreach (GameEvent item in EventChain.eventChain)
            {
                EventManager.AddEvent(item);
            }
        if (DoOnce == true)
        {
            var clawTarget = GetComponent<ClawTarget>();
            clawTarget.SetLightsVisible(DoOnce != DoneOnce || DoOnce == false);
        }
    }

    protected void Awake()
    {
        DoneOnce = JunkerGameMode.HasDoneStoryPart(EventChain.name);

        var clawTarget = GetComponent<ClawTarget>();
		if (clawTarget == null)
		{
			return;
		}

		clawTarget.SetLightsVisible(DoOnce == true && DoOnce != DoneOnce || DoOnce == false);

        switch (eventTrigger)
        {
            case E_EventTrigger.OnClawConnect:
                clawTarget.OnClawConnect += Add;
                break;
            case E_EventTrigger.OnShipDocked:
                clawTarget.OnShipDocked += Add;
                break;
            case E_EventTrigger.OnShipUnDocked:
                clawTarget.OnShipUnDocked += Add;
                break;
            default:
                break;
        }
    }

    
}
