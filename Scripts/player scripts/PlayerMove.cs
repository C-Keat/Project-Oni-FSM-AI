using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    AudioSource audioData;

    [SerializeField] private KeyCode attackKey;
   

    public bool enoughSTAM;
    public float damageDealt = 100;
    // input names for the keyboard 
    [SerializeField] private string horizontalInputName;
    [SerializeField] private string verticalInputName;

    //the values and keys associated with running
    [SerializeField] private float walkSpeed, runSpeed;
    [SerializeField] private float runBuildUpSpeed;
    [SerializeField] private KeyCode runKey;

    private float movementSpeed;

    // refrencing the character contoller
    private CharacterController charController;
    

    // using an animation curve as the jump
    [SerializeField] private AnimationCurve jumpFallOff;
    [SerializeField] private float jumpMultiplier;
    [SerializeField] private KeyCode jumpKey;

    public GameObject Target;

    // true/false jump
    private bool isJumping;

    private void Awake()
    {
        SetMovementSpeed();
        enoughSTAM = true;
        // accessing the character contoller through get component
        charController = GetComponent<CharacterController>();
        audioData = GetComponent<AudioSource>();
    }

    private void Update()
    {
        enoughSTAM = StaminaBar.Staminstance.CheckStam();
        Debug.Log("enough stamina " + enoughSTAM);
        PlayerMovement();
    }

    private void PlayerMovement()
    {

        // value of inputs for movements.
        float horizInput = Input.GetAxis(horizontalInputName);
        float vertInput = Input.GetAxis(verticalInputName);

        // using the input values as movement.
        Vector3 forwardMovement = transform.forward * vertInput;
        Vector3 rightMovement = transform.right * horizInput;


    // moving the character controller and setting the speed so that the player will move at the correct speed.
        charController.SimpleMove(Vector3.ClampMagnitude(forwardMovement + rightMovement, 1.0f) * movementSpeed);

       


        if( enoughSTAM)
        {
            enoughSTAM = StaminaBar.Staminstance.CheckStam();
            JumpInput();
            attackInput();
            SetMovementSpeed();
        }
        else movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, Time.deltaTime * runBuildUpSpeed);


    }

    private void SetMovementSpeed()
    {
        // changing the movement speed 
        if (Input.GetKey(runKey))
        {
            movementSpeed = Mathf.Lerp(movementSpeed, runSpeed, Time.deltaTime * runBuildUpSpeed);
            StaminaBar.Staminstance.UseStamina(1);
        }
        else
        {
            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, Time.deltaTime * runBuildUpSpeed);
            
        }
    }

    private void attackInput()
    {
        if (Input.GetKeyDown(attackKey) && enoughSTAM)
        {
            audioData.Play(0);
            Target.GetComponent<Vitals>().getHit(damageDealt);
            StaminaBar.Staminstance.UseStamina(120);
            //GameManager.GMinstance.ADDTOKILL(1);
        }
    }
    private void OnTriggerEnter(Collider trigger)
    {
        //Check to see if the tag on the collider is equal to Enemy
        if (trigger.tag == "enemy")
        {
            Target = trigger.gameObject;

            Debug.Log("Triggered by Enemy");
        }
        else Debug.Log("nothing triggered");
    }

    private void OnTriggerExit(Collider trigger)
    {
        //Check to see if the tag on the collider is equal to Enemy
        if (trigger.tag == "enemy")
        {
            Target = null;

        }
      
    }
    private void JumpInput()
    {
        

        // checking if the jump key has been pressed,
        if (Input.GetKeyDown(jumpKey) && !isJumping)
            {
                // if jumping then start jump event.
                isJumping = true;
                StartCoroutine(JumpEvent());
                StaminaBar.Staminstance.UseStamina(20);
                

            Debug.Log("jumping");
            }
      
    }
    // done as a coroutine.
    private IEnumerator JumpEvent()
    {
        // stting this as It would jitter when jumping next to an object.
        charController.slopeLimit = 90.0f;
        // keeps track of time in air
        float timeInAir = 0.0f;

        
        do
        {
            // animation curve falloff
            float jumpForce = jumpFallOff.Evaluate(timeInAir);
            // calling the character contoller move for jumping to apply force to jump
            charController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
            // tracking the time in the air.
            timeInAir += Time.deltaTime;
            yield return null;
            // to stop the player from clipping through the cealing 
        } while (!charController.isGrounded && charController.collisionFlags != CollisionFlags.Above);
        // stting this as It would jitter when jumping next to an object.
        charController.slopeLimit = 45.0f;
        // once the jump is completed setting isjumping to false
        isJumping = false;
    }
}
