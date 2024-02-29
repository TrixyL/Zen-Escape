using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    Animator transitionsAnimator;
    public RuntimeAnimatorController toLake;
    public RuntimeAnimatorController Laketo;
    public RuntimeAnimatorController toSky;
    public RuntimeAnimatorController Skyto;

    public GameObject creditsUI;
    public GameObject helpUI;
    public Animator hintsUI;

    //bool isFrom = false;

    AudioManager am;

    void OnEnable()
    {
        //Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called when the game is terminated
    void OnDisable()
    {
        //Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("OnSceneLoaded: " + scene.name);
        if (GameObject.FindWithTag("Animator"))
        {
            transitionsAnimator = GameObject.FindWithTag("Animator").GetComponent<Animator>();
        }

        //if (isFrom == true)
        //{
            

        //    isFrom = false;
        //}

        if (scene.name == "MainMenu")
        {
            transitionsAnimator.runtimeAnimatorController = toLake;
        }
        if (scene.name == "Levels")
        {
            transitionsAnimator.runtimeAnimatorController = toSky;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        am = AudioManager.instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LevelsToMenu()
    {
        StartCoroutine(MenuTransition());
    }

    public void MenuToLevels()
    {
        StartCoroutine(LevelsTransition());
    }
    public void GameToLevels()
    {
        SceneManager.LoadScene("Levels");
    }

    public void GameScene()
    {
        SceneManager.LoadScene("EscapeRoom2");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator MenuTransition()
    {
        transitionsAnimator.runtimeAnimatorController = Skyto;
        transitionsAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(0.9f);

        //isFrom = true;

        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator LevelsTransition()
    {
        transitionsAnimator.runtimeAnimatorController = Laketo;
        transitionsAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(0.9f);

        //isFrom = true;

        SceneManager.LoadScene("Levels");
    }

    public void VolumeSettings()
    {
        am.audioSettingsUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void ShowCredits()
    {
        creditsUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void HideCredits()
    {
        creditsUI.SetActive(false);
        Time.timeScale = 1;
    }

    public void ShowHelp()
    {
        helpUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void HideHelp()
    {
        helpUI.SetActive(false);
        Time.timeScale = 1;
    }

    public void OpenHints()
    {
        hintsUI.SetTrigger("toggle");
        //Time.timeScale = 0;
    }

    public void CloseHints()
    {
        hintsUI.SetTrigger("toggle");
        Time.timeScale = 1;
    }
}
