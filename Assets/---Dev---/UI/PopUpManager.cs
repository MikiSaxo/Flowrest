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

    private void Awake()
    {
        Instance = this;
    }

    public void InitPopUp(string fileName)
    {
        // GetComponent<LegendScroll>().InitLegend(sprites);
        if (fileName == String.Empty) return;
        
        string videoPath = Path.Combine(Application.streamingAssetsPath, $"PopUpVideo/{fileName}.mp4");
        
        _videoPlayer.url = videoPath;
        _videoPlayer.Play();
    }


    public void UpdatePopUp(bool state)
    {
        _parent.SetActive(state);
    }
}
