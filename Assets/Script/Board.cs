using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public static int gridWidth = 10;
    public static int gridHeight = 20;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public int scoreOneLine = 50;
    public int scoreTwoLine = 200;
    public int scoreThreeLine = 500;
    public int scoreFourLine = 1000;
    public int startHighScore1;
    public int startHighScore2;
    public int startHighScore3;
    public int startHighScore4;

    public Text TextScore;                  //- Reference to text score in canvas
    public Text TextLevel;
    public Text TextLines;

    public int currentLevel = 0;
    public int numberLinesCleared = 0;
    public static bool startingAtLevelZero;
    public static int startingAtLevel;
        
    private int countLineDeleted = 0;       //- Count the number of consecutive filled lines 

    public static int currentScore = 0;

    public AudioClip cleanLineSound;        //-sound for when line is deleted
   
    private AudioSource soundSource;

    private GameObject previewTetromino;    
    private GameObject nextTetromino;
    private bool gameStarted = false;
    private Vector2 previewTetrominoPosition = new Vector2(20.5f, 3.5f);

    public static float fallSpeed = 1.0f;
    public static bool isPause = false;

    public Canvas hub_canvas;
    public Canvas pause_canvas;
    
    // Start is called before the first frame update
    void Start()
    {
           
        currentScore = 0;
        TextScore.text = "0";
        SpawnNextTetromino();
        soundSource = GetComponent<AudioSource>();
        currentLevel = startingAtLevel;
        TextLevel.text = currentScore.ToString();
        TextLines.text = "0";
        startHighScore1 = PlayerPrefs.GetInt("highscore1");
        startHighScore2 = PlayerPrefs.GetInt("highscore2");
        startHighScore3 = PlayerPrefs.GetInt("highscore3");
        startHighScore4 = PlayerPrefs.GetInt("highscore4");
        
    }
        
    // Update is called once per frame
    void Update()
    {
            UpdateScore();
            UpdateUI();
            UpdateLevel();
            UpdateSpeed();
            checkUserInput();
        
    }

    /// <summary>
    /// Check User input when they get P key code for pause game
    /// </summary>
    void checkUserInput()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            if (Time.timeScale == 1)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    /// <summary>
    /// Update grid to check the entire grid to determine the position of the blocks
    /// </summary>
    /// <param name="tetromino"></param>
    public void UpdateGrid(Tetromino tetromino)
    {
        for(int y = 0; y <gridHeight; ++y)
        {
            for(int x = 0; x < gridWidth; ++x)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == tetromino.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }
        foreach(Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);
            if(pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    /// <summary>
    /// Update Level of user
    /// </summary>
    void UpdateLevel()
    {
        if(startingAtLevelZero==true || (startingAtLevelZero==false && numberLinesCleared/10 > startingAtLevel))
        {
            currentLevel = numberLinesCleared / 10;
        }
           
    }

    /// <summary>
    /// Update speed follow level of player
    /// </summary>
    void UpdateSpeed()
    {
        fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
    }

    /// <summary>
    /// pause game when get P button
    /// </summary>
    void PauseGame()
    {
        Time.timeScale = 0;
        isPause = true;
        soundSource.Pause();
        hub_canvas.enabled = false;
        pause_canvas.enabled = true;
    }

    /// <summary>
    /// Réume game when user get P button again
    /// </summary>
    void ResumeGame()
    {
        Time.timeScale = 1;
        isPause = false;
        soundSource.Play();
        hub_canvas.enabled = true;
        pause_canvas.enabled = false;
    }

    /// <summary>
    /// Determine the position of the blocks in grid
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Transform GetTransformAtGridPosition(Vector2 pos)
    {
        if(pos.y > gridHeight - 1)
        {
            return null;
        }
        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

    /// <summary>
    /// play audio when line is deleted
    /// </summary>
    public void PlayLineCleanSound()
    {
        soundSource.PlayOneShot(cleanLineSound);
    }

    /// <summary>
    /// Instantiate next tetromino
    /// </summary>
    public void SpawnNextTetromino()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(4.0f, 20.0f), Quaternion.identity);
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            previewTetromino.GetComponent<Tetromino>().enabled = false;
        }
        else
        {
            previewTetromino.transform.localPosition = new Vector2(4.0f, 20.0f);
            nextTetromino = previewTetromino;
            nextTetromino.GetComponent<Tetromino>().enabled = true;
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            previewTetromino.GetComponent<Tetromino>().enabled = false;
        }
         
    }

    /// <summary>
    /// Check if the block is over the grid
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool CheckIsInsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    }

    /// <summary>
    /// Round positon of tetromino
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector2 Round (Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    /// <summary>
    /// Get a random tetromino
    /// </summary>
    /// <returns></returns>
    string GetRandomTetromino()
    {
        int randomTetromino = Random.Range(1, 8);
        string randomTetrominoName = "Prefabs/Block_L";
        switch (randomTetromino)
        {
            case 1:
                randomTetrominoName = "Prefabs/Block_I";
                break;
            case 2:
                randomTetrominoName = "Prefabs/Block_J";
                break;
            case 3:
                randomTetrominoName = "Prefabs/Block_L";
                break;
            case 4:
                randomTetrominoName = "Prefabs/Block_O";
                break;
            case 5:
                randomTetrominoName = "Prefabs/Block_S";
                break;
            case 6:
                randomTetrominoName = "Prefabs/Block_T";
                break;
            case 7:
                randomTetrominoName = "Prefabs/Block_Z";
                break;
        }
        return randomTetrominoName;
    }

    /// <summary>
    /// Update score of player when consecutive lines deleted
    /// </summary>
    public void UpdateScore()
    {
        if(countLineDeleted > 0)
        {
            if(countLineDeleted == 1)
            {
                ClearedOneLine();
            }
            else if (countLineDeleted == 2)
            {
                ClearedTwoLine();
            }
            else if(countLineDeleted == 3)
            {
                ClearedThreeLine();
            }
            else if(countLineDeleted == 4)
            {
                ClearedFourLine();
            }
            countLineDeleted = 0;

            PlayLineCleanSound();
        }
        
    }

    /// <summary>
    /// Upade four high score in table when game over
    /// </summary>
    public void UpdateHighScore()
    {
        if(currentScore > startHighScore1)
        {
            PlayerPrefs.SetInt("highscore4", startHighScore3);
            PlayerPrefs.SetInt("highscore3", startHighScore2);
            PlayerPrefs.SetInt("highscore2", startHighScore1);
            PlayerPrefs.SetInt("highscore1", currentScore);
        }else if(currentScore > startHighScore2)
        {
            PlayerPrefs.SetInt("highscore4", startHighScore3);
            PlayerPrefs.SetInt("highscore3", startHighScore2);
            PlayerPrefs.SetInt("highscore2", currentScore);
        }else if(currentScore > startHighScore3)
        {
            PlayerPrefs.SetInt("highscore4", startHighScore3);
            PlayerPrefs.SetInt("highscore3", currentScore);
        }else if (currentScore > startHighScore4)
        {
            PlayerPrefs.SetInt("highscore4", currentScore);
        }
        PlayerPrefs.SetInt("lastScore", currentScore);
    }

    /// <summary>
    /// Change value in canvas
    /// </summary>
    public void UpdateUI()
    {
        TextScore.text = currentScore.ToString();
        TextLevel.text = currentLevel.ToString();
        TextLines.text = numberLinesCleared.ToString();
    }

    /// <summary>
    /// Change score when a line deleted
    /// </summary>
    void ClearedOneLine()
    {
        currentScore += scoreOneLine + (currentLevel*10);
        numberLinesCleared++;
    }

    /// <summary>
    /// Change score when two line deleted
    /// </summary>
    void ClearedTwoLine()
    {
        currentScore += scoreTwoLine * (currentLevel * 20);
        numberLinesCleared += 2;
    }

    /// <summary>
    /// Change score when three line deleted
    /// </summary>
    void ClearedThreeLine()
    {
        currentScore += scoreThreeLine + (currentLevel * 30);
        numberLinesCleared += 3;
    }

    /// <summary>
    /// Change score when four line deleted
    /// </summary>
    void ClearedFourLine()
    {
        currentScore += scoreFourLine + (currentLevel * 40);
        numberLinesCleared += 4;
    }

    /// <summary>
    /// Check line if it full
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool isFullRowAt(int y) 
    {
        for(int x = 0; x < gridWidth; x++)
        {
            if(grid[x, y] == null)
                return false;
        }
        //Neu tim thay 1 hang da day, ta tang bien dem
        countLineDeleted++;

        return true;
    }
    
    /// <summary>
    /// Clean full line 
    /// </summary>
    /// <param name="y"></param>
    public void deleteRowAt(int y)
    {
        for(int x = 0; x <gridWidth; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    /// <summary>
    /// Move line down under 1 row
    /// </summary>
    /// <param name="y"></param>
    public void MoveRowDown(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x, y] != null)
            {
                grid[x, y-1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    /// <summary>
    /// Move all down  1 row
    /// </summary>
    /// <param name="y"></param>
    public void MoveAllRowDown(int y)
    {
        for(int i = y; i <gridHeight; i++)
        {
            MoveRowDown(i);
        }
    }

    
    public void DeleteRowFull()
    {
        for(int y = 0; y < gridHeight; y++)
        {
            if (isFullRowAt(y))
            {
                deleteRowAt(y);
                MoveAllRowDown(y + 1);
                --y;
            }
        }
    }

    /// <summary>
    /// Check if the grid is full
    /// </summary>
    /// <param name="tetromino"></param>
    /// <returns></returns>
    public bool CheckIsAboveGrid(Tetromino tetromino)
    {
        for(int x = 0; x < gridWidth; x++)
        {
            foreach(Transform mino in tetromino.transform)
            {
                Vector2 pos = Round(mino.position);
                if(pos.y > gridHeight - 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Load scene GameOver
    /// </summary>
    public void GameOver()
    {
        UpdateHighScore();
        SceneManager.LoadScene("GameOver");
    }

    /// <summary>
    /// Load scene GameMenu
    /// </summary>
    public void BackHome()
    {        
        SceneManager.LoadScene("GameMenu");
    }

    /// <summary>
    /// Quit game 
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
