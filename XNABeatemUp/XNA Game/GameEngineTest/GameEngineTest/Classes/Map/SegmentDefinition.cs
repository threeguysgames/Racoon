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
    public  class SegmentDefinition
    {
        private string name;
        private int sourceIndex;
        private Rectangle srcRect;
        private int flags;

        public SegmentDefinition(string _name,
            int _sourceIndex,
            Rectangle _srcRect,
            int _flags)
        {
            name = _name;
            sourceIndex = _sourceIndex;
            srcRect = _srcRect;
            flags = _flags;
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
        public Rectangle SrcRect
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
