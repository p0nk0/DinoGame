using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class Meter : MonoBehaviour
{
    [Tooltip("Determines how the slider moves upon a change in value." +
             "\n\"None\": Instantly changes value." +
             "\n\"Constant Rate\": Rolls at a rate of \"Roll Value\" (as a percentage) per second." +
             "\n\"Contant Time\": Spends \"Role Value\" time rolling, regardless of value")]
    [SerializeField] protected RollType rollType;
    [Tooltip("See \"Roll Type\" for how this value is used.")]
    [SerializeField] protected float rollValue;

    protected float firstValue;
    protected float trueValue; // adjusts mySlider to reach this value overtime
    protected float rollTimer;
    protected float rollDuration;

    protected Slider mySlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        mySlider = GetComponent<Slider>();
        if(mySlider == null)
        {
            Debug.LogError("Meter: No slider found on this object.");
            //return;
        }
        trueValue = mySlider.value;
        firstValue = mySlider.value;
    }

    // Update is called once per frame
    void Update()
    {
        if (rollTimer < rollDuration)
        {
            mySlider.value = Mathf.Lerp(firstValue, trueValue, rollTimer / rollDuration);
            rollTimer += Time.deltaTime;
        }
        else
            mySlider.value = trueValue;
    }

    /***********************************************************************************
     * Visually updates the meter to value, and animates the meter if `rolling` is true.
     * (It's recommended that you use SetValuePercentage, especially if you don't know
     *  the min and max of the meter)
     ***********************************************************************************/
    private void SetValue(float val, bool rolling = false)
    {
        firstValue = mySlider.value;
        trueValue = val;

        if (rolling)
        {
            rollTimer = 0;
            switch (rollType)
            {
                case RollType.ConstantRate:
                    rollDuration = Mathf.Abs(mySlider.value - val) / (rollValue * (mySlider.maxValue - mySlider.minValue));
                    break;
                case RollType.ConstantTime:
                    rollDuration = rollValue;
                    break;
                default:
                    rollDuration = 0;
                    break;
            }
        }
        else
        {
            rollTimer = 0;
            rollDuration = 0;
            mySlider.value = trueValue;
            //Debug.Log(mySlider.value);
        }
    }

    /******************************************************
     * Visually updates the meter to be a percentage [0,1],
     * and animates the meter if `rolling` is true.
     ******************************************************/
    public void SetValuePercentage(float val, bool rolling = false)
    {
        SetValue(Mathf.LerpUnclamped(mySlider.minValue, mySlider.maxValue, val), rolling);
    }

    public void ResetValue(bool rolling = false)
    {
        SetValue(mySlider.minValue, rolling);
    }

    /************************************************************************
     * If you would like to directly edit the slider values (NOT RECOMMENDED)
     *************************************************************************/
    public Slider GetMySlider()
    {
        return mySlider;
    }

    public enum RollType
    {
        None,
        ConstantRate,
        ConstantTime
    }
}
