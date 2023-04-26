using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    [SerializeField]
    private int score = 0;

    [SerializeField]
    public Text scoreText;

    [SerializeField]
    public Text timeText;

    private bool hasUnlockedGun = false;
    private float gameTimer = 0.0f;
    private string timeString;

    private void Update()
    {
        UpdateTimer();
        updateText();
        passedMilestone();
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

    void passedMilestone() {
        if (!hasUnlockedGun && score >= 1500) {
            hasUnlockedGun = true;
        }
    }

    //Getters / Setters
    public int Score { get => score; set => score = value; }
    public bool HasUnlockedGun { get => hasUnlockedGun; set => hasUnlockedGun = value; }
}
