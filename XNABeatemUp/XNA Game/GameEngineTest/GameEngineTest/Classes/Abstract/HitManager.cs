using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace GameEngineTest
{
    public class HitManager
    {
        public static bool CheckHit(Particle p, Character[] c,  ParticleManager pMan)
        {
            bool r = false;
            CharDir tFace = GetFaceFromTraj(p.Trajectory);

            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] != null)
                {
                if (c[i].ID != p.Owner && c[i].TEAM != c[p.Owner].TEAM)
                {

                        if (c[i].DyingFrame < 0f && !c[i].Ethereal)
                        {
                            if (c[i].InHitBounds(p.Location))
                            {
                                //Damage
                                float hVal = 1f;

                                //Check what type of hit, respond differently
                                if (p is Rocket)
                                {
                                    Console.WriteLine(c[p.Owner].Name + " hit " + c[i].Name + " with rocket");

                                    if (tFace == CharDir.left)
                                        c[i].facing = CharDir.right;
                                    else
                                        c[i].facing = CharDir.left;

                                    pMan.MakeBloodSplash(p.Location, new Vector3(p.Trajectory.X/10,0,0));

                                    c[i].setAnim("idle");
                                    c[i].setAnim("hit");
                                    c[i].Slide(-300f);

                                    r = true;

                                    hVal *= 25;
                                }
                                else if (p is Hit)
                                {
                                    Console.WriteLine(c[p.Owner].Name + " hit " + c[i].Name + " with melee");

                                    c[i].facing = (tFace == CharDir.left) ? CharDir.right : CharDir.left;
                                    float tX = 1f;
                                    if (tFace == CharDir.left)
                                        tX = -1f;

                                    c[i].setAnim("idle");
                                    c[i].setAnim("hit");

                                    pMan.MakeBloodSplash(p.Location, new Vector3(0, 0, 0));


                                    if (c[i].state == CharState.ground)
                                        c[i].Slide(-100f);
                                    else if (c[i].state == CharState.air)
                                        c[i].Slide(-50f);

                                    switch (p.Flag)
                                    {
                                        case Character.TRIG_MELEE_DIAG_DOWN:
                                            Game1.SlowTime = 0.1f;
                                            break;

                                        case Character.TRIG_MELEE_DIAG_UP:
                                            Game1.SlowTime = 0.1f;
                                            break;

                                        case Character.TRIG_MELEE_UPPERCUT:
                                            c[i].trajectory.X = 100f * tX;
                                            c[i].setAnim("jumphit");
                                            c[i].SetJump(300f);
                                            Game1.SlowTime = 1f;
                                            c[i].useSlowTime = true;
                                            hVal *= 30;
                                            break;

                                        case Character.TRIG_MELEE_SMACKDOWN:
                                            c[i].trajectory.X = 10f * tX;
                                            hVal *= 10;
                                            break;
                                    }

                                    //Air juggling

                                    if (c[i].state == CharState.air)
                                    {
                                        if (c[i].animName == "jumphit")
                                        {
                                            c[i].setAnim("jumphit");
                                            c[i].SetJump(300f);
                                        }
                                    }

                                }

                                c[i].HP -= hVal;

                                if (hVal > 1)
                                {
                                    pMan.MakeHPHit(c[i].position, (long)hVal);
                                }

                                if (c[i].HP <= 0)
                                {
                                    if (c[i].animName == "hit")
                                        c[i].setAnim("diehit");
                                    c[i].killMe();
                                    
                                    if (c[i].ID == 0)
                                    {
                                        Game1.Menu.Die();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return r;
        }

        public static CharDir GetFaceFromTraj(Vector3 trajectory)
        {
            return (trajectory.X <= 0) ? CharDir.left : CharDir.right;
        }
    }


}
