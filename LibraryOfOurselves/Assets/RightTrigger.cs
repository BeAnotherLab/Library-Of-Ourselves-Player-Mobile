using UnityEngine;
using UnityEngine.InputSystem;

public class RightTrigger : MonoBehaviour
{
    public InputActionProperty trigger;

    void Update()
    {
        float value = trigger.action.ReadValue<float>();

        if (value > 0.8f)
        {
            Debug.Log("Trigger pressed");
        }
    }
}