using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SpellAttack : MonoBehaviour
{
    public GameObject spell,spell2,spell3,spell4;

    public GameObject targetEnem_go;

    [SerializeField] public GameObject player;
    private Animator anim;
    [SerializeField] public int damage;
    public Stat SpellPower;
    public int manacost;
    public bool isPurchased = false;
    public bool isCasting = false;
    public AudioSource audio;
    [SerializeField] public AudioClip[] audioClipArray;

    public Image spell_1, spell_2, spell_3, spell_4;

    // Update is called once per frame
    private void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    
    
    //Different spell couroutines

    /*
     * Fireball spell, uses target enemy script in ordert to get the location where the particle system needs to go
     */
    IEnumerator fireBall()
    {
        if (targetEnem_go.GetComponent<TargetEnemy>().targetEnem != null && player.GetComponent<PlayerHP>().currentMana>=15 && isCasting == false && anim.GetBool("isRunning") != true) {
            spell_1.color = new Color(spell_1.color.r, spell_1.color.g, spell_1.color.b, .1f);
            isCasting = true;
            anim.SetBool("isCasting", true);
            yield return new WaitForSeconds(2.5f);
            anim.SetBool("isCasting", false);
            audio.PlayOneShot(audioClipArray[0]);
            manacost = 5;
            if (targetEnem_go.GetComponent<TargetEnemy>().targetEnem.GetComponent<Enemy>().isDead != true)
            {
                GameObject fb1 = Instantiate(spell, transform.position, transform.rotation) as GameObject;
                fb1.transform.position = fb1.transform.position + new Vector3(0, 1, 0);
                Rigidbody rb = fb1.GetComponent<Rigidbody>();
                rb.velocity = transform.forward * 10;
                player.GetComponent<PlayerHP>().currentMana -= manacost;
                damage = 5 + SpellPower.GetValue();
                isCasting = false;
                targetEnem_go.GetComponent<TargetEnemy>().targetEnem.GetComponent<Enemy>().TakeDamage(damage);
                spell_1.color = new Color(spell_1.color.r, spell_1.color.g, spell_1.color.b, 1f);
            }
        }
    }

    /*
    * Lightning ball spell uses target enemy script in ordert to get the location where the particle system needs to go
    */
    IEnumerator lightningBall()
    {
        if(targetEnem_go.GetComponent<TargetEnemy>().targetEnem != null && player.GetComponent<PlayerHP>().currentMana >= 15 && isCasting == false && anim.GetBool("isRunning") != true)
        {
            spell_2.color = new Color(spell_1.color.r, spell_1.color.g, spell_1.color.b, .1f);
            isCasting = true;
            anim.SetBool("isCasting", true);
            yield return new WaitForSeconds(2.5f);
            anim.SetBool("isCasting", false);
            audio.PlayOneShot(audioClipArray[1]);
            manacost = 15;
            if (targetEnem_go.GetComponent<TargetEnemy>().targetEnem.GetComponent<Enemy>().isDead != true)
            {
                GameObject fb1 = Instantiate(spell2, transform.position, transform.rotation) as GameObject;
                fb1.transform.position = fb1.transform.position + new Vector3(0, 1, 0);
                Rigidbody rb = fb1.GetComponent<Rigidbody>();
                rb.velocity = transform.forward * 10;
                player.GetComponent<PlayerHP>().currentMana -= manacost;
                damage = 15 + SpellPower.GetValue();
                isCasting = false;
                targetEnem_go.GetComponent<TargetEnemy>().targetEnem.GetComponent<Enemy>().TakeDamage(damage);
                spell_2.color = new Color(spell_1.color.r, spell_1.color.g, spell_1.color.b, 1f);
            }
        } 
    }

    /*
     * Nova spell, the spell is going to root the enemies around the radius of the particle system
     */
    IEnumerator frostNova()
    {
        if (player.GetComponent<PlayerHP>().currentMana >= 25 && isCasting == false && anim.GetBool("isRunning") != true)
        {
            spell_3.color = new Color(spell_1.color.r, spell_1.color.g, spell_1.color.b, .1f);
            isCasting = true;
            anim.SetBool("isInstantCasting", true);
            yield return new WaitForSeconds(1);
            anim.SetBool("isInstantCasting", false);
            audio.PlayOneShot(audioClipArray[2]);
            manacost = 25;
            GameObject fb1 = Instantiate(spell3, transform.position, transform.rotation) as GameObject;
            fb1.transform.position = fb1.transform.position;
            player.GetComponent<PlayerHP>().currentMana -= manacost;
            isCasting = false;
            Collider[] colliders = Physics.OverlapSphere(transform.position, 15);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].CompareTag("Enemy"))
                {
                    colliders[i].GetComponent<Enemy>().isRooted = true;
                }
            }
            yield return new WaitForSeconds(.7f);
            Destroy(fb1);
            yield return new WaitForSeconds(5.3f);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].CompareTag("Enemy"))
                {
                    colliders[i].GetComponent<Enemy>().isRooted = false;
                }
            }
            spell_3.color = new Color(spell_1.color.r, spell_1.color.g, spell_1.color.b, 1f);
        }
    }

    /*
     * healing spell, the spell is going to heal our player.
     */
    IEnumerator healingSpell() 
    {
        if (player.GetComponent<PlayerHP>().currentMana >= 10 && isCasting == false && anim.GetBool("isRunning") != true)
        {
            spell_4.color = new Color(spell_1.color.r, spell_1.color.g, spell_1.color.b, .1f);
            isCasting = true;
            anim.SetBool("isCasting", true);
            yield return new WaitForSeconds(2.5f);
            anim.SetBool("isCasting", false);
            audio.PlayOneShot(audioClipArray[3]);
            manacost = 10;
            GameObject fb1 = Instantiate(spell4, transform.position, transform.rotation) as GameObject;
            player.GetComponent<PlayerHP>().currentMana -= manacost;
            isCasting = false;
            fb1.transform.position = fb1.transform.position + new Vector3(0, 1, 0);
            player.GetComponent<PlayerHP>().currentHealth += 10;
            yield return new WaitForSeconds(.7f);
            player.GetComponent<PlayerHP>().currentHealth += 10;
            yield return new WaitForSeconds(.7f);
            player.GetComponent<PlayerHP>().currentHealth += 10;
            yield return new WaitForSeconds(.7f);
            player.GetComponent<PlayerHP>().currentHealth += 10;
            yield return new WaitForSeconds(.7f);
            Destroy(fb1);
            spell_4.color = new Color(spell_1.color.r, spell_1.color.g, spell_1.color.b, 1f);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine("fireBall");
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine("lightningBall");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine("frostNova");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) 
        {
            StartCoroutine("healingSpell");
        }
    }
    
}
