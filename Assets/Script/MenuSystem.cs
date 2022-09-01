using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSystem: MonoBehaviour
{
    public Text levelText;
    public Text highscore1;
    public Text highscore2;
    public Text highscore3;
    public Text highscore4;

    public Text lastScore;

    void Start()
    {
        if (levelText != null)
            levelText.text = "0";
       
        if(highscore1 != null)
            highscore1.text = PlayerPrefs.GetInt("highscore1").ToString();

        if(highscore2 != null)
            highscore2.text = PlayerPrefs.GetInt("highscore2").ToString();

        if(highscore3 != null)
            highscore3.text = PlayerPrefs.GetInt("highscore3").ToString();

        if(highscore4 != null)
            highscore4.text = PlayerPrefs.GetInt("highscore4").ToString();

        if(lastScore != null)   
            lastScore.text = PlayerPrefs.GetInt("lastScore").ToString();
    }

    /// <summary>
    /// Open leve scene
    /// </summary>
    public void PlayAgain()
    {
        SceneManager.LoadScene("GameMenu");
    }

    public void PlayGame()
    {
        
        if (Board.startingAtLevel == 0)
        {
            Board.startingAtLevelZero = true;

        }
        else
        {
            Board.startingAtLevelZero = false;
        }
        SceneManager.LoadScene("Level");
    }

    /// <summary>
    /// Change level value
    /// </summary>
    /// <param name="value"></param>
    public void ChangeValue(float value)
    {
        Board.startingAtLevel = (int)value;
        levelText.text = value.ToString();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
