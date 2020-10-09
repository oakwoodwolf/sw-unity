using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDebug : MonoBehaviour
{
    InputHandler _input;
    [SerializeField] float Scale = 25f;
    Vector2 input;
    public RectTransform Handle;

    private void Start()
    {
        _input = InputHandler.instance;
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(_input.GetAxis("Horizontal"), _input.GetAxis("Vertical"));
        input = Vector2.ClampMagnitude(input, 1f);
        Handle.anchoredPosition = input * Scale;
    }
}
