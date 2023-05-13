using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _textNumber;
    [SerializeField] private GameObject _cadena;

    private int _levelNumber;
    private bool _isBlocked;
    
    public void Init(int levelNumber, bool isBlocked)
    {
        _textNumber.text = $"{levelNumber}";
        _levelNumber = levelNumber;
        _isBlocked = isBlocked;
        
        _cadena.SetActive(isBlocked);
        gameObject.GetComponent<Button>().interactable = !isBlocked;
        gameObject.GetComponent<PointerMotion>().UpdateCanEnter(!isBlocked);
    }

    public void OnClick()
    {
        if(MainMenuManager.Instance.IsLoading) return;
        
        if (BigManager.Instance == null)
        {
            print("Didn't found ----BigManager----");
            return;
        }

        BigManager.Instance.CurrentLevel = _levelNumber-1;
        MainMenuManager.Instance.LaunchMainScene();
    }

    public void UnlockLevel()
    {
        _cadena.SetActive(false);
        gameObject.GetComponent<Button>().interactable = true;
        gameObject.GetComponent<PointerMotion>().UpdateCanEnter(true);
    }
}
