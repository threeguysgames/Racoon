using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PPhysics;
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
    public class Entity
    {

        public Texture2D picture;
      
        public Rectangle hitBox;

        public float hitBoxScaleY;
        public float hitBoxScaleX;

        public Vector3 position;
        public Vector3 prevPos;
        public Vector3 trajectory;

        public float jumpY;

        public float scale;
        public float angle;

        public Vector2 centerOfObject;
    }
}
