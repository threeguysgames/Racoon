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
    public class Frame
    {
        Part[] parts;
        public string name;

        public Frame()
        {
            parts = new Part[16];
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = new Part();
            }

            name = String.Empty;
        }

        public Part[] Parts
        {
            get { return parts; }
        }
        public Part GetPart(int idx)
        {
            return parts[idx];
        }

        public void SetPart(int idx, Part _part)
        {
            parts[idx] = _part;
        }

        public Part[] GetPartArray()
        {
            return parts;
        }
    }
}
