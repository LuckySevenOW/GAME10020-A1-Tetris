using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TetrisManager tetrisManager;
    public TextMeshProUGUI scoreText;

    public void UpdateScore()
    {
        scoreText.text = $"SCORE: {tetrisManager.score}";
    }    
}
