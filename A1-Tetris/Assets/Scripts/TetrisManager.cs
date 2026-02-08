using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TetrisManager : MonoBehaviour
{
    // Why is this public? Other pieces of game code may need to access it. 
    public int score { get; private set; }

    public bool gameOver { get; private set; }

    public UnityEvent OnScoreChanged;
    public UnityEvent OnGameOver;

    public float currentTime;
    public TextMeshProUGUI timerText;

    private void Start()
    {
        SetGameOver(false);
    }

    public int CalculateScore(int linesCleared)
    {
        switch (linesCleared)
        {
            case 1: return 100;
            case 2: return 300;
            case 3: return 500;
            case 4: return 800;
            default: return 0;
        }    
    }

    public void ChangeScore(int amount)
    {
        score += amount;
        OnScoreChanged.Invoke();
    }

    public void SetGameOver(bool _gameOver)
    {
       if (!_gameOver)
       {
            // When the gameOver event is FALSE, reset the score.
            score = 0;
            ChangeScore(0);
       }
        
        gameOver = _gameOver;

        OnGameOver.Invoke();
    }

    // Updates the text on the timer to show how much time is left.
    public void UIUpdateTimer()
    {
        timerText.text = $"{currentTime:n0}";
    }

    public void GameTimer()
    {
        // Ticks down the current time
        currentTime -= Time.deltaTime; 
            
        // If the current time remaining hits 0, game over!
        if (currentTime <= 0)
        {
            SetGameOver(true);
        }
    }

}
