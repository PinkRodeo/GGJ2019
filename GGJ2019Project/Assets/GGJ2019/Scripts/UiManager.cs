using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public enum PanelType
{
    Narrative,
    NarrativeWithOptions
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


    private void Awake()
    {
        HideAllPanels();
        EventManager.OnEventStart += ShowChoices;
        EventManager.onEventListEmpty += EventListEmpty;
    }

    private void Update()
    {
        // Debug.Log(TextPrinter.IsPrinting());
        if (CurrentPanel != null)
        {
            if (TextPrinter != null && TextPrinter.IsPrinting() && Input.GetKeyDown(KeyCode.Space))
            {
                TextPrinter.ForceFinish();
                return;// prevent from multiple keys being pressed the same frame
            }
            switch (CurrentPanel.panelType)
            {
                case PanelType.Narrative:
                    if (!CurrentPanel.HasActiveButton())
                    {
                        if ((TextPrinter != null && !TextPrinter.IsPrinting() || TextPrinter == null) && Input.GetKeyDown(KeyCode.Space))
                            EventManager.onEventFinish.Invoke();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void ShowChoices(GameEvent gameEvent)
    {
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
        if(LastEvent == null)
        {
            Vector3 oldPos = Vector3.zero + CurrentPanel.panel.transform.position;

            CurrentPanel.panel.transform.position = oldPos + Vector3.down *250;
            CurrentPanel.panel.transform.DOMove(oldPos, .3f);
        }

        panel.HideAllButtons();

        if (panel.title != null)
            panel.title.text = CurrentEvent.title ?? LastEvent?.title ?? "";

        if (CurrentEvent.TextPrintInterval > 0)
        {
            TextPrinter = new DialogPrinter(CurrentEvent.description, CurrentEvent.TextPrintInterval, this);
            panel.Description.text = "";
            TextPrinter.OnTextUpdate += UpdateDescritpion;
            TextPrinter.OnPrinterFinished += AssignOptions;
            TextPrinter.OnPrinterFinished += UpdateSecodnDescription;
        }
        else
        {
            panel.Description.text = CurrentEvent.description;
            AssignOptions();
        }

        if (panel.image != null)
        {
            if (CurrentEvent.image == null || LastEvent?.image == null)
            {
                panel.image.gameObject.SetActive(false);
            }
            else
            {
                panel.image.gameObject.SetActive(true);
                panel.image.sprite = CurrentEvent.image ?? LastEvent?.image;
            }
        }

        if (panel.SecondDescription != null)
        {
            panel.SecondDescription.text = TextPrinter != null && TextPrinter.IsPrinting() ? "SPACE TO SKIP" : "SPACE TO CONTINUE >";

            panel.SecondDescription.gameObject.SetActive(true);
        }
    }

    void UpdateSecodnDescription()
    {
        if (CurrentPanel.SecondDescription != null)
            CurrentPanel.SecondDescription.text = TextPrinter.IsPrinting() ? "SPACE TO SKIP" : "SPACE TO CONTINUE >";
    }

    void AssignOptions()
    {
        for (int i = 0; i < CurrentPanel.KeuzeButtons.Length; i++)
        {
            CurrentPanel.KeuzeButtons[i].onClick.RemoveAllListeners();

            if (CurrentEvent != null && CurrentEvent.Keuzes != null && i >= CurrentEvent.Keuzes.Count)
            {
                CurrentPanel.KeuzeButtons[i].gameObject.SetActive(false);
            }
            else
            {
                CurrentPanel.KeuzeButtons[i].gameObject.SetActive(true);
                CurrentPanel.KeuzeButtons[i].GetComponentInChildren<Text>().text = CurrentEvent.Keuzes[i].text;

                if (CurrentEvent.Keuzes[i].PressedDialogs != null)
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
        EventManager.ReplaceCurrentEvents(CurrentEvent.Keuzes[0].PressedDialogs.eventChain);
    }

    void OnButton2Pressed()
    {
        EventManager.ReplaceCurrentEvents(CurrentEvent.Keuzes[1].PressedDialogs.eventChain);
    }

    private void UpdateDescritpion(string text)
    {
        CurrentPanel.Description.text = text;
    }



    private void EventListEmpty()
    {
        LastEvent = null;
        HideAllPanels();
    }

    void RemoveMenuOnChoicePressed()
    {
        EventManager.onEventFinish.Invoke();
    }

    DialogPanel GetPanelOfType(PanelType _panelType)
    {
        for (int i = 0; i < dialogPanels.Length; i++)
        {
            if (dialogPanels[i].panelType == _panelType)
                return dialogPanels[i];
        }

        Debug.LogWarning("no type of this panel found");
        return null;
    }

    void HideAllPanels()
    {
        foreach (DialogPanel item in dialogPanels)
        {
            item.panel.SetActive(false);
        }
    }
}
