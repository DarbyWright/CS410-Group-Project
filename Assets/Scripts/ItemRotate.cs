using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRotate : MonoBehaviour {

    public float speed = 2.0f;
    public int axis    = 1;

    // Update is called once per frame
    void Update() {
        // Rotate the object by 45 degrees across the axis per second
        if (axis == 0)
            transform.Rotate(new Vector3 (45, 0, 0) * (speed * Time.deltaTime));
        if (axis == 1)
            transform.Rotate(new Vector3 (0, 45, 0) * (speed * Time.deltaTime));
        if (axis == 2)
            transform.Rotate(new Vector3 (0, 0, 45) * (speed * Time.deltaTime));
    }
}
