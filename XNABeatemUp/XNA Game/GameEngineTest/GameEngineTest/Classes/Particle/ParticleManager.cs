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
    public  class ParticleManager
    {
        Particle[] particles = new Particle[1024];

        SpriteBatch sprite;

        public ParticleManager(SpriteBatch sprite)
        {
            this.sprite = sprite;
        }

        public void AddParticle(Particle newParticle)
        {
            AddParticle(newParticle, false);
        }

        public void AddParticle(Particle newParticle, bool background)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i] == null)
                {
                    particles[i] = newParticle;
                    particles[i].Background = background;
                    break;
                }
            }
        }

        public void UpdateParticles(float frameTime, Map map, Character[] c)
        {
            for(int i = 0; i < particles.Length; i++)
            {
                if (particles[i] != null)
                {
                    particles[i].Update(frameTime, map, this, c);
                    if (!particles[i].Exists)
                    {
                        particles[i] = null;
                    }
                }
            }
        }

        public void MakeRocket(Vector3 loc, Vector3 traj, CharDir facing, int ownerID)
        {
            switch (facing)
            {
                case CharDir.left:
                    AddParticle(new Rocket(loc,
                        new Vector3(-traj.X, traj.Y, traj.Z),
                        ownerID, facing));

                    MakeMuzzleFlash(loc, new Vector3(-traj.X, traj.Y,0.0f));
                    break;
                case CharDir.right:
                    AddParticle(new Rocket(loc, 
                        traj,
                        ownerID,facing));

                    MakeMuzzleFlash(loc, traj);
                    break;
            }
        }

        public void MakeHPHit(Vector3 location, long value)
        {
            AddParticle(new HitHP(value, location + new Vector3(0, -120, 0)));
        }

        public void MakeSpawnSmoke(Vector3 loc)
        {
            for (int i = 0; i < 20; i++)
            {
                AddParticle(new Smoke(
                    loc, Rand.GetRandomVector3(
                            -100f, -300f, 100f, -250f),
                            1.0f, 1f, 1f, 8.0f,
                            Rand.GetRandomFloat(0.25f, 0.5f),
                            Rand.GetRandomInt(0, 4)), true);
            }
        }

        public void MakeMuzzleFlash(Vector3 loc, Vector3 traj)
        {
            for (int i = 0; i < 16; i++)
            {
                AddParticle(new MuzzleFlash(
                    loc + (traj * (float)i) * .001f + Rand.GetRandomVector3(-5f, 5f, -5f, 5f),
                    traj / 5f,
                    (20f - (float)i) * 0.06f));
            }
            for (int i = 0; i < 4; i++)
            {
                AddParticle(new Smoke(
                    loc, Rand.GetRandomVector3(-30, 30, -100, 0),
                    0, 0, 0, 0.25f,
                    Rand.GetRandomFloat(0.25f, 1.0f),
                    Rand.GetRandomInt(0, 4)));
            }
        }
        
        public void MakeBloodSplash(Vector3 loc, Vector3 traj)
        {
            traj += Rand.GetRandomVector3(-100f, 100f, -100f, 100f);

            for (int i = 0; i < 32; i++)
            {
                AddParticle(new Blood(loc, traj *
                    Rand.GetRandomFloat(0.1f, 3.5f)
                    +
                    Rand.GetRandomVector3(-70f, 70f, -70f, 70f),
                    1f, 0f, 0f, 1f,
                    Rand.GetRandomFloat(0.01f, 0.25f),
                    Rand.GetRandomInt(0, 4)));

                AddParticle(new Blood(loc, traj *
                    Rand.GetRandomFloat(-0.2f, 0f)
                    +
                    Rand.GetRandomVector3(-120f, 120f, -120f, 120f),
                    1f, 0f, 0f, 1f,
                    Rand.GetRandomFloat(0.01f, 0.25f),
                    Rand.GetRandomInt(0, 4)));
            }
        }

        public void DrawParticles(Texture2D spritesTex, bool background)
        {
            sprite.Begin(SpriteBlendMode.AlphaBlend);

            foreach( Particle p in particles)
            {
                if (p != null)
                {
                    if (!p.Additive && p.Background == background)
                    {
                        p.Draw(sprite, spritesTex);
                    }
                }
            }

            sprite.End();

            sprite.Begin(SpriteBlendMode.Additive);

            foreach (Particle p in particles)
            {
                if (p != null)
                {
                    if (p.Additive && p.Background == background)
                    {
                        p.Draw(sprite, spritesTex);
                    }
                }
            }

            sprite.End();
        }

        public void Reset()
        {
            for (int i = 0; i < particles.Length; i++)
                particles[i] = null;
        }

    }
}
