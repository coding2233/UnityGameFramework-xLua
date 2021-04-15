using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;
using System.IO;
using SD = System.Diagnostics;
using UnityEngine.Networking;
using System.Net;
using marijnz.EditorCoroutines;

namespace Wanderer.GameFramework
{
    public class CommandBuild
    {
        #region menu item

        [MenuItem("Tools/Build/Asset/EditorTarget")]
        static void BuildEditorTargetAsset()
        {
            BuildAsset(EditorUserBuildSettings.activeBuildTarget);
        }

        [MenuItem("Tools/Build/Asset/[Auto Upload] EditorTarget &#P")]
        static void BuildEditorTargetAssetUpload()
        {
            BuildAsset(EditorUserBuildSettings.activeBuildTarget, true);
        }

        [MenuItem("Tools/Build/Asset/Android")]
        static void BuildAndroidAsset()
        {
            BuildAsset(BuildTarget.Android);
        }
        [MenuItem("Tools/Build/Asset/iOS")]
        static void BuildIOSAsset()
        {
            BuildAsset(BuildTarget.iOS);
        }

        [MenuItem("Tools/Build/Asset/[Auto Upload] Android %&#A")]
        static void BuildAndroidAssetUpload()
        {
            BuildAsset(BuildTarget.Android, true);
        }
        [MenuItem("Tools/Build/Asset/[Auto Upload] iOS %&#I")]
        static void BuildIOSAssetUpload()
        {
            BuildAsset(BuildTarget.iOS, true);
        }

        [MenuItem("Tools/Build/Official/[Auto Upload] Android Asset")]
        static void BuildOfficialAndroidAssetUpload()
        {
            bool result = EditorUtility.DisplayDialog("Waring!!!!!!!!!!!!!!", "Official release!", "ok", "cancel");
            if (result)
            {
                BuildAsset(BuildTarget.Android, true);
            }
        }

        [MenuItem("Tools/Build/Official/[Auto Upload] iOS Asset")]
        static void BuildOfficialIOSAssetUpload()
        {
            bool result = EditorUtility.DisplayDialog("Waring!!!!!!!!!!!!!!", "Official release!", "ok", "cancel");
            if (result)
            {
                BuildAsset(BuildTarget.iOS, true);
            }
        }

        [MenuItem("Tools/Build/Apk")]
        public static void BuildAndroidApk()
        {
            CSObjectWrapEditor.Generator.ClearAll();
            AssetDatabase.Refresh();
            //gen xlua
            CSObjectWrapEditor.Generator.GenAll();
            AssetDatabase.Refresh();

            //设置场景的场景的参数
            string launchScenePath = "Assets/Game/Scenes/main.unity";
            UnityEngine.SceneManagement.Scene activeScene = EditorSceneManager.OpenScene(launchScenePath);
            if (activeScene != null)
            {
                EditorSceneManager.SetActiveScene(activeScene);
                foreach (var item in activeScene.GetRootGameObjects())
                {
                    if (item.name.Equals("GameMode"))
                    {
                        GameMode gameMode = item.GetComponent<GameMode>();
                        gameMode.ConfigJsonData["ResourceUpdateType"] = (int)ResourceUpdateType.Update;
                        string configPath = AssetDatabase.GetAssetPath(gameMode.ConfigAsset);
                        File.WriteAllText(configPath, gameMode.ConfigJsonData.ToJson());
                        AssetDatabase.Refresh();
                        EditorUtility.SetDirty(gameMode);
                        EditorSceneManager.SaveOpenScenes();
                        AssetDatabase.Refresh();
                        break;
                    }
                }
            }

            //android keystore
            string path = Path.GetDirectoryName(Application.dataPath);
            string keyStorePath = Path.Combine(path, "tools/user.keystore");
            PlayerSettings.Android.keystoreName = keyStorePath;
            PlayerSettings.Android.keystorePass = "********";
            PlayerSettings.Android.keyaliasName = "********";
            PlayerSettings.Android.keyaliasPass = "********";

            //build apk
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
            // playerOptions.scenes = new string[] { "Assets/Core/Scenes/SampleScene.unity" };
            playerOptions.target = BuildTarget.Android;
            playerOptions.locationPathName = "build/Android/mini.apk";

            BuildPipeline.BuildPlayer(playerOptions);
            Debug.Log("build sccuess!");
        }

        #endregion

        #region 内部函数
        private static BuildTarget StringToBuildTarget(string buildTarget)
        {
            BuildTarget target = BuildTarget.StandaloneWindows;
            if (buildTarget.Contains("mac") || buildTarget.Contains("osx"))
            {
                target = BuildTarget.StandaloneOSX;
            }
            else if (buildTarget.Contains("linux"))
            {
                target = BuildTarget.StandaloneLinux64;
            }
            else if (buildTarget.Contains("android"))
            {
                target = BuildTarget.Android;
            }
            return target;
        }

