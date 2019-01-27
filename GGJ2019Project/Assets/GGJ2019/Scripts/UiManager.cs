using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using FMODUnity;

public enum PanelType
{
    Narrative,
    NarrativeWithOptions,
    CutScene
}

[System.Serializable]
public class DialogPanel
{
    public PanelType panelType;
    public GameObject panel;
    public Text title;
    public Image image;
    public Text Description;
    public Text SecondDescription;
    public Button[] KeuzeButtons;

    public bool HasActiveButton()
    {
        foreach (Button _button in KeuzeButtons)
        {
            if (_button.gameObject.activeSelf)
                return true;
        }
        return false;
    }

    public void HideAllButtons()
    {
        foreach (Button _button in KeuzeButtons)
        {
            _button.gameObject.SetActive(false);
        }
    }
}

public class UiManager : MonoBehaviour
{
    [SerializeField] DialogPanel[] dialogPanels;

    GameEvent LastEvent;
    GameEvent CurrentEvent;

    DialogPrinter TextPrinter;
    DialogPanel CurrentPanel;

    public StudioEventEmitter sfx_hideUI;

    private void Awake()
    {
        HideAllPanels();
        EventManager.OnEventStart += ShowChoices;
        EventManager.onEventListEmpty += EventListEmpty;


        if (EventManager.instance.eventQueue.Count != 0)
            ShowChoices(EventManager.instance.eventQueue.Peek());
    }

    private void Update()
    {
        // Debug.Log(TextPrinter.IsPrinting());
        if (CurrentPanel != null)
        {
            if (TextPrinter != null && TextPrinter.IsPrinting() && Input.GetMouseButtonDown(0))
            {
                TextPrinter.ForceFinish();
                return;// prevent from multiple keys being pressed the same frame
            }
            else if (!CurrentPanel.HasActiveButton())
            {
                if ((TextPrinter != null && !TextPrinter.IsPrinting() || TextPrinter == null) && Input.GetMouseButtonDown(0))
                    EventManager.onEventFinish.Invoke();
            }

        }
    }

    private void OnDestroy()
    {
        EventManager.OnEventStart -= ShowChoices;
        EventManager.onEventListEmpty -= EventListEmpty;
    }

    private void ShowChoices(GameEvent gameEvent)
    {
        //DontDestroyOnLoad(this);
        if (!gameObject.activeSelf)
            return;

        CurrentEvent = gameEvent;
        HideAllPanels();
        DrawMenu(GetPanelOfType(gameEvent.eventType));
        LastEvent = gameEvent;
    }

    private void DrawMenu(DialogPanel panel)
    {
        CurrentPanel = panel;
        panel.panel.SetActive(true);

        //als dit de eerste text menu van de reeks is
        if (LastEvent == null)
        {
            if (panel.panelType == PanelType.Narrative)
            {
                Vector3 oldPos = Vector3.zero + CurrentPanel.panel.transform.position;

                CurrentPanel.panel.transform.position = oldPos + Vector3.down * 250;
                CurrentPanel.panel.transform.DOMove(oldPos, .3f);
            }
        }

        panel.HideAllButtons();

        if (panel.title != null)
            panel.title.text = CurrentEvent.title ?? LastEvent?.title ?? "";

        if (CurrentEvent.TextPrintInterval > 0)
        {
            TextPrinter = new DialogPrinter(CurrentEvent.description, CurrentEvent.TextPrintInterval, this);
            panel.Description.text = "";
            TextPrinter.OnTextUpdate += UpdateDescription;
            TextPrinter.OnPrinterFinished += AssignOptions;
            TextPrinter.OnPrinterFinished += UpdateSecodnDescription;
        }
        else
        {
            panel.Description.text = CurrentEvent.description;
            AssignOptions();
        }

        if (panel.panelType == PanelType.Narrative)
        {
            if (panel.image != null)
                if (CurrentEvent.image != null)
                {
                    panel.image.gameObject.SetActive(true);
                    panel.image.sprite = CurrentEvent.image ?? LastEvent?.image;
                }
                else
                {
                    panel.image.gameObject.SetActive(false);
                }
        }
        else if (panel.panelType == PanelType.CutScene)
        {
            if (panel.image != null)
            {
                if (CurrentEvent.FadeTime == 0)
                {
                    panel.image.sprite = CurrentEvent.image;
                    panel.image.color = new Color(1, 1, 1, 1);
                }
                else
                {
                    Debug.Log("test");
                    panel.image.sprite = CurrentEvent.image;
                    panel.image.color = new Color(1, 1, 1, 1);
                    panel.image.DOKill(false);
                    panel.image.DOColor(new Color(1, 1, 1, 0), CurrentEvent.FadeTime);
                }
            }
        }

        if (panel.SecondDescription != null)
        {
            panel.SecondDescription.text = TextPrinter != null && TextPrinter.IsPrinting() ? "..." : "PRESS TO CONTINUE >";

            panel.SecondDescription.gameObject.SetActive(true);
        }
    }

