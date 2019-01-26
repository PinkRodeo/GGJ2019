using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogPrinter
{
    public delegate void PrintUpdate(string text);
    public delegate void PrinterFinishedDelegate();

    enum DialogState
    {
        none,
        printing,
        finished
    }

    string currentText = "";
    string totalText;
    float interval = 0;
    DialogState currentState;
    KeyCode chatInteractionKey = KeyCode.Space;

    public PrintUpdate OnTextUpdate;
    public PrinterFinishedDelegate OnPrinterFinished;

    public DialogPrinter(string text, float interval, MonoBehaviour objectBehaviour)
    {
        currentState = DialogState.none;
        currentText = "";
        totalText = text;
        this.interval = interval;

        objectBehaviour.StartCoroutine(PrintText());
    }

    public bool IsPrinting()
    {
        return currentState == DialogState.printing;
    }

    IEnumerator PrintText()
    {
        currentState = DialogState.printing;
        for (int i = 0; i < totalText.Length && currentState == DialogState.printing; i++)
        {
            currentText += totalText[i];
            //to prevent errors on empty delegate calls;
            try
            {
                OnTextUpdate(currentText);
            }
            catch (System.Exception) { }

            yield return new WaitForSecondsRealtime(0.05f * interval);
        }

        currentState = DialogState.finished;
        OnPrinterFinished.Invoke();
    }

    public void ForceFinish()
    {
        currentText = totalText;
        //to prevent errors on empty delegate calls;
        try
        {
            OnTextUpdate(currentText);
        }
        catch (System.Exception) { }

        currentState = DialogState.finished;
    }
}
