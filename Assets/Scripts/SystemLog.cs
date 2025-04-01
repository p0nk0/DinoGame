using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class SystemLog : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> lines;
    private List<string> messages;
    private int cursor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        messages = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /********************************************************
     * Adds a message to the log,
     * and returns any message that was removed from the log
     ********************************************************/
    public void LogMessage(string message)
    {
        string logTime = DateTime.UtcNow.AddHours(-4).ToString("HH:mm:ss");
        if (cursor <= lines.Count - 1) // free lines
        {
            messages.Add(logTime + " " + message);
            lines[cursor].text = logTime + " " + message;
            cursor++;
        }
        else // need to move lines back
        {
            messages.RemoveAt(0);
            messages.Add(logTime + " " + message);
            for (int i = 0; i < lines.Count; i++)
                lines[i].text = messages[i];
        }
    }
}
