using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject target;

    private Vector3 offset;

    public bool RotateAroundPlayer = true;

    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;

    public bool LookAtPlayer = false;

    public float RotationSpeed = 5.0f;

    // Start is called before the first frame update
    void Start() {
        offset = transform.position - target.transform.position;
    }

    // Update is called once per frame
    void LateUpdate() 
    {
        if(RotateAroundPlayer) 
        {
            Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * RotationSpeed, Vector3.up);

            offset = camTurnAngle * camTurnAngle * offset;
        }

        Vector3 newPos = target.transform.position + offset;

        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);

        if (LookAtPlayer || RotateAroundPlayer)
        {
            transform.LookAt(target.transform);
        }
    }

// Attempt at a more robust camera rig setup. Might continue to try to get it working

//     private Transform Camera;
//     public GameObject Target;

//     private Vector3 localRotation;
//     private float cameraDistance = 10f;

//     public float mouseSensitivity = 4f;
//     public float scrollSensitivity = 2f;
//     public float orbitDampening = 10f;
//     public float scrollDampening = 6f;
//     public bool cameraDisabled = false;


//     void start()
//     {
//         // this.CameraTransform = this.transform;
//         // this.TargetTransform = this.transform.parent;

//     }

//     void LateUpdate()
//     {
//         if (!cameraDisabled)
//         {
//             // Rotation of the camera based on mouse coords.
//             if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
//             {
//                 localRotation.x = Input.GetAxis("Mouse X") * mouseSensitivity;
//                 localRotation.y -= Input.GetAxis("Mouse Y") * mouseSensitivity;

//                 // Clamp the y rotation to horizon and not flipping over at the top
//                 localRotation.y = Mathf.Clamp(localRotation.y, 0, 90);

//             }

//             // Zooming input from our mouse scroll wheel
//             if (Input.GetAxis("Mouse ScrollWheel") != 0f)
//             {
//                 float scrollAmount = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;

//                 // Makes Camera zoom faster the further away it is from the targewt
//                 scrollAmount *= (this.cameraDistance * 0.3f);

//                 this.cameraDistance += scrollAmount * -1f;

//                 // This makes camera go no closer than 1.5 meters and no further away then 100 meters from target
//                 this.cameraDistance = Mathf.Clamp(this.cameraDistance, 1.5f, 100f);
//             }
//         }

//         // Actual Camera Rig Transformations
//         Quaternion QT = Quaternion.Euler(localRotation.y, localRotation.x, 0);
//         Target.transform.rotation = Quaternion.Lerp(Target.transform.rotation, QT, Time.deltaTime * orbitDampening);

//         if (Camera.localPosition.z != this.cameraDistance * -1f)
//         {
//            Camera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(Camera.localPosition.z, this.cameraDistance * -1f, Time.deltaTime * scrollDampening));
//         }
//     }
}   