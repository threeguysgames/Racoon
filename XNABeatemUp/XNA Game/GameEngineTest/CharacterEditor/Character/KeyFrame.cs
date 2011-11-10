using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CharacterEditor.Character
{
    class KeyFrame
    {
        public int FrameRef;
        public int Duration;

        string[] scripts;

        public KeyFrame()
        {
            FrameRef = -1;
            Duration = 0;

            scripts = new string[4];
            for (int i = 0; i < scripts.Length; i++)
                scripts[i] = String.Empty;
        }

        public string[] Scripts
        {
            get { return scripts; }
        }
    }
}
