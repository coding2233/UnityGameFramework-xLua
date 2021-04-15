#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Wanderer.GameFramework
{
    public class KingdomsUnityEditor : MonoBehaviour
    {
        public KingdomsConfig Config;

        public int SelectLevel = -1;

        public bool ShowBgRuntime = true;

        // public string DynamicSpritePath { get; private set; } = $"Assets/Game/MiniGames/KingdomsUIView/DynamicSprite";
        //prefab
        public string DynamicPrafabPath { get; private set; } = $"Assets/Game/MiniGames/KingdomsUIView/Prefabs";


        private void Start()
        {
            if (ShowBgRuntime)
            {
                var bgGO = new GameObject("BG").AddComponent<RectTransform>();
                bgGO.transform.SetParent(transform.parent);
                bgGO.transform.localScale = Vector3.one;
                bgGO.transform.localPosition = Vector3.zero;
                bgGO.transform.SetAsFirstSibling();
                bgGO.anchoredPosition3D = Vector3.zero;
                var gridLayout = bgGO.gameObject.AddComponent<GridLayoutGroup>();
                gridLayout.cellSize = Vector2.one * Config.Size;
                gridLayout.childAlignment = TextAnchor.MiddleCenter;
                gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayout.constraintCount = Config.Column;
                int size = Config.Row * Config.Column;
                for (int i = 0; i < Config.Row; i++)
                {
                    for (int j = 0; j < Config.Column; j++)
                    {
                        GameObject bgItem = new GameObject($"{i}_{j}");
                        bgItem.transform.SetParent(bgGO);
                        bgItem.transform.localScale = Vector3.one;
                        bgItem.transform.localPosition = Vector3.zero;
                        bgItem.AddComponent<Image>();
                        var outline = bgItem.AddComponent<Outline>();
                        outline.effectDistance = Vector2.one * 5;
                        outline.effectColor = Color.black;
                    }
                }
            }
        }

        private void OnValidate()
        {
            Resize();
        }

        private void OnDrawGizmos()
        {
            if(EditorApplication.isPlaying)
            return;

            //外框
            Gizmos.color = Color.green;
            var bounds = GetComponent<RectTransform>().GetBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            //分割线
            Vector3 miniPoint = bounds.center - bounds.size * 0.5f;
            float hightInterval = bounds.size.y / (float)Config.Row;
            for (int i = 0; i < Config.Row; i++)
            {
                Vector3 startPos = miniPoint + new Vector3(0, i * hightInterval, 0);
                Vector3 endPos = miniPoint + new Vector3(bounds.size.x, i * hightInterval, 0);
                Gizmos.DrawLine(startPos, endPos);
            }
            float widthInterval = bounds.size.x / (float)Config.Column;
            for (int i = 0; i < Config.Column; i++)
            {
                Vector3 startPos = miniPoint + new Vector3(i * widthInterval, 0, 0);
                Vector3 endPos = miniPoint + new Vector3(i * widthInterval, bounds.size.y, 0);
                Gizmos.DrawLine(startPos, endPos);
            }
            //绘制组件
            Vector2 cellInterval = new Vector2(widthInterval, hightInterval);
            if (SelectLevel >= 0 && SelectLevel < Config.Levels.Count)
            {
                Gizmos.color = Color.red;
                var levelConfig = Config.Levels[SelectLevel];
                foreach (var item in levelConfig.Items)
                {
                    Vector3 center = miniPoint + (Vector3)((item.Point + Vector2.one * 0.5f) * cellInterval);
                    Vector3 size = item.Size * cellInterval;
                    Gizmos.DrawWireCube(center, size);
                    //prefab
                    string prefabPath = GetPrafabPath(item.Sprite);
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    //prefab
                    if (prefab != null)
                    {
                        Vector2 texturePoint = new Vector2(center.x - size.x * 0.5f, center.y - size.y * 0.5f);
                        Rect textureRect = new Rect(texturePoint, size);
                        Sprite sprite = prefab.transform.Find("Image").GetComponent<Image>().sprite;
                        Gizmos.DrawGUITexture(textureRect, sprite.texture);
                    }
                }
            }
        }

        public void Resize()
        {
            Vector2 halfOne = Vector2.one * 0.5f;
            RectTransform targetRectTrans = GetComponent<RectTransform>();
            targetRectTrans.anchorMin = halfOne;
            targetRectTrans.anchorMax = halfOne;
            targetRectTrans.pivot = halfOne;
            targetRectTrans.localScale = Vector3.one;
            targetRectTrans.localRotation = Quaternion.identity;
            targetRectTrans.anchoredPosition = Vector2.zero;
            targetRectTrans.sizeDelta = new Vector2(Config.Size * Config.Column, Config.Size * Config.Row);
        }

        //prefab
        public string GetPrafabPath(string praFabName)
        {
            string prefabPath = $"{DynamicPrafabPath}/{praFabName}.prefab";
            return prefabPath;
        }
        [System.Serializable]
        public struct KingdomsConfig
        {
            /// <summary>
            /// 大小
            /// </summary>
            public int Size;
            /// <summary>
            /// 行数
            /// </summary>
            public int Row;
            /// <summary>
            /// 列数
            /// </summary>
            public int Column;
            /// <summary>
            /// 关卡信息
            /// </summary>
            public List<KingdomsLevelConfig> Levels;
        }

        [System.Serializable]
        public struct KingdomsLevelConfig
        {
            /// <summary>
            /// 关卡名称
            /// </summary>
            public string LevelName;
            /// <summary>
            /// 角色组件
            /// </summary>
            public string Player;
            /// <summary>
            /// 胜利组件
            /// </summary>
            public string Victory;
            /// <summary>
            /// 组件
            /// </summary>
            public List<KingdomsLevelItemConfig> Items;
        }

        [System.Serializable]
        public struct KingdomsLevelItemConfig
        {
            public string Name;
            public string Sprite;
            public Vector2 Size;
            public Vector2 Point;
        }

    }

    [CustomEditor(typeof(KingdomsUnityEditor))]
    public class KingdomsUnityEditorWindowsEditor : Editor
    {
        KingdomsUnityEditor _target;
        private const string _configPath = "Assets/Game/MiniGames/KingdomsUIView/Config/config.txt";
        private string[] _prefabNames;
        public void OnEnable()
        {
            _target = (KingdomsUnityEditor)target;
            var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new string[] { _target.DynamicPrafabPath });
            if (prefabGuids != null && prefabGuids.Length > 0)
            {
                _prefabNames = new string[prefabGuids.Length];
                for (int i = 0; i < prefabGuids.Length; i++)
                {
                    string spriteName = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
                    spriteName = Path.GetFileNameWithoutExtension(spriteName);
                    _prefabNames[i] = (spriteName);
                }
            }
            var configText = AssetDatabase.LoadAssetAtPath<TextAsset>(_configPath);
            if (configText != null)
            {
                _target.Config = JsonUtility.FromJson<KingdomsUnityEditor.KingdomsConfig>(configText.text);
                if (_target.Config.Levels.Count > 0)
                {
                    if (_target.SelectLevel == -1)
                    {
                        _target.SelectLevel = 0;
                    }
                }
                else
                {
                    _target.SelectLevel = -1;
                }
            }
            else
            {
                _target.Config.Size = 200;
                _target.Config.Row = 4;
                _target.Config.Column = 7;
                _target.SelectLevel = -1;
                SaveData();
            }
            _target.Resize();
        }



        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply", GUILayout.Width(60)))
            {
                //bool save = true;
                //foreach (var item in _target.Config.Levels)
                //{
                //	if (string.IsNullOrEmpty(item.LevelName))
                //	{
                //		save = false;
                //		EditorUtility.DisplayDialog("保证关卡")
                //	}
                //}
                SaveData();
                EditorUtility.DisplayDialog("保存数据", "保存数据", "ok");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("helpbox");
            GUILayout.Label("全局配置", EditorStyles.boldLabel);
            _target.Config.Size = EditorGUILayout.IntField("单位大小", _target.Config.Size);
            _target.Config.Row = EditorGUILayout.IntField("行数", _target.Config.Row);
            _target.Config.Column = EditorGUILayout.IntField("列数", _target.Config.Column);
            bool showBg = EditorGUILayout.Toggle("运行时显示背景", _target.ShowBgRuntime);
            if (showBg != _target.ShowBgRuntime)
            {
                _target.ShowBgRuntime = showBg;
                EditorUtility.SetDirty(_target);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("helpbox");
            GUILayout.Label($"关卡配置 ({_target.SelectLevel + 1}/{_target.Config.Levels.Count})", EditorStyles.boldLabel);
            if (_target.Config.Levels.Count > 0)
            {
                if (_target.SelectLevel < 0 || _target.SelectLevel >= _target.Config.Levels.Count)
                {
                    _target.SelectLevel = 0;
                    EditorUtility.SetDirty(_target);
                    //return;
                }

                string[] levelNames = new string[_target.Config.Levels.Count];
                for (int i = 0; i < _target.Config.Levels.Count; i++)
                {
                    levelNames[i] = _target.Config.Levels[i].LevelName;
                }
                int selectLevel = EditorGUILayout.Popup("选择关卡", _target.SelectLevel, levelNames);
                if (selectLevel != _target.SelectLevel)
                {
                    _target.SelectLevel = selectLevel;
                    EditorUtility.SetDirty(_target);
                }


                var levelConfig = _target.Config.Levels[_target.SelectLevel];

                GUILayout.BeginVertical("Box");
                levelConfig.LevelName = EditorGUILayout.TextField("关卡名称", levelConfig.LevelName);
                //删除当前关卡
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("删除当前关卡", GUILayout.Width(100)))
                {
                    if (EditorUtility.DisplayDialog("警告", $"是否删除当前关卡:{levelConfig.LevelName}", "ok", "cancel"))
                    {
                        _target.Config.Levels.RemoveAt(_target.SelectLevel);
                        //_target.SelectLevel--;
                        GUIUtility.ExitGUI();
                        return;
                    }
                }
                GUILayout.EndHorizontal();
                //渲染组件
                if (levelConfig.Items != null && levelConfig.Items.Count > 0)
                {
                    int playerItem = -1;
                    int victoryItem = -1;
                    string[] itemNames = new string[levelConfig.Items.Count];
                    for (int i = 0; i < levelConfig.Items.Count; i++)
                    {
                        itemNames[i] = levelConfig.Items[i].Name;
                        if (itemNames[i].Equals(levelConfig.Player))
                        {
                            playerItem = i;
                        }
                        if (itemNames[i].Equals(levelConfig.Victory))
                        {
                            victoryItem = i;
                        }
                    }
                    int newItem = EditorGUILayout.Popup("角色组件", playerItem, itemNames);
                    if (newItem != playerItem && newItem >= 0)
                    {
                        if (newItem < itemNames.Length)
                            levelConfig.Player = itemNames[newItem];
                    }
                    newItem = EditorGUILayout.Popup("胜利组件", victoryItem, itemNames);
                    if (newItem != playerItem && newItem >= 0)
                    {
                        if (newItem < itemNames.Length)
                            levelConfig.Victory = itemNames[newItem];
                    }

                    GUILayout.Label($"组件 ({levelConfig.Items.Count})");
                    for (int i = 0; i < levelConfig.Items.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("x", GUILayout.Width(30)))
                        {
                            if (EditorUtility.DisplayDialog("警告", $"是否删除当前组件:{levelConfig.Items[i].Name}", "ok", "cancel"))
                            {
                                _target.Config.Levels[_target.SelectLevel].Items.RemoveAt(i);
                                //levelConfig.Items.RemoveAt(i);
                               
                                GUIUtility.ExitGUI();
                                return;
                            }
                        }
                        GUILayout.EndHorizontal();
                        levelConfig.Items[i] = DrawLevelItem(levelConfig.Items[i]);
                    }
                }
                if (GUILayout.Button("添加组件"))
                {
                    if (levelConfig.Items == null)
                    {
                        levelConfig.Items = new List<KingdomsUnityEditor.KingdomsLevelItemConfig>();
                    }
                    KingdomsUnityEditor.KingdomsLevelItemConfig itemConfig;
                    if (levelConfig.Items.Count > 0)
                    {
                        itemConfig = levelConfig.Items[levelConfig.Items.Count - 1];
                    }
                    else
                    {
                        itemConfig = new KingdomsUnityEditor.KingdomsLevelItemConfig();
                    }
                    itemConfig.Name = System.Guid.NewGuid().ToString().Substring(0, 6);
                    levelConfig.Items.Add(itemConfig);
                }
                GUILayout.EndVertical();

                _target.Config.Levels[_target.SelectLevel] = levelConfig;
            }
            if (GUILayout.Button("添加关卡"))
            {
                KingdomsUnityEditor.KingdomsLevelConfig levelConfig;
                if (_target.Config.Levels.Count > 0)
                {
                    levelConfig = _target.Config.Levels[_target.Config.Levels.Count - 1];
                }
                else
                {
                    levelConfig = new KingdomsUnityEditor.KingdomsLevelConfig();
                }
                levelConfig.LevelName = System.Guid.NewGuid().ToString().Substring(0, 10);
                _target.Config.Levels.Add(levelConfig);
                _target.SelectLevel = _target.Config.Levels.Count - 1;
            }
            GUILayout.EndVertical();
            //base.OnInspectorGUI();

            
            if (GUILayout.Button("Apply", GUILayout.Width(60)))
            {
                //bool save = true;
                //foreach (var item in _target.Config.Levels)
                //{
                //	if (string.IsNullOrEmpty(item.LevelName))
                //	{
                //		save = false;
                //		EditorUtility.DisplayDialog("保证关卡")
                //	}
                //}
                SaveData();
                EditorUtility.DisplayDialog("保存数据", "保存数据", "ok");
            }
        }



        //绘制小组件
        private KingdomsUnityEditor.KingdomsLevelItemConfig DrawLevelItem(KingdomsUnityEditor.KingdomsLevelItemConfig itemConfig)
        {
            if (_prefabNames == null)
                return itemConfig;
            GUILayout.BeginHorizontal("box");
            GUILayout.BeginVertical();

            itemConfig.Name = EditorGUILayout.TextField("名称", itemConfig.Name);
            int selectSprite = -1;
            if (_prefabNames != null)
            {
                for (int i = 0; i < _prefabNames.Length; i++)
                {
                    if (_prefabNames[i].Equals(itemConfig.Sprite))
                    {
                        selectSprite = i;
                        break;
                    }
                }
            }

            int newSelectSprite = EditorGUILayout.Popup("图片", selectSprite, _prefabNames);
            if (newSelectSprite != selectSprite)
            {
                itemConfig.Sprite = _prefabNames[newSelectSprite];
            }


            //itemConfig.Sprite = EditorGUILayout.TextField("图片",itemConfig.Sprite);
            itemConfig.Size = EditorGUILayout.Vector2Field("大小", itemConfig.Size);
            itemConfig.Point = EditorGUILayout.Vector2Field("位置", itemConfig.Point);
            GUILayout.EndVertical();


            int iconSize = 100;
            string prefabPath = _target.GetPrafabPath(itemConfig.Sprite);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab != null)
            {
                GUILayout.Label("", GUILayout.Width(iconSize), GUILayout.Height(iconSize));
                Rect lastRect = GUILayoutUtility.GetLastRect();
                GUI.DrawTexture(new Rect(lastRect.position, Vector2.one * iconSize), prefab.transform.Find("Image").GetComponent<Image>().sprite.texture, ScaleMode.ScaleToFit);
            }
            GUILayout.EndHorizontal();
            return itemConfig;
        }

        //保存数据
        private void SaveData()
        {
            string configContent = JsonUtility.ToJson(_target.Config);
            File.WriteAllText(_configPath, configContent);
            AssetDatabase.Refresh();
        }

    }

}
#endif