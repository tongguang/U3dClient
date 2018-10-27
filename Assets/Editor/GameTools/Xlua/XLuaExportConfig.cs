﻿/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System.Collections.Generic;
using System;
using XLua;
using System.Reflection;
using System.Linq;

namespace U3dClient.GameTools
{
//配置的详细介绍请看Doc下《XLua的配置.doc》
    public static class ExportConfig
    {

//    [MenuItem("Test/Test1")]
//    public static void TestFunc()
//    {
//        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
//        {
//            if (assembly.FullName.Contains("UnityEngine"))
//            {
//                Debug.Log(string.Format("{0} ==== {1}", assembly.FullName, assembly.ManifestModule));
//            }
//        }
//    }

        /***************如果你全lua编程，可以参考这份自动化配置***************/
        //--------------begin 纯lua编程配置参考----------------------------
        static List<string> s_UnityExclude = new List<string>
        {
            "HideInInspector",
            "ExecuteInEditMode",
            "AddComponentMenu",
            "ContextMenu",
            "RequireComponent",
            "DisallowMultipleComponent",
            "SerializeField",
            "AssemblyIsEditorAssembly",
            "Attribute",
            "Types",
            "UnitySurrogateSelector",
            "TrackedReference",
            "TypeInferenceRules",
            "FFTWindow",
            "RPC",
            "Network",
            "MasterServer",
            "BitStream",
            "HostData",
            "ConnectionTesterStatus",
            "GUI",
            "EventType",
            "EventModifiers",
            "FontStyle",
            "TextAlignment",
            "TextEditor",
            "TextEditorDblClickSnapping",
            "TextGenerator",
            "TextClipping",
            "Gizmos",
            "ADBannerView",
            "ADInterstitialAd",
            "Android",
            "Tizen",
            "jvalue",
            "iPhone",
            "iOS",
            "Windows",
            "CalendarIdentifier",
            "CalendarUnit",
            "CalendarUnit",
            "ClusterInput",
            "FullScreenMovieControlMode",
            "FullScreenMovieScalingMode",
            "Handheld",
            "LocalNotification",
            "NotificationServices",
            "RemoteNotificationType",
            "RemoteNotification",
            "SamsungTV",
            "TextureCompressionQuality",
            "TouchScreenKeyboardType",
            "TouchScreenKeyboard",
            "MovieTexture",
            "UnityEngineInternal",
            "Terrain",
            "Tree",
            "SplatPrototype",
            "DetailPrototype",
            "DetailRenderMode",
            "MeshSubsetCombineUtility",
            "AOT",
            "Social",
            "Enumerator",
            "SendMouseEvents",
            "Cursor",
            "Flash",
            "ActionScript",
            "OnRequestRebuild",
            "Ping",
            "ShaderVariantCollection",
            "SimpleJson.Reflection",
            "CoroutineTween",
            "GraphicRebuildTracker",
            "Advertisements",
            "UnityEditor",
            "WSA",
            "EventProvider",
            "Apple",
            "ClusterInput",
            "Motion",
            "UnityEngine.UI.ReflectionMethodsCache",
            "NativeLeakDetection",
            "NativeLeakDetectionMode",
            "WWWAudioExtensions",
            "UnityEngine.Experimental",
            "UnityEngine.Purchasing",
            "UnityEngine.Analytics",
            "UnityEngine.SpatialTracking",
            "UnityEngine.TestRunner",
            "UnityEngine.XR",
        };

        static bool SIsUnityExcluded(Type type)
        {
            var fullName = type.FullName;
            for (int i = 0; i < s_UnityExclude.Count; i++)
            {
                if (fullName.Contains(s_UnityExclude[i]))
                {
                    return true;
                }
            }

            return false;
        }

        [LuaCallCSharp]
        public static IEnumerable<Type> s_LuaCallCSharp
        {
            get
            {
                var unityTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                    from type in assembly.GetExportedTypes()
                    where type.Namespace != null && type.Namespace.StartsWith("UnityEngine") && !SIsUnityExcluded(type)
                          && type.BaseType != typeof(MulticastDelegate) && !type.IsInterface && !type.IsEnum
                    select type);

                string[] customAssemblys = new string[]
                {
                    "Assembly-CSharp",
                };
                var customTypes = (from assembly in customAssemblys.Select(Assembly.Load)
                    from type in assembly.GetExportedTypes()
                    where type.Namespace == null || !type.Namespace.StartsWith("XLua")
                          && type.BaseType != typeof(MulticastDelegate) && !type.IsInterface && !type.IsEnum
                    select type);
                return unityTypes.Concat(customTypes);
            }
        }

