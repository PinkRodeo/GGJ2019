using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEvents : MonoBehaviour
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

    private void Start()
    {
        Add();
    }
}
