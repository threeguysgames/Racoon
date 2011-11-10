using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace GameEngineTest
{
    public class CharDef
    {
        Animation[] animations;
        Frame[] frames;
        public string path;

        public int headIndex;
        public int torsoIndex;
        public int legsIndex;
        public int weaponIndex;

        public CharDef()
        {
            animations = new Animation[64];
            for (int i = 0; i < animations.Length; i++)
            {
                animations[i] = new Animation();
            }
            frames = new Frame[512];

            for (int i = 0; i < frames.Length; i++)
                frames[i] = new Frame();

            //path = "char";

            headIndex = 0;
            legsIndex = 0;
            torsoIndex = 0;
            weaponIndex = 0;
        }
        public CharDef(string _path)
        {
            animations = new Animation[64];
            for (int i = 0; i < animations.Length; i++)
            {
                animations[i] = new Animation();
            }
            frames = new Frame[512];

            for (int i = 0; i < frames.Length; i++)
                frames[i] = new Frame();

            path = _path;

            headIndex = 0;
            legsIndex = 0;
            torsoIndex = 0;
            weaponIndex = 0;

            Read();
        }

        public void Write()
        {
            BinaryWriter b = new BinaryWriter(File.Open(@"data/" + path + ".cdx", FileMode.Create));

            b.Write(path);
            b.Write(headIndex);
            b.Write(torsoIndex);
            b.Write(legsIndex);
            b.Write(weaponIndex);

            for (int i = 0; i < animations.Length; i++)
            {
                b.Write(animations[i].name);

                for (int j = 0; j < animations[i].getKeyFrameArray().Length; j++)
                {
                    KeyFrame keyframe = animations[i].GetKeyFrame(j);
                    b.Write(keyframe.frameRef);
                    b.Write(keyframe.duration);
                }
            }

            for (int i = 0; i < frames.Length; i++)
            {
                b.Write(frames[i].name);

                for (int j = 0; j < frames[i].GetPartArray().Length; j++)
                {
                    Part p = frames[i].GetPart(j);
                    b.Write(p.index);
                    b.Write(p.location.X);
                    b.Write(p.location.Y);
                    b.Write(p.rotation);
                    b.Write(p.scaling.X);
                    b.Write(p.scaling.Y);
                    b.Write(p.flip);
                }
            }

            b.Close();

            Console.WriteLine("Saved.");
        }

        public void Read()
        {
            BinaryReader b = new
        BinaryReader(File.Open(@"data/" + path + ".cdx",
        FileMode.Open, FileAccess.Read));

            string path2 = b.ReadString();
            
            headIndex = b.ReadInt32();
            torsoIndex = b.ReadInt32();
            legsIndex = b.ReadInt32();
            weaponIndex = b.ReadInt32();


            for (int i = 0; i < animations.Length; i++)
            {
                animations[i].name = b.ReadString();

                for (int j = 0; j <
                    animations[i].keyFrames.Length; j++)
                {
                    KeyFrame keyframe = animations[i].keyFrames[j];
                    keyframe.frameRef = b.ReadInt32();
                    keyframe.duration = b.ReadInt32();

                    ScriptLine[] scripts = keyframe.Scripts;
                    for (int s = 0; s < scripts.Length; s++)
                        scripts[s] = new ScriptLine(b.ReadString());
                }
            }

            for (int i = 0; i < frames.Length; i++)
            {
                frames[i].name = b.ReadString();

                for (int j = 0; j < frames[i].Parts.Length; j++)
                {
                    Part p = frames[i].Parts[j];
                    p.index = b.ReadInt32();
                    p.location.X = b.ReadSingle();
                    p.location.Y = b.ReadSingle();
                    p.rotation = b.ReadSingle();
                    p.scaling.X = b.ReadSingle();
                    p.scaling.Y = b.ReadSingle();
                    p.flip = b.ReadInt32();
                }
            }

            b.Close();

            Console.WriteLine("Loaded.");

        }
        public Animation[] Animations
        {
            get { return animations; }
        }

        public Frame[] Frames
        {
            get { return frames; }
        }

        public Animation GetAnimation(int idx)
        {
            return animations[idx];
        }
        public Animation[] GetAnimationArray()
        {
            return animations;
        }

        public Frame GetFrame(int idx)
        {
            if (frames[idx].name == String.Empty)
                return frames[0];
            else
                return frames[idx];

            return frames[idx];
        }

        public void SetFrame(int idx, Frame _frame)
        {
            frames[idx] = _frame;
        }

        public Frame[] GetFrameArray()
        {
            return frames;
        }
    }
}
