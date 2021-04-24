using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ControlMapper : MonoBehaviour
{
    InputHandler _input;
    public AxisMapper AxisInputSetting;
    public ButtonMapper ButtonInputSetting;
    public float VerticalPadding;
    float ButtonStartOffset;
    private void Start()
    {
        _input = InputHandler.instance;
        SetAxisUI();
        SetButtonUI();
        AxisInputSetting.gameObject.SetActive(false);
        ButtonInputSetting.gameObject.SetActive(false);
    }

    public void SetAxisUI()
    {
        for (int i = 0; i < _input.axisInputs.Length; i++)
        {
            RectTransform InitialTrans = AxisInputSetting.GetComponent<RectTransform>();
            float Padding = InitialTrans.rect.height + VerticalPadding;
            AxisMapper _axis = Instantiate(AxisInputSetting);
            _axis.transform.SetParent(transform, false);
            _axis.Title.text = _input.axisInputs[i].Name;
            _axis.KeyboardPositive.text = _input.axisInputs[i].Positive;
            _axis.KeyboardNegative.text = _input.axisInputs[i].Negative;
            _axis.GamepadName.text = _input.axisInputs[i].GamepadAxis;
            _axis.EditIndex = i;
            _axis.mapper = this;
            RectTransform AxisTrans = _axis.GetComponent<RectTransform>();
            AxisTrans.anchoredPosition = InitialTrans.anchoredPosition - new Vector2(0, Padding * i);
            if (i + 1 >= _input.axisInputs.Length)
            {
                ButtonStartOffset = Padding * (i + 1);
            }
        }
    }

    public void SetButtonUI()
    {
        for (int i = 0; i < _input.buttonInputs.Length; i++)
        {
            RectTransform InitialRect = ButtonInputSetting.GetComponent<RectTransform>();
            float Padding = InitialRect.rect.height + VerticalPadding;
            ButtonMapper _button = Instantiate(ButtonInputSetting);
            _button.transform.SetParent(transform, false);
            _button.Title.text = _input.buttonInputs[i].Name;
            _button.KeyboardName.text = _input.buttonInputs[i].KeyboardInput;
            _button.GamepadName.text = _input.buttonInputs[i].GamepadInput;

            RectTransform ButtonRect = _button.GetComponent<RectTransform>();
            ButtonRect.anchoredPosition = InitialRect.anchoredPosition - new Vector2(0, Padding * i + ButtonStartOffset);
        }
    }
}
