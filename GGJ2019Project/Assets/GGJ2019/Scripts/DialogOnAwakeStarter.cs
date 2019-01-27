using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogOnAwakeStarter : MonoBehaviour
{
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

    private void Awake()
    {
        if (!JunkerGameMode.HasDoneStoryPart(EventChain.name))
            Add();
    }
}
