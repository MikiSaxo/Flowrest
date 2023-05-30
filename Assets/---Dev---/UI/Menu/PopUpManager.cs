using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;
    
    [SerializeField] private GameObject _parent;
    [SerializeField] private Image _image;
    [SerializeField] private VideoPlayer _videoPlayer;

    private bool _canOpenPopUp;
    
    private void Awake()
    {
        Instance = this;
    }

    public void InitPopUp(string fileName)
    {
        _canOpenPopUp = false;

        if (fileName == String.Empty) return;
        
        string videoPath = Path.Combine(Application.streamingAssetsPath, $"PopUpVideo/{fileName}.mp4");

        if (!File.Exists(videoPath))
        {
            Debug.LogError("Video PopUp Name doesn't exist");
            return;
        }

        _canOpenPopUp = true;
        _videoPlayer.url = videoPath;
        _videoPlayer.Play();
    }


    public void UpdatePopUp(bool state)
    {
        if(_canOpenPopUp)
            _parent.SetActive(state);
    }
}
