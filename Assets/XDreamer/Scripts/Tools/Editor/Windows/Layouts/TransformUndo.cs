using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Collections;

namespace XCSJ.EditorTools.Windows.Layouts
{
    public class TransformUndo : XUndo<TransformUndo.Cmd>
    {
        public void Record(List<Transform> transforms, Action betweenOldAndNewWhenRecord)
        {
            if (transforms == null || transforms.Count == 0 || betweenOldAndNewWhenRecord == null) return;
            var cmd = new Cmd();
            Record(transforms, cmd.oldInfos);
            betweenOldAndNewWhenRecord?.Invoke();
            Record(transforms, cmd.newInfos);
            Record(cmd);
        }

        public void Record(List<Transform> transforms, Func<bool> betweenOldAndNewWhenRecord)
        {
            var canRecord = transforms != null && transforms.Count > 0 && betweenOldAndNewWhenRecord != null;

            var old = canRecord ? TmpRecord(transforms) : null;
            if (betweenOldAndNewWhenRecord() && canRecord)
            {
                var cmd = new Cmd();
                cmd.oldInfos.AddRangeWithDistinct(old);
                Record(transforms, cmd.newInfos);
                Record(cmd);
            }
        }

        private Dictionary<Transform, Info> TmpRecord(List<Transform> transforms)
        {
            Dictionary<Transform, Info> kvs = new Dictionary<Transform, Info>();
            foreach (var t in transforms)
            {
                if (!t) continue;
                kvs[t] = new Info(t);
            }
            return kvs;
        }

        private void Record(List<Transform> transforms, Dictionary<Transform, Info> kvs)
        {
            foreach (var t in transforms)
            {
                if (!t) continue;
                kvs[t] = new Info(t);
            }
        }

        public class Info
        {
            public Vector3 position;
            public Vector3 localScale;
            public Vector3 eulerAngles;

            public Info(Transform transform)
            {
                position = transform.position;
                localScale = transform.localScale;
                eulerAngles = transform.eulerAngles;
            }
        }

        public class Cmd : BaseCommand<Transform, Info>
        {
            public override void Do(Transform key, Info value)
            {
                key.position = value.position;
                key.localScale = value.localScale;
                key.eulerAngles = value.eulerAngles;
            }

            public override void Undo(Transform key, Info value) => Do(key, value);
        }
    }
}
