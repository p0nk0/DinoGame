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
public class MessageListener : MonoBehaviour
{
    private enum ArduinoType { RFID, PuzzleBox, Dino }
    [SerializeField] private ArduinoType arduinoType;
    [SerializeField] private GameStateManager stateManager;

    // Invoked when a line of data is received from the serial device.
    void OnMessageArrived(string msg)
    {
        Debug.Log(arduinoType.ToString() + ": " + msg);
        if (arduinoType == ArduinoType.RFID) stateManager.HandleScannedItem(msg);
        else if (arduinoType == ArduinoType.PuzzleBox) stateManager.HandleLights(msg);
        else if (arduinoType == ArduinoType.Dino) {
            Debug.Log("HELLO ??");
        };
    }

    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    void OnConnectionEvent(bool success)
    {
        if (success)
            Debug.Log("Connection established");
        else
            Debug.Log("Connection attempt failed or disconnection detected");
    }
}
