using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using LitJson;

namespace Wanderer.GameFramework
{
    public class CreatingGameTemplateEditor : EditorWindow
    {
        static string _miniGamePath = Application.dataPath + "/Game/MiniGames/";
        static string _spinePath = Application.dataPath + "/Game/SpineResources/";
        static string _luaFilePath = Application.dataPath + "/Game/XLua/MiniGame/";
        static string _prefabPath = Application.dataPath + "/Game/Scripts/Editor/GameTemplate/GameTemplatePrefab.prefab";
        static string _luaScriptPath = Application.dataPath + "/Game/Scripts/Editor/GameTemplate/LuaGameScriptTemplate.lua.txt";
        static string _configName = "AssetBundleEditor.json";

        List<string> gameFolderList = new List<string>();
        List<string> deleteList = new List<string>();
        Vector2 scrollPos = Vector2.zero;

        bool createSpine = false;
        string gamePrefabName = "";
        string gameChineseName = "";
        float lineInterval = 10;
        float modeInterval = 15;

        [MenuItem("Tools/Creating Game Template")]
        static void Main()
        {
            GetWindow<CreatingGameTemplateEditor>("Creating Game Window");
        }

        private void OnEnable()
        {
            LoadGameList();
        }

        private void OnDisable()
        {

        }

