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
    [SerializeField] private GameObject _popUpBg;
    [SerializeField] private VideoPlayer _videoPlayer;
    
    [Header("Text")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;

    private bool _canOpenPopUp;
    
    private void Awake()
    {
        Instance = this;
    }

    public void InitPopUp(PopUpInfos[] popUpInfos)
    {
        
        
        if(LanguageManager.Instance.Tongue == Language.Francais)
            UpdatePopUp(popUpInfos[0].Title, popUpInfos[0].VideoName, popUpInfos[0].Description);
        else
            UpdatePopUp(popUpInfos[0].TitleEnglish, popUpInfos[0].VideoName, popUpInfos[0].DescriptionEnglish);
        
        if(GetComponent<LegendScroll>() != null)
            GetComponent<LegendScroll>().InitVideoLegend(popUpInfos);
    }

    public void UpdatePopUp(string title, string videoName, string description)
    {
        _titleText.text = title;
        
        gameObject.GetComponent<DialogPrefab>().InitDescOrder(description, false);

        _canOpenPopUp = false;
        
        var mapPath = $"PopUpVideo/{videoName}.mp4";

        // Initialize
        BetterStreamingAssets.Initialize();

        // Get the video path
        string videoPath = Path.Combine(BetterStreamingAssets.Root, mapPath);
        // print(videoPath);
        if (!BetterStreamingAssets.FileExists(mapPath))
        {
            ScreensManager.Instance.UpdatePopUpState(false);
            Debug.LogErrorFormat("Streaming asset not found: {0}", mapPath);
        }

        _canOpenPopUp = true;
        _videoPlayer.url = videoPath;
        _videoPlayer.Play();
    }

    public void UpdatePopUpState(bool state)
    {
        if (_canOpenPopUp)
        {
            _parent.SetActive(state);
            _popUpBg.SetActive(state);
            
            if(state)
                _parent.GetComponent<PointerMotion>().Bounce();
        }
    }
}
