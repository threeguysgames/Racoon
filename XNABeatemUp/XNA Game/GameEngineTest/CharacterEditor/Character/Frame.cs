using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CharacterEditor.Character
{
    class Frame
    {
        Part[] parts;

        public string Name;

        public Frame()
        {
            parts = new Part[16];
            for (int i = 0; i < parts.Length; i++)
                parts[i] = new Part();

            Name = String.Empty;
        }

        public Part[] Parts
        {
            get { return parts; }
        }
    }
}
