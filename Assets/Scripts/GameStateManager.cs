using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameState { wait, tutorial1, puzzle1, itemScanned, updateProgressBar, puzzle1Win, puzzle1Fail, tutorial2 }


public class GameStateManager : MonoBehaviour
{
    public GameState state = GameState.wait;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI progressBar; // TODO: Make this a Meter object
    // TODO: Add Progress Visualizer, which is a box of all the items that activates objects as they're scanned.
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float timeLimit = 60.0f;
    [SerializeField] private int maxScans = 5;
    private HashSet<string> itemsScanned = new HashSet<string>();
    private string scannedItem;
    private float startTime;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        stateText.text = state.ToString();

        if (startTime > 0) {
            float elapsedTime = Time.time - startTime;
            float timeLeft = Mathf.Max(timeLimit - elapsedTime, 0);

            if (timerText) timerText.text = $"Time: {timeLeft:F1}s";

            if (elapsedTime >= timeLimit && state != GameState.puzzle1Win && state != GameState.puzzle1Fail) {
                if (state == GameState.puzzle1 || state == GameState.updateProgressBar || state == GameState.itemScanned) {
                    state = GameState.puzzle1Fail;
                    Debug.Log("User ran out of time on puzzle 1");
                }
                else {
                    Debug.Log("User ran out of time on an unknown state");
                }
            }
        }
        
        switch (state) {
            case GameState.wait:
                 // transitions to tutorial1 if anything is scanned
                if (Input.anyKeyDown)
                {
                    Debug.Log("Transitioning to tutorial1 state.");
                    state = GameState.tutorial1;
                }
                break;

            case GameState.tutorial1:
                // transitions to puzzle1 if anything is scanned
                startTime = Time.time;
                itemsScanned = new HashSet<string>();
                progressBar.text = $"{itemsScanned.Count}/{maxScans} items scanned";
                
                if (Input.anyKeyDown) // TODO: or stay for only a few seconds
                {
                    Debug.Log("Transitioning to puzzle1 state.");
                    state = GameState.puzzle1;
                }
                break;

            case GameState.puzzle1:
                // transitions to itemScanned if anything is scanned
                // does nothing except for waiting for items to be scanned

                if (itemsScanned.Count >= maxScans)
                {
                    Debug.Log("All items scanned, transitioning to puzzle1Win state.");
                    state = GameState.puzzle1Win;
                }
                break;

            case GameState.itemScanned:
                if (itemsScanned.Add(scannedItem)) // if item is new
                {
                    Debug.Log("New item scanned: " + scannedItem);
                    state = GameState.updateProgressBar;
                }
                else // item is a duplicate
                {
                    Debug.Log("Item already scanned: " + scannedItem);
                    state = GameState.puzzle1;
                }
                break;

            case GameState.updateProgressBar:
                progressBar.text = $"{itemsScanned.Count}/{maxScans} items scanned";
                // TODO: Update progress bar UI, add scan log
                // TODO: Update progress visualizer
                state = GameState.puzzle1;
                Debug.Log("Updated progress bar, transitioning to puzzle1 state.");
                break;

            case GameState.puzzle1Win:
                // TODO: Play win cutscene
                // TODO: communicate with dinosaur
                state = GameState.tutorial2;
                startTime = 0;
                Debug.Log("Transitioning to tutorial2 state.");
                break;

            case GameState.puzzle1Fail:
                // TODO: Play fail cutscene
                // TODO: communicate with dinosaur
                state = GameState.tutorial2;
                startTime = 0;
                Debug.Log("Transitioning to tutorial2 state.");
                break;

            case GameState.tutorial2:
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Debug.Log("Transitioning to wait state.");
                    state = GameState.wait;
                }
                break;

        }
    }

    // this function is called by the RFID MessageListner whenever any item is scanned.
    public void HandleScannedItem (string item) {
        if (state == GameState.puzzle1 || state == GameState.tutorial1) {
            Debug.Log("Item scanned, transitioning to itemScanned state.");
            scannedItem = item;
            state = GameState.itemScanned;
        } else if (state == GameState.wait) {
            Debug.Log("Item scanned, transitioning to tutorial1 state.");
            state = GameState.tutorial1;
        } else {
            Debug.Log($"Item scanned, but current state is {state.ToString()}.");
            return;
        }
    }
}
