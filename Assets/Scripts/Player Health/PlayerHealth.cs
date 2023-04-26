using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    private float lerpTimer;
    private bool isDamageTaken;
    public float maxHealth = 100f;
    public float chipspeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;

    [Header("Sound On Death")]

    public AudioClip soundToPlay;
    public float volume;
    new AudioSource audio;
    public bool alreadyPlayed = false;


    // Start is called before the first frame update
    void Start()
    {
        //audio for death initialization
        audio = GetComponent<AudioSource>();

        isDamageTaken = false;
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

        

        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
        if (isDamageTaken)
        {
            TakeDamage(Random.Range(5, 10));
        }

        if (health <= 0)
        {
            if (!alreadyPlayed)
            {
                audio.PlayOneShot(soundToPlay, volume);
                alreadyPlayed = true;
            }
            //die
            GetComponent<Death_Handler>().HandleDeath();

        }
        else
        {
            Time.timeScale = 1.0f;
        }
        
    }
    public void UpdateHealthUI()
    {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;

        if(fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipspeed;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
        isDamageTaken = false; //restart
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.gameObject.CompareTag("enemyDamage"))
        {
            isDamageTaken = true;
        }
    }

 
}
