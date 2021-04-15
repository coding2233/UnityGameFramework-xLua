using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityExtension
{
    /// <summary>
    /// 摄像机射线获取
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="hit"></param>
    /// <returns></returns>
    public static bool Raycast(this Camera camera,out RaycastHit hit)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit);
    }

    /// <summary>
    /// 摄像机ScreenPosition转RectTransform
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="rect"></param>
    /// <param name="screenPos"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static bool ScreenPointToLocalPointInRectangle(this Camera camera,RectTransform rect,Vector2 screenPos, out Vector2 point)
    {
       return RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, camera,out point);
    }
}
