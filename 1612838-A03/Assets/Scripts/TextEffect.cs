using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextEffect : MonoBehaviour
{
    Text text;

    void Start()
    {
        text = GetComponent<Text>();    
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = Color.red;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = new Color(221, 200, 39);
    }
}
