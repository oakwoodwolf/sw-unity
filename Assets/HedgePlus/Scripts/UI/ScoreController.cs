using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ScoreController : MonoBehaviour
{
    public static ScoreController _score;
    public TextMeshProUGUI ScoreMain, ScoreShadow, ComboCounter;
    public Image ScoreTimer;
    public int Score;
    int Multiplier = 0;
    public int MaxMultiplier = 10;
    int ComboCount;
    public float ComboTimeout = 5f;
    public float ComboTimer { get; set; }

    string ExtendedScore (int value)
    {
        return string.Format("{00:0000000}", value);
    }

    private void Awake()
    {
        _score = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ScoreMain != null && ScoreShadow != null)
        {
            ScoreMain.text = Score.ToString();
            ScoreShadow.text = ExtendedScore(Score);
        }
        if (ComboCount > 0)
        {
            ComboCounter.text = ComboCount.ToString();
            ComboTimer -= Time.deltaTime;
            if (ComboTimer <= 0)
            {
                ComboCount = 0;
            }

            ScoreTimer.fillAmount = ComboTimer / ComboTimeout;
        }

        ComboCounter.transform.parent.gameObject.SetActive(ComboCount > 1);
    }

    public void AddScore (int InitialAmount)
    {
        ComboCount++;
        if (ComboCount < MaxMultiplier)
        {
            Multiplier = ComboCount;
        }
        Debug.Log("Added " + InitialAmount * Multiplier + " to score");
        Score += InitialAmount * Multiplier;
        ComboTimer = ComboTimeout;
    }
}
