using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[Serializable]
public class Switch
{
    private bool status = false; // true = on
    public Image buttonIcon;
    [Tooltip("Keep an all black copy of each path *behind* all the \"highlight\" paths in the scene (higher in the hierarchy).")]
    public List<GameObject> paths;
    [Tooltip("Lights are 0-indexed, and path[i] corresponds to connectedLights[i].")]
    public List<int> connectedLights;

    public void SetStatus(bool s, Sprite icon)
    {
        status = s;
        buttonIcon.sprite = icon;
        foreach (GameObject path in paths)
            path.SetActive(s);
    }

    public bool GetStatus()
    {
        return status;
    }
}

[Serializable]
public class Light
{
    private bool status = false; // true = on
    public Image lightIcon;
    [Tooltip("Switches are 0-indexed")]
    public List<int> connectedSwitches;

    public void CheckOn(List<Switch> switches, Sprite onSprite, Sprite offSprite)
    {
        // Read status of all connectedSwitches, and update
        status = false;
        for (int i = 0; i < connectedSwitches.Count; i++)
        {
            int switchi = connectedSwitches[i];
            if (switchi >= 0 && switchi < switches.Count)
                status = status || switches[switchi].GetStatus();
            else
                Debug.LogWarning("Light " + i + " is connected to switch " + switchi + ", which is out of bounds of [0," + switches.Count + ")!");
        }
        // Update UI
        lightIcon.sprite = status ? onSprite : offSprite;
    }

    public bool IsOn()
    {
        return status;
    }
}

// The Circuit System holds and controls the light switches
public class CircuitSystem : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] protected List<Switch> switches;
    [SerializeField] protected List<Light> lights;

    [Header("UI")]
    [SerializeField] protected Sprite switchOnSprite;
    [SerializeField] protected Sprite switchOffSprite;
    [SerializeField] protected Sprite lightOnSprite;
    [SerializeField] protected Sprite lightOffSprite;

    public void SetSwitches(string switchString)
    {
        // Parse string for 1s snd 0s, turn to bool
        int iterCount = Math.Min(switches.Count, switchString.Length);
        for (int i = 0; i < iterCount; i++)
        {
            int bit;
            if (int.TryParse(switchString.Substring(i,1), out bit) && (bit == 0) || (bit == 1))
            {
                if (bit == 1)
                    switches[i].SetStatus(true, switchOnSprite);
                else
                    switches[i].SetStatus(false, switchOffSprite);
            }
        }

        // CheckOn for all connected lights
        for (int i = 0; i < lights.Count; i++)
            lights[i].CheckOn(switches, lightOnSprite, lightOffSprite);
    }

    /******************************************************
     * Returns a string of 1's (on) and 0's (off),
     * where str[i] corresponds to the status of lights[i]
     ******************************************************/
    public string GetLights()
    {
        string fullStatus = "";
        for (int i = 0; i < lights.Count; i++)
        {
            if (lights[i].IsOn())
                fullStatus += "1";
            else
                fullStatus += "0";
        }
        return fullStatus;
    }
}
