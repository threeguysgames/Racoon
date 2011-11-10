using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditor.MapClasses
{
    class MapSegment
    {
        public Vector2 Location;

        int segmentIndex;

        public int Index
        {
            get { return segmentIndex; }
            set { segmentIndex = value; }
        }
    }
}