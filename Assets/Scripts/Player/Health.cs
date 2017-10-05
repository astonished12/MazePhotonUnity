using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Health : MonoBehaviour {
    public int startingHealth = 100;                            // The amount of health the player starts the game with.
    public int currentHealth;                                   // The current health the player has.
    public Slider healthSlider;                                 // Reference to the UI's health bar.
    public Image damageImage;
    public RawImage avatarImg;
    // Reference to an image to flash on the screen on being hurt.
    public AudioClip deathClip;                                 // The audio clip to play when the player dies.
    public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.


    Animator anim;                                              // Reference to the Animator component.
    AudioSource playerAudio;                                    // Reference to the AudioSource component.
    PlayerMovement playerMovement;                              // Reference to the player's movement.
    PlayerShoting playerShooting;                              // Reference to the PlayerShooting script.
    FirstPersonController controler;
    NetworkCharacter playerNetworkChracter;
    bool isDead;                                                // Whether the player is dead.
    public bool damaged;                                               // True when the player gets damaged.


    void Awake()
    {
        // Setting up the references.
        anim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        controler = GetComponent<FirstPersonController>();
        playerNetworkChracter = GetComponent<NetworkCharacter>();
        avatarImg.texture = GameObject.Find("CanvasMenu").transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<RawImage>().texture;

        playerShooting = GetComponentInChildren<PlayerShoting>();

        // Set the initial health of the player.
        currentHealth = startingHealth;
    }


    void Update()
    {
        // If the player has just been damaged...
        if (damaged)
        {
            // ... set the colour of the damageImage to the flash colour.
            damageImage.color = flashColour;
            TakeDamage(10);
        }
        // Otherwise...
        else
        {
            // ... transition the colour back to clear.
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        // Reset the damaged flag.
        damaged = false;
    }


    [PunRPC]
    public void TakeDamage(int amount)
    {
        // Set the damaged flag so the screen will flash.
        damaged = true;

        // Reduce the current health by the damage amount.
        currentHealth -= amount;

        // Set the health bar's value to the current health.
        healthSlider.value = currentHealth;

        // Play the hurt sound effect.
        playerAudio.Play();

        // If the player has lost all it's health and the death flag hasn't been set yet...
        if (currentHealth <= 0 && !isDead)
        {
            // ... it should die.
            Death();
        }
    }


    void Death()
    {



        playerNetworkChracter.GetComponent<PhotonView>().RPC("PlayerDies", PhotonTargets.All);

        // Set the death flag so this function won't be called again.
        isDead = true;

        // Turn off any remaining shooting effects.
        //playerShooting.DisableEffects();

        // Tell the animator that the player is dead.
        anim.SetBool("Die",true);

        // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
        playerAudio.clip = deathClip;
        playerAudio.Play();

        // Turn off the movement and shooting scripts.
        gameObject.transform.Find("FirstPersonCharacter").gameObject.SetActive(false);
        gameObject.transform.Find("HealthCrosshair").gameObject.SetActive(false);
        gameObject.GetComponent<FirstPersonController>().enabled = false;
        gameObject.GetComponent<PlayerMovement>().enabled = false;
        gameObject.GetComponent<NetworkCharacter>().enabled = false;
        gameObject.GetComponent<PlayerShoting>().enabled = false;
        gameObject.GetComponent<Health>().enabled = false;
        
        NetworkManager.standbyCamera.SetActive(true);


        StartCoroutine("SetDeadOff");
    }

    IEnumerator SetDeadOff()
    {
        yield return new WaitForSeconds(1.5f);
        anim.SetBool("Die", false);
        Respawn();

    }

    void Respawn()
    {
        playerNetworkChracter.GetComponent<PhotonView>().RPC("PlayerRespawns", PhotonTargets.All);

        // Turn on the movement and shooting scripts.
        gameObject.transform.Find("FirstPersonCharacter").gameObject.SetActive(true);
        gameObject.transform.Find("HealthCrosshair").gameObject.SetActive(true);
        gameObject.GetComponent<FirstPersonController>().enabled = true;
        gameObject.GetComponent<PlayerMovement>().enabled = true;
        gameObject.GetComponent<NetworkCharacter>().enabled = true;
        gameObject.GetComponent<PlayerShoting>().enabled = true;
        gameObject.GetComponent<Health>().enabled = true;

        NetworkManager.standbyCamera.SetActive(false);
    }

}
