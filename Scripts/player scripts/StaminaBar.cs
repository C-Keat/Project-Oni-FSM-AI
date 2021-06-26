using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{

    // true false to track if there is enough stamina to do an action
    public bool enoughSTA;
    
    // taking the stamina bar
    public Slider staminaBar;

    // setting the max stamina bar and a value for current stamina
    private int Maxstamina = 200;
    private int currentstamina;
    // how quickly the stamina increases
    private WaitForSeconds regentick = new WaitForSeconds(0.1f);

    private Coroutine regen;

    // allowing other scripts to access the stamina bar script in game.
    public static StaminaBar Staminstance;

    private void Update()
    {

        if (currentstamina >= 20)
        {
            enoughSTA = true;
        }
        else 


        Debug.Log("staminabar " + enoughSTA);
    }
    private void Awake()
    {
        // making this a single entity so that it doesnt creat multiples on player spawn
        if (Staminstance == null)
            Staminstance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // setting the values of stamina.
        currentstamina = Maxstamina;
        staminaBar.maxValue = Maxstamina;
        staminaBar.value = Maxstamina;
    }
    
    public void UseStamina(int amount)
    {
        // changing the stamina bar value
        if(currentstamina - amount >= -10)
        {
            currentstamina -= amount;

            staminaBar.value = currentstamina;

            if (regen != null)
                StopCoroutine(regen);
        
           regen = StartCoroutine(RegenStamina());

            enoughSTA = true;

        }
        else
        {
            Debug.Log("not enough stamina");
            enoughSTA = false;
        }
        

    }

    public bool CheckStam()
    {

        return enoughSTA;
    }

    private IEnumerator RegenStamina()
    {
        // managing the stamina regen of the player so that it waits 5 seconds before regaining
        yield return new WaitForSeconds(1);
        while(currentstamina < Maxstamina)
        {
            currentstamina += Maxstamina / 200;
            staminaBar.value = currentstamina;
            yield return regentick;

        }
        regen = null;
    }
}
