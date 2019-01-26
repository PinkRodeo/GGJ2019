using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OptionData
{
    public string text;
    public DialogScriptableObject PressedDialogs;
    //public string hoverText;

    public OptionData SetText(string _text) { text = _text; return this; }
    public OptionData SetPressedActionList(DialogScriptableObject _actions) { PressedDialogs = _actions; return this; }
    //public OptionData SetHoverText(string _hoverText) { hoverText = _hoverText; return this; }
}

[System.Serializable]
public class GameEvent
{
    [HideInInspector]
    public PanelType eventType;
    [HideInInspector]
    public Sprite image;
    [HideInInspector]
    public string title;
    [TextArea]
    public string description;
    public float TextPrintInterval = 1;
    public List<OptionData> Keuzes = new List<OptionData>();

    public GameEvent SetImage(Sprite _image) { image = _image; return this; }
    public GameEvent SetTitle(string _title) { title = _title; return this; }
    public GameEvent SetDescription(string _description) { description = _description; return this; }
    public GameEvent SetKeuzes(List<OptionData> _KeuzeData) { Keuzes = _KeuzeData; return this; }
    public GameEvent SetTextPrintInterval(float time) { TextPrintInterval = time; return this; }
}

public class EventManager : MonoBehaviour
{
    static EventManager instance;

    public delegate void GameEventDelegate(GameEvent excutedEvent);

    public delegate void OnChoiceSelectedDelegate(string choice);

    public delegate void Delegate();

    Queue<GameEvent> eventQue = new Queue<GameEvent>();

    public static GameEventDelegate OnEventStart;
    public static Delegate onEventFinish;
    public static Delegate onEventListEmpty;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }

        onEventFinish += FinishEvent;
    }

    public static void AddEvent(GameEvent gameEvent)
    {
        instance.eventQue.Enqueue(gameEvent);

        //als er geen ander item in de lijst is dan de item die we zojuist hebben toegevoegd
        if (instance.eventQue.Count == 1)
        {
            instance.ExecuteNextItemInQue();

        }
    }

    public static void ReplaceCurrentEvents(List<GameEvent> newEvents)
    {
        instance.eventQue.Clear();


        instance.eventQue.Enqueue(newEvents[0]);
        foreach (GameEvent gameEvent in newEvents)
        {
            instance.eventQue.Enqueue(gameEvent);
        }
        
        instance.ExecuteNextItemInQue();
    }

    public static void ClearEvents()
    {
        instance.eventQue.Clear();
        onEventListEmpty.Invoke();
    }

    void FinishEvent()
    {
        //als we momenteel geen event hebben
        if (eventQue.Count == 0)
            return;

        eventQue.Dequeue();

        //als er geen event overgebleven is
        if (eventQue.Count == 0)
        {
            onEventListEmpty.Invoke();
            return;
        }

        ExecuteNextItemInQue();
    }

    void ExecuteNextItemInQue()
    {
        OnEventStart.Invoke(eventQue.Peek());
    }
}