        private void OnGUI()
        {
            //创建游戏
            GUILayout.BeginVertical("helpbox", GUILayout.Height(Screen.height * 0.3f));
            GUILayout.Space(lineInterval);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Createing Game Template", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(lineInterval);
            GUILayout.Label("Game Prefab Name", EditorStyles.boldLabel);
            gamePrefabName = GUILayout.TextField(gamePrefabName);
            GUILayout.Space(lineInterval);
            GUILayout.Label("Game Chinese Name", EditorStyles.boldLabel);
            gameChineseName = GUILayout.TextField(gameChineseName);
            GUILayout.Space(lineInterval);
            createSpine = GUILayout.Toggle(createSpine, "Create Spine Folder");
            GUILayout.Space(lineInterval);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create", GUILayout.Width(Screen.width * 0.5f)))
            {
                CreateGameFile();
                LoadGameList();
                GUIUtility.ExitGUI();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(lineInterval);
            GUILayout.EndVertical();
            GUILayout.Space(modeInterval);

            ///删除游戏
            GUILayout.BeginVertical("helpbox", GUILayout.Height(Screen.height * 0.7f));
            GUILayout.Space(lineInterval);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Deleteing Game Template", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(lineInterval);
            ShowGameList();
            GUILayout.Space(lineInterval);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete", GUILayout.Width(Screen.width * 0.5f)))
            {
                DeleteGameFile();
                LoadGameList();
                GUIUtility.ExitGUI();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(lineInterval);
            GUILayout.EndVertical();
        }

        void LoadGameList()
        {
            gameFolderList = new List<string>();
            string[] folders = Directory.GetDirectories(_miniGamePath);
            for (int i = 0; i < folders.Length; i++)
            {
                string str = folders[i].Split('/')[folders[i].Split('/').Length - 1];
                gameFolderList.Add(str);
            }
        }

        void ShowGameList()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, "helpbox", GUILayout.Height(Screen.height * 0.45f));
            if (gameFolderList != null && gameFolderList.Count > 0)
            {
                foreach (var item in gameFolderList)
                {
                    bool select = deleteList.Contains(item);
                    bool result = GUILayout.Toggle(select, item);
                    if (select != result)
                    {
                        if (result)
                        {
                            deleteList.Add(item);
                        }
                        else
                        {
                            deleteList.Remove(item);
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
        }

        void CreateGameFile()
        {
            if (!string.IsNullOrEmpty(gamePrefabName))
            {
                //游戏AB包路径
                string path = _miniGamePath + gamePrefabName;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Directory.CreateDirectory(path + "/Audio");
                    Directory.CreateDirectory(path + "/Audio/Music");
                    Directory.CreateDirectory(path + "/Audio/Sound");
                    Directory.CreateDirectory(path + "/Audio/UISound");
                    Directory.CreateDirectory(path + "/DynamicSprite");
                    File.Copy(_prefabPath, path + $"/{gamePrefabName}.prefab");
                    Debug.Log("Game ABPath: " + path);
                    Debug.Log("Game AB Audio Path: " + path + "/Audio/Music");

                }
                else
                {
                    EditorUtility.DisplayDialog("Message", "游戏目录已经存在", "OK");
                    return;
                }

                //Spine资源路径
                if (createSpine)
                {
                    path = _spinePath + gamePrefabName;
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        Debug.Log("Game SpineAsset Path: " + path);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Message", "Spine文件目录已经存在", "OK");
                        return;
                    }
                }

                //游戏脚本路径
                path = _luaFilePath + gamePrefabName;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    string fileStr = File.ReadAllText(_luaScriptPath, System.Text.Encoding.UTF8);
                    string newStr = fileStr.Replace("Template", gamePrefabName);
                    File.WriteAllText(path + $"/{gamePrefabName}.lua.txt", newStr, System.Text.Encoding.UTF8);
                    Debug.Log("Game LuaSprite Path: " + path);
                }
                else
                {
                    EditorUtility.DisplayDialog("Message", "Lua脚本文件目录已经存在", "OK");
                    return;
                }
                AssetDatabase.Refresh();
                //更新AB配置文件
                AddInfoToAssetBundleConfig();
                EditorUtility.DisplayDialog("Message", "游戏目录创建成功", "OK");
            }
        }

        void DeleteGameFile()
        {
            if (deleteList.Count > 0)
            {
                if (EditorUtility.DisplayDialog("Message", "确定删除选中的游戏？", "确定", "取消"))
                {
                    if (EditorUtility.DisplayDialog("Message", "真的确定要删除选中的游戏？", "确定", "取消"))
                    {
                        //删除游戏文件
                        for (int i = 0; i < deleteList.Count; i++)
                        {
                            if (Directory.Exists(_miniGamePath + deleteList[i]))
                            {
                                Directory.Delete(_miniGamePath + deleteList[i], true);
                            }
                            if (Directory.Exists(_spinePath + deleteList[i]))
                            {
                                Directory.Delete(_spinePath + deleteList[i], true);
                            }
                            if (Directory.Exists(_luaFilePath + deleteList[i]))
                            {
                                Directory.Delete(_luaFilePath + deleteList[i], true);
                            }
                        }

                        //修改json配置文件
                        DeleteInfoToAssetBundleConfig();
                        AssetDatabase.Refresh();
                        EditorUtility.DisplayDialog("Message", "删除游戏成功", "OK");
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Message", "未选择需要删除的游戏", "OK");
                return;
            }
        }

        void AddInfoToAssetBundleConfig()
        {
            JsonData jd = ProjectSettingsConfig.LoadJsonData(_configName);
            if (jd != null && jd.Count > 0)
            {
                JsonData data = new JsonData();
                data["AssetBundleName"] = gamePrefabName;
                data["Annotation"] = gameChineseName;
                data["Variant"] = "";
                data["Filter"] = 17410;
                data["Split"] = false;
                data["SplitCount"] = 1;
                data["ForceUpdate"] = false;
                data["Preload"] = false;
                data["SearchInFolders"] = new JsonData();
                data["SearchInFolders"].SetJsonType(JsonType.Array);
                data["SearchInFolders"].Add("Assets/Game/MiniGames/" + gamePrefabName);
                jd.Add(data);
                ProjectSettingsConfig.SaveJsonData(_configName, jd);
                gamePrefabName = "";
                gameChineseName = "";
            }
        }

        void DeleteInfoToAssetBundleConfig()
        {
            JsonData jd = ProjectSettingsConfig.LoadJsonData(_configName);
            List<JsonData> tempList = new List<JsonData>();
            if (jd != null && jd.Count > 0)
            {
                for (int i = 0; i < jd.Count; i++)
                {
                    for (int j = 0; j < deleteList.Count; j++)
                    {
                        if ((string)jd[i]["AssetBundleName"] == deleteList[j])
                        {
                            tempList.Add(jd[i]);
                        }
                    }
                }
            }

            if (tempList.Count > 0)
            {
                for (int i = 0; i < tempList.Count; i++)
                {
                    jd.Remove(tempList[i]);
                }
            }
            ProjectSettingsConfig.SaveJsonData(_configName, jd);
        }
    }
}

