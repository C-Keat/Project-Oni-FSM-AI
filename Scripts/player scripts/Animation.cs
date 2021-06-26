using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    public bool enoughSTAM;
    [SerializeField] private KeyCode attackKey;
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private KeyCode runKey;
    public Animator anim;
   
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    
    void Update()
    {

        enoughSTAM = StaminaBar.Staminstance.CheckStam();

        if (enoughSTAM)
        {
            attackInput();
            jumpInput();
            runInput();
        }
        else anim.Play("idle");


    }

    private void attackInput()
    {

       
        if (Input.GetKeyDown(attackKey))
        {
            anim.Play("attack");
        }

    }
    private void jumpInput()
    {

        if (Input.GetKey(jumpKey))
        {
            anim.Play("jump");
        }

    }
    private void runInput()
    {

        if (Input.GetKeyDown(runKey))
        {
            anim.Play("run");
        }
        if (Input.GetKeyUp(runKey))
        {
            anim.Play("idle");
        }

    }
}
