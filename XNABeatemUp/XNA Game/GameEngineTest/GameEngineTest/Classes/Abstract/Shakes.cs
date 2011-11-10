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
    class Quake
    {
        private float val;

        public void Update()
        {
            if (val > 0f)
                val -= Game1.frameTime;
            else if (val < 0f)
                val = 0f;
        }

        public float Value
        {
            get { return val; }
            set { val = value; }
        }

        public Vector2 Vector
        {
            get
            {
                if (val <= 0f)
                    return Vector2.Zero;
                return Rand.GetRandomVector2(-val, val, -val, val) * 10f;
            }

        }
    }

    class Blast
    {
        private float val;
        private float mag;

        public Vector2 Center;

        public void Update()
        {
            if (val >= 0f)
                val -= Game1.frameTime * 5f;
            else if (val < 0f)
                val = 0f;
        }

        public float Value
        {
            get { return val; }
            set { val = value; }
        }

        public float Magnitude
        {
            get { return mag; }
            set { mag = value; }
        }
    }

    static class QuakeManager
    {
        public static Quake Quake;
        public static Blast Blast;

        static QuakeManager()
        {
            Quake = new Quake();
            Blast = new Blast();

        }

        public static void Update()
        {
            Quake.Update();
            Blast.Update();
        }

        public static void SetBlast(float mag, Vector2 center)
        {
            Blast.Value = mag;
            Blast.Magnitude = mag;
            Blast.Center = center;
        }

        public static void SetQuake(float value)
        {
            if (Quake.Value < value) Quake.Value = value;

        }

        

    }
}
