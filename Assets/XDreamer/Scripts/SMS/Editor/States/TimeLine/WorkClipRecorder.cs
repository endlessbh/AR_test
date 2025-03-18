using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.Maths;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.EditorSMS.States.TimeLine
{
    public class WorkClipRecorder
    {
        public double beginTime = 0;
        public double endTime = 0;
        public double timeLength = 0;

        public double beginPercent = 0;
        public double endPercent = 0;
        public double percentLength => endPercent - beginPercent;

        public double totalTimeLength { get; private set; }

        public UnityEngine.Object obj { get; private set; }

        public WorkClipRecorder(UnityEngine.Object obj) { this.obj = obj; }

        public WorkClipRecorder(UnityEngine.Object obj, IWorkClip workClip, double totalTimeLength)
        {
            this.obj = obj;
            Record(workClip, totalTimeLength);
        }

        public void Record(IWorkClip workClip, double totalTimeLength)
        {
            beginTime = workClip.beginTime;
            endTime = workClip.endTime;
            timeLength = workClip.timeLength;

            beginPercent = workClip.beginPercent;
            endPercent = workClip.endPercent;
            //percentLength = workClip.percentLength;

            this.totalTimeLength = totalTimeLength;
        }

        public void Recover(IWorkClip workClip)
        {
            UndoHelper.RegisterCompleteObjectUndo(obj);

            workClip.endPercent = MathX.Clamp(endPercent, 0, 1);
            workClip.beginPercent = MathX.Clamp(beginPercent, 0, endPercent);
            //workClip.percentLength = workClip.endPercent - workClip.beginPercent;

            workClip.beginTime = beginTime;
            workClip.endTime = endTime;
            //workClip.timeLength = workClip.endTime - workClip.beginTime;
        }

        public void KeepPercent()
        {
            beginTime = totalTimeLength * beginPercent;
            endTime = totalTimeLength * endPercent;
        }

        public void KeepBeginTime()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginPercent = beginTime / totalTimeLength;
            endTime = totalTimeLength * endPercent;
        }

        public void KeepEndTime()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endPercent = endTime / totalTimeLength;
            beginTime = totalTimeLength * beginPercent;            
        }

        public void KeepTimeLengthAndBeginPercent()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginTime = totalTimeLength * beginPercent;
            endTime = beginTime + timeLength;
            endPercent = endTime / totalTimeLength;
        }

        public void KeepTimeLengthAndEndPercent()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endTime = totalTimeLength * endPercent;
            beginTime = endTime - timeLength;
            beginPercent = beginTime / totalTimeLength;
        }

        public void KeepTime()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginPercent = beginTime / totalTimeLength;
            endPercent = endTime / totalTimeLength;
        }

        public void SetBeginTime(double newBeginTime)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginTime = newBeginTime;
            beginPercent = beginTime / totalTimeLength;
        }

        public void SetBeginPercent(double newBeginPercent)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginPercent = newBeginPercent;
            beginTime = beginPercent * totalTimeLength;
        }

        public void KeepTimeLengthOnBeginTime()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endTime = beginTime + timeLength;
            endPercent = endTime / totalTimeLength;
        }

        public void SetEndPercent(double newEndnPercent)
        {
            endPercent = newEndnPercent;
            endTime = endPercent * totalTimeLength;
        }

        public void KeepTimeLengthOnEndTime()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginTime = endTime - timeLength;
            beginPercent = beginTime / totalTimeLength;
        }

        public void SetEndTime(double newEndTime)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endTime = newEndTime;
            endPercent = endTime / totalTimeLength;
        }

        public void SetTimeLength(double newTimeLength)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endTime = beginTime + newTimeLength;

            if (endTime > totalTimeLength)
            {
                double offset = endTime - totalTimeLength;
                endTime = totalTimeLength;
                beginTime = MathX.Clamp(beginTime - offset, 0, endTime);
            }

            beginPercent = beginTime / totalTimeLength;
            endPercent = endTime / totalTimeLength;
        }

        public void OnTimeLengthChangeFixBeginTime(double newTimeLength)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endTime = beginTime + newTimeLength;
            endTime = MathX.Clamp(endTime, beginTime, totalTimeLength);

            endPercent = endTime / totalTimeLength;
        }

        public void OnTimeLengthChangeFixEndTime(double newTimeLength)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginTime = endTime - newTimeLength;
            beginTime = MathX.Clamp(beginTime, 0, endTime);

            beginPercent = beginTime / totalTimeLength;
        }
    }
}
