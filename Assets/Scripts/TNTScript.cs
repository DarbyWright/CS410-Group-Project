using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TNTScript : MonoBehaviour
{
    GameManager gameManager;
    public ParticleSystem particles;
    public Canvas canvas;

    AudioManager audioManager;
    bool lookForInput = false;
    
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        audioManager = FindAnyObjectByType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(lookForInput && Input.GetKeyDown(KeyCode.E))
        {
            canvas.gameObject.SetActive(false);
            particles.gameObject.SetActive(true);
            audioManager.PlaySFX("SFX_DynamiteExplode");
            StartCoroutine(WaitForExplosion());
        }
    }

    public IEnumerator WaitForExplosion()
    {
        yield return new WaitForSeconds(0.25f);
        gameManager.TNTExploded = true;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        canvas.gameObject.SetActive(true);
        lookForInput = true;
    }

    private void OnTriggerExit(Collider other)
    {
        canvas.gameObject.SetActive(false);
        lookForInput = false;
    }

}
