using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Handler : MonoBehaviour
{
    [SerializeField] Canvas gameOverCanvas;

     void Start()
    {
        gameOverCanvas.enabled = false;
    }

    public void HandleDeath()
    {
        gameOverCanvas.enabled=true;
        //stop the time
        Time.timeScale = 0f;
        FindObjectOfType<Weapon_Switcher>().enabled = false;

        Cursor.lockState= CursorLockMode.None;
        Cursor.visible=true;
    }
}
