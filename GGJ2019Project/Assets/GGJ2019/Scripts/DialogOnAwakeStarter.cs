using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogOnAwakeStarter : MonoBehaviour
{
    [SerializeField] DialogScriptableObject EventChain;

    [ContextMenu("AddItems")]
    public void Add()
    {
        if (EventChain != null)
            foreach (GameEvent item in EventChain.eventChain)
            {
                EventManager.AddEvent(item);
            }
    }

    private void Awake()
    {
        Add();
    }
}
