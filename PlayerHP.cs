using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    private int maxHealth;
    public int currentHealth;
    public Text healthText;
    public Text gameOverText;
    public Text manaText;
    public bool isDead;
    public Stat strength;
    public Stat intellect;
    public Stat spellPower;
    public Stat armor;
    public int currentMana;
    public int maxMana;
    public Animator anim;
    public GameObject deadUI;
    

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        isDead = false;
        currentHealth = 100 + strength.GetValue();
        currentMana = 100 + intellect.GetValue();
        maxHealth = currentHealth + strength.GetValue() ;
        maxMana = currentMana + intellect.GetValue();
        
    }

    private void Update()
    {
        CheckHealth();
        CheckMana();
        UpdateManaText();
        UpdateHealthText();
        PassiveManaRegen();
        if (Input.GetKeyDown(KeyCode.C)) {
            addstr();
            addsp();
            addint();
        }
        
    }
    //testing
    public void addstr()
    {
        strength.AddValue(10);
        maxHealth += strength.GetValue();
        UpdateHealthText();
    }
    public void addsp() 
    {
        spellPower.AddValue(10);
    }
    public void addint() 
    {
        intellect.AddValue(10);
        maxMana += intellect.GetValue();
        UpdateManaText();
    }
    /*
     * This method should only be called from another script. Upon being called,
     * the player will recieve damage based on the variable that is sent to this
     * method. Damage depends on what enemy/attack is being used on it. It will
     * remove that amount from the currentHealth variable, check to see if
     * the player has 0hp, and call the UpdateHealthText method. If the
     * current health value dips below 0, it sets it to a flat zero and flips the
     * isDead variable to TRUE, which will start the "death" sequence in the main
     * Player controller script.
     */


    public void TakeDamage(int damage)
    {
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;

            //Bre: if the player dies in the mines, the music stops and plays overworld again
            if (FindObjectOfType<AudioManager>().playing)
            {
                FindObjectOfType<AudioManager>().UnPause("Overworld");
                FindObjectOfType<AudioManager>().Stop("Mines");
            }

            anim.SetBool("isDead", true);
            deadUI.SetActive(true);

            if(deadUI.activeInHierarchy)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<Movement>().enabled = false;
            }
        }

        UpdateHealthText();
    }

    /*
      This function is used for the health value
      not to go over the maxHealth threshold.
     */
    public void CheckHealth() 
    {
        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
    }


    /*
     This function is used for the health value
     not to go over the maxHealth threshold.
    */
    public void CheckMana()
    {
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }
    }
    IEnumerator PassiveManaRegen() 
    {
        while (isDead == false && currentMana<maxMana) 
        {
            yield return new WaitForSeconds(5f);
            currentMana += intellect.GetValue();
        }
    }
    //removed for testing
    /*public void GiveHealth()
    {
        maxHealth = 1000;
        currentHealth = maxHealth;
        //UpdateHealthText(damage);
    } */

    public void UpdateHealthText()
    {
        healthText.text = "Health: " + currentHealth.ToString();
    }
    public void UpdateManaText()
    {
        manaText.text = "Mana: " + currentMana.ToString();
    }

}
