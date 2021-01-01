using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.AI;


[RequireComponent(typeof(Collider))]
public class PlayerHealth : MonoBehaviour
{
    public float health = 100;
    private Animator animator;
    public AudioSource audio;
    public AudioClip DeathClip;
    public GameObject gameobject;

    private StartRound sr;

    void Start()
    {
        animator = GetComponent<Animator>();
        sr = FindObjectOfType<StartRound>();
    }

    public void takeDamage(float damage)
    {
        if (damage<=0)
        {
            throw new UnityException("Damage Cannot be 0 Or Negative");
        }

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
       // Debug.Log(this.transform.name + " Is Dying" + GetComponent<Name>().name);
        
        sr.DieTrigger(GetComponent<Name>().name,this.transform);

        //if (ga)
        //{

        //}
        //Instantiate(gameobject, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
        //StartCoroutine("pleaseDie");
        
    }
    /*
    IEnumerator pleaseDie()
    {
        Debug.Log("Inside IEnumerator");
        audio.clip = DeathClip;
        audio.Play();

        animator.SetTrigger("Died");

        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
    */
}
