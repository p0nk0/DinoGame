using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Enables and disables items in a grid using keys
// and reorders elements in a Grid Layout as they're enabled/disabled
[RequireComponent(typeof(GridLayoutGroup))]

public class ProgressVisualizer : MonoBehaviour
{
    [SerializeField] protected List<string> objectIds;
    [SerializeField] protected List<GameObject> gameObjects;
    protected Dictionary<string, GameObject> idToObject;
    protected int numEnabledObjs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TODO: Ensure all gameObjects are child of this object (remove non children from list)
        
        // Ensure that unique objectID count and the count of gameObjects are equal

        // numEnabledObjs = number of enabled objects in gameObjects
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*************************************************************
     * Activates an object (visually) if it can be found using id
     *************************************************************/
    public void ActivateObjectByID(string id)
    {
        // If idToObject contains id, activate and reorder children using SetSiblingIndex
        if (idToObject.ContainsKey(id))
        {
            GameObject obj = idToObject[id];
            if (!obj.activeInHierarchy)
            {
                obj.transform.SetSiblingIndex(numEnabledObjs);
                numEnabledObjs++;
            }
        }
    }

    /***************************************************************
     * Deactivates an object (visually) if it can be found using id
     ***************************************************************/
    public void DeactivateObjectByID(string id)
    {
        // If idToObject contains id, deactivate
        if (idToObject.ContainsKey(id))
        {
            GameObject obj = idToObject[id];
            if (obj.activeInHierarchy)
                numEnabledObjs--;
        }
    }
}
