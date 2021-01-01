using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOnAwake : MonoBehaviour
{
    public Animator animator;
    void Start()
    {
        Debug.Log("DieOnAwake Function");
        animator.SetTrigger("Died");
    }
}
