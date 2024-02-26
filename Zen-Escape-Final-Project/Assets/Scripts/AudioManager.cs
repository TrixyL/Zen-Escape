using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource sfx;

    public AudioClip bgmBreakfastinParis;
    public AudioClip bgmColorfulFlowers;
    public AudioClip bgmCozyPlace;
    public AudioClip bgmMeadows;

    public AudioClip sfxClick;
    public AudioClip sfxCollect;
    public AudioClip sfxPaper;
    public AudioClip sfxCeramicBreak;
    public AudioClip sfxChangeBulb;
    public AudioClip sfxBeep;
    public AudioClip sfxSafeUnlock;
    public AudioClip sfxDoorLocked;
    public AudioClip sfxDoorUnlock;
    public AudioClip sfxEscapeSuccess;

    public GameObject audioSettingsUI;
    public Slider bgmSlider;
    public Slider sfxSlider;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // called first
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

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("OnSceneLoaded: " + scene.name);

        AudioClip currentBgm = bgm.clip;

        if (SceneManager.GetActiveScene().name == "EscapeRoom2")
        {
            bgm.clip = bgmColorfulFlowers;
        }
        else
        {
            bgm.clip = bgmBreakfastinParis;
        }

        if (currentBgm != bgm.clip)
        {
            bgm.Play();
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        if (bgmSlider)
        {
            bgmSlider.value = PlayerPrefs.GetFloat("BgmVolume", 1);
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1);
        }

        bgm.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && SceneManager.GetActiveScene().name != "EscapeRoom2")
        {
            PlaySFX(sfxClick);
        }
    }

    public void PlaySFX(AudioClip sound)
    {
        sfx.clip = sound;
        sfx.Play();
    }

    public void UpdateBgmVolume(float vol)
    {
        bgm.volume = vol;
    }
    public void UpdateSFXVolume(float vol)
    {
        sfx.volume = vol;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("BgmVolume", bgm.volume);
        PlayerPrefs.SetFloat("SFXVolume", sfx.volume);

        audioSettingsUI.SetActive(false);
        Time.timeScale = 1;
    }
}
