using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class AxisMapper : MonoBehaviour
{
    InputHandler _input;
    [HideInInspector] public ControlMapper mapper;
    public int EditIndex;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI KeyboardPositive;
    public TextMeshProUGUI KeyboardNegative;
    public TextMeshProUGUI GamepadName;

    private void Start()
    {
        _input = InputHandler.instance;
    }


}
