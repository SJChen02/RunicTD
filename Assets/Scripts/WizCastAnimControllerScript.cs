using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WizCastAnimControllerScript : MonoBehaviour
{

    private Animator animator;
    private Tower variableScript;

    void Start()
    {
        animator = GetComponent<Animator>();
        variableScript = GetComponent<Tower>(); // Or find it another way
    }

    void Update()
    {
        if (variableScript.CastCheck != null)
        {
            animator.enabled = true;
            animator.SetBool("IsActive", true);
        }
        // If no target, disable animator to return to initial pose
        else
        {
            animator.Play("WizardCast", 0, 0f);
            animator.SetBool("IsActive", false);
            animator.enabled = false;
        }
    }
}
