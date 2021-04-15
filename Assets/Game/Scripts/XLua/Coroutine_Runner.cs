using UnityEngine;
using XLua;
using System.Collections.Generic;
using System.Collections;
using System;

public class Coroutine_Runner : MonoBehaviour
{
	public IEnumerator Test()
	{
		yield return null;
	}

	public IEnumerator Create()
	{
		return Test();
	}

}


//public static class CoroutineConfig
//{
//    [LuaCallCSharp]
//    public static List<Type> LuaCallCSharp
//    {
//        get
//        {
//            return new List<Type>()
//            {
//                typeof(WaitForSeconds),
//                typeof(WWW)
//            };
//        }
//    }
//}
