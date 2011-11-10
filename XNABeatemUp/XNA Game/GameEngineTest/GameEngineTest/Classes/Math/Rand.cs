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
    public static class Rand
    {
        private static Random random;

        public static Random Random
        {
            get { return random; }
            private set { random = value; }
        }

        static Rand()
        {
            Random = new Random();
        }

        public static float GetRandomFloat(float fMin, float fMax)
        {
            return (float)random.NextDouble() * (fMax - fMin) + fMin;
        }

        public static double GetRandomDouble(double dMin, double dMax)
        {
            return random.NextDouble() * (dMax - dMin) + dMin;
        }

        public static Vector2 GetRandomVector2(float xMin, float xMax, float yMin, float yMax)
        {
            return new Vector2(GetRandomFloat(xMin, xMax), GetRandomFloat(yMin, yMax));
        }
        public static Vector3 GetRandomVector3(float xMin, float xMax, float yMin, float yMax)
        {
            return new Vector3(GetRandomFloat(xMin, xMax), GetRandomFloat(yMin, yMax),0.0f);
        }
        public static int GetRandomInt(int iMin, int iMax)
        {
            return random.Next(iMin, iMax);
        }
    }
}
