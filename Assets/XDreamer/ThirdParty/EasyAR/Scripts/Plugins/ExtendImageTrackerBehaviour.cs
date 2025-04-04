﻿#if XDREAMER_EASYAR_3_0_1
using easyar;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginEasyAR
{
    /// <summary>
    /// 扩展图像跟踪器
    /// </summary>
    [Name("扩展图像跟踪器")]
    public class ExtendImageTrackerBehaviour : BaseEasyARMB
    {
#if XDREAMER_EASYAR_3_0_1
        public enum CenterMode
        {
            SpecificTarget,
            FirstTarget,
            Camera
        }
        public int SimultaneousNum = 1;
        public CenterMode CenterTarget = CenterMode.Camera;
        public ExtendImageTargetController CenterImageTarget = null;
#endif

        public Camera TargetCamera = null;
#if XDREAMER_EASYAR_3_0_1
        public ImageTrackerMode Mode;
        private List<ExtendImageTargetController> targetControllers;
        private ImageTracker tracker = null;
        private Matrix4x4 centerTransform = Matrix4x4.identity;

        void Awake()
        {
            if (!ImageTracker.isAvailable())
            {
                throw new Exception("image tracker not support");
            }
            tracker = ImageTracker.createWithMode(Mode);
            tracker.setSimultaneousNum(SimultaneousNum);

            targetControllers = new List<ExtendImageTargetController>();
        }

        public FeedbackFrameSink Input()
        {
            if (tracker == null)
            {
                throw new Exception("image tracker is null");
            }
            return tracker.feedbackFrameSink();
        }

        public OutputFrameSource Output()
        {
            if (tracker == null)
            {
                throw new Exception("image tracker is null");
            }
            return tracker.outputFrameSource();
        }

        public void StartTracker()
        {
            if (tracker == null)
            {
                throw new Exception("image tracker is null");
            }
            tracker.start();
        }

        public void StopTracker()
        {
            if (tracker == null)
            {
                throw new Exception("image tracker is null");
            }
            tracker.stop();
        }

        public void CloseTracker()
        {
            if (tracker == null)
            {
                throw new Exception("image tracker is null");
            }
            tracker.close();
            tracker.Dispose();
            tracker = null;
        }

        public void UpdateFrame(ARSessionUpdateEventArgs args)
        {
            List<ExtendImageTargetController> currentTrackingControllers = new List<ExtendImageTargetController>();
            var results = args.OFrame.results();

            foreach (var _result in results)
            {
                ImageTrackerResult result = null;
                if (_result.OnSome)
                {
                    result = _result.Value as ImageTrackerResult;
                }

                if (result != null)
                {
                    var targetInstances = result.targetInstances();
                    int centerTargetId = -1;
                    if (TargetCamera == null)
                    {
                        Utility.SetMatrixOnTransform(Camera.main.transform, centerTransform);
                    }
                    else
                    {
                        Utility.SetMatrixOnTransform(TargetCamera.transform, centerTransform);
                    }
                    if (CenterImageTarget != null && CenterImageTarget.Target() != null && CenterTarget == CenterMode.SpecificTarget)
                    {
                        centerTargetId = CenterImageTarget.Target().runtimeID();
                    }
                    foreach (var targetInstance in targetInstances)
                    {
                        var target = targetInstance.target();
                        if (!target.OnSome)
                        {
                            continue;
                        }
                        var status = targetInstance.status();
                        foreach (var targetController in targetControllers)
                        {
                            var _target = targetController.Target();
                            if (target.Value.runtimeID() == _target.runtimeID())
                            {
                                if (status == TargetStatus.Tracked)
                                {
                                    if (!targetController.Tracked)
                                    {
                                        targetController.OnFound();
                                        targetController.Tracked = true;
                                    }
                                    var pose = Utility.Matrix44FToMatrix4x4(targetInstance.pose());
                                    pose = args.ImageRotationMatrixGlobal * pose;

                                    if (CenterTarget == CenterMode.FirstTarget && centerTargetId == -1)
                                    {
                                        centerTargetId = target.Value.runtimeID();
                                        CenterImageTarget = targetController;
                                    }

                                    if (centerTargetId != target.Value.runtimeID())
                                    {
                                        pose = centerTransform * pose;
                                        targetController.OnTracking(pose);
                                    }
                                    else
                                    {
                                        targetController.OnTracking(Matrix4x4.identity);
                                        centerTransform = pose.inverse;
                                    }
                                    currentTrackingControllers.Add(targetController);
                                }
                            }
                        }
                        target.Value.Dispose();
                        targetInstance.Dispose();
                    }
                    result.Dispose();
                }
            }
            foreach (var targetController in targetControllers)
            {
                bool contain = false;
                foreach (var item in currentTrackingControllers)
                {
                    if (item == targetController)
                    {
                        contain = true;
                    }
                }
                if (!contain && targetController.Tracked)
                {
                    targetController.OnLost();
                    targetController.Tracked = false;
                }
            }
        }

        public void LoadImageTarget(ExtendImageTargetController controller, System.Action<Target, bool> callback)
        {
            if (tracker == null)
            {
                throw new Exception("image tracker is null");
            }
            tracker.loadTarget(controller.Target(), EasyARBehaviour.Scheduler, callback);
            targetControllers.Add(controller);
        }

        public void UnloadImageTarget(ExtendImageTargetController controller, System.Action<Target, bool> callback)
        {
            if (tracker == null)
            {
                throw new Exception("image tracker is null");
            }
            tracker.unloadTarget(controller.Target(), EasyARBehaviour.Scheduler, callback);
            targetControllers.Remove(controller);
        }


        private void Start()
        {
            StartTracker();
        }

        private void OnDisable()
        {
            StopTracker();
        }

        private void OnDestroy()
        {
            CloseTracker();
        }
#endif
    }

    /// <summary>
    /// 基础EasyAR MB组件
    /// </summary>
    [RequireManager(typeof(EasyARManager))]
    public abstract class BaseEasyARMB : InteractProvider { }
}

