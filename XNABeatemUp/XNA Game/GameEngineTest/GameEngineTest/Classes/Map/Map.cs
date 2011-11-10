using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using GameEngineTest;

namespace GameEngineTest
{
    public class Map
    {
        public enum SegmentFlags
        {
            None = 0,
            Torch = 1,
            Fog = 2,
            ExitRight = 3,
            ExitLeft = 4
        }
        public const int LAYER_BACK = 0;
        public const int LAYER_MAP = 1;
        public const int LAYER_FORE = 2;

        SegmentDefinition[] segDef;
        MapSegment[,] mapSeg;
        public string[] transitionDest = { "", "", "", "" };
        int[,] col;
        Ledge[] ledges;
        private string path = "maps.mdx";
        public Texture2D iconsTex;
        public bool fog = false;

        public Bucket bucket;
        public MapScript mapScript;
        public MapFlags GlobalFlags;

        protected float pFrame;
        protected float frame;

        public float transInFrame = 0f;
        public float transOutFrame = 0f;

        public TransitionDirection transDir = TransitionDirection.None;
        
        public Map()
        {
            segDef = new SegmentDefinition[512]; //Loaded segments from file
            mapSeg = new MapSegment[3, 64]; //Layers, segments
            col = new int[20, 20];

            GlobalFlags = new MapFlags(64);

            /*Read textures from file*/
            ReadSegmentDefinitions();

            ledges = new Ledge[16];
            for (int i = 0; i < 16; i++)
            {
                ledges[i] = new Ledge();
            }
            
        }

        private void ReadSegmentDefinitions()
        {
            StreamReader s = new StreamReader(@"Content/gfx/texturesDefs.mdx");
            string t = "";
            int n;
            int currentTex = 0;
            int curDef = -1;
            Rectangle tRect = new Rectangle();
            string[] split;

            t = s.ReadLine();

            while (!s.EndOfStream)
            {
                t = s.ReadLine();
                if (t.StartsWith("#"))
                {
                    if (t.StartsWith("#src"))
                    {
                        split = t.Split(' ');
                        if (split.Length > 1)
                        {
                            n = Convert.ToInt32(split[1]);
                            currentTex = n - 1;
                        }
                    }
                }
                else
                {
                    curDef++;
                    string name = t;

                    t = s.ReadLine();
                    split = t.Split(' ');
                    if (split.Length > 3)
                    {
                        tRect.X = Convert.ToInt32(split[0]);
                        tRect.Y = Convert.ToInt32(split[1]);
                        tRect.Width =
                            Convert.ToInt32(split[2]) - tRect.X;
                        tRect.Height =
                            Convert.ToInt32(split[3]) - tRect.Y;
                    }
                    else
                        Console.WriteLine("read fail: " + name);

                    int tex = currentTex;

                    t = s.ReadLine();
                    int flags = Convert.ToInt32(t);

                    segDef[curDef] = new SegmentDefinition(name, tex, tRect, flags);
                }
            }
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }
        public SegmentDefinition[] SegmentDefinitions
        {
            get { return segDef; }
        }

        public MapSegment[,] Segments
        {
            get { return mapSeg; }
        }

        public Ledge[] Ledges
        {
            get { return ledges; }
        }

        public int AddSeg(int layer, int index)
        {
            for (int i = 0; i < 64; i++)
            {
                if (mapSeg[layer, i] == null)
                {
                    mapSeg[layer, i] = new MapSegment();
                    mapSeg[layer, i].Index = index;
                    return i;
                }
            }
            return -1;
        }

