using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameState { wait, tutorial1, puzzle1, itemScanned, updateProgressBar, puzzle1Win, puzzle1Fail, tutorial2, puzzle2, puzzle2Win, puzzle2Fail, puzzle2End};

public class GameStateManager : MonoBehaviour
{
    private readonly HashSet<GameState> puzzle1States = new HashSet<GameState> { GameState.tutorial1, GameState.puzzle1, GameState.itemScanned, GameState.updateProgressBar, GameState.puzzle1Win, GameState.puzzle1Fail };
    private readonly HashSet<GameState> puzzle2States = new HashSet<GameState> { GameState.tutorial2, GameState.puzzle2, GameState.puzzle2Win, GameState.puzzle2Fail, GameState.puzzle2End};
    
    private readonly HashSet<GameState> noTimeLimitStates = new HashSet<GameState> {GameState.puzzle1Win, GameState.puzzle1Fail, GameState.tutorial1, GameState.tutorial2, GameState.puzzle2Win, GameState.puzzle2Fail, GameState.puzzle2End};
    public GameState state = GameState.wait;

    [Header("Puzzle 1")]

    [SerializeField] private GameObject puzzle1;
    // [SerializeField] private TextMeshProUGUI scanLog;
    [SerializeField] private Meter progressBar;
    // TODO: Incorporate Progress Visualizer into state machine.
    //       This is a box of all the items that activates objects as they're scanned.
    [SerializeField] private ProgressVisualizer itemBox;

    [SerializeField] private SystemLog systemLog;

    [SerializeField] private float timeLimit1 = 30.0f;
    [SerializeField] private bool onlyValidProps = true;
    [SerializeField] private int maxScans = 5;
    private HashSet<string> itemsScanned = new HashSet<string>();
    private string scannedItem;
    private float startTime;

    [Header("Puzzle 2")]
    // TODO: Assign to empty game object and create diagram
    //       use this to perform switch-light logic and
    //       determine light configuration
    [SerializeField] private CircuitSystem circuitSystem;

    [SerializeField] private GameObject puzzle2;
    // [SerializeField] private TextMeshProUGUI lights; // string of 0s and 1s for now
    
    [SerializeField] private float timeLimit2 = 30.0f;

    [Header("UI")]
    [SerializeField] private GameObject resetPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject dinoPanel;


    [Header("Misc")]

    [SerializeField] public SerialController dino;
    [SerializeField] public AudioSource alarmSound;
    [SerializeField] public AudioSource dinoRoar;
    [SerializeField] public AudioSource dinoGrowl;
    [SerializeField] private AudioSource beep;

    [SerializeField] private bool debugMode = false;
    private HashSet<string> validProps = new HashSet<string> {
        " f3 2a 46 36", // test rfid card (dino saliva, Angie's ID)
        " 11 8d 07 7c", " f1 a6 f6 7b", // bugs in amber
        " 41 49 f5 7b", // dinosaur claw
        " 11 87 0a 7c"  // dinosaur bone

    }; 

    // we need this space in front of keys I guess
    private Dictionary<string, string> itemDescriptions = new Dictionary<string, string>
    {
        { " f3 2a 46 36", "Velociraptor Saliva" },
        { " 11 8d 07 7c", "Saperda Robusta in amber" },
        { " f1 a6 f6 7b", "Plectromerus tertiarius in amber" },
        { " 41 49 f5 7b", "Velociraptor Claw" },
        { " 11 87 0a 7c", "Microceratus Leg" }
    };


    // Start is called before the first frame update
    void Start()
    {
        puzzle1.SetActive(false);
        puzzle2.SetActive(false);
        resetPanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        dinoPanel.SetActive(false);
        Home();
    }

