using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wanderer.GameFramework;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public static class UGUIExtension
{
    public static Toggle GetToggle(this Transform transform, string toggleName)
    {
        Transform[] allTransform =TransformExtension.GetAllTransform(transform);
        for (int i = 0; i < allTransform.Length; i++)
        {
            Transform item = allTransform[i];
            if (item.name == toggleName)
            {
                return item.GetComponent<Toggle>();
            }
        }
        return null;
    }
    //设置精灵图片。
    public static void SetSprite(this UnityEngine.UI.Image img, string name)
    {
        GameMode.Resource.Asset.LoadAsset<Sprite>(name, (sprite) =>
        {
            img.sprite = sprite;
        });
    }
    public static InputField GetInput(this Transform transform, string name)
    {
        Transform[] allTransform =TransformExtension.GetAllTransform(transform);
        for (int i = 0; i < allTransform.Length; i++)
        {
            Transform item = allTransform[i];
            if (item.name == name)
            {
                return item.GetComponent<InputField>();
            }
        }
        return null;
    }

    public static RawImage GetRawImg(this Transform transform, string name)
    {
        Transform[] allTransform =TransformExtension.GetAllTransform(transform);
        for (int i = 0; i < allTransform.Length; i++)
        {
            Transform item = allTransform[i];
            if (item.name == name)
            {
                return item.GetComponent<RawImage>();
            }
        }
        return null;
    }

    public static Slider GetSlider(this Transform transform, string name)
    {
        Transform[] allTransform =TransformExtension.GetAllTransform(transform);
        for (int i = 0; i < allTransform.Length; i++)
        {
            Transform item = allTransform[i];
            if (item.name == name)
            {
                return item.GetComponent<Slider>();
            }
        }
        return null;
    }

    //按钮事件回调
    public static void OnClick(this Button button, UnityAction callback)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(callback);
    }

    //按钮事件回调
    public static void OnClick(this GameObject target, UnityAction callback)
    {
        target.GetComponent<Button>()?.OnClick(callback); 
    }

    //按钮事件回调
    public static void OnClick(this Transform target, UnityAction callback)
    {
        target.GetComponent<Button>()?.OnClick(callback);
    }

    //按钮事件回调
    public static void OnClick(this Image target, UnityAction callback)
    {
        target.GetComponent<Button>()?.OnClick(callback);
    }


    #region InputExtension

    /// <summary>
    /// 检测当前平台是否点击UI
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <returns></returns>
    public static bool IsPointerOverGameObject()
    {
        //移动端
#if UNITY_ANDROID || UNITY_IPHONE
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (IsPointerOverUI(Input.GetTouch(0).position))
                {
                    return true;
                }
            }
#else
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI(Input.mousePosition))
            {
                return true;
            }
        }
#endif
        return false;
    }

    /// <summary>
    /// 是否点击UI
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <returns></returns>
    private static bool IsPointerOverUI(Vector2 mousePosition)
    {
        //创建一个点击事件
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = mousePosition
        };
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        //向点击位置发射一条射线，检测是否点击UI
        EventSystem.current.RaycastAll(eventData, raycastResults);
        if (raycastResults.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 世界坐标转屏幕坐标
    /// </summary>
    /// <param name="worldPoint"></param>
    /// <returns></returns>
    public static Vector3 WorldToScreenPoint(Vector3 worldPoint)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPoint);
        return screenPoint;
    }

    /// <summary>
    /// 屏幕坐标转世界坐标
    /// </summary>
    /// <param name="worldPoint"></param>
    /// <returns></returns>
    public static Vector3 ScreenToWorldPoint(Vector3 screenPoint)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);
        return worldPoint;
    }

    /// <summary>
    /// 求两向量夹角
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <returns></returns>
    public static float CaluatureAngle(Vector3 pos1, Vector3 pos2)
    {
        Vector3 cross = Vector3.Cross(pos1, pos2);
        float angle = Vector2.Angle(pos1, pos2);
        return cross.z > 0 ? -angle : angle;
    }

    #endregion
}
