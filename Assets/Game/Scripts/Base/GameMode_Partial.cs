using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wanderer.GameFramework;

namespace Wanderer.GameFramework
{
    public partial class GameMode
    {

        public static XLuaManager XLua;

        [RuntimeInitializeOnLoadMethod]
        private static void OnAwakeAfter()
        {
            Debug.Log($"GameMode OnAwakeAfter");
            XLua = GameFrameworkMode.GetModule<XLuaManager>();
        }
    }
}

