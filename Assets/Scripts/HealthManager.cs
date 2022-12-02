using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    public PlayerController thePlayer;

    public TextMeshProUGUI HPText;

    public float invincibilityLenght;
    private float invincibilityCounter;

    public Renderer playerRenderer;
    private float flashCounter;
    public float flashLenght = 0.1f;

    private bool isRespawning;
    private Vector3 respawnPoint;

    public float respawnLenght;
    public Image blackScreen;
    private bool isFadeToBlack;
    private bool isFadeFromBlack;
    public float fadeSpeed;
    public float waitForFade;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        //thePlayer = FindObjectOfType<PlayerController>(); // as soon as the game starts the game manager knows where the player is

        respawnPoint = thePlayer.transform.position;
        
    }
        
    // Update is called once per frame
    void Update()
    {
        if(invincibilityCounter > 0)
        {
            invincibilityCounter -= Time.deltaTime;

            // player flashing after taking damage
            flashCounter -= Time.deltaTime;
            if(flashCounter <= 0)
            {
                playerRenderer.enabled = !playerRenderer.enabled;
                flashCounter = flashLenght;
            }
            // prevent player from disappearing
            if(invincibilityCounter <= 0)
            {
                playerRenderer.enabled = true;
            }
        }

        HPText.text = "HP: " + currentHealth;

        if (isFadeToBlack)
        {
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, Mathf.MoveTowards(blackScreen.color.a, 1f, fadeSpeed * Time.deltaTime));
            if(blackScreen.color.a == 1f)
            {
                isFadeToBlack = false;
            }
        }
        if (isFadeFromBlack)
        {
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, Mathf.MoveTowards(blackScreen.color.a, 0f, fadeSpeed * Time.deltaTime));
            if (blackScreen.color.a == 0f)
            {
                isFadeFromBlack = false;
            }
        }

    }

    public void HurtPlayer(int damage, Vector3 direction)
    {
        if (invincibilityCounter <= 0)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Respawn();
            }
            else
            {

                thePlayer.KnockBack(direction);

                invincibilityCounter = invincibilityLenght;

                playerRenderer.enabled = false;

                flashCounter = flashLenght;
            }
        }
    }
    
    public void Respawn()
    {
        if (!isRespawning)
        {
            StartCoroutine("RespawnCo"); // starting an IEnumerator Coroutine
        }
    }
    public IEnumerator RespawnCo()
    {

        GameObject player = GameObject.Find("Player"); // find player and disable character controller
        CharacterController charController = player.GetComponent<CharacterController>();
        charController.enabled = false;
        isRespawning = true;
        thePlayer.gameObject.SetActive(false); // player disappears

        yield return new WaitForSeconds(respawnLenght); //finish the code above, wait for these amounts of seconds, do the code after

        isFadeToBlack = true;

        yield return new WaitForSeconds(waitForFade);

        isFadeToBlack = false;
        isFadeFromBlack = true;

        isRespawning = false;

        thePlayer.gameObject.SetActive(true); // player appearance

        thePlayer.transform.position = respawnPoint;
        currentHealth = maxHealth;

        invincibilityCounter = invincibilityLenght; // a bit of invincibility after respawn
        playerRenderer.enabled = false;
        flashCounter = flashLenght;

        charController.enabled = true; // enable character controller
    }
}
