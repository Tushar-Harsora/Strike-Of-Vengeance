using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Test : MonoBehaviour
{
    //public GameObject go;
    //private NavMeshAgent nma;

    // Start is called before the first frame update
    void Start()
    {
        //nma=go.GetComponent<NavMeshAgent>();
        //go.GetComponent<NavMeshAgent>().SetDestination(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.X))
        {
            //Debug.Log("Test1");
            //Time.timeScale = 0f;
            StartCoroutine("SS");
        }
    }

    IEnumerator SS()
    {
        ScreenCapture.CaptureScreenshot("StrikeOfVengeance.jpeg");
        yield return null;
    }
}
