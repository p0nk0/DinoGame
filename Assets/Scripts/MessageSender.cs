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
public class MessageSenderOld : MonoBehaviour
{
    public SerialController serialController; // Reference to the SerialController

    void Start()
    {
        // Automatically find and assign the SerialController from the scene.
        serialController = GetComponent<SerialController>();
    }
    
    void Update()
    {
        // Test message sending
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("sending r");
            serialController.SendSerialMessage("r\n");
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("sending j");
            serialController.SendSerialMessage("j\n");
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("sending h");
            serialController.SendSerialMessage("h");
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("sending g");
            serialController.SendSerialMessage("g");
        }
    }
}
