using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;//for the navmesh

public class EnemyAI : MonoBehaviour
{
    GameObject curTarget;
    Transform myTransform;
    Vector3 patrolSpawnPosition;
    Vitals myVitals;
    private NavMeshAgent Mob;
    PlayerVitals playerVitals;
    NavMeshAgent nav;

    //patrol variables
    public GameObject moveSpot;
    private float waitTime;
    public float startWaitTime;
    public float minX, maxX, minY, maxY, minZ, maxZ;

    //staying on the ground variabls
    private float currentHight;

    //finding the right movespot
    public GameObject bestMoveSpot;

    //spawing the patrol point
    public GameObject patrolPoint;

    public AudioSource walkSound;
    public AudioSource gunShot;
  


    Animator anim;


    [SerializeField] float minAttackDistance = 3, maxAttackDistance = 15, moveSpeed = 15, patrolSpeed = 5;

    [SerializeField] float fireCooldown = 1F;
    [SerializeField] float damageDealt = 20F;
    float curFireCooldown = 0, distanceToTarget;

    public float sightRange;
    public float viewingAngle;

    Vector3 targetLastKnowLocation;
    Path currentPath = null;

    public enum ai_states
    {
        idle,
        move,
        combat,
        investigate,
        patrol
    }
    public ai_states state = ai_states.idle;

    // Start is called before the first frame update
    void Awake()
    {
        myTransform = transform;
        patrolSpawnPosition = transform.position;
        Instantiate(patrolPoint, patrolSpawnPosition,Quaternion.identity);
        curTarget = GameObject.FindGameObjectWithTag("Player");
        moveSpot = GameObject.FindGameObjectWithTag("MoveSpot");
                
        myVitals = GetComponent<Vitals>();
        playerVitals = GetComponent<PlayerVitals>();
        anim = GetComponent<Animator>();
        waitTime = startWaitTime;
        nav = GetComponent<NavMeshAgent>();
        moveSpot = GetBestMoveSpot();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(myVitals.getcCurHealth() > 0)
        {
            switch(state)
            {
                case ai_states.idle:
                    stateIdle();
                    break;
                case ai_states.move:
                    stateMove();
                    break;
                case ai_states.combat:
                    stateCombat();
                    break;
                case ai_states.investigate:
                    stateInvestigate();
                    break;
                case ai_states.patrol:
                    statePatrol();
                    break;
                default:
                    break;
             
            }
        }
        else
        {
            anim.SetBool("move", false);
            // to be able to investing last knows position we implement dead, instead of destroying the target
            if(GetComponent<BoxCollider>() != null)
            {
                Destroy(GetComponent<BoxCollider>());
            }
            nav.enabled = false;
            Quaternion deathRotation = Quaternion.Euler(-90, myTransform.rotation.eulerAngles.y, myTransform.rotation.eulerAngles.z);
            if(myTransform.rotation != deathRotation)
            {
                myTransform.rotation = deathRotation;
                GameManager.GMinstance.ADDTOKILL(1);

            }
            
        }
        
    }

