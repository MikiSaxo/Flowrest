using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        StartCoroutine(WaitGoToMainMenu());
    }

    IEnumerator WaitGoToMainMenu()
    {
        TransiManager.Instance.LaunchGrownOn();
        yield return new WaitForSeconds(TransiManager.Instance.GetTimeForGrowOn());
        AudioManager.Instance.StopMusic("MenuMusic");
        AudioManager.Instance.StopMusic("MainMusic");
        SceneManager.LoadScene(0);
    }
}
