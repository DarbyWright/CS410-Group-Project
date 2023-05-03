using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehavior : MonoBehaviour
{
    public float lifeTime = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterTime());
    }

    IEnumerator DestroyAfterTime() { 
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
    
}
