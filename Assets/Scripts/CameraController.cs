using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject player;

    private Vector3 offset;

    public bool RotateAroundPlayer = true;

    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;

    public bool LookAtPlayer = false;

    public float RotationSpeed = 5.0f;

    // Start is called before the first frame update
    void Start() {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate() 
    {
        if(RotateAroundPlayer) 
        {
            Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * RotationSpeed, Vector3.up);


            offset = camTurnAngle * camTurnAngle * offset;
        }

        Vector3 newPos = player.transform.position + offset;

        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);

        if (LookAtPlayer || RotateAroundPlayer)
            transform.LookAt(player.transform);

        // transform.position = player.transform.position + offset;
    }
}
