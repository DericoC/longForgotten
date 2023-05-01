using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{

    [SerializeField] private int score = 0;
    [SerializeField] public Text scoreText;
    [SerializeField] public Text timeText;
    [SerializeField] public Text roundText;

    private bool hasUnlockedGun;
    private bool hasUnlockedRifle;
    private float gameTimer = 0.0f;
    private string timeString;

    private void Start()
    {
        hasUnlockedGun = false;
        hasUnlockedRifle = false;
    }

    private void Update()
    {
        updateTimer();
        updateText();
        passedMilestone();
    }

    void updateTimer()
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
        roundText.text = "Round: " + SpawnerController.currentRound;
    }

    void passedMilestone() {
        if (!hasUnlockedGun && SpawnerController.currentRound == 3) {
            hasUnlockedGun = true;
        }

        if (!hasUnlockedRifle && SpawnerController.currentRound == 6)
        {
            hasUnlockedRifle = true;
        }
    }

    public void resetScoreController() {
        hasUnlockedGun = false;
        hasUnlockedRifle = false;
        score = 0;
        gameTimer = 0.0f;
        SpawnerController.currentRound = 1;
        SpawnerController.zombies = 0;
    }

    //Getters / Setters
    public int Score { get => score; set => score = value; }
    public bool HasUnlockedGun { get => hasUnlockedGun; set => hasUnlockedGun = value; }
    public bool HasUnlockedRifle { get => hasUnlockedRifle; set => hasUnlockedRifle = value; }
}
