using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using System;
using System.IO;
using UnityEngine.Networking;
using Wanderer.GameFramework;
using GF = Wanderer.GameFramework;
using DG.Tweening;

[FSM(FSMStateType.OverStart)]
public class LaunchOverStartState : FSMState<GameStateContext>
{
    private string _updateAssetbundleName = "updateview";
    private string _updateAssetPath = "Assets/Game/Update/Canvas_UpdateView.prefab";
    private LuaBehaviour _updateViewInstance;
    private string _changeFlag;
    private AssetBundle _updateAB;

    public override void OnInit(FSM<GameStateContext> fsm)
    {
        base.OnInit(fsm);
    }

    public override void OnEnter(FSM<GameStateContext> fsm)
    {
        base.OnEnter(fsm);

        _changeFlag = null;
        LoadAsset(_updateAssetPath, (updateView) =>
        {
            _updateViewInstance = updateView.GetComponent<LuaBehaviour>();
            _updateViewInstance.OnLuaCallCSharp += OnLuaCallCSharp;

           // GameMode.Debugger.GetWindow<ProfilerWindow>().AutoTakeSample();
        });
    }

    public override void OnExit(FSM<GameStateContext> fsm)
    {
        if (_updateViewInstance != null)
        {
            var t = Input.gyro.attitude * Quaternion.Euler(0, 0, 0);
            _updateViewInstance.OnLuaCallCSharp -= OnLuaCallCSharp;
            GameObject.Destroy(_updateViewInstance.gameObject);
            _updateViewInstance = null;
        }
        //卸载ab包
        if (_updateAB != null)
        {
            _updateAB.Unload(true);
        }
        base.OnExit(fsm);
    }


    public override void OnUpdate(FSM<GameStateContext> fsm)
    {
        if (string.IsNullOrEmpty(_changeFlag))
            return;

        base.OnUpdate(fsm);

		//选择更新 | 读取本地 | 编辑器
		switch (GameMode.Resource.ResUpdateType)
		{
			case ResourceUpdateType.None:
				ChangeState<DoLuaState>(fsm);
				break;
			case ResourceUpdateType.Update:
				ChangeState<DoLuaState>(fsm);
				break;
			case ResourceUpdateType.Local:
				ChangeState<DoLuaState>(fsm);
				break;
			case ResourceUpdateType.Editor:
//#if UNITY_EDITOR
//				GameMode.Resource.SetResourceHelper(new EditorAssetsHelper());
//#else
//        		//如果在非编辑器模式下选择了Editor，使用本地文件
//                throw new GameException("Please select the correct resource type, not ResourceUpdateType.Editor!");
//#endif
                ChangeState<DoLuaState>(fsm);
                break;
		}
	}


    #region 事件回调
    //lua调用c#的函数
    private void OnLuaCallCSharp(string data)
    {
        _changeFlag = data;
    }
	#endregion

	#region 内部函数
	//加载资源
	private async void LoadAsset(string asset,Action<GameObject> callback)
    {
        GameObject updateView = null;
        if (GameMode.Resource.ResUpdateType == ResourceUpdateType.Editor)
        {
#if UNITY_EDITOR
            updateView = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(asset);
#endif
        }
        else if (GameMode.Resource.ResUpdateType == ResourceUpdateType.Update)
        {
            string assetPath = Path.Combine(Application.persistentDataPath, _updateAssetbundleName);
            Log.Info($"Load updateview assetbundle. {assetPath}");
            if (!File.Exists(assetPath))
            {
                //加载ab包
                await LoadAssetbundle((ab) =>
                {
                    updateView = ab.LoadAsset<GameObject>(asset);
                    _updateAB = ab;
                });
            }
            else
            {
                using (var stream = new EncryptFileStream(assetPath, FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 4, false))
                {
                    _updateAB = AssetBundle.LoadFromStream(stream);
                    updateView = _updateAB.LoadAsset<GameObject>(asset);
                }
            }
		}

        if (updateView != null)
        {
            callback.Invoke(GameObject.Instantiate(updateView));
        }
        else
        {
            throw new GameException($"Can't find updateview gameobject !!");
        }
    }

    /// <summary>
    /// 加载ab包
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator LoadAssetbundle(Action<AssetBundle> callback)
    {
        string assetPath ;
#if TEST
        assetPath = GameMode.Resource.ResTestUpdatePath;
#else
        assetPath = GameMode.Resource.ResOfficialUpdatePath;
#endif
        assetPath = $"{assetPath}{GF.Utility.GetRuntimePlatformName()}/{_updateAssetbundleName}";

        using (UnityWebRequest request = new UnityWebRequest(assetPath))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                throw new GameException($"Can't read assetbundle file from streamingasset: {assetPath} error: {request.error}");
            }
            yield return null;
            AssetBundle ab;
            byte[] buffer = request.downloadHandler.data;
            using (MemoryStream stream = new EncryptMemoryStream(buffer))
            {
                ab = AssetBundle.LoadFromStream(stream);
                if (ab == null)
                {
                    throw new GameException($"Can't read assetbundle file from streamingasset: {assetPath} error: {request.error}");
                }
                callback?.Invoke(ab);
            }
        }
    }
#endregion

}