    // Update is called once per frame
    void Update()
    {
        // if (puzzle1States.Contains(state)) {
        //     stateText1.text = state.ToString();
        // } else {
        //     stateText2.text = state.ToString();
        // }

        // Debug.Log(startTime);

        if (startTime > 0) {
            float elapsedTime = Time.time - startTime;

            float timeLeft;

            if (puzzle1States.Contains(state)) {
                timeLeft = Mathf.Max(timeLimit1 - elapsedTime, 0);
                // if (timerText1) timerText1.text = $"Time: {timeLeft:F1}s";
            } else {
                timeLeft = Mathf.Max(timeLimit2 - elapsedTime, 0);
                // if (timerText2) timerText2.text = $"Time: {timeLeft:F1}s";
            }

            // Debug.Log($"Time Left: {timeLeft:F1}s");

            if (puzzle1States.Contains(state) && !noTimeLimitStates.Contains(state) && elapsedTime >= timeLimit1) {
                state = GameState.puzzle1Fail;
                Debug.Log("User ran out of time on puzzle 1");
            } else if (puzzle2States.Contains(state) &&  !noTimeLimitStates.Contains(state) && elapsedTime >= timeLimit2) {
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
                puzzle1.SetActive(false);
                puzzle2.SetActive(false);
                dinoPanel.SetActive(false);

                if (Input.anyKeyDown) // TODO: or anything is scanned
                {
                    Debug.Log("Transitioning to tutorial1 state.");
                    state = GameState.tutorial1;
                }
                break;

            case GameState.tutorial1:
                // transitions to puzzle1 automatically
                startTime = Time.time;
                itemsScanned = new HashSet<string>();
                if(progressBar!=null) {
                    progressBar.ResetValue();
                } else {
                    Debug.LogError("Progress bar is null.");
                }
                if(puzzle1!=null)puzzle1.SetActive(true);
                if(resetPanel!=null)resetPanel.SetActive(false);
                systemLog.LogMessage("");
                systemLog.LogMessage("");
                systemLog.LogMessage("");
                systemLog.LogMessage("");
                systemLog.LogMessage("");
                systemLog.LogMessage("Successfully initialized.");
                systemLog.LogMessage("Waiting for items to be scanned ...");
                Debug.Log("Transitioning to puzzle1 state.");
                state = GameState.puzzle1;
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
                    beep.Play();
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
                // scanLog.text += $"\n{itemDescriptions[scannedItem]} Scanned!";
                systemLog.LogMessage("DNA Sample Scanned: " + itemDescriptions[scannedItem]);
                progressBar.SetValuePercentage((float)itemsScanned.Count/maxScans);
                state = GameState.puzzle1;
                Debug.Log("Updated progress bar, transitioning to puzzle1 state.");
                break;

            case GameState.puzzle1Win:
                // dino growl
                StartCoroutine(Puzzle1WinSequence());
                state = GameState.tutorial2;
                break;

            case GameState.puzzle1Fail:
                // dino roar
                StartCoroutine(Puzzle1FailSequence());
                state = GameState.tutorial2;
                break;

            case GameState.tutorial2:
                //state to wait in while the puzzle1 coroutines play
                // puzzle1.SetActive(false);
                Debug.Log("Entered tutorial2 state.");
                // puzzle2.SetActive(true);
                dinoPanel.SetActive(true);
                startTime = Time.time;
                Debug.Log("Starting puzzle2 timer.");
                // lights.text = " 000000";
                circuitSystem.SetSwitches("000000");
                state = GameState.puzzle2;
                Debug.Log("Transitioning to puzzle2 state.");
                break;
            
            case GameState.puzzle2:
                // transitions to puzzle2Win if lights are correct
                // everything done via handlelights
                break;

            case GameState.puzzle2Win:
                // dino growl sound effect
                StartCoroutine(Puzzle2WinSequence());
                state = GameState.puzzle2End;
                break;

            case GameState.puzzle2Fail:
                StartCoroutine(Puzzle2FailSequence());
                state = GameState.puzzle2End;
                break;

            case GameState.puzzle2End:
                // does nothing, waits while puzzle2coroutines end
                Debug.Log("Entered end state.");
                break;
        }
    }

    private IEnumerator JumpScare() {
        dino.SendSerialMessage("j\n");
        yield return 0;
    }

    private IEnumerator Roar() {
        dinoRoar.Play();
        dino.SendSerialMessage("r\n");
        yield return 0;
    }

    private IEnumerator Growl() {
        dinoGrowl.Play();
        dino.SendSerialMessage("g\n");
        yield return 0;
    }

    private IEnumerator Home() {
        dino.SendSerialMessage("h\n");
        yield return 0;
    }

    private IEnumerator Puzzle1WinSequence()
    {
        Debug.Log("Puzzle 1 Win Sequence");
        yield return StartCoroutine(JumpScare());
        yield return StartCoroutine(Growl());
        yield return new WaitForSeconds(3.5f);
        state = GameState.puzzle2;
        startTime = Time.time;
        puzzle2.SetActive(true);
        dinoPanel.SetActive(false);
        Debug.Log("Transitioning to puzzle2 state.");
    }

    private IEnumerator Puzzle1FailSequence()
    {
        Debug.Log("Puzzle 1 Fail Sequence");
        yield return StartCoroutine(JumpScare());
        yield return StartCoroutine(Roar());
        yield return new WaitForSeconds(1.2f);
        state = GameState.puzzle2;
        startTime = Time.time;
        puzzle2.SetActive(true);
        dinoPanel.SetActive(false);
        Debug.Log("Transitioning to puzzle2 state.");
    }

    private IEnumerator Puzzle2WinSequence()
    {
        Debug.Log("Puzzle 2 Win Sequence");
        alarmSound.Play();
        // puzzle2.SetActive(false);
        winPanel.SetActive(true);
        yield return StartCoroutine(Roar());
        yield return new WaitForSeconds(1.2f);
        yield return StartCoroutine(Home());
        yield return new WaitForSeconds(2f);
        startTime = 0;
        state = GameState.wait;
        Debug.Log("Transitioning to wait state.");
    }

    private IEnumerator Puzzle2FailSequence()
    {
        Debug.Log("Puzzle 2 Fail Sequence");
        // puzzle2.SetActive(false);
        losePanel.SetActive(true);
        yield return StartCoroutine(Roar());
        yield return new WaitForSeconds(1.2f);
        yield return StartCoroutine(Home());
        yield return new WaitForSeconds(2f);
        startTime = 0;
        state = GameState.wait;
        Debug.Log("Transitioning to wait state.");    }

    // this function is called by the RFID MessageListner whenever any item is scanned.
    public void HandleScannedItem(string item) {
        if (state == GameState.puzzle1) {
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
            // lights.text = lightString.Trim();
            circuitSystem.SetSwitches(lightString.Trim());
            Debug.Log($"Lights updated: {lightString.Trim()}");
            if (lightString.Trim() == "101001")
            {
                Debug.Log("Correct lights, transitioning to puzzle2Win state.");
                state = GameState.puzzle2Win;
            }
        } else {
            Debug.Log($"Lights updated, but current state is {state.ToString()}.");
            return;
        }
    }
}
