using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    public CanvasGroup canvas;
    public bool fadein;
    public bool fadeout;

    public float timeToFade;
    

    // Update is called once per frame
    void Update()
    {
        if(fadein == true)
        {
            if(canvas.alpha < 1)
            {
                canvas.alpha += timeToFade * Time.deltaTime;
                if(canvas.alpha > 1)
                {
                    fadein = false;
                }
            }
        }


        if(fadeout == true)
        {
            if(canvas.alpha >= 0)
            {
                canvas.alpha -= timeToFade * Time.deltaTime;
                if(canvas.alpha == 0)
                {
                    fadeout = false;
                }
            }
        }

    }


    public void FadeIn()
    {
        fadein = true;
    }

    public void FadeOut()
    {
        fadeout = true;
    }

}
