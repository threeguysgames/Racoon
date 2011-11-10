using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditor.MapClasses
{
    class SegmentDefinition
    {
        private string name;
        private int sourceIndex;

        private Rectangle srcRect;
        private int flags;

        public SegmentDefinition(string name, int sourceIndex, Rectangle srcRect, int flags)
        {
            Name = name;
            SourceIndex = sourceIndex;
            SourceRect = srcRect;
            Flags = flags;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int SourceIndex
        {
            get { return sourceIndex; }
            set { sourceIndex = value; }
        }

        public Rectangle SourceRect
        {
            get { return srcRect; }
            set { srcRect = value; }
        }

        public int Flags
        {
            get { return flags; }
            set { flags = value; }
        }
    }
}
