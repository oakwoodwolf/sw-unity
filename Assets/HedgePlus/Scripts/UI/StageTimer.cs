using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StageTimer : MonoBehaviour
{
    public TextMeshProUGUI timerDisplay;
    public bool UpdateTimer = true;
    float Minutes = 0;
    float Seconds = 0;
    float Centi = 0;

    string FormatForTimer (int a, int b, int c)
    {
        return string.Format("{00:00}:{01:00}:{02:00}", a, b, c);
    }
    void Update()
    {
        if (UpdateTimer)
        {
            Centi += Time.deltaTime * 100f;
            if (Centi >= 100)
            {
                Seconds++;
                Centi = 0;
            }
            if (Seconds >= 60)
            {
                Minutes++;
                Seconds = 0;
            }
        }

        timerDisplay.text = FormatForTimer((int)Minutes, (int)Seconds, (int)Centi);
    }
}
