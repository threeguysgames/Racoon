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
    public class Animation
    {
            
        public string name;
        public KeyFrame[] keyFrames;

        public Animation()
        {
            name = String.Empty;
            keyFrames = new KeyFrame[64];
            for (int i = 0; i < keyFrames.Length; i++)
                keyFrames[i] = new KeyFrame();
        }

        public KeyFrame GetKeyFrame(int idx)
        {
            return keyFrames[idx];
        }

        public void SetKeyFrame(int idx, KeyFrame _keyFrame)
        {
            keyFrames[idx] = _keyFrame;
        }

        public KeyFrame[] getKeyFrameArray()
        {
            return keyFrames;
        }

    }
}
