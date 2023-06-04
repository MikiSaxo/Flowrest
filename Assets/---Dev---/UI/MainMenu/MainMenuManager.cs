using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;
    public bool IsLoading { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        TransiManager.Instance.LaunchShrink();
    }

    public void LaunchMainScene()
    {
        AudioManager.Instance.StopMusic("MainMusic");
        AudioManager.Instance.StopMusic("MenuMusic");
        AudioManager.Instance.PlayMusic("MainMusic");
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