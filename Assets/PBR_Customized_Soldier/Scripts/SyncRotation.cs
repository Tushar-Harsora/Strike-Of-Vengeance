using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncRotation : MonoBehaviour
{

    public Transform target;
    public float speedRotation = 2f;

    // Update is called once per frame
    void Update()
    {
        //Quaternion newrot = target.rotation * transform.rotation ;
        transform.rotation= Quaternion.Slerp(transform.rotation,target.rotation,Time.deltaTime*speedRotation);
    }
}
