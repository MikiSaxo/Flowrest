using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;
    public bool IsLoading { get; set; }

    [SerializeField] private SpawnAnimButtons _mainScreen;
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private GameObject _popUpNewGame;
    
    [Header("Android")] public bool IsAndroid;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("CurrentDialogData")))
        {
            _continueButton.GetComponent<Button>().interactable = false;
            _continueButton.GetComponent<PointerMotion>().UpdateCanEnter(false);
        }
        else
        {
            _continueButton.GetComponent<Button>().interactable = true;
            _continueButton.GetComponent<PointerMotion>().UpdateCanEnter(true);
        }
           
        TransiManager.Instance.LaunchShrink();
        
        //_mainScreen.LaunchSpawnAnimDelay();
    }

    public void PopUpIfNewGame()
    {
        var hasGame = PlayerPrefs.GetString("CurrentDialogData");
        if (!string.IsNullOrEmpty(hasGame))
        {
            _popUpNewGame.SetActive(true);
            _popUpNewGame.GetComponent<PointerMotion>().Bounce();
        }
        else
        {
            LaunchNewGame();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            LaunchNewGame();
    }

    public void LaunchNewGame()
    {
        PlayerPrefs.SetString("CurrentDialogData", String.Empty);
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.SetString("Upgrades", String.Empty);
        LaunchMainScene();
    }
    public void LaunchMainScene()
    {
        AudioManager.Instance.StopMusic("MainMusic");
        AudioManager.Instance.StopMusic("MenuMusic");
        AudioManager.Instance.PlayMusicLong("MainMusic");
        
        // AudioManager.Instance.LaunchPlayLoop();
        
        StartCoroutine(WaitToLaunchMainScene());
    }

    IEnumerator WaitToLaunchMainScene()
    {
        IsLoading = true;
        TransiManager.Instance.LaunchGrownOn();
        yield return new WaitForSeconds(TransiManager.Instance.GetTimeForGrowOn());
        IsLoading = false;
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
//         //If we are running in a standalone build of the game
// #if UNITY_STANDALONE
//         //Quit the application
//         Application.Quit();
// #endif
//
//         //If we are running in the editor
// #if UNITY_EDITOR
//         //Stop playing the scene
//         UnityEditor.EditorApplication.isPlaying = false;
// #endif
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
            Application.Quit();
#elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
#endif
    }
}