using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;
    
    [Header("Setup")]
    [SerializeField] private GameObject _parent;
    [SerializeField] private VideoPlayer _videoPlayer;
    
    [Header("Text")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;

    
    private bool _canOpenPopUp;
    
    private void Awake()
    {
        Instance = this;
    }

    public void InitPopUp(PopUpInfos[] filesName)
    {
        // _canOpenPopUp = false;
        //
        // if (filesName.Length <= 0) return;
        //
        // string videoPath = Path.Combine(Application.streamingAssetsPath, $"PopUpVideo/{filesName[0].VideoName}.mp4");
        //
        // if (!File.Exists(videoPath))
        // {
        //     Debug.LogError("Video PopUp Name doesn't exist");
        //     return;
        // }
        //
        // _canOpenPopUp = true;
        // _videoPlayer.url = videoPath;
        // _videoPlayer.Play();
        
        UpdatePopUp(filesName[0].Title, filesName[0].VideoName, filesName[0].Description);
    }

    public void UpdatePopUp(string title, string videoName, string description)
    {
        _titleText.text = title;
        _descriptionText.text = description;
        
        _canOpenPopUp = false;

        string videoPath = Path.Combine(Application.streamingAssetsPath, $"PopUpVideo/{videoName}.mp4");

        if (!File.Exists(videoPath))
        {
            Debug.LogError("Video PopUp Name doesn't exist");
            return;
        }

        _canOpenPopUp = true;
        _videoPlayer.url = videoPath;
        _videoPlayer.Play();
    }

    public void UpdatePopUpState(bool state)
    {
        if(_canOpenPopUp)
            _parent.SetActive(state);
    }
}