        public void Draw(SpriteBatch sprite,
            Texture2D[] mapsTex,
            int startLayer, int endLayer)
        {
            Rectangle sRect = new Rectangle();
            Rectangle dRect = new Rectangle();

            sprite.Begin(SpriteBlendMode.AlphaBlend);

            for (int l = startLayer; l < endLayer; l++)
            {
                float scale = 1.0f;
                Color color = Color.White;
                if (l == 0)
                {
                    color = Color.Gray;
                    scale = 0.75f;
                }
                else if (l == 2)
                {
                    color = Color.DarkGray;
                    scale = 1.25f;
                }

                for (int i = 0; i < 64; i++)
                {
                    if (mapSeg[l, i] != null)
                    {
                        sRect = segDef[mapSeg[l, i].Index].SrcRect;
                        dRect.X = (int)(mapSeg[l, i].Location.X * 2f - Game1.scroll.X * scale);
                        dRect.Y = (int)(mapSeg[l, i].Location.Y * 2f - Game1.scroll.Y * scale);
                        dRect.Width = (int)(sRect.Width * scale);
                        dRect.Height = (int)(sRect.Height * scale);

                        sprite.Draw(mapsTex[segDef[mapSeg[l, i].Index].SourceIndex],
                            dRect, sRect, color);
                    }
                }

            }


            Rectangle rect = new Rectangle();


            Color tColor = new Color();

            rect.X = 32;
            rect.Y = 0;
            rect.Width = 32;
            rect.Height = 32;

            for (int i = 0; i < 16; i++)
            {
                if (this.Ledges[i] != null && this.ledges[i].totalNodes > 0)
                {
                    for (int n = 0; n < this.ledges[i].totalNodes; n++)
                    {
                        Vector2 tVec;

                        tVec = this.Ledges[i].Nodes[n];
                        tVec -= Game1.scroll;
                        tVec.X -= 5.0f;

                        sprite.Draw(iconsTex, tVec, rect, tColor,
                            0.0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0.0f);

                        if (n < this.ledges[i].totalNodes - 1)
                        {
                            Vector2 nVec;

                            nVec = this.Ledges[i].Nodes[n + 1];
                            nVec -= Game1.scroll;
                            nVec.X -= 4.0f;

                            for (int x = 1; x < 20; x++)
                            {
                                Vector2 iVec = (nVec - tVec) * ((float)x / 20f) + tVec;

                                Color nColor = new Color(255, 255, 255, 75);

                                if (this.ledges[i].flags == 1)
                                    nColor = new Color(255, 0, 0, 75);

                                sprite.Draw(iconsTex, iVec, rect, nColor, 0.0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0.0f);
                            }
                        }
                    }

                    Vector2 lVec;

                    lVec = this.Ledges[i].Nodes[0];
                    lVec -= Game1.scroll;
                    lVec.X -= 5.0f;

                    sprite.Draw(iconsTex, lVec, rect, tColor,
                        0.0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0.0f);


                    Vector2 nlVec;

                    nlVec = this.Ledges[i].Nodes[this.ledges[i].totalNodes - 1];
                    nlVec -= Game1.scroll;
                    nlVec.X -= 4.0f;

                    for (int x = 1; x < 20; x++)
                    {
                        Vector2 iVec = (nlVec - lVec) * ((float)x / 20f) + lVec;

                        Color nColor = new Color(0, 0, 0, 100);

                        if (this.ledges[i].flags == 1)
                            nColor = new Color(255, 0, 0, 75);

                        sprite.Draw(iconsTex, iVec, rect, nColor, 0.0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0.0f);
                    }

                }
            }




            sprite.End();

        }

        public int GetHoveredSegment(int x, int y, int l, Vector2 scroll)
        {
            float scale = 1.0f;
            
            if (l == 0)
                scale = 0.75f;
            else if (l == 2)
                scale = 1.25f;
            
            scale *= 0.5f;

            for (int i = 63; i >= 0; i--)
            {
                if (mapSeg[l, i] != null)
                {
                    Rectangle sRect = segDef[mapSeg[l, i].Index].SrcRect;
                    Rectangle dRect = new Rectangle(
                        (int)(mapSeg[l, i].Location.X - scroll.X * scale),
                        (int)(mapSeg[l, i].Location.Y - scroll.Y * scale),
                        (int)(sRect.Width * scale),
                        (int)(sRect.Height * scale));
                    if (dRect.Contains(x, y))
                        return i;
                }

            }

            return -1;
        }

        public int[,] Grid
        {
            get { return col; }
        }

