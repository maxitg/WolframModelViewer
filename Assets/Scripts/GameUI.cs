using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class GameUI : MonoBehaviour
{

    private static GameController gameControl;
    private static GameInputReader gameInputReader;
    private static GraphController graphControl;

    // Sand clock 
    private GameObject sandClock;
    private Animator sandClockAnimator;

    internal void SandClockSetActive(bool state)
    {
        sandClock.SetActive(state);
    }

    // Use this for initialization
    void Awake()
    {
        gameControl = GetComponent<GameController>();
        gameInputReader = GetComponent<GameInputReader>();
        graphControl = GetComponent<GraphController>();

        // Sand clock
        sandClock = GameObject.Find("Sand Clock");
        sandClockAnimator = sandClock.GetComponent<Animator>();
        sandClockAnimator.Play("In");
    }

}
