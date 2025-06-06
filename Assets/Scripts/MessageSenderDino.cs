/**
 * Ardity (Serial Communication for Arduino + Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */

using UnityEngine;
using System.Collections;

/**
 * When creating your message listeners you need to implement these two methods:
 *  - OnMessageArrived
 *  - OnConnectionEvent
 */
public class MessageSender : MonoBehaviour
{
    public SerialController serialController; // Reference to the SerialController

    void Start()
    {
        // Automatically find and assign the SerialController from the scene.
        serialController = GetComponent<SerialController>();
    }

    new void SendMessage(string message)
    {
        Debug.Log("Dino sender: Sending message: " + message);
        if (serialController != null)
        {
            serialController.SendSerialMessage(message);
        }
        else
        {
            Debug.LogWarning("Dino sender: SerialController is not assigned.");
        }
    }
}
