using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject vidasUI;
    public GameObject panelGameOver;
    public GameObject panelMain;
    public GameObject panelLoad;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void GameOver()
    {
        panelGameOver.SetActive(true);
    }

    public void MainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void InGame()
    {
        SceneManager.LoadScene("InGame");
    }

    public void Load()
    {
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        panelLoad.SetActive(true);

        AsyncOperation asynload = SceneManager.LoadSceneAsync("InGame");
        while (!asynload.isDone)
        {
            yield return null;
        }
        
    }
}
