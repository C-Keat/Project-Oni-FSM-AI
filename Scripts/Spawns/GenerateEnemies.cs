using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateEnemies : MonoBehaviour
{


    public GameObject enemy;
    public float numbOfEnemy = 0;
    protected float repeatTime = 20f;
    protected float numberOFSpawned = 0;
    public int killamount;

   // public float waitTime, startWaitTime = 8f;




    private void Start()
    {
        killamount = GameManager.GMinstance.GETKILLAMOUNT();

        repeatTime = numbOfEnemy;
        InvokeRepeating("Spawn", 2f, repeatTime);
        Debug.Log("number of enemies" + numbOfEnemy + "number of enemies" + killamount);
        if (killamount <= 0)
        {
            numbOfEnemy = 1;
            Debug.Log("number of enemies" + numbOfEnemy + "number of enemies" + killamount);
        }
        if(killamount >=3)
        {
            numbOfEnemy = 3;
            Debug.Log("number of enemies" + numbOfEnemy + "number of enemies" + killamount);
        }
        if(killamount >= 5)
        {
            numbOfEnemy = 5;
        }
        //else
        //{
        //    numbOfEnemy = 1;
        //}


    }

    void Spawn()
    {
        //waitTime = 0;
        while (numberOFSpawned < numbOfEnemy)
        {

            
                Instantiate(enemy, transform.position, Quaternion.identity);
                numberOFSpawned++;
        
           
        }
        
    }




}
