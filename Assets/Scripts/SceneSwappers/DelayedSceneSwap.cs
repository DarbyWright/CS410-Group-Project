using UnityEngine;
using UnityEngine.SceneManagement;

public class DelayedSceneSwap : MonoBehaviour
{
    [SerializeField]
    private float delayBeforeLoading = 10f;

    private float timeElapsed;


    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > delayBeforeLoading)
        {
            SceneManager.LoadScene(0);
        }
    }
}
