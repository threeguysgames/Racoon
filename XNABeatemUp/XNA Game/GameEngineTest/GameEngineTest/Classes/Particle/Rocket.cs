using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace GameEngineTest
{
    class Rocket : Particle
    {
        public CharDir face;
        public Rocket(Vector3 loc, Vector3 traj, int ownerID, CharDir facing)
        {
            location = loc;
            trajectory = traj;
            this.owner = ownerID;

            Vector2 traj_ = new Vector2(traj.X, traj.Y);

            rotation = GlobalFunctions.GetAngle(Vector2.Zero, traj_);

            Exists = true;
            frame = 5.0f;
            Additive = false;
            face = facing;
            
        }

        public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c)
        {
            location.Z += trajectory.Z * gameTime;
            
            if(HitManager.CheckHit(this,c,pMan))
            {
                frame = 0f;
            }

            if (CheckParticleCol())
            {
                QuakeManager.SetQuake(.5f);
                for (int i = 0; i < 12; i++)
                {
                    pMan.AddParticle(new RocketDust(
                        location,
                        Rand.GetRandomVector3(
                            -40f, 40f, -30f, -30f),
                        1.0f, 0.8f, 0.6f, 1.0f,
                        Rand.GetRandomFloat(0.25f, 0.5f),
                        Rand.GetRandomInt(0, 4)), true);
                }

                KillMe();
            }

            for(int i = 0; i < 2; i++)
            {
                pMan.AddParticle(new Smoke(location,
                            Rand.GetRandomVector3(
                            -100f, -70f, -30f, 30f),
                            1.0f, 1f, 1f, 8.0f,
                            Rand.GetRandomFloat(0.25f, 0.5f),
                            Rand.GetRandomInt(0, 4)), true);
            }

            base.Update(gameTime, map, pMan, c);
        }

        public override void Draw(SpriteBatch sprite, Texture2D spritesTex)
        {
            Vector2 location_ = new Vector2(GameLocation.X, GameLocation.Y + location.Z);

            if (face == CharDir.right)
            {
                sprite.Draw(spritesTex, location_,
                    new Rectangle(192, 192, 64, 64),
                    Color.White,
                    rotation, new Vector2(32.0f, 32.0f),
                    new Vector2(1f, 1f),
                    SpriteEffects.FlipHorizontally, 1.0f);
            }
            else
            {
                sprite.Draw(spritesTex, location_,
                    new Rectangle(192, 192, 64, 64),
                    Color.White,
                    rotation, new Vector2(32.0f, 32.0f),
                    new Vector2(1f, 1f),
                    SpriteEffects.FlipHorizontally, 1.0f);
            }
        }

    }
}
