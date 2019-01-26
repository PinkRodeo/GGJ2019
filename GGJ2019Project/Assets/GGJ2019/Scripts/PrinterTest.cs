using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class PrinterTest : MonoBehaviour
{
    DialogPrinter dialogPrinter;

    [SerializeField] string testText;
    [SerializeField] float interval;

    // Start is called before the first frame update
    void Start()
    {
        dialogPrinter = new DialogPrinter(testText, interval, this);
        dialogPrinter.OnTextUpdate += UpdateText;
    }

    void UpdateText(string text)
    {
        GetComponent<Text>().text = text;
    }
}
