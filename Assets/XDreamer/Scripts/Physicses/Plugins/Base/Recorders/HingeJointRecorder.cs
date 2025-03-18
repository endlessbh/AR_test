using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginPhysicses.Base.Recorders
{
    public class HingeJointRecorder : JointRecorder<HingeJoint, HingeJointRecorder.Info>
    {

        public class Info : ISingleRecord<HingeJoint>
        {
            public HingeJoint _hingeJoint;

            private bool useMotor;

            private JointMotor motor;

            public void Record(HingeJoint hingeJoint)
            {
                useMotor = hingeJoint.useMotor;
                motor = hingeJoint.motor;
            }

            public void Recover()
            {
                _hingeJoint.useMotor = useMotor;
                _hingeJoint.motor = motor;
            }
        }
    }
}
