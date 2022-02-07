using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string sceneToLoad;
    public int timeSec;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(timeSec);
        SceneManager.LoadScene(sceneToLoad);
    }
}
