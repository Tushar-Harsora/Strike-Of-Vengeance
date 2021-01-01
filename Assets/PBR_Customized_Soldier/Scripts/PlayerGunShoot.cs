using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerGunShoot : MonoBehaviour
{

    [SerializeField] Camera camera;
    public float damage = 10;
    public float nextTimeToFire = 0.2f;
    private bool CanShoot=true;

    public int fireRate = 15;
    PlayerHealth playerhealth;

    public GameObject[] impactPrefabs;

    Queue<GameObject> particles = new Queue<GameObject>(10);

    private AudioSource audio;
    public AudioClip ShootClip;

    public int BulletInMag=25;
    public int BulletRemaining=75;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        BulletInMag = 25;
        BulletRemaining = 75;
        CanShoot = true;
    }


    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && CanShoot)
        {
          //  Debug.Log(BulletInMag+" Bullet In Mag");
            if (BulletInMag>0)
            {
                Shoot();
            }
            nextTimeToFire = Time.time + 1f / fireRate;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    public void Shoot()
    {
        //if (BulletInMag<=1)
        //{
        //    if (BulletInMag<=0)
        //    {

        //    }
        //    else
        //    {
        //        BulletRemaining -= 25;
        //        BulletInMag = 25;
        //    }

        //}
        
        RaycastHit raycastHit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out raycastHit))
        {
            playerhealth = raycastHit.transform.GetComponent<PlayerHealth>();
            Name EnemyName = raycastHit.transform.GetComponent<Name>();
           
         //   Debug.Log(raycastHit.transform.tag);

            if (particles.Count >= 10)
            {
                Destroy(particles.Dequeue());
            }

            BulletInMag--;
            switch (raycastHit.transform.tag)
            {
                case "Brick":
                    Instance(0, raycastHit); break;
                case "Concrete":
                    Instance(1, raycastHit); break;
                case "Dirt":
                    Instance(2, raycastHit); break;
                case "Player":
                    break;
                case "Glass":
                    Instance(4, raycastHit); break;
                case "Metal":
                    Instance(5, raycastHit); break;
                case "Water":
                    Instance(6, raycastHit); break;
                case "Wood":
                    Instance(7, raycastHit); break;
                default:
                    break;
                    
            }

            if(EnemyName != null)
            {
                if (this.GetComponent<Name>().name != EnemyName.name)
                {
                    if (playerhealth != null)
                    {
                        audio.PlayOneShot(ShootClip);
                        Instance(3, raycastHit);
                        if (raycastHit.collider is SphereCollider)
                        {
                            playerhealth.takeDamage(damage * 5);
                            Debug.Log("HeadShot");
                        }
                        else
                        {
                            playerhealth.takeDamage(damage);
                        }

                    }
                }
                else
                {
                    BulletInMag++;
                }
            }
            else
            {
                audio.PlayOneShot(ShootClip);
            }
            
            

        }
    }

    public void Reload()
    {
        StartCoroutine(PlayReload());
        for (int i = BulletInMag; i < 25 ; i++)
        {
            if (BulletRemaining>0)
            {
                BulletRemaining--;
                BulletInMag++;
            }
            else
            {
                Debug.Log("No Ammo Left");
                break;
            }
        }
    }

    IEnumerator PlayReload()
    {
        CanShoot = false;
        yield return new WaitForSeconds(4f);
        CanShoot = true;
    }

    void Instance(int index, RaycastHit position)
    {
        particles.Enqueue(Instantiate(impactPrefabs[index], position.point, Quaternion.Euler(position.normal.x - 90, position.normal.y, position.normal.z)));
    }
}
