using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneFade : MonoBehaviour
{
    FadeInOut fade;
    // Start is called before the first frame update
    void Start()
    {
        fade = FindAnyObjectByType<FadeInOut>();
        StartCoroutine(LoadDelay());
    }

    public IEnumerator LoadDelay()
    {
        yield return new WaitForSeconds(0.5f);
        fade.FadeOut();
    }

}
