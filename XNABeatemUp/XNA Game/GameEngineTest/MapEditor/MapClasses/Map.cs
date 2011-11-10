using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditor.MapClasses
{
    class Map
    {
        SegmentDefinition[] segDef;
        MapSegment[,] mapSeg;
        int[,] col;
        Ledge[] ledges;
        public Texture2D iconsTex;
        public String[] Scripts;

        string path = "map";

        public Map()
        {
            segDef = new SegmentDefinition[512];
            mapSeg = new MapSegment[3, 64];
            col = new int[20, 20];
            Scripts =  new String[128];

            ledges = new Ledge[16];

            for (int i = 0; i < 16; i++)
                ledges[i] = new Ledge();

            for (int i = 0; i < Scripts.Length; i++)
                Scripts[i] = "";

            ReadSegmentDefinitions();

            

            
        }

        public void Draw(SpriteBatch sprite, Texture2D[] mapsTex, Vector2 scroll)
        {
            Rectangle sRect = new Rectangle();
            Rectangle dRect = new Rectangle();

            sprite.Begin(SpriteBlendMode.AlphaBlend);

            for (int l = 0; l < 3; l++)
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

                scale *= 0.5f;

                for (int i = 0; i < 64; i++)
                {
                    if (mapSeg[l, i] != null)
                    {
                        sRect = segDef[mapSeg[l, i].Index].SourceRect;

                        dRect.X = (int)(mapSeg[l, i].Location.X - scroll.X * scale);
                        dRect.Y = (int)(mapSeg[l, i].Location.Y - scroll.Y * scale);
                        dRect.Width = (int)(sRect.Width * scale);
                        dRect.Height = (int)(sRect.Height * scale);

                        sprite.Draw(mapsTex[segDef[mapSeg[l, i].Index].SourceIndex],
                            dRect,
                            sRect,
                            color);
                    }
                }
            }

            sprite.End();
        }

        public void Write()
        {
            BinaryWriter file = new BinaryWriter(File.Open(@"data/" + Path + ".mdx", FileMode.Create));

            for (int i = 0; i < ledges.Length; i++)
            {
                file.Write(ledges[i].TotalNodes);
                for (int n = 0; n < ledges[i].TotalNodes; n++)
                {
                    file.Write(ledges[i].Nodes[n].X);
                    file.Write(ledges[i].Nodes[n].Y);
                }
                file.Write(ledges[i].Flags);
            }

            for (int l = 0; l < 3; l++)
            {
                for (int i = 0; i < 64; i++)
                {
                    if (mapSeg[l, i] == null)
                        file.Write(-1);
                    else
                    {
                        file.Write(mapSeg[l, i].Index);
                        file.Write(mapSeg[l, i].Location.X);
                        file.Write(mapSeg[l, i].Location.Y);
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

            for (int i = 0; i < Scripts.Length; i++)
            {
                file.Write(Scripts[i]);
            }

            file.Close();
        }

        public void Read()
        {
            BinaryReader file = new BinaryReader(File.Open(@"data/" + Path + ".mdx", FileMode.Open));

            for (int i = 0; i < ledges.Length; i++)
            {
                ledges[i] = new Ledge();
                ledges[i].TotalNodes = file.ReadInt32();
                for (int n = 0; n < ledges[i].TotalNodes; n++)
                {
                    ledges[i].Nodes[n] = new Vector2(file.ReadSingle(), file.ReadSingle());
                }
                ledges[i].Flags = file.ReadInt32();
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

            for (int i = 0; i < Scripts.Length; i++)
            {
                Scripts[i] = file.ReadString();
            }
            file.Close();
        }

        private void ReadSegmentDefinitions()
        {
            StreamReader s = new StreamReader(@"Content/texturesDefs.mdx");

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
                        tRect.Width = Convert.ToInt32(split[2]) - tRect.X;
                        tRect.Height = Convert.ToInt32(split[3]) - tRect.Y;
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
                    Rectangle sRect = segDef[mapSeg[l, i].Index].SourceRect;
                    Rectangle dRect = new Rectangle(
                        (int)(mapSeg[l, i].Location.X - scroll.X * scale),
                        (int)(mapSeg[l, i].Location.Y - scroll.Y * scale),
                        (int)(sRect.Width * scale),
                        (int)(sRect.Height * scale)
                        );

                    if (dRect.Contains(x, y))
                        return i;
                }
            }

            return -1;
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

        public Ledge[] Ledges
        {
            get { return ledges; }
        }

        public int[,] Grid
        {
            get { return col; }
        }

        public MapSegment[,] Segments
        {
            get { return mapSeg; }
        }

        public SegmentDefinition[] SegmentDefinitions
        {
            get { return segDef; }
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }
    }
}