    void UpdateSecodnDescription()
    {
        if (CurrentPanel.SecondDescription != null)
            CurrentPanel.SecondDescription.text = TextPrinter.IsPrinting() ? "..." : "PRESS TO CONTINUE >";
    }

    void AssignOptions()
    {
        for (int i = 0; i < CurrentPanel.KeuzeButtons.Length; i++)
        {
            CurrentPanel.KeuzeButtons[i].onClick.RemoveAllListeners();

            if (CurrentEvent != null && CurrentEvent.Choices != null && i >= CurrentEvent.Choices.Count)
            {
                CurrentPanel.KeuzeButtons[i].gameObject.SetActive(false);
            }
            else
            {
                CurrentPanel.KeuzeButtons[i].gameObject.SetActive(true);
                CurrentPanel.KeuzeButtons[i].GetComponentInChildren<Text>().text = CurrentEvent.Choices[i].text;

                if (CurrentEvent.Choices[i].PressedDialogs != null)
                {
                    if (i == 0)
                        CurrentPanel.KeuzeButtons[i].onClick.AddListener(() => OnButton1Pressed());
                    else
                        CurrentPanel.KeuzeButtons[i].onClick.AddListener(() => OnButton2Pressed());
                    //
                }

                CurrentPanel.KeuzeButtons[i].onClick.AddListener(RemoveMenuOnChoicePressed);
            }

        }

        if (CurrentPanel.SecondDescription != null && CurrentPanel.HasActiveButton())
        {
            CurrentPanel.SecondDescription.gameObject.SetActive(false);
        }
    }

    void OnButton1Pressed()
    {
        EventManager.ReplaceCurrentEvents(CurrentEvent.Choices[0].PressedDialogs.eventChain);
    }

    void OnButton2Pressed()
    {
        EventManager.ReplaceCurrentEvents(CurrentEvent.Choices[1].PressedDialogs.eventChain);
    }

    private void UpdateDescription(string text)
    {
        CurrentPanel.Description.text = text;
    }

    private void EventListEmpty()
    {
        //Destroy(gameObject);
        LastEvent = null;
        HideAllPanels();
        sfx_hideUI.Play();

    }

    void RemoveMenuOnChoicePressed()
    {
        EventManager.onEventFinish.Invoke();
    }

    DialogPanel GetPanelOfType(PanelType _panelType)
    {
        for (int i = 0; i < dialogPanels.Length; i++)
        {
            if (dialogPanels[i].panelType == _panelType &&
                dialogPanels[i].panel != null)
                return dialogPanels[i];
        }

        Debug.LogError("no type of this panel found " + _panelType.ToString());
        return null;
    }

    void HideAllPanels()
    {
        foreach (DialogPanel item in dialogPanels)
        {
            if (item?.panel != null)
                item.panel.SetActive(false);
        }
    }
}
