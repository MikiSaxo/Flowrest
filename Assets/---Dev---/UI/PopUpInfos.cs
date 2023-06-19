using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PopUpInfos
{
    public Sprite[] ImgPopUp;
    [Header("French")]
    public string Title;
    public string Description;
    [Header("English")]
    public string TitleEnglish;
    public string DescriptionEnglish;
}