        ////自动把LuaCallCSharp涉及到的delegate加到CSharpCallLua列表，后续可以直接用lua函数做callback
        [CSharpCallLua]
        public static List<Type> s_CSharpCallLua
        {
            get
            {
                var lua_call_csharp = s_LuaCallCSharp;
                var delegate_types = new List<Type>();
                var flag = BindingFlags.Public | BindingFlags.Instance
                                               | BindingFlags.Static | BindingFlags.IgnoreCase |
                                               BindingFlags.DeclaredOnly;
                foreach (var field in (from type in lua_call_csharp select type).SelectMany(
                    type => type.GetFields(flag)))
                {
                    if (typeof(Delegate).IsAssignableFrom(field.FieldType))
                    {
                        delegate_types.Add(field.FieldType);
                    }
                }

                foreach (var method in (from type in lua_call_csharp select type).SelectMany(type =>
                    type.GetMethods(flag)))
                {
                    if (typeof(Delegate).IsAssignableFrom(method.ReturnType))
                    {
                        delegate_types.Add(method.ReturnType);
                    }

                    foreach (var param in method.GetParameters())
                    {
                        var paramType = param.ParameterType.IsByRef
                            ? param.ParameterType.GetElementType()
                            : param.ParameterType;
                        if (typeof(Delegate).IsAssignableFrom(paramType))
                        {
                            delegate_types.Add(paramType);
                        }
                    }
                }

                return delegate_types.Distinct().ToList();
            }
        }
        //--------------end 纯lua编程配置参考----------------------------

        /***************热补丁可以参考这份自动化配置***************/
        [Hotfix]
        static IEnumerable<Type> s_HotfixInject
        {
            get
            {
                return (from type in Assembly.Load("Assembly-CSharp").GetExportedTypes()
                    where type.Namespace == null || !type.Namespace.StartsWith("XLua")
                    select type);
            }
        }

        //--------------begin 热补丁自动化配置-------------------------
        static bool SHasGenericParameter(Type type)
        {
            if (type.IsGenericTypeDefinition) return true;
            if (type.IsGenericParameter) return true;
            if (type.IsByRef || type.IsArray)
            {
                return SHasGenericParameter(type.GetElementType());
            }

            if (type.IsGenericType)
            {
                foreach (var typeArg in type.GetGenericArguments())
                {
                    if (SHasGenericParameter(typeArg))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // 配置某Assembly下所有涉及到的delegate到CSharpCallLua下，Hotfix下拿不准那些delegate需要适配到lua function可以这么配置
        [CSharpCallLua]
        static IEnumerable<Type> s_AllDelegate
        {
            get
            {
                BindingFlags flag = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static |
                                    BindingFlags.Public;
                List<Type> allTypes = new List<Type>();
                var allAssemblys = new Assembly[]
                {
                    Assembly.Load("Assembly-CSharp")
                };
                foreach (var t in (from assembly in allAssemblys from type in assembly.GetTypes() select type))
                {
                    var p = t;
                    while (p != null)
                    {
                        allTypes.Add(p);
                        p = p.BaseType;
                    }
                }

                allTypes = allTypes.Distinct().ToList();
                var allMethods = from type in allTypes
                    from method in type.GetMethods(flag)
                    select method;
                var returnTypes = from method in allMethods
                    select method.ReturnType;
                var paramTypes = allMethods.SelectMany(m => m.GetParameters()).Select(pinfo =>
                    pinfo.ParameterType.IsByRef ? pinfo.ParameterType.GetElementType() : pinfo.ParameterType);
                var fieldTypes = from type in allTypes
                    from field in type.GetFields(flag)
                    select field.FieldType;
                return (returnTypes.Concat(paramTypes).Concat(fieldTypes))
                    .Where(t => t.BaseType == typeof(MulticastDelegate) && !SHasGenericParameter(t)).Distinct();
            }
        }
        //--------------end 热补丁自动化配置-------------------------

        //黑名单
        [BlackList] public static List<List<string>> s_BlackList = new List<List<string>>()
        {
            new List<string>() {"System.Xml.XmlNodeList", "ItemOf"},
            new List<string>() {"UnityEngine.WWW", "movie"},
#if UNITY_WEBGL
                new List<string>(){"UnityEngine.WWW", "threadPriority"},
    #endif
            new List<string>() {"UnityEngine.Texture2D", "alphaIsTransparency"},
            new List<string>() {"UnityEngine.Security", "GetChainOfTrustValue"},
            new List<string>() {"UnityEngine.CanvasRenderer", "onRequestRebuild"},
            new List<string>() {"UnityEngine.Light", "areaSize"},
            new List<string>() {"UnityEngine.Light", "lightmapBakeType"},
            new List<string>() {"UnityEngine.WWW", "MovieTexture"},
            new List<string>() {"UnityEngine.WWW", "GetMovieTexture"},
            new List<string>() {"UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup"},
#if !UNITY_WEBPLAYER
            new List<string>() {"UnityEngine.Application", "ExternalEval"},
#endif
            new List<string>() {"UnityEngine.GameObject", "networkView"}, //4.6.2 not support
            new List<string>() {"UnityEngine.Component", "networkView"}, //4.6.2 not support
            new List<string>()
            {
                "System.IO.FileInfo",
                "GetAccessControl",
                "System.Security.AccessControl.AccessControlSections"
            },
            new List<string>() {"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
            new List<string>()
            {
                "System.IO.DirectoryInfo",
                "GetAccessControl",
                "System.Security.AccessControl.AccessControlSections"
            },
            new List<string>()
            {
                "System.IO.DirectoryInfo",
                "SetAccessControl",
                "System.Security.AccessControl.DirectorySecurity"
            },
            new List<string>()
            {
                "System.IO.DirectoryInfo",
                "CreateSubdirectory",
                "System.String",
                "System.Security.AccessControl.DirectorySecurity"
            },
            new List<string>() {"System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity"},
            new List<string>() {"UnityEngine.MonoBehaviour", "runInEditMode"},
        };
    }
}