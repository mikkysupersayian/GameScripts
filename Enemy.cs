/*
 * Author: Colby Schaeding
 * CPS 498 - Spring 2021
 * 
 * Script name: Enemy.cs
 * Description: This script handles everything related to enemy AI.
 *              Three major "enemy states" include Patrol, Chase,
 *              and Attack. Patrol will see the enemy periodically
 *              and randomly choosing a new, valid spot on the navMesh to
 *              walk to. Chase will find a target (the player) and
 *              indefinitely move towards them. Attack will play an
 *              animation and deal damage to the player when they 
 *              get too close.
 *              
 */



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    [SerializeField]
    Transform target;
    [SerializeField]
    float chaseRange = 5f;
    [SerializeField]
    float turnSpeed = 5f;
    public bool atTarget = false;
    [SerializeField] Vector3 enemyPos;
    public bool isDead;

    NavMeshAgent navMeshAgent;
    float distanceToTarget = Mathf.Infinity;
    bool isProvoked;

    [Header("Enemy Info")]
    public string enemyName;
    public int enemyLevel;
    public int enemyHealth;
    public int enemyXP;

    // attack variables
    private bool attackReady;
    private float attackDelay;
    private float attackCooldownTime;

    // Patrol AI variables
    private float timeUntilNewPath;
    Vector3 destination;
    NavMeshPath path;
    private bool validPath;
    private bool inCoroutine;
    public bool isRooted;

    //currently unused Patrol variables
    private Vector3 patrolPoint;
    bool isPatrolPointSet;
    private float patrolPointRange;

    // life and death related variables

    private float timeDelayUntilDeath;

    private GameObject targetRender;

    //George: added as it is nessessary to allow enemies to drop items.
    private LootSystem getItem;

    //Breahna: Added sounds for enemy attack and death.
    public AudioSource soundAttack;
    public AudioSource soundDead;

    private void Awake()
    {
        enemyXP = (enemyHealth / 2) * enemyLevel;
    }

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        isProvoked = false;
        isDead = false;
        inCoroutine = false;
        isRooted = false;
        path = new NavMeshPath();
        timeUntilNewPath = 5f;
        attackReady = true;
        attackDelay = 0.5f;
        attackCooldownTime = 2f;
        timeDelayUntilDeath = 4f;

        //George: added to allow loot system to work.
        getItem = GetComponent<LootSystem>();

        Transform parent = transform;

        for(int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            if(child.tag == "TargetRender")
            {
                targetRender = child.gameObject;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfEnemyDead();
        CheckIfPlayerDead();
        CheckIfRooted();
        enemyPos = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y, gameObject.transform.position.z);

        distanceToTarget = Vector3.Distance(target.position, transform.position);
        if (isProvoked)
        {
            EngageTarget();
        }
        else if (distanceToTarget <= chaseRange)
        {
            isProvoked = true;
            GetComponent<Animator>().SetBool("provoked", true);
        }

        // experimental code to begin enemy patrolling
        // only patrols when unprovoked and wont start
        // more than one patrol at a time
        if (!isProvoked && !inCoroutine)
        {
            StartCoroutine(StartPatrol());
        }
        
        if(atTarget)
        {
            targetRender.SetActive(true);
        } else
        {
            targetRender.SetActive(false);
        }
    }

    /*
     * This method checks whether or not the enemy
     * this script is attached to has reached a health
     * value at or below zero. If it has, the function
     * begins the enemy's "death" sequence
     */
    private void CheckIfEnemyDead()
    {
        if (isDead)
        {
            soundDead.Play(); //Bre: Enemy death sound
            StartCoroutine(StartDeathSequence());
        }
    }
    /*
      This method checks if the enemy is rooted
     */
    private void CheckIfRooted() {
        if (isRooted == true) {
        
        }
    }

    /*
     * Virtually the same as CheckIfEnemyDead,
     * except this one checks if the Player
     * has died.
     */
    private void CheckIfPlayerDead()
    {
        if (target.GetComponent<PlayerHP>().isDead == true)
        {
            //target.tag = "Dead";
            GetComponent<Animator>().SetBool("attack", false);
            GetComponent<Animator>().SetBool("move", false);
            GetComponent<Animator>().SetTrigger("victory");
            enabled = false;
            isProvoked = false;
        }
    }

    /* This function calls the FaceTarget() function,
     * then will either call the AttackTarget() function
     * if the player is within the navMesh stopping distance,
     * or call the ChaseTarget() function if the player is
     * slightly outside of the stopping distance range. */
    private void EngageTarget()
    {

        FaceTarget();
        if (distanceToTarget > navMeshAgent.stoppingDistance + 0.85f)
        {
            ChaseTarget();
        }

        if (distanceToTarget <= navMeshAgent.stoppingDistance + 0.85f)
        {
            AttackTarget();
        }
    }

    /* When the enemy begins to engage the target,
     * this functions rotates the enemy to properly 
     * face them. */
    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    /* When the target enters a certain range, the
     * enemy begins to chase the target down, constantly
     * moving towards the target in an unrelenting fashion. */
    private void ChaseTarget()
    {
        if (isRooted == false)
        {
            GetComponent<Animator>().SetBool("attack", false);
            GetComponent<Animator>().SetBool("move", true);
            navMeshAgent.SetDestination(target.position);
        }
    }

    /* This method automatically provokes the enemy,
     * regardless of whether or not the player entered
     * its "danger zone." */
    public void OnDamageTaken()
    {
        isProvoked = true;
        GetComponent<Animator>().SetBool("provoked", true);
    }

    /* When the enemy is very close to the player,
     * it will play an attack animation and reduce
     * the HP of its target */
    private void AttackTarget()
    {
        GetComponent<Animator>().SetBool("attack", true);
        GetComponent<Animator>().SetBool("move", false);
       // Debug.Log(name + " has seeked and is destroying " + target.name);
        PlayerHP playerHealth = target.gameObject.GetComponent<PlayerHP>();
        if ((playerHealth != null) && (attackReady))
        {
            StartCoroutine(AttackCooldown());
        }
    }

    /*
     * This function serves as the enemy's main attack source.
     * When the script reaches this method, it flags attackReady
     * to FALSE to prevent multiple attacks from occuring at the 
     * same time. It tells the animator to stop the "move" animation
     * and to begin the "attack" animation. A delay is set in place
     * to allow the animation to line up with the damage reduction, 
     * which occurs shortly after. Lastly, a longer attack cooldown delay
     * occurs again to prevent huge bursts of damage, before finally
     * allowing another attack to happen by re-flagging the attackReady 
     * variable as TRUE
     */
    IEnumerator AttackCooldown()
    {
        attackReady = false;
        GetComponent<Animator>().SetBool("move", false);
        GetComponent<Animator>().SetBool("attack", true);
        soundAttack.Play(); //Bre: sound plays when enemy attacks
        yield return new WaitForSeconds(attackDelay);
        PlayerHP playerHealth = target.gameObject.GetComponent<PlayerHP>();
        playerHealth.TakeDamage(10);
        yield return new WaitForSeconds(attackCooldownTime);
        attackReady = true;
    }
    
    /* This function randomizes a new x and z coordinate
     * within a certain range (of postive AND negative numbers),
     * stores them in a variable called 'pos' and returns 'pos'
     * back to the function that called this one. */
    Vector3 GetNewRandomPosition()
    {
        float randX = Random.Range(-12, 12);
        float randZ = Random.Range(-12, 12);

        Vector3 pos = new Vector3(transform.position.x + randX, transform.position.y, transform.position.z + randZ);

        return pos;
    }

    /* This function sets a cooroutine variable to true so that
     * it cannot be run multiple times at once. It will continue 
     * to fetch new destinations (and paths to those destinations)
     * until one of them is valid, wait a small amount of time, then
     * move the enemy to that new location. Afterwards it resets the
     * cooroutine variable so that it may be called again for another
     * patrol session. */
    IEnumerator StartPatrol()
    {
        inCoroutine = true;
        yield return new WaitForSeconds(timeUntilNewPath);

        validPath = false;
        while (!validPath)
        {
            GetNewPath();
            validPath = navMeshAgent.CalculatePath(destination, path);
        }

        inCoroutine = false;
    }

    /* This function is responsible for calling the
     * GetNewRandomPosition() function and using that
     * data to tell the navMeshAgent to move to that
     * new destination. */
    private void GetNewPath()
    {
        destination = GetNewRandomPosition();
        GetComponent<Animator>().SetBool("move", true);
        navMeshAgent.SetDestination(destination);
    }

    

    /*
     * This method begins when the enemy's hp reaches zero.
     * It will halt all other animations/movement, play
     * the enemy's death animation, then destroy the object
     * after a few seconds.
     */
    IEnumerator StartDeathSequence()
    {
       // soundDead.Play(); //Bre: Enemy death sound
        inCoroutine = true;
        GetComponent<Animator>().SetBool("attack", false);
        GetComponent<Animator>().SetBool("move", false);
        isProvoked = false;
        GetComponent<Animator>().SetBool("provoked", false);
        GetComponent<Animator>().SetTrigger("death");
        //soundDead.Play(); //Bre: Enemy death sound
        yield return new WaitForSeconds(timeDelayUntilDeath);
        enabled = false;
       // Debug.Log("make it this far?");
        navMeshAgent.enabled = false;
        //Debug.Log("how about here?"); 
        isProvoked = false;

        //George: added this if here to check if the enemy drops loot befoer they are despawned.
        if (getItem != null)
        {
          getItem.LootDrop();
        }

        Destroy(this.gameObject);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacteristics>().addExp(enemyXP);
    }
    /*
     * The method for the enemy to take damage 
     * from the player.
     * */
    public void TakeDamage(int damage)
    {
        if (!isProvoked)
        {
            OnDamageTaken();
        }

        enemyHealth -= damage;

        if (enemyHealth <= 0)
        {
            enemyHealth = 0;
            isDead = true;
        }

        UpdateEnemyHealthText(damage);
    }

    /*
     * This function is primarily used for testing purposes,
     * and simply sends a message to the debug log to inform
     * how much damage the enemy took
     */
    private void UpdateEnemyHealthText(int damage)
    {
        //healthText.text = "Health: " + currentHealth.ToString();
        Debug.Log("Enemy HP reduced by " + damage + ". Current HP = " + enemyHealth);
    }
    

    public Vector3 getEnemyPos() {
        return enemyPos;
    }
}