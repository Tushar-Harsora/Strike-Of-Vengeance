using UnityEngine;

public class GunPositionSync : MonoBehaviour
{
    //[SerializeField] Transform cameraTransform;
    //[SerializeField] Transform handMount;
    //[SerializeField] Transform gunPivot;
    [SerializeField] Transform rightHandHold;
    [SerializeField] Transform leftHandHold;
    //[SerializeField] float threshold = 10f;
    //[SerializeField] float smoothing = 5f;

    float pitch;
    Vector3 lastOffset;
    float lastSyncedPitch;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        
        //lastOffset = handMount.position - transform.position;
    }

    void OnAnimatorIK()
    {
        if (!anim)
        {
            Debug.Log("No Animator Found");
            return;
        }

        
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
        anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandHold.position);
        anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandHold.rotation);
        //Debug.Log("Position IK");

        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandHold.position);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandHold.rotation);
        //Debug.Log("Rotation IK");
    }
}