    void stateIdle()
    {
        Debug.Log("Entered idel");
       // Debug.Log(Vector3.Distance(myTransform.position, curTarget.transform.position));
        if (curTarget != null && curTarget.GetComponent<PlayerVitals>().getcCurHealth() > 0 && CanSeeTarget() == true)
        {
            
            if (Vector3.Distance(myTransform.position,curTarget.transform.position) <= maxAttackDistance && Vector3.Distance(myTransform.position, curTarget.transform.position) >= minAttackDistance && CanSeeTarget() == true)
            {
                //Debug.Log(Vector3.Distance(myTransform.position, curTarget.transform.position));
                //attack
                state = ai_states.combat;
            }
            else
            {
                anim.SetBool("move", true);
                //move
                state = ai_states.move;
            }
        }
        else
        {
            Debug.Log("moving to Idel");
            anim.SetBool("move", true);
            state = ai_states.patrol;
        }
       
    }
    void stateMove()
    {
        walkSound.Play();

        if (curTarget != null && curTarget.GetComponent<PlayerVitals>().getcCurHealth() > 0 && CanSeeTarget()==true)
        {
            if (!CanSeeTarget())
            {
                //If I can't see the target anymore, Ill need to investigate last know position

                targetLastKnowLocation = curTarget.transform.position;

                //we need to calculate a path towards the targets last know position and we do so using the Unity NavMesh combined with some custom code

                currentPath = CalculatePath(myTransform.position, targetLastKnowLocation);
                state = ai_states.investigate;

                return;
            }

            currentHight = this.transform.position.y;

            myTransform.LookAt(curTarget.transform);
            if (Vector3.Distance(myTransform.position, curTarget.transform.position) > maxAttackDistance)
            {
                Debug.Log("Moving closer to target");
                //move clsoer to target
                myTransform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                transform.position = new Vector3(myTransform.position.x, currentHight, transform.position.z);
                //transform.position = Vector3.MoveTowards(transform.position, new Vector3(curTarget.transform.position.x, transform.position.y, curTarget.transform.position.z), moveSpeed);

            }
            else if (Vector3.Distance(myTransform.position, curTarget.transform.position) < minAttackDistance)
            {
                Debug.Log("Moving away from target");
                //move away from target
                
                myTransform.Translate(Vector3.forward * -1 * moveSpeed * Time.deltaTime);
                transform.position = new Vector3(myTransform.position.x, currentHight, transform.position.z);

                //transform.position = Vector3.MoveTowards(transform.position, new Vector3(curTarget.transform.position.x, transform.position.y, curTarget.transform.position.z), moveSpeed);
            }
            else
            {
                anim.SetBool("move", false);
                //attack
                state = ai_states.combat;
            }

        }
        else
        {
            anim.SetBool("move", true);
            state = ai_states.patrol;
        }
    }
    void stateCombat()
    {
        if (!CanSeeTarget()) //if the target escapes while in combat
        {
            //If I can't see the target anymore, Ill need to investigate last know position

            targetLastKnowLocation = curTarget.transform.position;

            //we need to calculate a path towards the targets last know position and we do so using the Unity NavMesh combined with some custom code

            currentPath = CalculatePath(myTransform.position, targetLastKnowLocation);
            anim.SetBool("move", true);
            state = ai_states.investigate;

            return;
        }

        Debug.Log("Attacking");
        if (curTarget != null && curTarget.GetComponent<PlayerVitals>().getcCurHealth() > 0 && CanSeeTarget()==true)
        {

            myTransform.LookAt(curTarget.transform);

            if (Vector3.Distance(myTransform.position, curTarget.transform.position) <= maxAttackDistance && Vector3.Distance(myTransform.position, curTarget.transform.position) >= minAttackDistance)
            {
                //attack
                if(curFireCooldown <= 0)
                {
                    anim.SetTrigger("fire");
                    gunShot.Play();

                    curTarget.GetComponent<PlayerVitals>().getHit(damageDealt);
                    curFireCooldown = fireCooldown;
                }
                else
                {
                    curFireCooldown -= 1 * Time.deltaTime;
                }
            }
            else
            {
                anim.SetBool("move", true);
                //move
                state = ai_states.move;
            }

        }
        else
        {
            //if we killed the target. we investigate its position to see if there is any other targets or to make sure we killed it

            if (curTarget != null && curTarget.GetComponent<Vitals>().getcCurHealth() <= 0)
            {
                //If I can't see the target anymore, Ill need to investigate last know position
                targetLastKnowLocation = curTarget.transform.position;
                //we need to calculate a path towards the targets last know position and we do so using the Unity NavMesh combined with some custom code

                currentPath = CalculatePath(myTransform.position, targetLastKnowLocation);
                anim.SetBool("move", true);
                state = ai_states.investigate;
            }
            else
            {
                state = ai_states.idle;
            }       
           
        }
    }
   
