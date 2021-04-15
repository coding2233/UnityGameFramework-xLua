using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class TransformExtension
{
    /// <summary>
    /// 获取十六进制数据对应的颜色
    /// </summary>
    /// <param name="colorValue"></param>
    /// <returns></returns>
    public static Color ParseHtmlString(string colorValue)
    {
        Color outcolor = Color.white;
        ColorUtility.TryParseHtmlString(colorValue, out outcolor);
        return outcolor;
    }

    public static Vector3 GetTargetTransLocalPos(this Transform transform, Vector3 targetTransPos)
    {
        Vector3 returnValue = transform.worldToLocalMatrix.MultiplyPoint(targetTransPos);
        return returnValue;
    }
    public static void PauseGame()
    {
        Time.timeScale = 0.0001f;
    }
    public static void UnPauseGame()
    {
        Time.timeScale = 1f;
    }
    public static void SetMulitTouchEnable(bool b)
    {
        Input.multiTouchEnabled = b;
    }

    public static Vector2 GetRectOffsetMin(this GameObject go)
    {
        return go.transform.GetComponent<RectTransform>().offsetMin;
    }

    public static Vector2 GetRectOffsetMax(this GameObject go)
    {
        return go.transform.GetComponent<RectTransform>().offsetMax;
    }

    public static void SetRectOffsetMin(this GameObject go, Vector2 ofm)
    {
        go.transform.GetComponent<RectTransform>().offsetMin = ofm;
    }

    public static void SetRectOffsetMax(this GameObject go, Vector2 ofm)
    {
        go.transform.GetComponent<RectTransform>().offsetMax = ofm;
    }


    public static GameObject SetName(this GameObject selfObj, string setName)
    {
        selfObj.name = setName;
        return selfObj;
    }
    /// <summary>
    /// 只包含所有子节点的Transform组件 不包括自己和 孙物体
    /// </summary>
    /// <param name="transformParnt"></param>
    /// <returns></returns>
    public static Transform[] GetChilTransforms(Transform transformParnt)
    {
        Transform[] chileTransforms = new Transform[transformParnt.childCount];
        for (int i = 0; i < transformParnt.childCount; i++)
        {
            chileTransforms[i] = transformParnt.GetChild(i);

        }
        return chileTransforms;
    }
    //获取父节点下面所有的 Transform 组件。
    public static Transform[] GetAllTransform(Transform transform)
    {
        Transform[] allTransform = transform.GetComponentsInChildren<Transform>(true);

        return allTransform;
    }
    public static Transform GetActiveTransform(this Transform transform, string tranName)
    {
        Transform[] allTransform = transform.GetComponentsInChildren<Transform>(false);
        for (int i = 0; i < allTransform.Length; i++)
        {
            Transform item = allTransform[i];
            if (item.name == tranName)
            {
                return item;
            }
        }
        return null;
    }
    public static Transform GetTransform(this Transform transform, string tranName)
    {
        Transform[] allTransform = GetAllTransform(transform);
        for (int i = 0; i < allTransform.Length; i++)
        {
            Transform item = allTransform[i];
            if (item.name == tranName)
            {
                return item;
            }
        }
        return null;
    }
    public static GameObject GetGameObject(this Transform transform, string objName)
    {
        Transform[] allTransform = GetAllTransform(transform);
        for (int i = 0; i < allTransform.Length; i++)
        {
            Transform item = allTransform[i];
            if (item.name == objName)
            {
                return item.gameObject;
            }
        }
        return null;
    }

    public static GameObject GetWorldGameObject(string objName)
    {
        return GameObject.Find(objName);
    }

    /// <summary>
    /// 查找层级界面的游戏物体
    /// </summary>
    /// <param name="transName"></param>
    /// <returns></returns>
    public static Transform GetHierarchyTrans(string transName)
    {
        Transform trans = null;
        trans = GameObject.Find(transName).transform;
        return trans;
    }

    public static GameObject Clone(Transform prefab, string objectName = null)
    {
        GameObject mCloneObj = GameObject.Instantiate(prefab).gameObject;

        if (objectName != null)
        {
            mCloneObj.name = objectName;
        }
        return mCloneObj;
    }
    public static GameObject Clone(Transform prefab, Transform parent, string objectName = null)
    {
        GameObject mCloneObj = GameObject.Instantiate(prefab).gameObject;

        if (objectName != null)
        {
            mCloneObj.name = objectName;
        }

        mCloneObj.transform.SetParent(parent, false);

        return mCloneObj;
    }
    public static GameObject Clone(this GameObject prefab, Transform parent, string objectName = null)
    {
        GameObject mCloneObj = GameObject.Instantiate(prefab).gameObject;

        if (objectName != null)
        {
            mCloneObj.name = objectName;
        }

        mCloneObj.transform.SetParent(parent, false);

        return mCloneObj;
    }

    #region Random
    public static int GetRandomInt(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    public static float GetRandomfloat(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    #endregion

    #region Transform Extension
    /// <summary>
    /// 缓存的一些变量,免得每次声明
    /// </summary>
    private static Vector3 mLocalPos;
    private static Vector3 mScale;
    private static Vector3 mPos;

    public static Vector3 GetLocalPosition(this Transform transform)
    {
        return transform.localPosition;
    }
    public static Quaternion GetLocalRotation(this Transform transform)
    {
        return transform.localRotation;
    }
    public static Quaternion GetRotation(this Transform transform)
    {
        return transform.rotation;
    }
    public static Vector3 GetLocalScale(this Transform transform)
    {
        return transform.localScale;
    }
    public static Vector3 GetPosition(this Transform transform)
    {
        return transform.position;
    }
    public static Vector3 GetGlobalScale(this Transform transform)
    {

        return transform.lossyScale;
    }
    public static Vector3 GetScale(this Transform transform)
    {
        return transform.lossyScale;
    }
    public static Vector3 GetWorldScale(this Transform transform)
    {
        return transform.lossyScale;
    }
    public static Vector3 GetLossyScale(this Transform transform)
    {
        return transform.lossyScale;
    }

    public static Transform Parent(this Transform transform, Transform parent)
    {
        transform.SetParent(parent == null ? null : parent);
        return transform;
    }
    public static Transform AsRootTransform(this Transform transform)
    {
        transform.SetParent(null);
        return transform;
    }
    public static Transform LocalIdentity(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        return transform;
    }
    public static Transform LocalPosition(this Transform transform, Vector3 localPos)
    {
        transform.localPosition = localPos;
        return transform;
    }
    public static Transform LocalEulerAngles(this Transform transform, float x, float y, float z)
    {
        transform.localEulerAngles = new Vector3(x, y, z);
        return transform;
    }
    public static Transform LocalPosition(this Transform transform, float x, float y, float z)
    {
        transform.localPosition = new Vector3(x, y, z);
        return transform;
    }
    public static Transform LocalPosition(this Transform transform, float x, float y)
    {
        mLocalPos = transform.localPosition;
        mLocalPos.x = x;
        mLocalPos.y = y;
        transform.localPosition = mLocalPos;
        return transform;
    }
    public static Transform LocalPositionX(this Transform transform, float x)
    {
        mLocalPos = transform.localPosition;
        mLocalPos.x = x;
        transform.localPosition = mLocalPos;
        return transform;
    }
    public static Transform LocalPositionY(this Transform transform, float y)
    {
        mLocalPos = transform.localPosition;
        mLocalPos.y = y;
        transform.localPosition = mLocalPos;
        return transform;
    }
    public static Transform LocalPositionZ(this Transform transform, float z)
    {
        mLocalPos = transform.localPosition;
        mLocalPos.z = z;
        transform.localPosition = mLocalPos;
        return transform;
    }
    public static Transform LocalPositionIdentity(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        return transform;
    }
    public static Transform LocalRotation(this Transform transform, Quaternion localRotation)
    {
        transform.localRotation = localRotation;
        return transform;
    }
    public static Transform LocalRotationIdentity(this Transform transform)
    {
        transform.localRotation = Quaternion.identity;
        return transform;
    }
    public static Transform LocalScale(this Transform transform, Vector3 scale)
    {
        transform.localScale = scale;
        return transform;
    }
    public static Transform LocalScale(this Transform transform, float xyz)
    {
        transform.localScale = Vector3.one * xyz;
        return transform;
    }
    public static Transform LocalScale(this Transform transform, float x, float y, float z)
    {
        mScale = transform.localScale;
        mScale.x = x;
        mScale.y = y;
        mScale.z = z;
        transform.localScale = mScale;
        return transform;
    }
    public static Transform LocalScale(this Transform transform, float x, float y)
    {
        mScale = transform.localScale;
        mScale.x = x;
        mScale.y = y;
        transform.localScale = mScale;
        return transform;
    }
    public static Transform LocalScaleX(this Transform transform, float x)
    {
        mScale = transform.localScale;
        mScale.x = x;
        transform.localScale = mScale;
        return transform;
    }
    public static Transform LocalScaleY(this Transform transform, float y)
    {
        mScale = transform.localScale;
        mScale.y = y;
        transform.localScale = mScale;
        return transform;
    }
    public static Transform LocalScaleZ(this Transform transform, float z)
    {
        mScale = transform.localScale;
        mScale.z = z;
        transform.localScale = mScale;
        return transform;
    }
    public static Transform LocalScaleIdentity(this Transform transform)
    {
        transform.localScale = Vector3.one;
        return transform;
    }
    public static Transform Identity(this Transform transform)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        return transform;
    }
    public static Transform Position(this Transform transform, Vector3 position)
    {
        transform.position = position;
        return transform;
    }
    public static Transform Position(this Transform transform, float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
        return transform;
    }
    public static Transform Position(this Transform transform, float x, float y)
    {
        mPos = transform.position;
        mPos.x = x;
        mPos.y = y;
        transform.position = mPos;
        return transform;
    }
    public static Transform PositionIdentity(this Transform transform)
    {
        transform.position = Vector3.zero;
        return transform;
    }
    public static Transform PositionX(this Transform transform, float x)
    {
        mPos = transform.position;
        mPos.x = x;
        transform.position = mPos;
        return transform;
    }
    public static Transform PositionX(this Transform transform, Func<float, float> xSetter)
    {
        mPos = transform.position;
        mPos.x = xSetter(mPos.x);
        transform.position = mPos;
        return transform;
    }
    public static Transform PositionY(this Transform transform, float y)
    {
        mPos = transform.position;
        mPos.y = y;
        transform.position = mPos;
        return transform;
    }
    public static Transform PositionY(this Transform transform, Func<float, float> ySetter)
    {
        mPos = transform.position;
        mPos.y = ySetter(mPos.y);
        transform.position = mPos;
        return transform;
    }
    public static Transform PositionZ(this Transform transform, float z)
    {
        mPos = transform.position;
        mPos.z = z;
        transform.position = mPos;
        return transform;
    }
    public static Transform PositionZ(this Transform transform, Func<float, float> zSetter)
    {
        mPos = transform.position;
        mPos.z = zSetter(mPos.z);
        transform.position = mPos;
        return transform;
    }
    public static Transform RotationIdentity(this Transform transform)
    {
        transform.rotation = Quaternion.identity;
        return transform;
    }
    public static Transform Rotation(this Transform transform, Quaternion rotation)
    {
        transform.rotation = rotation;
        return transform;
    }
    public static Transform DestroyAllChild(this Transform transform)
    {
        var childCount = transform.childCount;
        for (var i = 0; i < childCount; i++)
        {
            UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
        }
        return transform;
    }
    public static Transform AsLastSibling(this Transform transform)
    {
        transform.SetAsLastSibling();
        return transform;
    }
    public static Transform AsFirstSibling(this Transform transform)
    {
        transform.SetAsFirstSibling();
        return transform;
    }
    public static Transform SiblingIndex(this Transform transform, int index)
    {
        transform.SetSiblingIndex(index);
        return transform;
    }
    public static Transform FindByPath(this Transform selfTrans, string path)
    {
        return selfTrans.Find(path.Replace(".", "/"));
    }
    public static Transform SeekTrans(this Transform selfTransform, string uniqueName)
    {
        var childTrans = selfTransform.Find(uniqueName);

        if (null != childTrans)
            return childTrans;

        foreach (Transform trans in selfTransform)
        {
            childTrans = trans.SeekTrans(uniqueName);

            if (null != childTrans)
                return childTrans;
        }

        return null;
    }
    public static void CopyDataFromTrans(this Transform selfTrans, Transform fromTrans)
    {
        selfTrans.SetParent(fromTrans.parent);
        selfTrans.localPosition = fromTrans.localPosition;
        selfTrans.localRotation = fromTrans.localRotation;
        selfTrans.localScale = fromTrans.localScale;
    }
    /// <summary>
    /// 递归遍历子物体，并调用函数
    /// </summary>
    /// <param name="tfParent"></param>
    /// <param name="action"></param>
    public static void ActionRecursion(this Transform tfParent, Action<Transform> action)
    {
        action(tfParent);
        foreach (Transform tfChild in tfParent)
        {
            tfChild.ActionRecursion(action);
        }
    }
    /// <summary>
    /// 递归遍历查找指定的名字的子物体
    /// </summary>
    /// <param name="tfParent">当前Transform</param>
    /// <param name="name">目标名</param>
    /// <param name="stringComparison">字符串比较规则</param>
    /// <returns></returns>
    public static Transform FindChildRecursion(this Transform tfParent, string name, StringComparison stringComparison = StringComparison.Ordinal)
    {
        if (tfParent.name.Equals(name, stringComparison))
        {
            //Debug.Log("Hit " + tfParent.name);
            return tfParent;
        }

        foreach (Transform tfChild in tfParent)
        {
            Transform tfFinal = null;
            tfFinal = tfChild.FindChildRecursion(name, stringComparison);
            if (tfFinal)
            {
                return tfFinal;
            }
        }

        return null;
    }
    /// <summary>
    /// 递归遍历查找相应条件的子物体
    /// </summary>
    /// <param name="tfParent">当前Transform</param>
    /// <param name="predicate">条件</param>
    /// <returns></returns>
    public static Transform FindChildRecursion(this Transform tfParent, Func<Transform, bool> predicate)
    {
        if (predicate(tfParent))
        {
            Debug.Log("Hit " + tfParent.name);
            return tfParent;
        }

        foreach (Transform tfChild in tfParent)
        {
            Transform tfFinal = null;
            tfFinal = tfChild.FindChildRecursion(predicate);
            if (tfFinal)
            {
                return tfFinal;
            }
        }

        return null;
    }
    public static string GetPath(this Transform transform)
    {
        var sb = new System.Text.StringBuilder();
        var t = transform;
        while (true)
        {
            sb.Insert(0, t.name);
            t = t.parent;
            if (t)
            {
                sb.Insert(0, "/");
            }
            else
            {
                return sb.ToString();
            }
        }
    }
    #endregion

    #region RectTransformExtension
    public static Vector2 GetPosInRootTrans(this RectTransform selfRectTransform, Transform rootTrans)
    {
        return RectTransformUtility.CalculateRelativeRectTransformBounds(rootTrans, selfRectTransform).center;
    }

    /// <summary>
    /// 获取RectTransform的真实Bounds
    /// </summary>
    /// <param name="rectTrans"></param>
    /// <returns></returns>
    public static Bounds GetBounds(this RectTransform rectTrans)
    {
        var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(rectTrans);
        bounds.center += rectTrans.position;
        bounds.size = new Vector3(bounds.size.x * rectTrans.lossyScale.x, bounds.size.y * rectTrans.lossyScale.y, bounds.size.z * rectTrans.lossyScale.z);
        return bounds;
    }

    public static RectTransform AnchorPosX(this RectTransform selfRectTrans, float anchorPosX)
    {
        var anchorPos = selfRectTrans.anchoredPosition;
        anchorPos.x = anchorPosX;
        selfRectTrans.anchoredPosition = anchorPos;
        return selfRectTrans;
    }
    public static RectTransform AnchorPosY(this RectTransform selfRectTrans, float anchorPosY)
    {
        var anchorPos = selfRectTrans.anchoredPosition;
        anchorPos.y = anchorPosY;
        selfRectTrans.anchoredPosition = anchorPos;
        return selfRectTrans;
    }
    public static RectTransform SetSizeWidth(this RectTransform selfRectTrans, float sizeWidth)
    {
        var sizeDelta = selfRectTrans.sizeDelta;
        sizeDelta.x = sizeWidth;
        selfRectTrans.sizeDelta = sizeDelta;
        return selfRectTrans;
    }
    public static RectTransform SetSizeHeight(this RectTransform selfRectTrans, float sizeHeight)
    {
        var sizeDelta = selfRectTrans.sizeDelta;
        sizeDelta.y = sizeHeight;
        selfRectTrans.sizeDelta = sizeDelta;
        return selfRectTrans;
    }
    public static Vector2 GetWorldSize(this RectTransform selfRectTrans)
    {
        return RectTransformUtility.CalculateRelativeRectTransformBounds(selfRectTrans).size;
    }
    #endregion

    #region UGUI Extension

    public static CanvasGroup GetCanvasGroup(this Transform transform, string groupName = null)
    {
        if (groupName != null)
        {
            Transform[] allTransform = GetAllTransform(transform);
            for (int i = 0; i < allTransform.Length; i++)
            {
                Transform item = allTransform[i];
                if (item.name == groupName)
                {
                    return item.GetComponent<CanvasGroup>();
                }
            }
        }
        else if (groupName == null)
        {

            return transform.GetComponent<CanvasGroup>();
        }

        return null;
    }


    //button
    public static Button GetButton(this Transform transform, string btnName)
    {
        Transform[] allTransform = GetAllTransform(transform);
        for (int i = 0; i < allTransform.Length; i++)
        {
            Transform item = allTransform[i];
            if (item.name == btnName)
            {
                return item.GetComponent<Button>();
            }
        }
        return null;
    }
    public static Button GetButton(this GameObject gameObject, string btnName)
    {
        Transform[] allTransform = GetAllTransform(gameObject.transform);
        for (int i = 0; i < allTransform.Length; i++)
        {
            Transform item = allTransform[i];
            if (item.name == btnName)
            {
                return item.GetComponent<Button>();
            }
        }

        return null;
    }
    public static Button GetButton(this Transform transform)
    {
        return transform.GetComponent<Button>();
    }
    public static Button GetButton(this GameObject gameObject)
    {
        return gameObject.GetComponent<Button>();
    }
    public static Button[] GetButtonsInChildren(this Transform transform)
    {
        return transform.GetComponentsInChildren<Button>(true);
    }
    //Text
    public static Text GetText(this Transform transform, string txtName)
    {
        Transform[] allTransform = GetAllTransform(transform);
        for (int i = 0; i < allTransform.Length; i++)
        {
            Transform item = allTransform[i];
            if (item.name == txtName)
            {
                return item.GetComponent<Text>();
            }
        }
        return null;
    }
    public static Text GetText(this GameObject gameObject, string txtName)
    {
        Transform[] allTransform = GetAllTransform(gameObject.transform);
        for (int i = 0; i < allTransform.Length; i++)
        {
            Transform item = allTransform[i];
            if (item.name == txtName)
            {
                return item.GetComponent<Text>();
            }
        }
        return null;
    }
    public static Text GetText(this Transform transform)
    {
        return transform.GetComponent<Text>();
    }
    public static Text GetText(this GameObject gameObject)
    {
        return gameObject.GetComponent<Text>();
    }
    public static Text[] GetTextsInChildren(this Transform transform)
    {
        return transform.GetComponentsInChildren<Text>(true);
    }
    //Image
    public static Image GetImage(this Transform transform, string imgName)
    {
        Transform[] allTransform = GetAllTransform(transform);
        for (int i = 0; i < allTransform.Length; i++)
        {
            Transform item = allTransform[i];
            if (item.name == imgName)
            {
                return item.GetComponent<Image>();
            }
        }
        return null;
    }
    public static Image GetImage(this GameObject gameObject, string imgName)
    {
        Transform[] allTransform = GetAllTransform(gameObject.transform);
        for (int i = 0; i < allTransform.Length; i++)
        {
            Transform item = allTransform[i];
            if (item.name == imgName)
            {
                return item.GetComponent<Image>();
            }
        }
        return null;
    }
    public static Image GetImage(this Transform transform)
    {
        return transform.GetComponent<Image>();
    }
    public static Image GetImage(this GameObject gameObject)
    {
        return gameObject.GetComponent<Image>();
    }
    public static Image FillAmount(this Image selfImage, float fillamount)
    {
        selfImage.fillAmount = fillamount;
        return selfImage;
    }
    public static Image[] GetImagesInChildren(this Transform transform)
    {
        return transform.GetComponentsInChildren<Image>(true);
    }


    ////ScrollRocker
    //public static ScrollRocker GetScrollRocker(this Transform transform, string animatorName)
    //{
    //    Transform[] allTransform = GetAllTransform(transform);
    //    for (int i = 0; i < allTransform.Length; i++)
    //    {
    //        Transform item = allTransform[i];
    //        if (item.name == animatorName)
    //        {
    //            return item.GetComponent<ScrollRocker>();
    //        }
    //    }
    //    return null;
    //}
    //public static ScrollRocker GetScrollRocker(this GameObject gameObject, string animatorName)
    //{
    //    Transform[] allTransform = GetAllTransform(gameObject.transform);
    //    for (int i = 0; i < allTransform.Length; i++)
    //    {
    //        Transform item = allTransform[i];
    //        if (item.name == animatorName)
    //        {
    //            return item.GetComponent<ScrollRocker>();
    //        }
    //    }
    //    return null;
    //}
    //public static ScrollRocker GetScrollRocker(this Transform transform)
    //{
    //    return transform.GetComponent<ScrollRocker>();
    //}
    //public static ScrollRocker GetScrollRocker(this GameObject gameObject)
    //{
    //    return gameObject.GetComponent<ScrollRocker>();
    //}





    #endregion

    //rhapsody code


    public static Transform SetHide(this Transform selfTransform)
    {
        selfTransform.gameObject.SetActive(false);
        return selfTransform;
    }

    public static Transform SetShow(this Transform selfTransform)
    {
        selfTransform.gameObject.SetActive(true);
        return selfTransform;
    }

    public static GameObject Show(this GameObject selfObj)
    {
        selfObj.SetActive(true);
        return selfObj;
    }

    public static GameObject Hide(this GameObject selfObj)
    {
        selfObj.SetActive(false);
        return selfObj;
    }



}
