using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.Toolkit.PathWindowCore
{
    /// <summary>
    /// 路径信息管理器
    /// </summary>
    [Name("路径信息管理器")]
    [System.Serializable]
    public class PathInfoManager
    {
        public int pathCount => pathInfos.Count;

        public List<PathInfo> pathInfos = new List<PathInfo>();

        /// <summary>
        /// 当选择的路径信息切换时回调；参数1为新路径信息对象，参数2未旧路径信息对象；
        /// </summary>
        public static Action<PathInfo, PathInfo> onSelectedPathInfoChanged;

        public PathInfo selectedPathInfo
        {
            get
            {
                return _selectedPathInfo;
            }
            set
            {
                var tmp = _selectedPathInfo;
                value?.OnSelected();
                if (_selectedPathInfo == value && _selectedPathInfo != null)
                {
                    _selectedPathInfo.effective = true;

                    SetSceneView();
                }
                _selectedPathInfo = value;

                if (_selectedPathInfo != null)
                {
                    var sc = _selectedPathInfo.pathObj as StateComponent;
                    if (sc)
                    {
                        selectedPathInfoString = SMSHelper.StateToString(sc.parent);
                    }
                }

                onSelectedPathInfoChanged?.Invoke(_selectedPathInfo, tmp);
            }
        }

        public string selectedPathInfoString = "";

        private PathInfo _selectedPathInfo = null;

        /// <summary>
        /// 关联状态机
        /// </summary>
        public SubStateMachine subSM
        {
            get
            {
                return _subSM;
            }
            set
            {
                if (!_subSM || _subSM != value)
                {
                    _subSM = value;

                    Clear();
                    LoadPathFromSM();

                    var state = SMSHelper.StringToState(selectedPathInfoString);
                    if (state)
                    {
                        selectedPathInfo = pathInfos.Find(p =>
                        {
                            if (p.pathObj != null)
                            {
                                var sc = p.pathObj as StateComponent;
                                if (sc && sc.parent == state)
                                {
                                    return true;
                                }
                            }
                            return false;
                        });
                    }
                    else
                    {
                        selectedPathInfo = null;
                    }
                }
            }
        }

        private SubStateMachine _subSM = null;

        public bool smValid => subSM && !(subSM is RootStateMachine);

        public int stateCount = 0;

        public int effectivePathInfoCount => pathInfos.Count(p => p.effective);

        public List<Type> pathTypes { get; private set; } = new List<Type>();

        public void Reset()
        {
            subSM = null;
            stateCount = 0;
            _selectedPathInfo = null;
            Clear();
        }

        public void OnEnable()
        {
            Reset();
            pathTypes.Clear();
            TypeHelper.FindTypeInAppWithInterface(typeof(IPath)).ForEach(t => pathTypes.Add(t));
        }

        public void Update()
        {
            if (subSM && stateCount != subSM.states.Count)
            {
                LoadPathFromSM();
            }
        }

        public void ImportData()
        {
            pathInfos.ForEach(p=>p.ImportData());
        }

        public void ExportData()
        {
            pathInfos.ForEach(p => p.ExportData());
        }

        public void LoadPathFromSM()
        {
            if (!subSM) return;
            try
            {
                stateCount = subSM.states.Count;

                // 从状态机中获取路径动画
                List<IPath> pathMotions = new List<IPath>();
                if (PathWindowOption.weakInstance.onlyRecordMoveInCurrentStateMachine)
                {
                    subSM.states.Foreach(s =>
                    {
#if CSHARP_7_3_OR_NEWER
                        if (s.GetComponent<IPath>() is IPath p && p != null)
#else
                        var p = s.GetComponent<IPath>();
                        if (p != null)
#endif
                        {
                            pathMotions.Add(p);
                        }
                    });
                }
                else
                {
                    pathMotions = subSM.GetComponentsInChildren<IPath>().ToList();
                }

                // 删除状态机中已经不存在的路径动画
                for (int i = pathInfos.Count - 1; i >= 0; --i)
                {
                    var path = pathInfos[i].pathObj;
                    if (!pathMotions.Contains(path))
                    {
                        pathInfos.RemoveAt(i);
                    }
                }

                // 添加新的路径动画
                foreach (var path in pathMotions)
                {
                    Add(path);
                }
            }
            catch (Exception e)
            {
                Debug.Log(nameof(PathInfoManager) + ":" + e.ToString());
            }
        }

        public List<PathInfo> CreateMultipleState(IEnumerable<GameObject> gameObjects, Type type)
        {
            List<PathInfo> pathInfos = new List<PathInfo>();
            gameObjects.Foreach(go =>
            {
                if (Create(new GameObject[] { go }, type) is PathInfo pathInfo)
                {
                    pathInfos.Add(pathInfo);
                }
            });
            return pathInfos;
        }

        public PathInfo CreateSingleState(IEnumerable<GameObject> gameObjects, Type type, string name = "")
        {
            return Create(gameObjects, type, name);
        }

        public void OnRecordChange(bool recording)
        {
            // 启动录制
            if (recording)
            {
                pathInfos.ForEach(data => data.OnBeginRecord());
            }
            else // 停止录制
            {
                pathInfos.ForEach(data => data.OnEndRecord());
            }
        }

        public void RecordSelectedPath()
        {
            selectedPathInfo?.Record();
        }

        public void Record()
        {
            pathInfos.ForEach(data => data.Record());
        }

        private void SetSceneView()
        {
            // 场景视图聚焦
            if (SceneView.lastActiveSceneView)
            {
                if (_selectedPathInfo.firstTransform)
                {
                    SceneView.lastActiveSceneView.FrameSelected();
                }
                else
                {
                    SceneView.lastActiveSceneView.Frame(new Bounds(_selectedPathInfo.virtualPointPosition, Vector3.one), false);
                }
            }
        }

        public void SetDefaultSelectedPathInfo()
        {
            selectedPathInfo = pathInfos.FirstOrDefault();
        }

        public void SelectedPathInfo(GameObject gameObject)
        {
            if (gameObject)
            {
                selectedPathInfo = Find(gameObject);
            }
        }

        #region 数据创建，添加，删除和查询操作

        /// <summary>
        /// 默认状态名
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="pathType"></param>
        /// <returns></returns>
        public static string DefaultStateName(GameObject gameObject, Type pathType)
        {
            return gameObject.name + "_" + CommonFun.Name(pathType);
        }

        public PathInfo Create(IEnumerable<GameObject> gameObjects, Type pathType, string name = "")
        {
            if (gameObjects == null || gameObjects.Count() == 0 || !subSM) return null;

            var state = subSM.CreateNormalState(string.IsNullOrEmpty(name) ? DefaultStateName(gameObjects.First(), pathType) : name, pathType);
            var path = state.GetComponent<IPath>();
            if (path != null)
            {
                gameObjects.Foreach(go =>
                {
                    if (go)
                    {
                        path.AddTransform(go.transform);
                    }
                });
            }

            return Add(state.GetComponent<IPath>());
        }

        public PathInfo Add(IPath pathObj)
        {
            if (pathObj==null || Contain(pathObj)) return null;

            var mi = new PathInfo(pathObj);
            pathInfos.Add(mi);
            return mi;
        }

        public bool Remove(PathInfo data)
        {
            if (selectedPathInfo == data)
            {
                selectedPathInfo = null;
            }
            data.Delete();
            return pathInfos.Remove(data);
        }

        public void Delete()
        {
            selectedPathInfo = null;
            pathInfos.ForEach(p => p.Delete());
            Clear();
        }

        public void Clear()
        {
            pathInfos.Clear();
        }

        public void ClearPathData()
        {
            pathInfos.ForEach(d => d.Clear());
        }

        public bool Contain(IPath pathObj)
        {
            return pathInfos.Exists(data => data.pathObj == pathObj);
        }

        public PathInfo Find(IPath pathObj)
        {
            return pathInfos.Find(data => data.pathObj == pathObj);
        }

        public PathInfo Find(GameObject go)
        {
            if (!go) return null;

            return pathInfos.Find(data => data.pathObj.transforms.Contains(go.transform));
        }

        public void SetEffective(bool effective)
        {
            pathInfos.ForEach(m => m.effective = effective);
        }

        public void SetDisplayPath(bool display)
        {
            pathInfos.ForEach(m => m.displayPath = display);
        }

        public bool ExistsTransformOutofPath()
        {
            return pathInfos.Exists(pi=>pi.IsTransformOutOfPathWhenRecording()); 
        }

        #endregion

        #region 路径基础调整算法

        public void ReverseMotion() => pathInfos.ForEach(p => p.ReverseMotion());

        /// <summary>
        /// 反转路径 起点不变，路径方向和原来呈现对称负方向
        /// </summary>
        public void OppositeDirectionPath() => pathInfos.ForEach(pathData => pathData.OppositeDirectionPath());

        public void MoveToBegin() => pathInfos.ForEach(data => data.MoveToBegin());

        public void MoveToEnd() => pathInfos.ForEach(data => data.MoveToEnd());

        #endregion
    }
}
