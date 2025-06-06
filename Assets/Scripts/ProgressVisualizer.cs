using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Enables and disables items in a grid using keys
// and reorders elements in a Grid Layout as they're enabled/disabled
[RequireComponent(typeof(GridLayoutGroup))]

public class ProgressVisualizer : MonoBehaviour
{
    [Tooltip("Used to create an id to item mapping")]
    [SerializeField] protected List<string> itemIds;
    [Tooltip("Used to create an id to item mapping")]
    [SerializeField] protected List<GameObject> items;
    [Tooltip("Sprite color when scanned (be sure to check the alpha value!)")]
    [SerializeField] protected Color scannedColor;
    [Tooltip("Sprite color when not yet scanned (be sure to check the alpha value!)")]
    [SerializeField] protected Color notScannedColor;
    protected Dictionary<string, (GameObject, bool)> idToItem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        idToItem = new Dictionary<string, (GameObject, bool)>();
        // Ensure all gameObjects are child of this object (remove non children from list)
        for (int i = 0; i < items.Count; i++)
        {
            if (!items[i].transform.IsChildOf(transform))
            {
                Debug.LogWarning("Object \"" + items[i] + "\" is not a child of the GridLayoutGroup \"" + gameObject + "\".");
                items.RemoveAt(i);
                i--;
            }
        }

        
        int j = 0;
        while (j < itemIds.Count && j < items.Count)
        {
            if (idToItem.ContainsKey(itemIds[j]))
                Debug.LogWarning("Duplicate id \"" + itemIds + "\" found! Failed to map to item \"" + items[j] + "\"");
            else
                idToItem.Add(itemIds[j], (items[j], false));

            j++;
        }

        if (itemIds.Count > items.Count)
            Debug.LogWarning("Some ids did not get mapped to any items");
        else if (itemIds.Count < items.Count)
            Debug.LogWarning("Some items did get receive any ids.");

        // numEnabledObjs = number of enabled objects in gameObjects
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*************************************************************
     * Activates an object (visually) if it can be found using id
     *************************************************************/
    public void ActivateItemByID(string id)
    {
        Debug.Log("attempt");
        // If idToObject contains id, activate and reorder children using SetSiblingIndex
        if (idToItem.ContainsKey(id))
        {
            (GameObject, bool) obj = idToItem[id];
            if (!obj.Item2)
            {
                Image image = obj.Item1.GetComponent<Image>();
                if (image)
                    image.color = scannedColor;

                obj.Item2 = true;
            }
        }
    }

    /***************************************************************
     * Deactivates an object (visually) if it can be found using id
     ***************************************************************/
    public void DeactivateObjectByID(string id)
    {
        // If idToObject contains id, deactivate
        if (idToItem.ContainsKey(id))
        {
            (GameObject, bool) obj = idToItem[id];
            if (obj.Item2)
            {
                Image image = obj.Item1.GetComponent<Image>();
                if (image)
                    image.color = notScannedColor;

                obj.Item2 = false;
            }
        }
    }
}
