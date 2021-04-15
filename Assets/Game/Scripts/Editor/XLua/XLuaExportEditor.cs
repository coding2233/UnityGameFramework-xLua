using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using XLua;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Text;
using LitJson;
using UnityEngine.Advertisements;

namespace Wanderer.GameFramework
{
    /// <summary>
    /// lua导出配置文件
    /// </summary>
    public static class XLuaExportSettings
    {
        private const string _settingName= "XLuaExportSettings.json";
        private const string _assemblyKey = "Assembly";
        private const string _nameSpaceKey = "NameSpace";
        private const string _excludeTypeKey = "ExcludeType";
        private const string _doNotGenKey = "DoNotGen";
        private const string _blackListKey = "BlackList";

        private static JsonData _config;
        public static JsonData Config
        {
            get 
            {
                if (_config == null)
                {
                    _config=ProjectSettingsConfig.LoadJsonData(_settingName);
                    if (_config == null)
                    {
                        _config = new JsonData();
                        //namespace
                        JsonData nsData = new JsonData();
                        nsData.SetJsonType(JsonType.Array);
						foreach (var item in _nameSpaces)
						{
                            nsData.Add(item);
                        }
                        _config[_nameSpaceKey] = nsData;
                        //Assembly
                        JsonData caData = new JsonData();
                        caData.SetJsonType(JsonType.Array);
                        foreach (var item in _customAssemblies)
                        {
                            caData.Add(item);
                        }
                        _config[_assemblyKey] = caData;
                    }
                }
                return _config;
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public static void Save()
        {
            ProjectSettingsConfig.SaveJsonData(_settingName, Config);
        }

        private static HashSet<string> _nameSpaces = new HashSet<string>()
        {
            "UnityEngine",
            "UnityEngine.UI"
        };
        private static HashSet<string> _customAssemblies = new HashSet<string>() {
            "Assembly-CSharp",
            "GameFramework"
        };

        /// <summary>
        /// 获取所有的命名空间
        /// </summary>
        /// <returns></returns>
        public static List<string> GetNamespaces()
        {
            return GetData(_nameSpaceKey);
        }

        /// <summary>
        /// 获取所有的程序集
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAssemblies()
        {
            return GetData(_assemblyKey);
        }

        /// <summary>
        /// 获取排除在外的类型
        /// </summary>
        /// <returns></returns>
        public static List<string> GetExcludeTypes()
        {
            return GetData(_excludeTypeKey);
        }

        /// <summary>
        /// DoNotGen
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDoNotGen()
        {
            return GetData(_doNotGenKey);
        }

        /// <summary>
        /// BlackList
        /// </summary>
        /// <returns></returns>
        public static List<string> GetBlackList()
        {
            return GetData(_blackListKey);
        }


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<string> GetData(string key)
        {
            if (Config.Keys.Contains(key))
            {
                List<string> datas = new List<string>();
                JsonData caData = Config[key];
                for (int i = 0; i < caData.Count; i++)
                {
                    datas.Add((string)caData[i]);
                }
                return datas;
            }
            return null;
        }


        /// <summary>
        /// 更新命名空间
        /// </summary>
        /// <param name="data"></param>
        public static void UpdateNamespaces(List<string> data)
        {
            UpdateData(_nameSpaceKey, data);
        }


        /// <summary>
        /// 更新程序集
        /// </summary>
        /// <param name="data"></param>
        public static void UpdateAssemblies(List<string> data)
        {
            UpdateData(_assemblyKey, data);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void UpdateData(string key, List<string> data)
        {
            if (data != null && data.Count > 0)
            {
                JsonData jsonData = Config[key];
                jsonData.Clear();
                foreach (var item in data)
                {
                    jsonData.Add(item);
                }
            }
        }
    }

    public class XLuaExportEditor : EditorWindow
    {
        private Vector2 _nameSpaceScrollPos = Vector2.zero;
        private Vector2 _customScrollPos = Vector2.zero;
        private Vector2 _excludeTypesScrollPos = Vector2.zero;
        private Vector2 _xluaDoNotGenScrollPos = Vector2.zero;
        private Vector2 _xluaBlackListScrollPos = Vector2.zero;
        // 命名空间 自定义程序集
        private HashSet<string> _allUnityNamespaces = new HashSet<string>();
        private List<string> _selectNamespace = new List<string>();
        private HashSet<string> _allCustomAssembly = new HashSet<string>();
        private List<string> _customAssembly = new List<string>();
        //other
        //private List<string> _excludeTypes = new List<string>();
        //private List<string> _doNotGen = new List<string>();
        //private List<string> _blackList = new List<string>();

        //     private List<string> 
        //  private string _newAssembly = "";

        [MenuItem("Tools/XLua Export Editor")]
        static void Main()
        {
            GetWindow<XLuaExportEditor>("XLuaExportEditor");
        }

        void OnEnable()
        {
            _selectNamespace = XLuaExportSettings.GetNamespaces();
            _customAssembly = XLuaExportSettings.GetAssemblies();
            //XLuaExportSettings.GetExcludeTypes();
            //XLuaExportSettings.GetDoNotGen();
            //XLuaExportSettings.GetBlackList();

            _allUnityNamespaces.Clear();
            var unityNamespaces = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                                   from type in assembly.GetExportedTypes()
                                   where type.Namespace != null
                                   select type.Namespace);
            foreach (var item in unityNamespaces)
            {
                _allUnityNamespaces.Add(item);
            }

            // var temp = from assembly in AppDomain.CurrentDomain.GetAssemblies()
            //            where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder) && !assembly.FullName.StartsWith("Unity")
            //            select assembly;

            _allCustomAssembly.Clear();
            var compilationAssemblies = UnityEditor.Compilation.CompilationPipeline.GetAssemblies();
            foreach (var item in compilationAssemblies)
            {
                if (item.flags != UnityEditor.Compilation.AssemblyFlags.EditorAssembly)
                {
                    _allCustomAssembly.Add(item.name);
                }
            }


        }

        void OnDisable()
        {

        }

        void OnGUI()
        {
            //xlua的生成工具
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Generated Code"))
            {
                CSObjectWrapEditor.Generator.ClearAll();
            }
            if (GUILayout.Button("Generate Code"))
            {
                CSObjectWrapEditor.Generator.GenAll();
            }
            GUILayout.EndHorizontal();
            //保存数据
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Apply", GUILayout.Width(100)))
			{
                XLuaExportSettings.UpdateNamespaces(_selectNamespace);
                XLuaExportSettings.UpdateAssemblies(_customAssembly);
                XLuaExportSettings.Save();
                EditorUtility.DisplayDialog("Tip", "Save data", "OK");
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginVertical();
            //xlua生成筛选
            GUILayout.BeginHorizontal();
            //需要包含的命名空间
            NamespaceDraw();
            //自定义的程序集
            CustomAssemblyDraw();
            GUILayout.EndHorizontal();

           
            GUILayout.BeginHorizontal(GUILayout.Height(Screen.height*0.33f));
            //排除类型
            ExcludeTypesDraw();
            //xlua DoNotGen
            XLuaDoNotGenDraw();
            //xlua BlackList
            XLuaBlackListDraw();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        //需要包含的命名空间
        void NamespaceDraw()
        {
            GUILayout.BeginVertical("helpbox", GUILayout.Width(Screen.width * 0.66f));
            GUILayout.Label("Include Namespace", EditorStyles.boldLabel);
   //         GUILayout.BeginHorizontal();
   //         GUILayout.FlexibleSpace();
			//if (GUILayout.Button("Apply", GUILayout.Width(100)))
			//{
			//	XLuaExportSettings.SaveNamespaces(_selectNamespace);
			//}
			//GUILayout.EndHorizontal();
            _nameSpaceScrollPos = GUILayout.BeginScrollView(_nameSpaceScrollPos, "helpbox");

            foreach (var item in _allUnityNamespaces)
            {
                bool select = _selectNamespace.Contains(item);
                bool result = GUILayout.Toggle(select, item);
                if (select != result)
                {
                    if (result)
                    {
                        _selectNamespace.Add(item);
                    }
                    else
                    {
                        _selectNamespace.Remove(item);
                    }
                }
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }


        //自定义的程序集
        void CustomAssemblyDraw()
        {
            GUILayout.BeginVertical("helpbox");
            GUILayout.Label("Custom Assembly", EditorStyles.boldLabel);
            if (_customAssembly != null && _customAssembly.Count > 0)
            {
                //GUILayout.BeginHorizontal();
                //GUILayout.FlexibleSpace();
                //if (GUILayout.Button("Apply", GUILayout.Width(100)))
                //{
                //    XLuaExportSettings.SaveAssembly(_customAssembly);
                //}
                //GUILayout.EndHorizontal();

                _customScrollPos = GUILayout.BeginScrollView(_customScrollPos, "helpbox");

                foreach (var item in _allCustomAssembly)
                {
                    bool select = _customAssembly.Contains(item);
                    bool result = GUILayout.Toggle(select, item);
                    if (select != result)
                    {
                        if (result)
                        {
                            _customAssembly.Add(item);
                        }
                        else
                        {
                            _customAssembly.Remove(item);
                        }
                    }
                }
                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();
        }

    
        //排除类型
        void ExcludeTypesDraw()
        {
            GUILayout.BeginVertical("helpbox");
            GUILayout.Label("Exclude Types", EditorStyles.boldLabel);
            _excludeTypesScrollPos = GUILayout.BeginScrollView(_excludeTypesScrollPos, "helpbox");
            //    GUILayout.SelectionGrid
            GUILayout.Label("Waiting for development...");
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }
        //xlua DoNotGen
        void XLuaDoNotGenDraw()
        {
            GUILayout.BeginVertical("helpbox");
            GUILayout.Label("DoNotGen", EditorStyles.boldLabel);

            _xluaDoNotGenScrollPos = GUILayout.BeginScrollView(_xluaDoNotGenScrollPos, "helpbox");
            GUILayout.Label("Waiting for development...");
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }
        //xlua BlackList
        void XLuaBlackListDraw()
        {
            GUILayout.BeginVertical("helpbox");
            GUILayout.Label("Black List", EditorStyles.boldLabel);

            _xluaBlackListScrollPos = GUILayout.BeginScrollView(_xluaBlackListScrollPos, "helpbox");
            GUILayout.Label("Waiting for development...");
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }



    }


    /// <summary>
    /// xlua导出配置
    /// </summary>
    public static class XLuaExportConfig
    {
        /***************如果你全lua编程，可以参考这份自动化配置***************/
        //--------------begin 纯lua编程配置参考----------------------------
        static List<string> exclude = new List<string> {
            "HideInInspector", "ExecuteInEditMode",
            "AddComponentMenu", "ContextMenu",
            "RequireComponent", "DisallowMultipleComponent",
            "SerializeField", "AssemblyIsEditorAssembly",
            "Attribute", "Types",
            "UnitySurrogateSelector", "TrackedReference",
            "TypeInferenceRules", "UnityEngine.FFTWindow",
            "RPC", "Network", "MasterServer",
            "BitStream", "HostData",
            "ConnectionTesterStatus", "UnityEngine.GUI", "UnityEngine.EventType",
            "EventModifiers", "FontStyle", "TextAlignment",
            "TextEditor", "TextEditorDblClickSnapping",
            "TextGenerator", "TextClipping", "Gizmos",
            "ADBannerView", "ADInterstitialAd",
            "Android", "Tizen", "jvalue",
            "iPhone", "iOS", "Windows", "CalendarIdentifier",
            "CalendarUnit", "CalendarUnit",
            "ClusterInput", "FullScreenMovieControlMode",
            "FullScreenMovieScalingMode", "Handheld",
            "LocalNotification", "NotificationServices",
            "RemoteNotificationType", "RemoteNotification",
            "SamsungTV", "TextureCompressionQuality",
            "TouchScreenKeyboardType", "TouchScreenKeyboard",
            "MovieTexture", "UnityEngineInternal",
            "Terrain", "Tree", "SplatPrototype",
            "DetailPrototype", "DetailRenderMode",
            "MeshSubsetCombineUtility", "AOT", "Social", "Enumerator",
            "SendMouseEvents", "Cursor", "Flash", "ActionScript",
            "OnRequestRebuild", "Ping",
            "ShaderVariantCollection", "SimpleJson.Reflection",
            "CoroutineTween", "GraphicRebuildTracker",
            "Advertisements", "UnityEditor", "WSA",
            "EventProvider", "Apple",
            "ClusterInput", "Motion",
            "UnityEngine.UI.ReflectionMethodsCache", "NativeLeakDetection",
            "NativeLeakDetectionMode", "WWWAudioExtensions", "UnityEngine.Experimental",
            "EditorAssetsHelper","UnityEngine.CanvasRenderer.OnRequestRebuild","EditorCall"
        };

        //是否属于当前的类
        static bool isExcluded(Type type)
        {
            var fullName = type.FullName==null? type.Name: type.FullName;
            for (int i = 0; i < exclude.Count; i++)
            {
                if (fullName.Contains(exclude[i]))
                {
                    return true;
                }
            }
            return false;
        }

        [LuaCallCSharp]
        public static IEnumerable<Type> LuaCallCSharp
        {
            get
            {
                // List<string> namespaces = new List<string>() // 在这里添加名字空间
                // {
                //     "UnityEngine",
                //     "UnityEngine.UI"
                // };
                List<string> namespaces = XLuaExportSettings.GetNamespaces();
                var unityTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                  where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                                  from type in assembly.GetExportedTypes()
                                  where (type.Namespace != null && namespaces.Contains(type.Namespace)) && !isExcluded(type)
                                          && type.BaseType != typeof(MulticastDelegate) && !type.IsInterface && !type.IsEnum
                                  select type);

                // string[] customAssemblys = new string[] {
                //     "Assembly-CSharp",
                //     "GameFramework",
                //     "Libraries",
                // };

                List<string> customAssemblys = XLuaExportSettings.GetAssemblies();
                var customTypes = (from assembly in customAssemblys.Select(s => Assembly.Load(s))
                                   from type in assembly.GetExportedTypes()
                                   where (type.Namespace == null || !type.Namespace.StartsWith("XLua")) && !isExcluded(type)
                                         && type.BaseType != typeof(MulticastDelegate)
                                   select type);
                return unityTypes.Concat(customTypes);
            }
        }

        //自动把LuaCallCSharp涉及到的delegate加到CSharpCallLua列表，后续可以直接用lua函数做callback
        [CSharpCallLua]
        public static List<Type> CSharpCallLua
        {
            get
            {
                var lua_call_csharp = LuaCallCSharp;
                var delegate_types = new List<Type>();
                var flag = BindingFlags.Public | BindingFlags.Instance
                    | BindingFlags.Static | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly;
                foreach (var field in (from type in lua_call_csharp select type).SelectMany(type => type.GetFields(flag)))
                {
                    if (typeof(Delegate).IsAssignableFrom(field.FieldType) && !isExcluded(field.FieldType))
                    {
                        delegate_types.Add(field.FieldType);
                    }
                }

                foreach (var method in (from type in lua_call_csharp select type).SelectMany(type => type.GetMethods(flag)))
                {
                    if (typeof(Delegate).IsAssignableFrom(method.ReturnType) && !isExcluded(method.ReturnType))
                    {
                        delegate_types.Add(method.ReturnType);
                    }
                    foreach (var param in method.GetParameters())
                    {
                        var paramType = param.ParameterType.IsByRef ? param.ParameterType.GetElementType() : param.ParameterType;

                        if (typeof(Delegate).IsAssignableFrom(paramType) && !isExcluded(paramType))
                        {
                            delegate_types.Add(paramType);
                        }
                    }
                }
                return delegate_types.Where(t => t.BaseType == typeof(MulticastDelegate) && !hasGenericParameter(t) && !delegateHasEditorRef(t)).Distinct().ToList();
            }
        }

        // CSharpCallLua 静态列表
		[CSharpCallLua]
		public static List<Type> CSharpCallLuaAction = new List<Type>() {
				typeof(Action),
				typeof(Action<int>),
				typeof(Action<float>),
				typeof(Action<double>),
                typeof(Action<string>),
                typeof(Action<Vector2>),
				typeof(Action<Vector3>),
                typeof(Action<ShowResult>),
                typeof(Action<RectTransform>),
				typeof(UnityEngine.Events.UnityAction),
				typeof(UnityEngine.Events.UnityAction<int>),
				typeof(UnityEngine.Events.UnityAction<float>),
				typeof(UnityEngine.Events.UnityAction<double>),
                typeof(UnityEngine.Events.UnityAction<string>),
                typeof(UnityEngine.Events.UnityAction<Vector2>),
				typeof(UnityEngine.Events.UnityAction<Vector3>),
				typeof(System.Collections.IEnumerator),
	};

    // LuaCallCsharp 静态列表
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharpTypes = new List<Type>() {
            typeof(System.DateTime)
    };

        [DoNotGen]
    public static Dictionary<Type, List<string>> XLuaDoNotGen
    {
		get
		{
            Dictionary<Type, List<string>> dtl = new Dictionary<Type, List<string>>();
            //dtl.Add(typeof(UnityEngine.CanvasRenderer), new List<string>() { "OnRequestRebuild", "onRequestRebuild" });
            return dtl;
        }
    }

        //--------------end 纯lua编程配置参考----------------------------

        /***************热补丁可以参考这份自动化配置***************/
        //[Hotfix]
        //static IEnumerable<Type> HotfixInject
        //{
        //    get
        //    {
        //        return (from type in Assembly.Load("Assembly-CSharp").GetTypes()
        //                where type.Namespace == null || !type.Namespace.StartsWith("XLua")
        //                select type);
        //    }
        //}
        //--------------begin 热补丁自动化配置-------------------------
        static bool hasGenericParameter(Type type)
        {
            if (type.IsGenericTypeDefinition) return true;
            if (type.IsGenericParameter) return true;
            if (type.IsByRef || type.IsArray)
            {
                return hasGenericParameter(type.GetElementType());
            }
            if (type.IsGenericType)
            {
                foreach (var typeArg in type.GetGenericArguments())
                {
                    if (hasGenericParameter(typeArg))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static bool typeHasEditorRef(Type type)
        {
            if (type.Namespace != null && (type.Namespace == "UnityEditor" || type.Namespace.StartsWith("UnityEditor.")))
            {
                return true;
            }
            if (type.IsNested)
            {
                return typeHasEditorRef(type.DeclaringType);
            }
            if (type.IsByRef || type.IsArray)
            {
                return typeHasEditorRef(type.GetElementType());
            }
            if (type.IsGenericType)
            {
                foreach (var typeArg in type.GetGenericArguments())
                {
                    if (typeHasEditorRef(typeArg))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static bool delegateHasEditorRef(Type delegateType)
        {
            if (typeHasEditorRef(delegateType)) return true;
            var method = delegateType.GetMethod("Invoke");
            if (method == null)
            {
                return false;
            }
            if (typeHasEditorRef(method.ReturnType)) return true;
            return method.GetParameters().Any(pinfo => typeHasEditorRef(pinfo.ParameterType));
        }

        // 配置某Assembly下所有涉及到的delegate到CSharpCallLua下，Hotfix下拿不准那些delegate需要适配到lua function可以这么配置
        //[CSharpCallLua]
        //static IEnumerable<Type> AllDelegate
        //{
        //    get
        //    {
        //        BindingFlags flag = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
        //        List<Type> allTypes = new List<Type>();
        //        var allAssemblys = new Assembly[]
        //        {
        //            Assembly.Load("Assembly-CSharp")
        //        };
        //        foreach (var t in (from assembly in allAssemblys from type in assembly.GetTypes() select type))
        //        {
        //            var p = t;
        //            while (p != null)
        //            {
        //                allTypes.Add(p);
        //                p = p.BaseType;
        //            }
        //        }
        //        allTypes = allTypes.Distinct().ToList<string>;
        //        var allMethods = from type in allTypes
        //                         from method in type.GetMethods(flag)
        //                         select method;
        //        var returnTypes = from method in allMethods
        //                          select method.ReturnType;
        //        var paramTypes = allMethods.SelectMany(m => m.GetParameters()).Select(pinfo => pinfo.ParameterType.IsByRef ? pinfo.ParameterType.GetElementType() : pinfo.ParameterType);
        //        var fieldTypes = from type in allTypes
        //                         from field in type.GetFields(flag)
        //                         select field.FieldType;
        //        return (returnTypes.Concat(paramTypes).Concat(fieldTypes)).Where(t => t.BaseType == typeof(MulticastDelegate) && !hasGenericParameter(t) && !delegateHasEditorRef(t)).Distinct();
        //    }
        //}
        //--------------end 热补丁自动化配置-------------------------


        //黑名单
        [BlackList]
        public static List<List<string>> BlackList  {
            get
            {
                List<List<string>> bl = new List<List<string>>()  
                {
                    new List<string>{"UnityEngine.UI.Text", "OnRebuildRequested"},
                    new List<string>{"UnityEngine.UI.Graphic", "OnRebuildRequested"},
                    new List<string>{"UnityEngine.AnimatorControllerParameter", "name"},
                    new List<string>{"UnityEngine.AudioSettings", "GetSpatializerPluginNames"},
                    new List<string>{"UnityEngine.AudioSettings", "SetSpatializerPluginName", "System.String"},
                    new List<string>{"UnityEngine.DrivenRectTransformTracker", "StopRecordingUndo"},
                    new List<string>{"UnityEngine.DrivenRectTransformTracker", "StartRecordingUndo"},
                    new List<string>{"UnityEngine.Caching", "SetNoBackupFlag", "UnityEngine.CachedAssetBundle"},
                    new List<string>{"UnityEngine.Caching", "ResetNoBackupFlag", "UnityEngine.CachedAssetBundle"},
                    new List<string>{"UnityEngine.Caching", "SetNoBackupFlag", "System.String", "UnityEngine.Hash128"},
                    new List<string>{"UnityEngine.Caching", "ResetNoBackupFlag", "System.String", "UnityEngine.Hash128"},
                    new List<string>{"UnityEngine.Input", "IsJoystickPreconfigured", "System.String"},
                    new List<string>{"UnityEngine.LightProbeGroup", "dering"},
                    new List<string>{"UnityEngine.LightProbeGroup", "probePositions"},
                    new List<string>{"UnityEngine.Light", "SetLightDirty"},
                    new List<string>{"UnityEngine.Light", "shadowRadius"},
                    new List<string>{"UnityEngine.Light", "shadowAngle"},
                    new List<string>{"UnityEngine.ParticleSystemForceField", "FindAll"},
                    new List<string>{"UnityEngine.QualitySettings", "streamingMipmapsRenderersPerFrame"},
                    new List<string>{"UnityEngine.Texture", "imageContentsHash"},
                    new List<string>{ "UnityEngine.AssetBundle", "SetAssetBundleDecryptKey" ,"System.String"},
                    new List<string>{ "UnityEngine.BuildCompression", "enableProtect" },
                    new List<string>{ "UnityEngine.MeshRenderer", "scaleInLightmap" },
                    new List<string>{ "UnityEngine.MeshRenderer", "receiveGI" },
                    new List<string>{ "UnityEngine.MeshRenderer", "stitchLightmapSeams" },
                    new List<string>{ "UnityEngine.ParticleSystemRenderer", "supportsMeshInstancing" },
                    new List<string>{ "UnityEngine.UI.DefaultControls", "factory" },
                    new List<string>(){ "RenderHeads.Media.AVProVideo.MediaPlayer", "SaveFrameToPng"},
                    new List<string>(){ "RenderHeads.Media.AVProVideo.MediaPlayer", "GetPlatformOptions","RenderHeads.Media.AVProVideo.Platform"},
					new List<string>(){ "RenderHeads.Media.AVProVideo.MediaPlayer", "GetPlatformOptionsVariable", "RenderHeads.Media.AVProVideo.Platform"},
					new List<string>(){ "RenderHeads.Media.AVProVideo.MediaPlayer", "GetPath", "RenderHeads.Media.AVProVideo.MediaPlayer.FileLocation"},
					new List<string>(){ "RenderHeads.Media.AVProVideo.MediaPlayer", "GetFilePath", "System.String", "RenderHeads.Media.AVProVideo.MediaPlayer.FileLocation"},
					new List<string>(){ "Wanderer.GameFramework.MathLevelItem", "LevelItem"},
					new List<string>(){ "Wanderer.GameFramework.MathLevelItem", "newStr"},
					new List<string>(){ "Wanderer.GameFramework.MathLevelItem", "newTypeName"},
					new List<string>(){ "Wanderer.GameFramework.MathLevelItem", "typeNameOne"},
                };
                return bl;
            }
        }

    }
}
