using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Save_btn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Sprite pressed;

    private Sprite normal;
    private Image buttonImage;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        normal = buttonImage.sprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (pressed != null)
        {
            buttonImage.sprite = pressed;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (normal != null)
        {
            buttonImage.sprite = normal;
        }
    }
}
