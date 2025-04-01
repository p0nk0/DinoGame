using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameState { wait, tutorial1, puzzle1, itemScanned, updateProgressBar, puzzle1Win, puzzle1Fail, tutorial2, puzzle2, puzzle2Win, puzzle2Fail};

public class GameStateManager : MonoBehaviour
{
    private readonly HashSet<GameState> puzzle1States = new HashSet<GameState> { GameState.tutorial1, GameState.puzzle1, GameState.itemScanned, GameState.updateProgressBar, GameState.puzzle1Win, GameState.puzzle1Fail };
    private readonly HashSet<GameState> puzzle2States = new HashSet<GameState> { GameState.tutorial2, GameState.puzzle2, GameState.puzzle2Win, GameState.puzzle2Fail };
    public GameState state = GameState.wait;

    [Header("Puzzle 1")]

    [SerializeField] private GameObject puzzle1;
    [SerializeField] private TextMeshProUGUI stateText1;
    [SerializeField] private TextMeshProUGUI scanLog;
    [SerializeField] private Meter progressBar;
    // TODO: Incorporate Progress Visualizer into state machine.
    //       This is a box of all the items that activates objects as they're scanned.
    [SerializeField] private ProgressVisualizer itemBox;

    //TODO: update systemLog when scanning
    [SerializeField] private SystemLog systemLog;


    [SerializeField] private TextMeshProUGUI timerText1;
    [SerializeField] private float timeLimit1 = 30.0f;
    [SerializeField] private bool onlyValidProps = true;
    [SerializeField] private int maxScans = 5;
    private HashSet<string> itemsScanned = new HashSet<string>();
    private string scannedItem;
    private float startTime;

    private HashSet<string> validProps = new HashSet<string> {
        " f3 2a 46 36", // test rfid card
        " 11 8d 07 7c", " f1 a6 f6 7b", // bugs in amber
        " 41 49 f5 7b", // dinosaur claw
        " 11 87 0a 7c"  // dinosaur bone

    }; 

    // we need this space in front of keys I guess
    private Dictionary<string, string> itemDescriptions = new Dictionary<string, string>
    {
        { " f3 2a 46 36", "Test RFID Card" },
        { " 11 8d 07 7c", "Bug in Amber 1" },
        { " f1 a6 f6 7b", "Bug in Amber 2" },
        { " 41 49 f5 7b", "Dinosaur Claw" },
        { " 11 87 0a 7c", "Dinosaur Bone" }
    };


    [Header("Puzzle 2")]

    [SerializeField] private GameObject puzzle2;
    [SerializeField] private TextMeshProUGUI stateText2;
    [SerializeField] private TextMeshProUGUI lights; // string of 0s and 1s for now
    [SerializeField] private TextMeshProUGUI timerText2;
    [SerializeField] private float timeLimit2 = 30.0f;

    [Header("UI")]
    [SerializeField] private GameObject resetPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Misc")]

    [SerializeField] private bool debugMode = false;

    // Start is called before the first frame update
    void Start()
    {
        puzzle1.SetActive(false);
        puzzle2.SetActive(false);
        resetPanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (puzzle1States.Contains(state)) {
            stateText1.text = state.ToString();
        } else {
            stateText2.text = state.ToString();
        }

        if (startTime > 0) {
            float elapsedTime = Time.time - startTime;

            if (puzzle1States.Contains(state)) {
                float timeLeft = Mathf.Max(timeLimit1 - elapsedTime, 0);
                if (timerText1) timerText1.text = $"Time: {timeLeft:F1}s";
            } else {
                float timeLeft = Mathf.Max(timeLimit2 - elapsedTime, 0);
                if (timerText2) timerText2.text = $"Time: {timeLeft:F1}s";
            }

            if (puzzle1States.Contains(state) && elapsedTime >= timeLimit1 && state != GameState.puzzle1Win && state != GameState.puzzle1Fail) {
                state = GameState.puzzle1Fail;
                Debug.Log("User ran out of time on puzzle 1");
            } else if (puzzle2States.Contains(state) && elapsedTime >= timeLimit2 && state != GameState.puzzle2Win && state != GameState.puzzle2Fail) {
                state = GameState.puzzle2Fail;
                Debug.Log("User ran out of time on puzzle 2");
            }
        }
        
        switch (state) {
            case GameState.wait:
                // transitions to tutorial1 if anything is scanned
                resetPanel.SetActive(true);
                winPanel.SetActive(false);
                losePanel.SetActive(false); 

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
                scanLog.text = $"Waiting to scan ...";
                progressBar.ResetValue();
                puzzle1.SetActive(true);
                resetPanel.SetActive(false);
                
                if (Input.anyKeyDown) // TODO: or stay for only a few seconds
                {
                    Debug.Log("Transitioning to puzzle1 state.");
                    state = GameState.puzzle1;
                }
                break;

            case GameState.puzzle1:
                // transitions to itemScanned if anything is scanned
                // does nothing except for waiting for items to be scanned

                if (debugMode && Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("DEBUG MODE: Skipping to puzzle1Win state.");
                    state = GameState.puzzle1Win;
                }
                else

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
                scanLog.text += $"\n{itemDescriptions[scannedItem]} Scanned!";
                progressBar.SetValuePercentage((float)itemsScanned.Count/maxScans);
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
                puzzle1.SetActive(false);
                puzzle2.SetActive(true);
                startTime = Time.time;
                lights.text = " 000000";
                if (Input.anyKeyDown) // TODO: or stay for only a few seconds
                {
                    Debug.Log("Transitioning to puzzle2 state.");
                    state = GameState.puzzle2;
                }
                break;
            
            case GameState.puzzle2:
                // transitions to puzzle2Win if lights are correct
                { if (lights.text == "101001")
                    {
                        Debug.Log("Correct lights, transitioning to puzzle2Win state.");
                        state = GameState.puzzle2Win;
                    }
                }
                break;

            case GameState.puzzle2Win:
                // TODO: Play win cutscene
                // TODO: communicate with dinosaur
                puzzle2.SetActive(false);
                winPanel.SetActive(true);
                startTime = 0;
                if (Input.anyKeyDown) // TODO: stay for only a few seconds
                {
                    Debug.Log("Transitioning to wait state.");
                    state = GameState.wait;
                }
                break;

            case GameState.puzzle2Fail:
                // TODO: Play fail cutscene
                // TODO: communicate with dinosaur
                puzzle2.SetActive(false);
                losePanel.SetActive(true); 
                startTime = 0;
                if (Input.anyKeyDown) // TODO: stay for only a few seconds
                {
                    Debug.Log("Transitioning to wait state.");
                    state = GameState.wait;
                }
                break;

        }
    }

    // this function is called by the RFID MessageListner whenever any item is scanned.
    public void HandleScannedItem(string item) {
        if (state == GameState.puzzle1 || state == GameState.tutorial1) {
            if (onlyValidProps && !itemDescriptions.ContainsKey(item)) {
                Debug.Log("Invalid item scanned: " + item);
                return;
            }
            Debug.Log($"Item scanned: {itemDescriptions[item]}, transitioning to itemScanned state.");
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

    public void HandleLights(string lightString) {
        if (state == GameState.puzzle2) {
            lights.text = lightString;
        } else {
            Debug.Log($"Lights updated, but current state is {state.ToString()}.");
            return;
        }
    }
}
