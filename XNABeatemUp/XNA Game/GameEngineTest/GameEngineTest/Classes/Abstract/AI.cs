using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngineTest
{
    public class AI
    {
        public enum AI_JOB
        {
            JOB_IDLE = 0,
            JOB_MELEE_CHASE = 1,
            JOB_SHOOT_CHASE = 2,
            JOB_AVOID = 3,
            JOB_ROCKET_CHASE = 4
        }

        protected AI_JOB job = AI_JOB.JOB_IDLE;
        protected int targID = -1;
        protected float jobFrame = 0f;
        protected CharDir attackSide = CharDir.right;

        protected Character me;

        public virtual void Update(Character[] c, int Id, Map map)
        {
            me = c[Id];
            me.keyLeft = false;
            me.keyRight = false;
            me.keyUp = false;
            me.keyDown = false;
            me.meleeDown = false;
            me.secondDown = false;
            me.jumpDown = false;

            jobFrame -= Game1.frameTime;

            DoJob(c, Id);
        }

        protected void DoJob(Character[] c, int Id)
        {
            switch (job)
            {
                case AI_JOB.JOB_IDLE:
                    //do nothing
                    break;
                    
                case AI_JOB.JOB_MELEE_CHASE:
                    

                    if (targID > -1)
                    {
                        FaceTarg(c);

                        if (targID > -1)
                        {
                            if (!ChaseTarg(c, 80f, 10f))
                            {
                                if(targID > -1)
                                    me.secondDown = true;
                            }
                        }
                        else
                            FindTarg(c);
                    }
                    else
                        targID = FindTarg(c);
                    break;

                case AI_JOB.JOB_ROCKET_CHASE:

                    if (targID > -1)
                    {
                        FaceTarg(c);

                        if (targID > -1)
                        {
                            if (!ChaseTarg(c, 400f, 5f))
                            {
                                if (targID > -1)
                                    me.meleeDown = true;
                            }
                        }
                        else
                            FindTarg(c);
                    }
                    else
                        targID = FindTarg(c);
                    break;

                case AI_JOB.JOB_AVOID:
                    if (targID > -1)
                    {
                        AvoidTarg(c, 200f, 50f);
                    }
                    else
                        targID = FindTarg(c);
                    break;
            }

        }

        protected void ChooseSide()
        {
            float r = Rand.GetRandomFloat(0f, 1f);

            if (r < 0.5f)
                attackSide = CharDir.left;
            else
                attackSide = CharDir.right;
                
        }

        protected int FindTarg(Character[] c)
        {
            int closest = -1;
            float d = 0f;

            for (int i = 0; i < c.Length; i++)
            {
                if (i != me.ID)
                {
                    if (c[i] != null)
                    {
                        if (c[i].TEAM != me.TEAM)
                        {
                            //Find distance to character
                            float newD = (me.position - c[i].position).Length();

                            if (closest == -1 || newD < d)
                            {
                                d = newD;
                                closest = c[i].ID;
                            }
                        }

                    }

                }
            }

            return closest;
        }

        protected bool ChaseTarg(Character[] c, float XDistance, float YDistance)
        {
            if (c[targID] == null)
            {
                targID = -1;
                return false;
            }

            bool flag = false;
          

            if (attackSide == CharDir.left)
                XDistance *= -1;

            if (me.position.X > c[targID].position.X + XDistance)
            {
                me.keyLeft = true;
                flag = true;
            }
            if (me.position.X < c[targID].position.X - XDistance)
            {
                me.keyRight = true;
                flag = true;
            }
            if (me.position.Y > c[targID].position.Y + YDistance)
            {
                me.keyUp = true;
                flag = true;
            }
            if (me.position.Y < c[targID].position.Y - YDistance)
            {
                me.keyDown = true;
                flag = true;
            }

            return flag;
        }

        protected bool AvoidTarg(Character[] c, float XDistance, float YDistance)
        {
            bool flag = false;
            if (c[targID] == null)
            {
                targID = -1;
                return false;
            }

            if (me.position.X < c[targID].position.X + XDistance)
            {
                me.keyRight = true;
                me.facing = CharDir.right;
                flag = true;
            }
            if (me.position.Y < c[targID].position.Y + YDistance)
            {
                me.keyDown = true;
                flag = true;
            }

            return flag;
        }

        protected bool FaceTarg(Character[] c)
        {
            if (c[targID] == null)
            {
                targID = -1;
                return false;
            }

            if (me.position.X > c[targID].position.X && me.facing == CharDir.right)
            {
                me.keyLeft = true;
                me.facing = CharDir.left;
                return true;
            }

            if (me.position.X < c[targID].position.X && me.facing == CharDir.left)
            {
                me.keyRight = true;
                me.facing = CharDir.right;
                return true;
            }

            return false;
        }
    }
}
