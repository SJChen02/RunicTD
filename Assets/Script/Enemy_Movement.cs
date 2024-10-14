using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb; //a reference to RigidBody

    [Header("Attributes")]
    [SerializeField] private float moveSpeed;  //read move speed of a object from unity (if we want standard move speed from another script then change this).


    private Transform target;
    private int pathIndex =  0;

    private void Start()
    {
        target = PathManager.main.path[pathIndex]; //this is for enemy object to read through the array of waypoint to move to
    }

    private void Update()
    {
        if (Vector3.Distance(target.position, transform.position) <= 0.1f)
        {
            pathIndex++;
           

            if(pathIndex == PathManager.main.path.Length)
            {
                Destroy(gameObject);
                return;
            } 
            else
            {
                target = PathManager.main.path[pathIndex];
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 direction = (target.position - transform.position);

        

        Vector3 movement = direction.normalized * moveSpeed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }
}