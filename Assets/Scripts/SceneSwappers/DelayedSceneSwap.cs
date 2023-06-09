using UnityEngine;
using UnityEngine.SceneManagement;

public class DelayedSceneSwap : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField]
    private float delayBeforeLoading = 10f;

    private float timeElapsed;


    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > delayBeforeLoading)
        {
            Application.Quit();
        }
    }
}
