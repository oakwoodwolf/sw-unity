using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ButtonInput
{
    public string Name;
    public string KeyboardInput;
    public string GamepadInput;
}
[System.Serializable]
public class AxisInput
{
    public string Name;
    public string Positive;
    public string Negative;
    public string GamepadAxis;
    [HideInInspector] public float Value = 0;
    [HideInInspector] public float TargetValue = 0;
    public float Damping = 0.1f;
    float Velocity;
    public void UpdateAxisValue()
    {
        Value = Mathf.SmoothDamp(Value, TargetValue, ref Velocity, Damping);
        Value = HedgeMath.ClampFloat(Value, -1, 1);
    }
}
public class InputHandler : MonoBehaviour
{
    public static InputHandler instance;
    public bool UseGamepad;
    public ButtonInput[] buttonInputs;
    public AxisInput[] axisInputs;
    public enum ButtonState { Get, Down, Up}
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        for (int i = 0; i < axisInputs.Length; i++)
        {
            axisInputs[i].UpdateAxisValue();
        }
    }
    /// <summary>
    /// Returns a button input given the desired state of the button
    /// </summary>
    /// <param name="Name">Name of the button we want to get</param>
    /// <param name="Type">Button state to return</param>
    /// <returns></returns>
    public bool GetButton(string Name, ButtonState Type)
    {
        bool tmp = false;
        if (!PauseMenuControl.Paused)
        {
            if (buttonInputs.Length == 0)
            {
                //This will give us an error and return false if there are no button inputs.
                Debug.LogError("InputHandler: No Input buttons have been added.");
            }
            else
            {
                ButtonInput _button = new ButtonInput();
                //Use a for loop to cycle through available inputs to find the one we want.
                for (int i = 0; i < buttonInputs.Length; i++)
                {
                    if (buttonInputs[i].Name == Name)
                    {
                        _button = buttonInputs[i];
                    }
                }
                //Now that we have our button, we just use the ButtonState to return a certain input type of the key we want.
                switch (Type)
                {
                    case ButtonState.Get:
                        tmp = Input.GetKey(_button.GamepadInput) || Input.GetKey(_button.KeyboardInput);
                        break;
                    case ButtonState.Down:
                        tmp = Input.GetKeyDown(_button.GamepadInput) || Input.GetKeyDown(_button.KeyboardInput);
                        break;
                    case ButtonState.Up:
                        tmp = Input.GetKeyUp(_button.GamepadInput) || Input.GetKeyUp(_button.KeyboardInput);
                        break;
                }
            }
        }
        return tmp;
    }
    /// <summary>
    /// Returns a button input given the desired state of the button
    /// </summary>
    /// <param name="Index">Array index of the desired button</param>
    /// <param name="Type">Button state to return</param>
    /// <returns></returns>
    public bool GetButton(int Index, ButtonState Type)
    {
        bool tmp = false;
        if (!PauseMenuControl.Paused)
        {
            if (buttonInputs.Length == 0)
            {
                //This will give us an error and return false if there are no button inputs.
                Debug.LogError("InputHandler: No Input buttons have been added.");
            }
            else
            {
                //Use a for loop to cycle through available inputs to find the one we want.
                ButtonInput _button = buttonInputs[Index];
                //Now that we have our button, we will use a foreach loop to return every key assigned to that input.
                switch (Type)
                {
                    case ButtonState.Get:
                        tmp = Input.GetKey(_button.GamepadInput) || Input.GetKey(_button.KeyboardInput);
                        break;
                    case ButtonState.Down:
                        tmp = Input.GetKeyDown(_button.GamepadInput) || Input.GetKeyDown(_button.KeyboardInput);
                        break;
                    case ButtonState.Up:
                        tmp = Input.GetKeyUp(_button.GamepadInput) || Input.GetKeyUp(_button.KeyboardInput);
                        break;
                }
            }
        }
        return tmp;
    }
    /// <summary>
    /// Returns an axis value.
    /// </summary>
    /// <param name="name">Name of the axis we want to return</param>
    /// <returns></returns>
    public float GetAxis (string name)
    {
        float tmp = 0;
        AxisInput _axis = new AxisInput();
        if (!PauseMenuControl.Paused)
        {
            //Like with button inputs, we will first cycle through our available axis inputs.
            for (int a = 0; a < axisInputs.Length; a++)
            {
                if (axisInputs[a].Name == name)
                {
                    _axis = axisInputs[a];
                }
            }
            float TargetInput = _axis.Value;
            ///For keyboard input axes, we set a target value and add or subtract it depending on if positive/negative inputs are held or released.
            if (Input.GetKeyDown(_axis.Positive) || Input.GetKeyUp(_axis.Negative))
            {
                _axis.TargetValue += 1;
            }
            if (Input.GetKeyUp(_axis.Positive) || Input.GetKeyDown(_axis.Negative))
            {
                _axis.TargetValue -= 1;
            }
            if (!Input.GetKey(_axis.Positive) && !Input.GetKey(_axis.Negative))
            {
                _axis.TargetValue = 0;
            }
            tmp = _axis.Value + Input.GetAxisRaw(_axis.GamepadAxis);
            tmp = HedgeMath.ClampFloat(tmp, -1, 1);
        }

        return tmp;
    }
    /// <summary>
    /// Returns an axis value;
    /// </summary>
    /// <param name="Index">Array index of the axis we want to return</param>
    /// <returns></returns>
    public float GetAxis(int Index)
    {
        float tmp = 0;
        AxisInput _axis = axisInputs[Index];
        if (!PauseMenuControl.Paused)
        {
            float TargetInput = _axis.Value;
                ///For keyboard input axes, we set a target value and add or subtract it depending on if positive/negative inputs are held or released.
            if (Input.GetKeyDown(_axis.Positive) || Input.GetKeyUp(_axis.Negative))
            {
                _axis.TargetValue += 1;
            }
            if (Input.GetKeyUp(_axis.Positive) || Input.GetKeyDown(_axis.Negative))
            {
                _axis.TargetValue -= 1;
            }
            if (!Input.GetKey(_axis.Positive) && !Input.GetKey(_axis.Negative))
            {
                _axis.TargetValue = 0;
            }
            tmp = _axis.Value + Input.GetAxisRaw(_axis.GamepadAxis);
            tmp = HedgeMath.ClampFloat(tmp, -1, 1);
        }

        return tmp;
    }
}
