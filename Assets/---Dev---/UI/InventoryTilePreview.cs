using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTilePreview : MonoBehaviour
{
    // [SerializeField] private GameObject _parent;
    [SerializeField] private Image _iconTile;

    public void InitPreviewTile(Sprite sprite, Color color)
    {
        gameObject.SetActive(true);
        
        _iconTile.sprite = sprite;
        _iconTile.color = color;
    }

    public void DeactivatePreviewTile()
    {
        gameObject.SetActive(false);
    }
}
