using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [Header("Enemy Set Up")]
    public GameObject[] enemyLoad;
    public GameObject[] enemySpawn;
    public Transform[] enemySpawnPosition;
    public int enemyNumber;
    public bool[] isRespawning;

    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        for (int i = 0; i < enemyLoad.Length; i++)
        {
            if (enemyLoad[i] == null)
            {
                enemyNumber = i;
                if (isRespawning[i] != true)
                {
                    isRespawning[i] = true;
                    StartCoroutine("RespawnEnemy");
                }

            }
        }
    }

    IEnumerator RespawnEnemy() 
    {
        Debug.Log("EnemyRespawning");
        yield return new WaitForSeconds(10f);
        GameObject enemyNewLoad = Instantiate(enemySpawn[enemyNumber], enemySpawnPosition[enemyNumber].position, Quaternion.identity) as GameObject;
        enemyLoad[enemyNumber] = enemyNewLoad;
        isRespawning[enemyNumber] = false;
    }
}

