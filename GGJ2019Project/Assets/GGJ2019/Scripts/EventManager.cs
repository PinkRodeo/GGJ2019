using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using FMODUnity;

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
    public PanelType eventType;
    public Sprite image;
    [HideInInspector]
    public string title;
    [TextArea]
    public string description;
    public float TextPrintInterval = 1;
    public List<OptionData> Choices = new List<OptionData>();

    public GameEvent SetImage(Sprite _image) { image = _image; return this; }
    public GameEvent SetTitle(string _title) { title = _title; return this; }
    public GameEvent SetDescription(string _description) { description = _description; return this; }
    public GameEvent SetChoices(List<OptionData> _choiceData) { Choices = _choiceData; return this; }
    public GameEvent SetTextPrintInterval(float time) { TextPrintInterval = time; return this; }
}

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    public delegate void GameEventDelegate(GameEvent excutedEvent);

    public delegate void OnChoiceSelectedDelegate(string choice);

    public delegate void Delegate();

    Queue<GameEvent> eventQueue = new Queue<GameEvent>();

    public static GameEventDelegate OnEventStart;
    public static Delegate onEventFinish;
    public static Delegate onEventListEmpty;
    public StudioEventEmitter sfx_showUI;


    private void Awake()
    {
        if (instance != null)
        {
            GameObject.Destroy(this);
            return;
        }
        else
        {
            instance = this;
        }

        onEventFinish += FinishEvent;
    }

    protected void Update()
    {
        if (IsEventActive())
        {
            Time.timeScale = 0.2f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public static void AddEvent(GameEvent gameEvent)
    {
        instance.eventQueue.Enqueue(gameEvent);

        //als er geen ander item in de lijst is dan de item die we zojuist hebben toegevoegd
        if (instance.eventQueue.Count == 1)
        {
            instance.ExecuteNextItemInQue();

            if (instance.sfx_showUI != null)
                instance.sfx_showUI.Play();
        }
    }

    public static void ReplaceCurrentEvents(List<GameEvent> newEvents)
    {
        instance.eventQueue.Clear();


        instance.eventQueue.Enqueue(newEvents[0]);
        foreach (GameEvent gameEvent in newEvents)
        {
            instance.eventQueue.Enqueue(gameEvent);
        }

        instance.ExecuteNextItemInQue();
    }

    public static void ClearEvents()
    {
        instance.eventQueue.Clear();
        onEventListEmpty.Invoke();
    }

    void FinishEvent()
    {
        //als we momenteel geen event hebben
        if (eventQueue.Count == 0)
            return;

        eventQueue.Dequeue();

        //als er geen event overgebleven is
        if (eventQueue.Count == 0)
        {
            onEventListEmpty.Invoke();
            return;
        }

        ExecuteNextItemInQue();
    }

    void ExecuteNextItemInQue()
    {
        OnEventStart.Invoke(eventQueue.Peek());
    }

    public bool IsEventActive()
    {
        return eventQueue.Count != 0;
    }
}
