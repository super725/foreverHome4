using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    // Code adapted from Comp-3 Interactive channel on youtube

    public float radius;
    [Range(0, 360)]
    public float angle;
    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

<<<<<<< HEAD
    void Start()
=======
    private void Awake()
    {
        LevelGeneration.OnReady += OnMapReady;
    }

    void OnMapReady()
>>>>>>> origin/RayBranch
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        if (playerRef == null)
        {
            Debug.LogError("Could not find GameObject with tag 'Player'");
        }
        StartCoroutine(FOVRoutine());
    }

    //Every 2 milliseconds, check if player is within LOS. Saves on performance. 
    private IEnumerator FOVRoutine()
    {
<<<<<<< HEAD
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while(true)
        {
            yield return wait;
            FieldOfViewCheck();
=======
        //float delay = 0.2f;
        //WaitForSeconds wait = new WaitForSeconds(delay);

        while(true)
        {
            //yield return wait;
            FieldOfViewCheck();
            yield return new WaitForSeconds(0.2f);
>>>>>>> origin/RayBranch
        }
    }

    private void FieldOfViewCheck()
    {
        //This is whats actually looking for player
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        if(rangeChecks.Length != 0){
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if(Vector3.Angle(transform.forward, directionToTarget) < angle/2)
            {
                float distanceToTarget =Vector3.Distance(transform.position, target.position);
            
                if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask)){
                    canSeePlayer = true;
                }else{
                    canSeePlayer = false;
                }
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer){
            canSeePlayer = false;
        }
    }
}
