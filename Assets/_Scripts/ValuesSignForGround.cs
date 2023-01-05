using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ValuesSignForGround : MonoBehaviour
{
    public static ValuesSignForGround Instance;
    
    [Header("Setup UI")] [SerializeField] private TextMeshProUGUI _current;
    [SerializeField] private TextMeshProUGUI _temperature;
    [SerializeField] private TextMeshProUGUI _humidity;
    [Header("Values")] [SerializeField] private string[] _currentInfo;

    private void Awake()
    {
        Instance = this;
    }

    public void NoValue()
    {
        _temperature.gameObject.SetActive(false);
        _humidity.gameObject.SetActive(false);

        _current.text = _currentInfo[0];
    }

    public void ChangeValues(float temperature, float humidity)
    {
        _temperature.gameObject.SetActive(true);
        _humidity.gameObject.SetActive(true);

        // Barbare
        string tempe = temperature + "     ";
        tempe = tempe.Substring(0, 4);
        string humi = humidity + "     ";
        humi = humi.Substring(0, 4);

        _current.text = _currentInfo[1];
        _temperature.text = "Temperature : " + tempe;
        _humidity.text = "Humidity : " + humi;
    }
}