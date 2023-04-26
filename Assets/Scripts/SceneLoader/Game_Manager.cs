using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour
{
    public static Game_Manager instance = null;

    public GameObject youWinText;



    public float resetDelay;

    private void Start()
    {
        youWinText.SetActive(false);
    }
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else if(instance != null)
        {
            Destroy(gameObject);
        }
    }

    public void Win()
    {
        //Display a win message
        youWinText.SetActive(true);
        //slow down time for dramatic effect
        Time.timeScale = 0.0000001f;
        //reset the game
        Invoke("Reset", resetDelay);

    }


    

    

    private void Reset()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
}
