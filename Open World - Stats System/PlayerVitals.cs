using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerVitals : MonoBehaviour
{

    public Slider healthSlider;
    public int maxHealth, healthFallRate;

    public Slider thirstSlider;
    public int maxThirst, thirstFallRate;

    public Slider hungerSlider;
    public int maxHunger, hungerFallRate;

    public Slider staminaSlider;
    public int maxStamina, staminaFallMultipier, staminaRegenMultiplier;
    private int staminaFallRate, staminaRegenRate;

    [Header("Temperature Settings")]
    public float freezingTemp;
    public float currentTemp;
    public float normalTemp;
    public float heatTemp;
    public Text tempNumber;
    public Image tempBG;

    private CharacterController characterController;
    private FirstPersonController playerController;

    void Start()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

        thirstSlider.maxValue = maxThirst;
        thirstSlider.value = maxThirst;

        hungerSlider.maxValue = maxHunger;
        hungerSlider.value = maxHunger;

        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = maxStamina;

        staminaFallRate = 1;
        staminaRegenRate = 1;

        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<FirstPersonController>();
    }

    void Update()
    {
        // =========== Health Control ===========
        // Controlls health loss due to starvation and/or dehydration.
        if (hungerSlider.value <= 0 && thirstSlider.value <= 0)
        {
            healthSlider.value -= Time.deltaTime / healthFallRate * 2;
        }
        else if (hungerSlider.value <= 0 || thirstSlider.value <= 0 || currentTemp <= freezingTemp || currentTemp >= heatTemp)
        {
            healthSlider.value -= Time.deltaTime / healthFallRate;
        }

        if (healthSlider.value <= 0)
        {
            CharacterDeath();
        }
        // ======================================

        // =========== Hunger Control ===========
        if (hungerSlider.value >= 0)
            hungerSlider.value -= Time.deltaTime / hungerFallRate;

        else if (hungerSlider.value <= 0)
            hungerSlider.value = 0;

        else if (hungerSlider.value >= maxHunger)
            hungerSlider.value = maxHunger;
        // =======================================

        // =========== Thirst Control ===========
        if (thirstSlider.value >= 0)
            thirstSlider.value -= Time.deltaTime / thirstFallRate;

        else if (thirstSlider.value <= 0)
            thirstSlider.value = 0;

        else if (thirstSlider.value >= maxThirst)
            thirstSlider.value = maxThirst;
        // =======================================

        // =========== Stamina Control ==========
        // Deplete stamina when character is running.
        if (characterController.velocity.magnitude > 0 && Input.GetKey(KeyCode.LeftShift))
        {
            staminaSlider.value -= Time.deltaTime / staminaFallRate * staminaFallMultipier;

            // Raise the character's temp when running.
            if (staminaSlider.value > 0)
            {
                currentTemp += Time.deltaTime / 5;
            }
        }

        // If character is not running then regenerate stamina.
        else
        {
            staminaSlider.value += Time.deltaTime / staminaRegenRate * staminaRegenMultiplier;

            // If character is not running then lower the their temp back to normal.
            if (currentTemp >= normalTemp)
            {
                currentTemp -= Time.deltaTime / 10;
            }
        }

        if (staminaSlider.value >= maxStamina)
            staminaSlider.value = maxStamina;

        // Player will walk when out of stamina.
        else if (staminaSlider.value <= 0)
        {
            staminaSlider.value = 0;
            playerController.m_RunSpeed = playerController.m_WalkSpeed;
        }

        // While character has stamina, they can run.
        else if (staminaSlider.value >= 0)
            playerController.m_RunSpeed = playerController.m_RunSpeedNorm;
        // =======================================

        // =========== Temperature Control ==========
        if (currentTemp <= freezingTemp)
        {
            tempBG.color = Color.blue;
        }

        else if (currentTemp >= heatTemp - 0.1)
        {
            tempBG.color = Color.red;
            UpdateTemp();
        }

        else
        {
            tempBG.color = Color.green;
            UpdateTemp();
        }
        // ==========================================

    }

    void UpdateTemp()
    {
        tempNumber.text = currentTemp.ToString("00.0");
    }

    void CharacterDeath()
    {
        //Soon...
    }
}
