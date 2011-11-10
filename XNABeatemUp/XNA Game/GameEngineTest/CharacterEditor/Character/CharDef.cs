using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CharacterEditor.Character
{
    class CharDef
    {
        Animation[] animations;
        Frame[] frames;

        public string Path;

        public int HeadIndex;
        public int TorsoIndex;
        public int LegsIndex;
        public int WeaponIndex;

        public CharDef()
        {
            animations = new Animation[64];
            for (int i = 0; i < animations.Length; i++)
                animations[i] = new Animation();

            frames = new Frame[512];
            for (int i = 0; i < frames.Length; i++)
                frames[i] = new Frame();

            Path = "guy";
        }

        public void Write()
        {
            BinaryWriter b = new BinaryWriter(File.Open(@"data/" + Path + ".cdx", FileMode.Create));

            b.Write(Path);
            b.Write(HeadIndex);
            b.Write(TorsoIndex);
            b.Write(LegsIndex);
            b.Write(WeaponIndex);

            for (int i = 0; i < animations.Length; i++)
            {
                b.Write(animations[i].Name);

                for (int j = 0; j <
                    animations[i].KeyFrames.Length; j++)
                {
                    KeyFrame keyframe = animations[i].KeyFrames[j];
                    b.Write(keyframe.FrameRef);
                    b.Write(keyframe.Duration);
                    String[] scripts = keyframe.Scripts;
                    for (int s = 0; s < scripts.Length; s++)
                        b.Write(scripts[s]);
                }
            }

            for (int i = 0; i < frames.Length; i++)
            {
                b.Write(frames[i].Name);

                for (int j = 0; j < frames[i].Parts.Length; j++)
                {
                    Part p = frames[i].Parts[j];
                    b.Write(p.Index);
                    b.Write(p.Location.X);
                    b.Write(p.Location.Y);
                    b.Write(p.Rotation);
                    b.Write(p.Scaling.X);
                    b.Write(p.Scaling.Y);
                    b.Write(p.Flip);
                }
            }

            b.Close();
        }

        public void Read()
        {
            try
            {
                BinaryReader b = new
                BinaryReader(File.Open(@"data/" + Path + ".cdx",
                FileMode.Open, FileAccess.Read));

                Path = b.ReadString();
                HeadIndex = b.ReadInt32();
                TorsoIndex = b.ReadInt32();
                LegsIndex = b.ReadInt32();
                WeaponIndex = b.ReadInt32();


                for (int i = 0; i < animations.Length; i++)
                {
                    animations[i].Name = b.ReadString();

                    for (int j = 0; j <
                        animations[i].KeyFrames.Length; j++)
                    {
                        KeyFrame keyframe = animations[i].KeyFrames[j];
                        keyframe.FrameRef = b.ReadInt32();
                        keyframe.Duration = b.ReadInt32();

                        string[] scripts = keyframe.Scripts;
                        for (int s = 0; s < scripts.Length; s++)
                            scripts[s] = b.ReadString();
                    }
                }

                for (int i = 0; i < frames.Length; i++)
                {
                    frames[i].Name = b.ReadString();

                    for (int j = 0; j < frames[i].Parts.Length; j++)
                    {
                        Part p = frames[i].Parts[j];
                        p.Index = b.ReadInt32();
                        p.Location.X = b.ReadSingle();
                        p.Location.Y = b.ReadSingle();
                        p.Rotation = b.ReadSingle();
                        p.Scaling.X = b.ReadSingle();
                        p.Scaling.Y = b.ReadSingle();
                        p.Flip = b.ReadInt32();
                    }
                }

                b.Close();

                Console.WriteLine("Loaded.");
            }

            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace.ToString());
            }

        }

        public Animation[] Animations
        {
            get { return animations; }
        }

        public Frame[] Frames
        {
            get { return frames; }
        }
    }
}
