using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoveringText : MonoBehaviour
{
    public Image img;
    public Transform target;

    // Update is called once per frame
    void Update()
    {

        Vector2 pos = Camera.main.WorldToScreenPoint(target.position);

        if (Vector3.Dot((target.position - transform.position), transform.forward) < 0)
        {
            img.enabled = false;
        }
        else
        {
            img.enabled = true;
        }

        img.transform.position = pos;
    }
}
