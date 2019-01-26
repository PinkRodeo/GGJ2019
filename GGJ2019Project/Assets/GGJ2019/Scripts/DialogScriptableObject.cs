using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="DialogChain",menuName ="Dialog")]
public class DialogScriptableObject : ScriptableObject
{
    [SerializeField]
    public List<GameEvent> eventChain = new List<GameEvent>();
    
}
