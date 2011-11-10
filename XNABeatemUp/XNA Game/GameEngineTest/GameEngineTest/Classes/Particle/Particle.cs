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
    public class Particle
    {
        protected Vector3 location;
        protected Vector3 trajectory;

        protected float frame;
        protected float r, g, b, a;

        protected float size;
        protected float rotation;

        protected int flag;

        protected int owner;        //Owner of the particle

        public bool Exists;      //Whether or not to kill the particle
        public  bool Background;  //Background particle

        private bool additive;

        public bool Additive
        {
            get { return additive; }
            protected set { additive = value; }
        }

        public Vector3 GameLocation
        {
            get { return location - new Vector3(Game1.scroll.X,Game1.scroll.Y, 0.0f);}
        }

        public Vector3 Trajectory
        {
            get { return trajectory; }
        }

        public Vector3 Location
        {
            get { return location; }
        }

        public int Owner
        {
            get { return owner; }
        }

        public Particle()
        {
            Exists = false;
        }

        public int Flag
        {
            get {return flag;}
        }

        public virtual void Update(float gameTime, Map map, ParticleManager pMan, Character[] c)
        {
            location += trajectory * gameTime;
            frame -= gameTime;
            if (frame < 0.0f) KillMe();
        }

        public virtual void KillMe()
        {
            Exists = false;
        }

        public virtual void Draw(SpriteBatch sprite, Texture2D spritesTex)
        {

        }

        public bool CheckParticleCol()
        {
            if (location.Z > 0)
            {
                location.Z = 0;
                return true;
            }
            else
                return false;
        }
    }
}
