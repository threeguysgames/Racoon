using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CharacterEditor.Character
{
    class Part
    {
        public Vector2 Location;
        public float Rotation;
        public Vector2 Scaling;

        public int Index, Flip;

        public Part()
        {
            Index = -1;
            Scaling = new Vector2(1f, 1f);
        }
    }
}
