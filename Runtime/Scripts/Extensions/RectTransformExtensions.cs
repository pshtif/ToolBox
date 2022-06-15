/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

public static class RectTransformExtensions
{
    public static Rect GetScreenSpaceRect(this RectTransform p_rectTransform)
    { 
        Vector2 size= Vector2.Scale(p_rectTransform.rect.size, p_rectTransform.lossyScale); 
        float x= p_rectTransform.position.x + p_rectTransform.anchoredPosition.x;
        float y= Screen.height - p_rectTransform.position.y - p_rectTransform.anchoredPosition.y;

        return new Rect(x, y, size.x, size.y);
    }
}