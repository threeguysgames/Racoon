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
    public class Part
    {
        public Vector2 location;
        public float rotation;
        public Vector2 scaling;
        public int index;
        public int flip;

        public Part()
        {
            index = -1;
            scaling = new Vector2(1.0f, 1.0f);
        }
    }
}
