using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    float fall = 0;
    private float fallSpeed;
   

    public bool allowRotation = true;
    public bool limitRotation = false;

    public int bonusScore = 0;                   //- Scores are added when players play for a long time    
    public float individualScoreTime;           //- Play time to add points

    public AudioClip moveSound;                 //- sound for where tetromino moved
    public AudioClip rotateSound;                //- sound for where tetromino rotate
    public AudioClip landSound;                 //sound for where tetromino lands
    private AudioSource soundSource;            //- variable for source of audio clip

    private float continuosVerticalSpeed = 0.05f;   //- The speed at which the tetromino will move when the down arrow button is held down
    private float continousHorizontalSpeed = 0.1f; //- The speed at which the tetromino will move when the left or the right arrow button is held down
    private float buttonDownWaitMax = 0.2f;     //- How long to wait before the tetromino recognizes that a button is being held down

    private float verticalTimer = 0;    //- Time for tetromino move left, right
    private float horizontalTimer = 0;  //- Time for tetromino falls.
    private float buttonDownWaitTimeHorizontal = 0;
    private float buttonDownWaitTimeVertical = 0;

    private bool moveImediateHorizontal = false;
    private bool moveImediateVertical = false;

    // Start is called before the first frame update
    void Start()
    {
       
        soundSource = GetComponent<AudioSource>();
        
    }

    /// <summary>
    /// Change fall speed of tetromino
    /// </summary>
    void UpdateFallSpeed()
    {
        fallSpeed = Board.fallSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Board.isPause)
        {
            checkUserInput();
            UpdateIndividualScore();
            UpdateFallSpeed();
        }
       
    }

    /// <summary>
    /// Method to add bonus score
    /// </summary>
    public void UpdateIndividualScore()
    {
        //- If the playing time is less than 1 then we need more playing time
        if (individualScoreTime < 10)
        {
            individualScoreTime += Time.deltaTime;
        }
        else
        {
            individualScoreTime = 0;
            //- If the time is greater than 1 then the bonus will be limited from 0 to 90, 
            //- Use "Mathf.Max" to avoid the case of negative plus points
            bonusScore = Mathf.Min(100, bonusScore + 10);
        }
       
    }

    /// <summary>
    /// Checks the user input
    /// </summary>
    void checkUserInput()
    {
        if(Input.GetKeyUp(KeyCode.LeftArrow)|| Input.GetKeyUp(KeyCode.RightArrow))
        {
            moveImediateHorizontal = false;            
            horizontalTimer = 0;            
            buttonDownWaitTimeHorizontal = 0;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            moveImediateVertical = false;
            verticalTimer = 0;
            buttonDownWaitTimeVertical = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveRight();
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveLeft();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
                   
        }
        if (Input.GetKey(KeyCode.DownArrow)|| Time.time - fall >= fallSpeed)
        {
            moveDown();
        }

    }

    /// <summary>
    /// Moves the left
    /// </summary>
    void moveLeft()
    {
        if (moveImediateHorizontal)
        {
            if (buttonDownWaitTimeHorizontal < buttonDownWaitMax)
            {
                buttonDownWaitTimeHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }
        if (!moveImediateHorizontal)
        {
            moveImediateHorizontal = true;
        }

        horizontalTimer = 0;

        transform.position += new Vector3(-1, 0, 0);
        if (CheckIsValiPosition())
        {
            FindObjectOfType<Board>().UpdateGrid(this);
            PlayMoveSound();
        }
        else
        {
            transform.position += new Vector3(1, 0, 0);
        }
    }

    /// <summary>
    /// Moves the right
    /// </summary>
    void moveRight()
    {
        if (moveImediateHorizontal)
        {
            if (buttonDownWaitTimeHorizontal < buttonDownWaitMax)
            {
                buttonDownWaitTimeHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }
        if (!moveImediateHorizontal)
        {
            moveImediateHorizontal = true;
        }

        horizontalTimer = 0;

        transform.position += new Vector3(1, 0, 0);
        if (CheckIsValiPosition())
        {
            FindObjectOfType<Board>().UpdateGrid(this);
            PlayMoveSound();
        }
        else
        {
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    /// <summary>
    /// Moves Down
    /// </summary>
    void moveDown()
    {
        if (moveImediateVertical)
        {
            if (buttonDownWaitTimeVertical < buttonDownWaitMax)
            {
                buttonDownWaitTimeVertical += Time.deltaTime;
                return;
            }

            if (verticalTimer < continuosVerticalSpeed)
            {
                verticalTimer += Time.deltaTime;
                return;
            }
        }
        if (!moveImediateVertical)
            moveImediateVertical = true;

        verticalTimer = 0;


        transform.position += new Vector3(0, -1, 0);
        if (CheckIsValiPosition())
        {
            FindObjectOfType<Board>().UpdateGrid(this);
            if (Input.GetKey(KeyCode.DownArrow))
            {
                PlayMoveSound();
            }
        }
        else
        {
            transform.position += new Vector3(0, 1, 0);
            FindObjectOfType<Board>().DeleteRowFull();
            if (FindObjectOfType<Board>().CheckIsAboveGrid(this))
            {
                FindObjectOfType<Board>().GameOver();
            }
            PlayLandSound();
            //- spawn next tetromino 
            FindObjectOfType<Board>().SpawnNextTetromino();
            Board.currentScore += bonusScore;
            enabled = false;
        }
        fall = Time.time;
    }

    /// <summary>
    /// Rotate 
    /// </summary>
    void Rotate()
    {
        if (allowRotation)
        {
            if (limitRotation)
            {
                if (transform.rotation.eulerAngles.z >= 90)
                {
                    transform.Rotate(0, 0, -90);
                }
                else
                {
                    transform.Rotate(0, 0, 90);
                }
            }
            else
            {
                transform.Rotate(0, 0, 90);
            }
            if (CheckIsValiPosition())
            {
                FindObjectOfType<Board>().UpdateGrid(this);
                PlayRotateSound();
            }
            else
            {
                if (limitRotation)
                {
                    if (transform.rotation.eulerAngles.z >= 90)
                    {
                        transform.Rotate(0, 0, -90);
                    }
                    else
                    {
                        transform.Rotate(0, 0, 90);
                    }
                }
                else
                {
                    transform.Rotate(0, 0, -90);
                }
            }

        }
    }

    /// <summary>
    /// Check position of tetromino 
    /// </summary>
    /// <returns></returns>
    bool CheckIsValiPosition()
    {
        foreach(Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<Board>().Round(mino.position);
            if(FindObjectOfType<Board>().CheckIsInsideGrid(pos) == false)
            {
                return false;
            }
            if(FindObjectOfType<Board>().GetTransformAtGridPosition(pos) != null 
                && FindObjectOfType<Board>().GetTransformAtGridPosition(pos).parent != transform)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// play audio clip when tetromino is move left, right, down.
    /// </summary>
    void PlayMoveSound()
    {
        soundSource.PlayOneShot(moveSound);
    }

    /// <summary>
    /// play audio clip when tetromino is rotated.
    /// </summary>
    void PlayRotateSound()
    {
        soundSource.PlayOneShot(rotateSound);
    }

    /// <summary>
    /// play audio when tetromino lands.
    /// </summary>
    void PlayLandSound()
    {
        soundSource.PlayOneShot(landSound);
    }
}
