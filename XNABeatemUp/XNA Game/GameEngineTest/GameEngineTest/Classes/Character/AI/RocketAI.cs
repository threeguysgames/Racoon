using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngineTest
{
    public class RocketAI : AI
    {
        public override void Update(Character[] c, int Id, Map map)
        {
            me = c[Id];

            if (jobFrame < 0f)
            {
                float r = Rand.GetRandomFloat(0f, 1f);

                if (r < 0.6f)
                {
                    job = AI_JOB.JOB_ROCKET_CHASE;
                    jobFrame = Rand.GetRandomFloat(1f, 4f);
                    targID = FindTarg(c);
                }
                else if (r < 0.61f)
                {
                    job = AI_JOB.JOB_AVOID;
                    jobFrame = Rand.GetRandomFloat(1f, 2f);
                    targID = FindTarg(c);
                }
                else
                {
                    job = AI_JOB.JOB_IDLE;
                    jobFrame = Rand.GetRandomFloat(.25f, .5f);
                }
            }

            base.Update(c, Id, map);
        }
    }
}
