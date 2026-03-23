using Unity.Cinemachine;
using UnityEngine;

// Represents a customer
// responsible for:
// giving feedback reaction after getting served
// managing the waiting timer of the customer (not sure)
// if time is up, the customer leaves (state to idle) (giving a bad reaction and decrease in score)

// create state machine with states:
// idle -> run a timer, after which set state to ready
// ready -> run a timer for customer waiting, timeout -> set state to idle (bad reaction and decrease in score)
// in-service -> currently getting served by player, give feedback reaction and set state to idle
public class Customer : MonoBehaviour
{
    private CinemachineStateDrivenCamera _stateCamera;

    private void Start()
    {
        _stateCamera = GetComponentInChildren<CinemachineStateDrivenCamera>();

        if (_stateCamera == null)
        {
            Debug.LogError("No state camera found in customer!");
            enabled = false;
            return;
        }

        // _stateCamera.Instructions = new CinemachineStateDrivenCamera.Instruction[]
        // {
        //     new() {
        //         Camera = "Idle",
        //         m_Weight = 1f
        //     }
        // };
    }

    /// <summary>
    /// Turns on or off the camera
    /// </summary>
    /// <param name="on"></param>
    public void TurnCamera(bool on = true)
    {
        _stateCamera.enabled = on;
    }
}