using UnityEngine;

public class EditorLevelParent : MonoBehaviour
{
    public static EditorLevelParent Instance;

    private void Awake()
    {
        Instance = this;
    }
}