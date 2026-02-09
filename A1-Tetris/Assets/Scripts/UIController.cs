using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TetrisManager tetrisManager;
    public TextMeshProUGUI scoreText;

    public GameObject endGamePanel;

    public void UIUpdateScore()
    {
        scoreText.text = $"SCORE: {tetrisManager.score:n0}";
    }

    public void UpdateGameOver()
    {
        // When the game over Event is broadcast, the End Game Panel will show when the game is over.
        // It will hide when the game resets.
        endGamePanel.SetActive(tetrisManager.gameOver);

        // Stops the music when the game over screen comes up so doesn't just keep playing.
        // Also so that it doesn't just have continuous music between restarts (which can get annoying). 
        tetrisManager.StopMusic();
    }

    public void PlayAgain()
    {
        // Setting the game over to false resets the game.
        tetrisManager.SetGameOver(false);
    }
}