        public static void BuildPlayer()
        {
            string[] args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains("-customBuildTarget"))
                {
                    string buildTarget = args[++i].Trim();
                    BuildTarget target = StringToBuildTarget(buildTarget);
                    BuildPlayer(target);
                    return;
                }
            }
        }

        private static void BuildPlayer(BuildTarget buildTarget)
        {
            CSObjectWrapEditor.Generator.ClearAll();
            AssetDatabase.Refresh();
            //gen xlua
            CSObjectWrapEditor.Generator.GenAll();
            AssetDatabase.Refresh();

            //设置场景的场景的参数
            string launchScenePath = "Assets/Game/Scenes/main.unity";
            UnityEngine.SceneManagement.Scene activeScene = EditorSceneManager.OpenScene(launchScenePath);
            if (activeScene != null)
            {
                EditorSceneManager.SetActiveScene(activeScene);
                foreach (var item in activeScene.GetRootGameObjects())
                {
                    if (item.name.Equals("GameMode"))
                    {
                        GameMode gameMode = item.GetComponent<GameMode>();
                        gameMode.ConfigJsonData["ResourceUpdateType"] = (int)ResourceUpdateType.Update;
                        string configPath = AssetDatabase.GetAssetPath(gameMode.ConfigAsset);
                        File.WriteAllText(configPath, gameMode.ConfigJsonData.ToJson());
                        AssetDatabase.Refresh();
                        EditorUtility.SetDirty(gameMode);
                        EditorSceneManager.SaveOpenScenes();
                        AssetDatabase.Refresh();
                        break;
                    }
                }
            }

            if (buildTarget == BuildTarget.Android)
            {
                //android keystore
                string path = Path.GetDirectoryName(Application.dataPath);
                string keyStorePath = Path.Combine(path, "tools/user.keystore");
                PlayerSettings.Android.keystoreName = keyStorePath;
                PlayerSettings.Android.keystorePass = "********";
                PlayerSettings.Android.keyaliasName = "********";
                PlayerSettings.Android.keyaliasPass = "********";
            }

            //build
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
            // playerOptions.scenes = new string[] { "Assets/Core/Scenes/SampleScene.unity" };
            playerOptions.target = buildTarget;
            var buildTargetPath = $"build/{AssetBundleBuildEditor.BuildTargetToString(buildTarget)}";
            if (!Directory.Exists(buildTargetPath))
            {
                Directory.CreateDirectory(buildTargetPath);
            }

            if (buildTarget == BuildTarget.Android)
            {
                buildTargetPath = $"{buildTargetPath}/{Application.productName}.apk";
            }
            else if (buildTarget == BuildTarget.StandaloneWindows)
            {
                buildTargetPath = $"{buildTargetPath}/{Application.productName}.exe";
            }
            else if (buildTarget == BuildTarget.StandaloneLinux64)
            {
                buildTargetPath = $"{buildTargetPath}/{Application.productName}.x86_64";
            }
            else if (buildTarget == BuildTarget.StandaloneOSX)
            {
                buildTargetPath = $"{buildTargetPath}/{Application.productName}";
            }
            playerOptions.locationPathName = buildTargetPath;

            BuildPipeline.BuildPlayer(playerOptions);
            Debug.Log("build sccuess!");
        }
        public static void BuildAsset()
        {
            string[] args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains("-customBuildTarget"))
                {
                    string buildTarget = args[++i].Trim();
                    BuildTarget target = StringToBuildTarget(buildTarget);
                    BuildAsset(target);
                    return;
                }
            }
        }
        //打包资源
        private static void BuildAsset(BuildTarget buildTarget, bool autoUpload = false, string remotePath = "oss://unity-res-mini-games")
        {
            string buildPath = AssetBundleBuildEditor.BuildAssetBundles(buildTarget);
            if (autoUpload)
            {
                //获取完整的路径
                buildPath = Path.GetFullPath(buildPath);

                string directoryPath = Path.GetFullPath(Path.GetDirectoryName(Application.dataPath));
                if (!buildPath.Contains(directoryPath))
                {
                    buildPath = Path.Combine(directoryPath, buildPath);
                }
                //上传服务器
                remotePath = $"{remotePath}/{AssetBundleBuildEditor.BuildTargetToString(buildTarget)}";
                OssFolderUpload(buildPath, remotePath);

                //string targetName = AssetBundleBuildEditor.BuildTargetToString(buildTarget).ToLower();
                ////开始协程
                //FileUpload fileUpload = new FileUpload();
                //EditorCoroutines.StartCoroutine(fileUpload.FilesUpload(buildPath, targetName, "http://127.0.0.1:19321/"), fileUpload) ;
            }
        }

        //oss 文件上传
        private static void OssFolderUpload(string localPath, string remotePath)
        {
            SD.Process proc = new SD.Process();
            string dataPath = Path.GetDirectoryName(Application.dataPath);
            proc.StartInfo.FileName = Path.Combine(dataPath, "tools/ossutil64/ossutil.bat");
            //proc.StartInfo.Arguments=$"{Path.Combine(dataPath,"build/Full/1_1_5_69/Android")} oss://qwe-0811/Android";
            proc.StartInfo.Arguments = $"{localPath} {remotePath}";
            proc.Start();
            proc.WaitForExit();

            Debug.Log("Oss upload to complete!");
        }

        //class FileUpload
        //{
        //	//文件上传
        //	public IEnumerator FilesUpload(string buildPath,string targetName, string url)
        //	{
        //		string[] files = Directory.GetFiles(buildPath);
        //		WWWForm form = new WWWForm();
        //		foreach (var item in files)
        //		{
        //			string fileName = $"{targetName}/{Path.GetFileName(item)}";
        //			form.AddBinaryData(fileName, File.ReadAllBytes(item));
        //		}

        //		var webRequest = UnityWebRequest.Post(url, form);
        //		yield return webRequest.SendWebRequest();
        //		Debug.Log($"上传完成: {webRequest.downloadHandler.text}");
        //	}
        //}


        #endregion
    }
}