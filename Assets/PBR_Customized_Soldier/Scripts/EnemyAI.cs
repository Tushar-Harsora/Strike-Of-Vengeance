using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent),typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    public Transform target;

    public float MinDistance = 3f;
    public float AttackDistance = 20f;
    public float FollowDistance = 20f;
    public string TargetIdentity;

    public float AttackProbability = 0.5f;
    public float HitAccuracy = 0.5f;
    public AudioClip[] m_FootstepSounds;
    private AudioSource m_AudioSource;
    public float turnSpeed = 100f;

    private NavMeshAgent nav;
    private Animator anim;

    private BotGunShoot gunShoot;

    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        gunShoot = GetComponent<BotGunShoot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;
        
        if (TargetIdentity == "human")
        {
            float distance = Vector3.Distance(target.transform.position, transform.position);
            bool shoot = false;
            bool follow = distance < FollowDistance;
            bool stayFollow = distance < MinDistance;

            if (follow)
            {
                float random = Random.Range(0f, 1f);

                if (random > (1f - AttackProbability) && distance < AttackDistance)
                {
                    shoot = true;
                }
            }
            nav.SetDestination(target.transform.position);
            //PlayFootStepAudio();
            // nav.speed = 10.0f;
            anim.SetFloat("Forward", 1);


            if (stayFollow)
            {
                nav.SetDestination(transform.position);

                anim.SetFloat("Forward", 0);
                anim.SetFloat("Strafe", 0);
            }
            if (stayFollow)
            {
                float random = Random.Range(0f, 1f);

                if (random > (1f - AttackProbability))
                {
                shoot = true;
                    
                transform.LookAt(target.transform);
                    
                //Debug.Log(target.transform.position+"lookat");
                }
            }
            if (shoot)
            {
                gunShoot.Shoot();
            }
          //  Debug.Log("human");
        }
        if (TargetIdentity == "waypoint")
        {
            nav.SetDestination(target.position);
            nav.autoRepath = true;
            //Debug.Log("waypoint");
            anim.SetFloat("Forward", 1);
        }
        
    }
    private void PlayFootStepAudio()
    {
        
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, m_FootstepSounds.Length);
        m_AudioSource.clip = m_FootstepSounds[n];
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        m_FootstepSounds[n] = m_FootstepSounds[0];
        m_FootstepSounds[0] = m_AudioSource.clip;
    }
}
