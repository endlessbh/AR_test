using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Collections;

namespace XCSJ.EditorTools.Windows.Layouts
{
    public class RectTransformUndo : XUndo<RectTransformUndo.Cmd>
    {
        public void Record(List<RectTransform> rectTransforms, Action betweenOldAndNewWhenRecord)
        {
            if (rectTransforms == null || rectTransforms.Count == 0 || betweenOldAndNewWhenRecord == null) return;
            var cmd = new Cmd();
            Record(rectTransforms, cmd.oldInfos);
            betweenOldAndNewWhenRecord?.Invoke();
            Record(rectTransforms, cmd.newInfos);
            Record(cmd);
        }

        public void Record(List<RectTransform> rectTransforms, Func<bool> betweenOldAndNewWhenRecord)
        {
            var canRecord = rectTransforms != null && rectTransforms.Count > 0 && betweenOldAndNewWhenRecord != null;

            var old = canRecord ? TmpRecord(rectTransforms) : null;
            if (betweenOldAndNewWhenRecord() && canRecord)
            {
                var cmd = new Cmd();
                cmd.oldInfos.AddRangeWithDistinct(old);
                Record(rectTransforms, cmd.newInfos);
                Record(cmd);
            }
        }

        private Dictionary<RectTransform, Info> TmpRecord(List<RectTransform> rectTransforms)
        {
            Dictionary<RectTransform, Info> kvs = new Dictionary<RectTransform, Info>();
            foreach (var t in rectTransforms)
            {
                if (!t) continue;
                kvs[t] = new Info(t);
            }
            return kvs;
        }

        private void Record(List<RectTransform> rectTransforms, Dictionary<RectTransform, Info> kvs)
        {
            foreach (var t in rectTransforms)
            {
                if (!t) continue;
                kvs[t] = new Info(t);
            }
        }

        public class Info
        {
            public Vector2 size;
            public Vector3 position;
            public Vector3 right;

            public Info(RectTransform rectTransform)
            {
                size = rectTransform.rect.size;
                position = rectTransform.position;
                right = rectTransform.right;
            }
        }

        public class Cmd : BaseCommand<RectTransform, Info>
        {
            public override void Do(RectTransform key, Info value)
            {
                key.sizeDelta = value.size;
                key.position = value.position;
                key.right = value.right;
            }

            public override void Undo(RectTransform key, Info value) => Do(key, value);
        }
    }
}
