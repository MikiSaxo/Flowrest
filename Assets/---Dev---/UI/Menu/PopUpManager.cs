using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;

    [Header("Setup")] [SerializeField] private GameObject _parent;
    [SerializeField] private GameObject _popUpBg;
    [SerializeField] private Image _imgPopUp;
    [SerializeField] private float _timingBetweenImg;

    [Header("Text")] [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;

    private bool _canOpenPopUp;
    private Sprite[] _currentImgPopUp;
    private int _currentIndexImg;
    private float _currentTiming;

    private void Awake()
    {
        Instance = this;
    }

    public void InitPopUp(PopUpInfos[] popUpInfos)
    {
        if (LanguageManager.Instance.Tongue == Language.Francais)
            UpdatePopUp(popUpInfos[0].Title, popUpInfos[0].ImgPopUp,
                popUpInfos[0].Description);
        else
            UpdatePopUp(popUpInfos[0].TitleEnglish, popUpInfos[0].ImgPopUp,
                popUpInfos[0].DescriptionEnglish);

        if (GetComponent<LegendScroll>() != null)
            GetComponent<LegendScroll>().InitVideoLegend(popUpInfos);
    }

    public void UpdatePopUp(string title, Sprite[] imgPopUp, string description)
    {
        _titleText.text = title;

        gameObject.GetComponent<DialogPrefab>().InitDescOrder(description, false);

        _canOpenPopUp = false;
        if (imgPopUp != null && imgPopUp.Length > 0)
            _currentImgPopUp = imgPopUp;
        else
        {
            _currentImgPopUp = Array.Empty<Sprite>();
            _imgPopUp.sprite = null;
        }

        // var mapPath = $"PopUpVideo/{videoName}.mp4";
        //
        // // Initialize
        // BetterStreamingAssets.Initialize();
        //
        // // Get the video path
        // string videoPath = Path.Combine(BetterStreamingAssets.Root, mapPath);
        // // print(videoPath);
        // if (!BetterStreamingAssets.FileExists(mapPath))
        // {
        //     ScreensManager.Instance.UpdatePopUpState(false);
        //     Debug.LogErrorFormat("Streaming asset not found: {0}", mapPath);
        // }
        //
        _canOpenPopUp = true;
        // _videoPlayer.url = videoPath;
        // _videoPlayer.Play();
    }

    private void Update()
    {
        if (_currentImgPopUp == null || _currentImgPopUp.Length == 0) return;

        _currentTiming -= Time.deltaTime;

        if (_currentTiming <= 0)
        {
            _currentIndexImg++;
            if (_currentIndexImg >= _currentImgPopUp.Length)
                _currentIndexImg = 0;

            _imgPopUp.sprite = _currentImgPopUp[_currentIndexImg];

            _currentTiming += _timingBetweenImg;
        }
    }

    public void UpdatePopUpState(bool state)
    {
        if (_canOpenPopUp)
        {
            _parent.SetActive(state);
            _popUpBg.SetActive(state);

            if (state)
                _parent.GetComponent<PointerMotion>().Bounce();
        }
    }

    public void GoToPageOnePopUp()
    {
        GetComponent<LegendScroll>().GoToPageOne();
    }
}