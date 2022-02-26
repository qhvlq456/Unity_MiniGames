using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
    int startingHealth = 100;
    int currentHealth;
    Slider healthSlider;
    Image damageImage;
    AudioSource pAudio;
    float flashSpeed = 5f;
    Color flashColor = new Color(1,0,0,0.1f);
    bool damaged = false;

    // player dead
    Animator animator;
    PlayerMovement playMovement;
    PlayerShooting playShooting;
    public bool isDead = false;
    public AudioClip deadClip;
    public AudioClip hurtClip;
    // player spawn
    [SerializeField]
    Transform playerSpawnTransform;

    private void Awake() {
        healthSlider = GameObject.Find("HeartSlider").GetComponent<Slider>();
        damageImage = GameObject.Find("DamageEffect").GetComponent<Image>();

        pAudio = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        playMovement = GetComponent<PlayerMovement>();
        playShooting = GetComponentInChildren<PlayerShooting>();
    }
    void Start()
    {
        currentHealth = startingHealth;

        healthSlider.maxValue = startingHealth;
        healthSlider.value = startingHealth;
    }

    void Update()
    {
        if(NightmareManager.Manager.isGameOver) return;
        
        if(damaged)
        {
            damageImage.color = flashColor;
        }
        else
        {
            damageImage.color = Color.Lerp(damageImage.color,Color.clear,flashSpeed * Time.deltaTime);
        }

        damaged = false;
    }
    void Initialization()
    {        
        currentHealth = startingHealth;

        healthSlider.maxValue = startingHealth;
        healthSlider.value = startingHealth;
        
        isDead = false;
    }
    public void TakeDamage(int amount)
    {
        damaged = true;
        currentHealth -= amount;
        healthSlider.value = currentHealth;
        pAudio.Play();

        if(currentHealth <= 0 && !isDead)
            Death();
    }
    void Death()
    {
        NightmareManager.Manager.UpdateLife();
        
        isDead = true;

        playShooting.DisableEffects();

        animator.SetTrigger("Die");

        pAudio.clip = deadClip;
        pAudio.Play();

        playMovement.enabled = false; // enabled 되었음 enabled에서 초기화 하면 되지 않을까라고 한번 생각만 해본다
        playShooting.enabled = false;
    }

    public void RestartLevel()
    {
        Debug.Log("RestartLevel");
        StartCoroutine(PlayerSpawn());
    }
    IEnumerator PlayerSpawn()
    {
        yield return new WaitForSeconds(3f);

        Initialization();
        transform.position = playerSpawnTransform.position;

        animator.SetTrigger("IsRespawn");

        pAudio.clip = hurtClip;

        playMovement.enabled = true;
        playShooting.enabled = true;
    }
}
