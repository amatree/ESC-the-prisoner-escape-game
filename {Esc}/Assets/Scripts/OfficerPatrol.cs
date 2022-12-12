using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OfficerPatrol : MonoBehaviour
{
    public Transform[] Checkpoints;

   private NavMeshAgent agent;

    public Transform player;

    public Transform MainTarget;

    //Variables 

    // int currentCheckpoint;

    private int currentCheckpoint = 0;

    public float OfficerSpeed = 10f; // office speed can change in unity 

    public float awareAI;   // have to set this to a ditance to start the officer to follow but changes must set it to change with vector movment 

    public float playerDistance; // constantly changing 

    public float damping = 6.0f;

   //public UnityEngine.AI.NavMeshAgent agent;

    public float distance = 5;



   

    // Start is called before the first frame update
    void Start()
    {

        //UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent = GetComponent<NavMeshAgent>(); // new

        agent.destination = MainTarget.position;

       agent.autoBraking = false;

        //This check point is were officer starts.
        // currentCheckpoint = 0;

        Patrol();

    }

    // Update is called once per frame
    void Update()
    {

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Patrol();
        }

        playerDistance = Vector3.Distance(player.position, transform.position);

        //Patrol();

        if (playerDistance <= awareAI)
        {
            LookAtPlayer();
            Debug.Log("Seen");

            Chase();

        }
       
    }

    void Patrol()
    {
        /*
         if (transform.position != Checkpoints[currentCheckpoint].position)
         {
             transform.position = Vector3.MoveTowards(transform.position, Checkpoints[currentCheckpoint].position, OfficerSpeed * Time.deltaTime);
         }
         else
         {
             currentCheckpoint = (currentCheckpoint + 1) % Checkpoints.Length;
         }
        */
        // Returns if no points have been set up
        if (Checkpoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = Checkpoints[currentCheckpoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        currentCheckpoint = (currentCheckpoint + 1) % Checkpoints.Length;


    }

    void LookAtPlayer() 
    {
        transform.LookAt(player);
    }
    void Chase() 
    {
        transform.Translate(Vector3.forward * OfficerSpeed * Time.deltaTime);
    }
}

