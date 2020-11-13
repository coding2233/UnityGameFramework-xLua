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

namespace Wanderer.GameEditor
{
    /// <summary>
    /// lua导出配置文件
    /// </summary>
    public static class XLuaExportSettings
    {
        private static string _settingsAssemblyPath = "ProjectSettings/XLuaExportSettings.Assembly.txt";
        private static string SettingAssemblyPath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Application.dataPath), _settingsAssemblyPath);
            }
        }
        private static string _settingsNamespacePath = "ProjectSettings/XLuaExportSettings.Namespace.txt";
        private static string SettingNamespacePath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Application.dataPath), _settingsNamespacePath);
            }
        }

        private static HashSet<string> _nameSpaces = new HashSet<string>()
        {
            "UnityEngine",
            "UnityEngine.UI"
        };
        private static HashSet<string> _customAssemblys = new HashSet<string>() {
            "Assembly-CSharp",
            "GameFramework"
        };

        public static List<string> GetAssemblys()
        {
            if (File.Exists(SettingAssemblyPath))
            {
                string[] args = File.ReadAllText(SettingAssemblyPath).Split('\n');
                if (args != null)
                {
                    foreach (var item in args)
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            _customAssemblys.Add(item.Trim());
                        }
                    }
                }
            }
            return _customAssemblys.ToList(); ;
        }

        public static List<string> GetNamespaces()
        {
            if (File.Exists(SettingNamespacePath))
            {
                string[] args = File.ReadAllText(SettingNamespacePath).Split('\n');
                if (args != null)
                {
                    foreach (var item in args)
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            _nameSpaces.Add(item.Trim());
                        }
                    }
                }
            }
            return _nameSpaces.ToList(); ;
        }

        public static void SaveAssembly(List<string> assemblies)
        {
            if (assemblies != null && assemblies.Count > 0)
            {
                _customAssemblys.Clear();
                StringBuilder content = new StringBuilder();
                foreach (var item in assemblies)
                {
                    _customAssemblys.Add(item);
                    content.AppendLine(item);
                }
                SaveData(SettingAssemblyPath, content.ToString());
            }
        }

        public static void SaveNamespaces(List<string> namespaces)
        {
            if (namespaces != null && namespaces.Count > 0)
            {
                _nameSpaces.Clear();
                StringBuilder content = new StringBuilder();
                foreach (var item in namespaces)
                {
                    _nameSpaces.Add(item);
                    content.AppendLine(item);
                }
                SaveData(SettingNamespacePath, content.ToString());
            }
        }

        //保存内存
        private static void SaveData(string path, string content)
        {
            if (string.IsNullOrEmpty(content))
                return;

            if (File.Exists(path))
                File.Delete(path);

            File.WriteAllText(path, content);

            EditorUtility.DisplayDialog("Tip", "Save data", "ok");
        }



    }

    public class XLuaExportEditor : EditorWindow
    {
        private Vector2 _nameSpaceScrollPos = Vector2.zero;
        private Vector2 _customScrollPos = Vector2.zero;

        private HashSet<string> _allUnityNamespaces = new HashSet<string>();
        private List<string> _selectNamespace = new List<string>();
        private HashSet<string> _allCustomAssembly = new HashSet<string>();
        private List<string> _customAssembly = new List<string>();
        //  private string _newAssembly = "";

        [MenuItem("Tools/XLua Export Editor")]
        static void Main()
        {
            GetWindow<XLuaExportEditor>("XLuaExportEditor");
        }

        void OnEnable()
        {
            _customAssembly = XLuaExportSettings.GetAssemblys();
            _selectNamespace = XLuaExportSettings.GetNamespaces();

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

            // foreach (var item in temp)
            // {
            //     Debug.Log($"{item.FullName}");
            // }
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
            
            //xlua生成筛选
            GUILayout.BeginHorizontal();
            //需要包含的命名空间
            NamespaceDraw();
            //自定义的程序集
            CustomAssemblyDraw();
            GUILayout.EndHorizontal();
        }

        //需要包含的命名空间
        void NamespaceDraw()
        {
            GUILayout.BeginVertical("helpbox", GUILayout.Width(Screen.width * 0.66f));
            GUILayout.Label("Include Namespace", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply", GUILayout.Width(100)))
            {
                XLuaExportSettings.SaveNamespaces(_selectNamespace);
            }
            GUILayout.EndHorizontal();
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
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Apply", GUILayout.Width(100)))
                {
                    XLuaExportSettings.SaveAssembly(_customAssembly);
                }
                GUILayout.EndHorizontal();

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
            "TypeInferenceRules", "FFTWindow",
            "RPC", "Network", "MasterServer",
            "BitStream", "HostData",
            "ConnectionTesterStatus", "GUI", "EventType",
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
        };

        static bool isExcluded(Type type)
        {
            var fullName = type.FullName;
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
                                  where type.Namespace != null && namespaces.Contains(type.Namespace) && !isExcluded(type)
                                          && type.BaseType != typeof(MulticastDelegate) && !type.IsInterface && !type.IsEnum
                                  select type);

                // string[] customAssemblys = new string[] {
                //     "Assembly-CSharp",
                //     "GameFramework",
                //     "Libraries",
                // };
                List<string> customAssemblys = XLuaExportSettings.GetAssemblys();
                var customTypes = (from assembly in customAssemblys.Select(s => Assembly.Load(s))
                                   from type in assembly.GetExportedTypes()
                                   where type.Namespace == null || !type.Namespace.StartsWith("XLua")
                                           && type.BaseType != typeof(MulticastDelegate) && !type.IsInterface
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
                    if (typeof(Delegate).IsAssignableFrom(field.FieldType))
                    {
                        delegate_types.Add(field.FieldType);
                    }
                }

                foreach (var method in (from type in lua_call_csharp select type).SelectMany(type => type.GetMethods(flag)))
                {
                    if (typeof(Delegate).IsAssignableFrom(method.ReturnType))
                    {
                        delegate_types.Add(method.ReturnType);
                    }
                    foreach (var param in method.GetParameters())
                    {
                        var paramType = param.ParameterType.IsByRef ? param.ParameterType.GetElementType() : param.ParameterType;
                        if (typeof(Delegate).IsAssignableFrom(paramType))
                        {
                            delegate_types.Add(paramType);
                        }
                    }
                }
                return delegate_types.Where(t => t.BaseType == typeof(MulticastDelegate) && !hasGenericParameter(t) && !delegateHasEditorRef(t)).Distinct().ToList();
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
        //        allTypes = allTypes.Distinct().ToList();
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
    }


}
