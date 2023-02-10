using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelHorizontalScroller : MonoBehaviour
{
    [SerializeField] float horizontalSpeed;

    RectTransform rectTransform;

    float defaultPositionx;
    float width;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        defaultPositionx = rectTransform.position.x;
        width = rectTransform.sizeDelta.x;
    }

    void Update()
    {
        rectTransform.Translate(horizontalSpeed * Time.deltaTime, 0 , 0);
        
        if(defaultPositionx - rectTransform.position.x < -width)
        {
            rectTransform.Translate(-width, 0, 0);
        } else if(defaultPositionx - rectTransform.position.x > width)
        {
            rectTransform.Translate(width, 0, 0);
        }
    }
}
