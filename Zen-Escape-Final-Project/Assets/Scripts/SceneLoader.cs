using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    //AudioManager am;

    // Start is called before the first frame update
    void Start()
    {
        //am = AudioManager.instance;

        //if (SceneManager.GetActiveScene().name == "EscapeRoom2")
        //{
        //    am.bg
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            MainMenuScene();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LevelsScene();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameScene();
        }
    }

    public void MainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LevelsScene()
    {
        SceneManager.LoadScene("Levels");
    }

    public void GameScene()
    {
        SceneManager.LoadScene("EscapeRoom2");
    }
}