        public void Write()
        {
            BinaryWriter file = new BinaryWriter(File.Open(@"data/" + path + ".mdx", FileMode.Create));

            for (int i = 0; i < ledges.Length; i++)
            {
                file.Write(ledges[i].totalNodes);
                for (int n = 0; n < ledges[i].totalNodes; n++)
                {
                    file.Write(ledges[i].Nodes[n].X);
                    file.Write(ledges[i].Nodes[n].Y);
                }
                file.Write(ledges[i].flags);
            }

            for (int l = 0; l < 3; l++)
            {
                for( int i = 0; i < 64; i++)
                {
                    if(mapSeg[l,i] == null)
                        file.Write(-1);
                    else
                    {
                        file.Write(mapSeg[l,i].Index);
                        file.Write(mapSeg[l,i].Location.X);
                        file.Write(mapSeg[l,i].Location.Y);
                    }
                }
            }
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    file.Write(col[x, y]);
                }
            }
            file.Close();
        }

        public void Read()
        {
            BinaryReader file = new BinaryReader(File.Open(@"data/" + path + ".mdx", FileMode.Open));

            for (int i = 0; i < ledges.Length; i++)
            {
                ledges[i] = new Ledge();
                ledges[i].totalNodes = file.ReadInt32();
                for (int n = 0; n < ledges[i].totalNodes; n++)
                {
                    ledges[i].Nodes[n] = new Vector2(file.ReadSingle()*2f, file.ReadSingle()*2f);
                }
                ledges[i].flags = file.ReadInt32();
            }
            for (int l = 0; l < 3; l++)
            {
                for (int i = 0; i < 64; i++)
                {
                    int t = file.ReadInt32();
                    if (t == -1)
                        mapSeg[l, i] = null;
                    else
                    {
                        mapSeg[l, i] = new MapSegment();
                        mapSeg[l, i].Index = t;
                        mapSeg[l, i].Location = new Vector2(file.ReadSingle(), file.ReadSingle());
                    }
                }
            }

            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    col[x, y] = file.ReadInt32();
                }
            }

            mapScript = new MapScript(this);

            for (int i = 0; i < mapScript.Lines.Length; i++)
            {
                String s = file.ReadString();
                if (s.Length > 0)
                    mapScript.Lines[i] = new MapScriptLine(s);
                else
                    mapScript.Lines[i] = null;
            }

            file.Close();

            bucket = null;
            fog = false;
            

            for (int i = 0; i < ledges.Length; i++)
            {
                if(ledges[i].hasEdges())
                    ledges[i].populateEdges();
            }

            if (mapScript.GotoTag("init"))
                mapScript.IsReading = true;
            
        }

        public void Update(ParticleManager pMan,Character[] c)
        {
            if (mapScript.IsReading)
                mapScript.DoScript(c);

            if (bucket != null)
                if (!bucket.isEmpty)
                    bucket.Update(c);

            frame += Game1.frameTime;

            for (int i = 0; i < 64; i++)
            {
                if (mapSeg[LAYER_MAP, i] != null)
                {
                    if (segDef[mapSeg[LAYER_MAP, i].Index].Flags == (int)SegmentFlags.Torch)
                    {
                        Vector3 loc1 = new Vector3(mapSeg[LAYER_MAP, i].Location.X * 2f + 40, mapSeg[LAYER_MAP, i].Location.Y * 2f +13, 0.0f);
                        Vector3 loc2 = new Vector3(mapSeg[LAYER_MAP, i].Location.X * 2f + 40, mapSeg[LAYER_MAP, i].Location.Y * 2f + 37, 0.0f);

                        pMan.AddParticle(new Smoke(
                            loc1,
                            Rand.GetRandomVector3(
                            -50f, 50f, -300f, -200f),
                            1.0f, 0.8f, 0.6f, 1.0f,
                            Rand.GetRandomFloat(0.25f, 0.5f),
                            Rand.GetRandomInt(0, 4)), true);

                        pMan.AddParticle(new Fire(
                            loc2,
                            Rand.GetRandomVector3(
                            -30f, 30f, -250f, -200f),
                            Rand.GetRandomFloat(0.25f, 0.75f),
                            Rand.GetRandomInt(0, 4)), true);
                    }

                    if (segDef[mapSeg[LAYER_MAP, i].Index].Flags == (int)SegmentFlags.ExitRight)
                    {
                        if (transOutFrame == 0)
                        {
                            if (c[0].position.X > (mapSeg[LAYER_MAP, i].Location.X*2))
                            {
                                if (transitionDest[(int)TransitionDirection.Right] != "")
                                    transOutFrame = 1f;
                                transDir = TransitionDirection.Right;
                            }
                        }
                    }
                    if (segDef[mapSeg[LAYER_MAP, i].Index].Flags == (int)SegmentFlags.ExitLeft)
                    {
                        if (c[0].position.X < (mapSeg[LAYER_MAP, i].Location.X))
                        {
                            if (transitionDest[(int)TransitionDirection.Left] != "")
                                transOutFrame = 1f;
                            transDir = TransitionDirection.Left;
                        }
                    }
                }
            }

            if (transOutFrame > 0f)
            {
                transOutFrame -= Game1.frameTime * 3f;
                if (transOutFrame <= 0f)
                {
                    path = transitionDest[(int)transDir];
                    Read();
                    transInFrame = 1.1f;
                    for (int i = 1; i < c.Length; i++)
                        c[i] = null;

                    pMan.Reset();
                    
                }
            }
            if (transInFrame > 0f)
                transInFrame -= Game1.frameTime * 3f;

            if (mapScript.IsReading)
                mapScript.DoScript(c);
        }

        public float GetTransVal()
        {
            if (transInFrame > 0f)
                return transInFrame;
            if (transOutFrame > 0f)
                return 1 - transOutFrame;
            return 0;
        }

        public void CheckTransition(Character[] c)
        {
            if(transOutFrame <= 0f && transInFrame <= 0f)
            {
                if(c[0].DyingFrame > 0f)
                    return;

                
            }
        }
    }
}
