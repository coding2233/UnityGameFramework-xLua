using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wanderer.GameFramework;

namespace Wanderer.GameFramework
{
    public partial class GameMode
    {

        [RuntimeInitializeOnLoadMethod]
        private static void OnAwakeAfter()
        {
            Debug.Log($"GameMode OnAwakeAfter");
          
        }
    }
}

