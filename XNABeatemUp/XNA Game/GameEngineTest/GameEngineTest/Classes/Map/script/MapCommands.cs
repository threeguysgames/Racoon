using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngineTest
{
    public enum MapCommands
    {
        Fog = 0,
        Monster,
        MakeBucket,
        AddBucket,
        IfNotBucketGoto,
        Wait,
        SetFlag,
        IfTrueGoto,
        IfFalseGoto,
        SetGlobalFlag,
        IfGlobalTrueGoto,
        IfGlobalFalseGoto,
        Stop,
        Tag,
        IfScrollXGreaterGoto,
        MonsterLeft,
        MonsterRight,
        SetExit,
        SetLeftExit,
        SetRightExit,
        SetLeftEntrance,
        SetRightEntrance,
        SetEntrance,
        SetIntroEntrance

    }
}
