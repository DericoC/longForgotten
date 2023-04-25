using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public int score = 0;

    private float gameTimer = 0.0f;

    private void Update()
    {
        gameTimer += Time.deltaTime;
    }

}
