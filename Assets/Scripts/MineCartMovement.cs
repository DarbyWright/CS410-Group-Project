using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class MineCartMovement : MonoBehaviour
{
    private bool movingLeft;
    public float speed = 5f;
    public float MaxLeft = -25f;
    public float MaxRight = 28f;

    void Start()
    {
        movingLeft = true;
    }

    void Update()
    {
        if (movingLeft)
        {
            transform.position += (Vector3.back * Time.deltaTime * speed);

            if (transform.position.z <= MaxLeft)
            {
                movingLeft = false;
            }
        }
        else
        {
            transform.position += (Vector3.forward * Time.deltaTime * speed);

            if (transform.position.z >= MaxRight)
            {
                movingLeft = true;
            }
        }
    }

}
