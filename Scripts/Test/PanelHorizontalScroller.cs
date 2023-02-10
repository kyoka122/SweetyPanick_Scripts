using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImageHorizontalScroller : MonoBehaviour
{
    [SerializeField] float horizontalSpeed;

    RectTransform rectTransform;

    Vector3 defaultPosition;
    float defaultPositionx;
    float imageWidth;

    float screenWidth;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        defaultPositionx = rectTransform.anchoredPosition.x;
        imageWidth = rectTransform.sizeDelta.x;

        defaultPosition = new Vector3(defaultPositionx, 0, 0);

        screenWidth = Screen.width;

        Debug.Log(defaultPositionx);
        Debug.Log(imageWidth);
        Debug.Log(screenWidth);
    }

    void Update()
    {
        rectTransform.Translate(horizontalSpeed * Time.deltaTime, 0 , 0);
        
        if(defaultPositionx - rectTransform.anchoredPosition.x < -imageWidth + screenWidth)
        {
            rectTransform.anchoredPosition = defaultPosition;
        } else if(defaultPositionx - rectTransform.anchoredPosition.x > imageWidth - screenWidth)
        {
            rectTransform.anchoredPosition = defaultPosition;
        }
    }
}
