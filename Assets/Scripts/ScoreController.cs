using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    [SerializeField]
    public int score = 0;

    [SerializeField]
    public Text scoreText;

    [SerializeField]
    public Text timeText;

    private float gameTimer = 0.0f;
    private string timeString;

    private void Update()
    {
        UpdateTimer();
        updateText();
    }

    void UpdateTimer()
    {
        gameTimer += Time.deltaTime;
        // Format the game time into minutes and seconds
        TimeSpan time = TimeSpan.FromSeconds(gameTimer);
        timeString = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
    }


    void updateText()
    {
        scoreText.text = "Score: " + score;
        timeText.text = "Time: " + timeString;
    }
}