    void stateInvestigate()
    {
        walkSound.Play();
        if(currentPath != null)
        {

            if (currentPath.reachedEndNode())
            {//if we reached the end we start looking for a target agin in idle
                anim.SetBool("move", false);

                currentPath = null;
                //curTarget = null;

                state = ai_states.idle;
                return;
            }

            Vector3 nodePosition = currentPath.GetNextNode();

            if (Vector3.Distance(myTransform.position, nodePosition) < 1)
            {
                //if we reached the current node then we begin going towards the next node
                currentPath.currentPathIndex++;
            }
            else
            {
                //else we move towards the current node
                myTransform.LookAt(nodePosition);
                myTransform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            }
        }
        else
        {
            //if we dont have a path we look for a target
            anim.SetBool("move", false);

            currentPath = null;
            //curTarget = null;

            state = ai_states.idle;
        }
    }

    void statePatrol()
    {
      
        Debug.Log("Patrol entered");
       
        if(curTarget != null && curTarget.GetComponent<PlayerVitals>().getcCurHealth() > 0 && CanSeeTarget() == false)
        {
            nav.enabled = true;
            nav.SetDestination(moveSpot.transform.position);
            if (Vector3.Distance(transform.position, moveSpot.transform.position) < 3f)
            {
                if (waitTime <= 0)
                {
                    
                    anim.SetBool("move", true);
                    walkSound.Play();
                    moveSpot.transform.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
                    waitTime = startWaitTime;
                }
                else
                {
                    waitTime -= Time.deltaTime;
                    anim.SetBool("move", false);
                }
            }
        }
        else
        {
            //moveSpot = myTransform.position;
            state = ai_states.move;
            nav.enabled = false;
        }

        

       
    }

    GameObject GetBestMoveSpot()
    {
        
        moveSpot = GameObject.FindGameObjectWithTag("MoveSpot");
        GameObject[] allMoveSpots = GameObject.FindGameObjectsWithTag("MoveSpot");
        

        bestMoveSpot = moveSpot;
        for (int i = 0; i < allMoveSpots.Length; i++)
        {
            moveSpot = allMoveSpots[i];
            Debug.Log(moveSpot);


            
            
            if (Vector3.Distance(moveSpot.transform.position, myTransform.position) < Vector3.Distance(bestMoveSpot.transform.position, myTransform.position))
            {
                bestMoveSpot = moveSpot;
            }



        }
        return bestMoveSpot;
    }


    // Is the target visible
    public bool CanSeeTarget()
    {
        // Check target is alive
        if (curTarget != null)
        {
            // Get distance to target
            distanceToTarget = Vector3.Distance(curTarget.transform.position, transform.position);
            // Check in visible range
            if (sightRange > distanceToTarget)
            {

                //Debug.Log(distanceToTarget);

                
                // Direction of target from agent
                Vector3 targetDirection = curTarget.transform.position - transform.position;
                // Angle between agent and Target
                float angle = Vector3.Angle(targetDirection, transform.forward);
                // Convert to positive value
                angle = System.Math.Abs(angle);
                // Is the target within the viewing angle. Ignores obstacles 
                if (angle < (viewingAngle / 2))
                {
                    Vector3 myPosition = myTransform.position;
                    myPosition.y = myTransform.position.y + 0.5F; //raycast comeing from the center of the body

                    Vector3 enemyPosition = curTarget.transform.position;
                    enemyPosition.y = curTarget.transform.position.y + 0.5F; //raycast going towards the enemy/player

                    Vector3 directionTowardsEnemy = enemyPosition - myPosition;

                    RaycastHit hit; //record of what we hit with racast

                    //cast ray towards current target, make the racast line infinity in length 
                    if (Physics.Raycast(myPosition,directionTowardsEnemy,out hit, Mathf.Infinity))
                    {
                        //if the ray hit the target, then we know that we can see it
                        if (hit.transform == curTarget.transform)
                        {
                            Debug.Log("Target Seen");

                            return true;
                        }
                      
                    }
                   

                }
            }
        }
        Debug.Log("Target not Seen");
        return false;
    }

    Path CalculatePath(Vector3 source, Vector3 destination)
    {
        NavMeshPath nvPath = new NavMeshPath();
        NavMesh.CalculatePath(source, destination, NavMesh.AllAreas, nvPath); //calculates a path using unity navmesh

        Path path = new Path(nvPath.corners);

        return path;

    }

}
