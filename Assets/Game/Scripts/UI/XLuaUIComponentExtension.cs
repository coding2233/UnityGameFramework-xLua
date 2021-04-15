using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wanderer.GameFramework
{
    public static class XLuaUIComponentExtension
    {
        /// <summary>
        /// 1 竖屏
        /// 3 横屏
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="orientation"></param>
        public static void SetOrientation(this UIManager ui, int orientation)
        {
            Screen.orientation = (ScreenOrientation)orientation;
            var referenceResolution = orientation == 1 ? new Vector2(1080, 1920) : new Vector2(1920, 1080);
            GameObject.FindObjectOfType<UIComponent>().GetComponent<CanvasScaler>().referenceResolution = referenceResolution;
        }
    }

}
