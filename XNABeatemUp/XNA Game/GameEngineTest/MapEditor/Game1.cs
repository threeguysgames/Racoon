using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using MapEditor.MapClasses;

using TextLib;

namespace MapEditor
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Map map;
        Text text;
        SpriteFont font;

        Texture2D[] mapsTex;
        Texture2D nullTex;
        Texture2D iconsTex;

        int mosX, mosY;
        bool rightMouseDown;
        bool midMouseDown;
        bool mouseClick;

        Vector2 scroll;

        int mouseDragSeg = -1;
        int curLayer = 1;
        int pMosX, pMosY;

        int curLedge = 0;
        int curNode = 0;

        int scriptScroll;
        int selScript = -1;

        const int COLOR_NONE = 0;
        const int COLOR_YELLOW = 1;
        const int COLOR_GREEN = 2;

        KeyboardState oldKeyState;

        DrawingMode drawType = DrawingMode.SegmentSelection;
        EditingMode editMode = EditingMode.None;

        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            map = new Map();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            font = Content.Load<SpriteFont>(@"Fonts/Arial");

            text = new Text(spriteBatch, font);

            nullTex = Content.Load<Texture2D>(@"gfx/1x1");
            mapsTex = new Texture2D[1];

            for (int i = 0; i < mapsTex.Length; i++)
                mapsTex[i] = Content.Load<Texture2D>(@"gfx/maps" + (i + 1).ToString());

            iconsTex = Content.Load<Texture2D>(@"gfx/icons");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private bool GetCanEdit()
        {
            if (mosX > 100 && mosX < 500 && mosY > 100 && mosY < 550)
                return true;

            return false;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            MouseState mState = Mouse.GetState();

            mosX = mState.X;
            mosY = mState.Y;

            bool pMouseDown = rightMouseDown;

            if (mState.LeftButton == ButtonState.Pressed)
            {
                if (!rightMouseDown && GetCanEdit())
                {
                    if (drawType == DrawingMode.SegmentSelection)
                    {
                        int f = map.GetHoveredSegment(mosX, mosY, curLayer, scroll);

                        if (f != -1)
                            mouseDragSeg = f;
                    }
                    else if (drawType == DrawingMode.CollisionMap)
                    {
                        int x = (mosX + (int)(scroll.X / 2)) / 32;
                        int y = (mosY + (int)(scroll.Y / 2)) / 32;
                        if (x >= 0 && y >= 0 && x < 20 && y < 20)
                        {
                            if (mState.LeftButton == ButtonState.Pressed)
                                map.Grid[x, y] = 1;
                            else if (mState.RightButton == ButtonState.Pressed)
                                map.Grid[x, y] = 0;
                        }
                    }
                    else if (drawType == DrawingMode.Ledges)
                    {
                        if (map.Ledges[curLedge] == null)
                            map.Ledges[curLedge] = new Ledge();

                        if(map.Ledges[curLedge].TotalNodes < 15)
                        {
                            map.Ledges[curLedge].Nodes[map.Ledges[curLedge].TotalNodes] =
                                new Vector2(mosX, mosY) + scroll / 2f;

                            map.Ledges[curLedge].TotalNodes++;
                        }
                    }
                    else if (drawType == DrawingMode.Script)
                    {
                        if (selScript > -1)
                        {
                            if (mosX < 400)
                            {
                                map.Scripts[selScript] += (" " + ((int)((float)mosX + scroll.X / 2f)).ToString() + " " +
                                    ((int)((float)mosY + scroll.Y / 2f)).ToString());
                            }

                        }
                    }
                }
                rightMouseDown = true;
            }
            else
                rightMouseDown = false;

            if (mState.RightButton == ButtonState.Pressed)
            {
                    if (drawType == DrawingMode.Ledges)
                    {
                        if (map.Ledges[curLedge].TotalNodes > 0)
                        {
                            float lastNodeX = map.Ledges[curLedge].Nodes[map.Ledges[curLedge].TotalNodes - 1].X;
                            float lastNodeY = map.Ledges[curLedge].Nodes[map.Ledges[curLedge].TotalNodes - 1].Y;

                            if ((mosX > (lastNodeX - 10) && mosX < (lastNodeX + 10)) &&
                               (mosY > (lastNodeY - 10) && mosY < (lastNodeY + 10)))
                            {
                                Console.WriteLine("Delete node\n");


                                map.Ledges[curLedge].TotalNodes--;
                            }
                        }
                    }
            }

            midMouseDown = (mState.MiddleButton == ButtonState.Pressed);

            if (pMouseDown && !rightMouseDown) mouseClick = true;

            if (mouseDragSeg > -1)
            {
                if (!rightMouseDown)
                    mouseDragSeg = -1;
                else
                {
                    Vector2 loc = map.Segments[curLayer, mouseDragSeg].Location;

                    loc.X += (mosX - pMosX);
                    loc.Y += (mosY - pMosY);

                    map.Segments[curLayer, mouseDragSeg].Location = loc;
                }
            }

            if (midMouseDown)
            {
                scroll.X -= (mosX - pMosX) * 2f;
                scroll.Y -= (mosY - pMosY) * 2f;
            }

            pMosX = mosX;
            pMosY = mosY;

            UpdateKeys();

            base.Update(gameTime);
        }

        private void UpdateKeys()
        {
            KeyboardState keyState = Keyboard.GetState();

            Keys[] currentKeys = keyState.GetPressedKeys();
            Keys[] lastKeys = oldKeyState.GetPressedKeys();

            bool found = false;

            for (int i = 0; i < currentKeys.Length; i++)
            {
                found = false;

                for (int y = 0; y < lastKeys.Length; y++)
                {
                    if (currentKeys[i] == lastKeys[y])
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) 
                    PressKey(currentKeys[i]);
            }

            oldKeyState = keyState;
        }
        
        private bool ScriptEnter()
        {
            if (selScript >= map.Scripts.Length - 1)
                return false;
            for (int i = map.Scripts.Length - 1; i > selScript; i--)
                map.Scripts[i] = map.Scripts[i - 1];
            selScript++;
            return true;
        }

        private void ScriptDown()
        {
            if (selScript >= map.Scripts.Length - 1)
                return;
            selScript++;
        }
        private void ScriptUp()
        {
            if (selScript <= 0)
                return;

            selScript--;
        }
        private bool ScriptDelLine()
        {
            if (selScript <= 0)
                return false;
            for (int i = selScript; i < map.Scripts.Length - 1; i++)
                map.Scripts[i] = map.Scripts[i + 1];
            return true;
        }

        private void PressKey(Keys key)
        {
            string t = String.Empty;

            switch (editMode)
            {
                case EditingMode.Path:
                    t = map.Path;
                    break;

                case EditingMode.Script:
                    if (selScript < 0)
                        return;
                    t = map.Scripts[selScript];
                    break;
                default:
                    return;
            }

            bool delLine = false;

            if (key == Keys.Back)
            {
                if (t.Length > 0) t = t.Substring(0, t.Length - 1);
                else if (editMode == EditingMode.Script)
                    delLine = ScriptDelLine();
            }
            else if (key == Keys.Enter)
            {
                if (editMode == EditingMode.Script)
                {
                    if (ScriptEnter())
                        t = "";

                    
                }
                else
                    editMode = EditingMode.None;
            }
            else if (key == Keys.Down)
            {
                ScriptDown();
                t = "";
            }
            else if (key == Keys.Up)
            {
                ScriptUp();
                t = "";
            }
            else
            {
                t = (t + (char)key).ToLower();
            }

            if (!delLine)
            {
                switch (editMode)
                {
                    case EditingMode.Path:
                        map.Path = t;
                        break;

                    case EditingMode.Script:
                        map.Scripts[selScript] = t;
                        break;
                }
            }
            else
                selScript--;

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            #region Part One
            /*
            text.Size = 3.0f;
            text.Color = new Color(0, 0, 0, 125);
            for (int i = 0; i < 3; i++)
            {
                if (i == 2)
                    text.Color = Color.White;

                text.DrawText(25 - i * 2, 250 - i * 2, "Zombie Smashers XNA FTW!");
            }
            */
            #endregion

            map.Draw(spriteBatch, mapsTex, scroll);

            switch(drawType)
            {
                case DrawingMode.SegmentSelection:
                    DrawMapSegments();
                    break;
                case DrawingMode.Ledges:
                    DrawLedgePalette();
                    break;

                case DrawingMode.Script:
                    break;
                    
            }

            if (DrawButton(5, 65, 3, mosX, mosY, mouseClick))
                map.Write();

            if (DrawButton(40, 65, 4, mosX, mosY, mouseClick))
                map.Read();

            DrawGrid();
            DrawLedges();
            DrawText();
            DrawCursor();

            base.Draw(gameTime);
        }

        private bool DrawButton(int x, int y, int index, int mosX, int mosY, bool mouseClick)
        {
            bool r = false;

            Rectangle sRect = new Rectangle(32 * (index % 8), 32 * (index / 8), 32, 32);
            Rectangle dRect = new Rectangle(x, y, 32, 32);

            if (dRect.Contains(mosX, mosY))
            {
                dRect.X -= 1;
                dRect.Y -= 1;
                dRect.Width += 2;
                dRect.Height += 2;

                if (mouseClick)
                    r = true;
            }

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(iconsTex, dRect, sRect, Color.White);
            spriteBatch.End();

            return r;
        }

        private void DrawLedgePalette()
        {
            for (int i = 0; i < 16; i++)
            {
                if (map.Ledges[i] == null)
                    continue;

                int y = 50 + i * 20;
                if (curLedge == i)
                {
                    text.Color = Color.Lime;
                    text.DrawText(520, y, "ledge " + i.ToString());
                }
                else
                {
                    if (text.DrawClickText(520, y, "ledge " + i.ToString(),
                        mosX, mosY, mouseClick))
                        curLedge = i;
                }

                text.Color = Color.White;
                text.DrawText(620, y, "n" + map.Ledges[i].TotalNodes.ToString());

                if (text.DrawClickText(680, y, "f" + map.Ledges[i].Flags.ToString(), mosX, mosY, mouseClick))
                    map.Ledges[i].Flags = (map.Ledges[i].Flags + 1) % 2;
            }
        }

        private void DrawGrid()
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 20; x++)
                {
                    Rectangle dRect = new Rectangle(
                        x * 32 - (int)(scroll.X / 2),
                        y * 32 - (int)(scroll.Y / 2),
                        32,
                        32
                        );

                    if (x < 19)
                        spriteBatch.Draw(nullTex,
                            new Rectangle(dRect.X, dRect.Y, 32, 1),
                            new Color(255, 0, 0, 100));

                    if (y < 19)
                        spriteBatch.Draw(nullTex,
                            new Rectangle(dRect.X, dRect.Y, 1, 32),
                            new Color(255, 0, 0, 100));

                    if (x < 19 && y < 19)
                    {
                        if (map.Grid[x, y] == 1)
                            spriteBatch.Draw(nullTex, dRect,
                                new Color(255, 0, 0, 100));
                    }
                }
            }

            Color oColor = new Color(255, 255, 255, 100);
            spriteBatch.Draw(nullTex, new Rectangle(100, 50, 400, 1), oColor);
            spriteBatch.Draw(nullTex, new Rectangle(100, 50, 1, 500), oColor);
            spriteBatch.Draw(nullTex, new Rectangle(500, 50, 1, 500), oColor);
            spriteBatch.Draw(nullTex, new Rectangle(100, 550, 400, 1), oColor);

            spriteBatch.End();
        }

        private void DrawLedges()
        {
            Rectangle rect = new Rectangle();

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            Color tColor = new Color();

            rect.X = 32;
            rect.Y = 0;
            rect.Width = 32;
            rect.Height = 32;

            for (int i = 0; i < 16; i++)
            {
                if (map.Ledges[i] != null && map.Ledges[i].TotalNodes > 0)
                {
                    for (int n = 0; n < map.Ledges[i].TotalNodes; n++)
                    {
                        Vector2 tVec;

                        tVec = map.Ledges[i].Nodes[n];
                        tVec -= scroll / 2.0f;
                        tVec.X -= 5.0f;

                        if (curLedge == i)
                            tColor = Color.Yellow;
                        else
                            tColor = Color.White;

                        spriteBatch.Draw(iconsTex, tVec, rect, tColor,
                            0.0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0.0f);

                        if (n < map.Ledges[i].TotalNodes - 1)
                        {
                            Vector2 nVec;

                            nVec = map.Ledges[i].Nodes[n + 1];
                            nVec -= scroll / 2.0f;
                            nVec.X -= 4.0f;

                            for (int x = 1; x < 20; x++)
                            {
                                Vector2 iVec = (nVec - tVec) * ((float)x / 20f) + tVec;

                                Color nColor = new Color(255, 255, 255, 75);

                                if (map.Ledges[i].Flags == 1)
                                    nColor = new Color(255, 0, 0, 75);

                                spriteBatch.Draw(iconsTex, iVec, rect, nColor, 0.0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0.0f);
                            }
                        }
                    }

                    Vector2 lVec;

                    lVec = map.Ledges[i].Nodes[0];
                    lVec -= scroll / 2.0f;
                    lVec.X -= 5.0f;

                    if (curLedge == i)
                        tColor = Color.Yellow;
                    else
                        tColor = Color.White;

                    spriteBatch.Draw(iconsTex, lVec, rect, tColor,
                        0.0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0.0f);


                        Vector2 nlVec;

                        nlVec = map.Ledges[i].Nodes[map.Ledges[i].TotalNodes-1];
                        nlVec -= scroll / 2.0f;
                        nlVec.X -= 4.0f;

                        for (int x = 1; x < 20; x++)
                        {
                            Vector2 iVec = (nlVec - lVec) * ((float)x / 20f) + lVec;

                            Color nColor = new Color(255, 255, 255, 75);

                            if (map.Ledges[i].Flags == 1)
                                nColor = new Color(255, 0, 0, 75);

                            spriteBatch.Draw(iconsTex, iVec, rect, nColor, 0.0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0.0f);
                        }
                    
                }
            }

            spriteBatch.End();
        }

        private void DrawText()
        {
            string layerName = "map";
            switch (curLayer)
            {
                case 0:
                    layerName = "back";
                    break;
                case 1:
                    layerName = "mid";
                    break;
                case 2:
                    layerName = "fore";
                    break;
            }

            if (text.DrawClickText(5, 5, "layer: " + layerName, mosX, mosY, mouseClick))
                curLayer = (curLayer + 1) % 3;

            switch (drawType)
            {
                case DrawingMode.SegmentSelection:
                    layerName = "select";
                    break;
                case DrawingMode.CollisionMap:
                    layerName = "col";
                    break;
                case DrawingMode.Ledges:
                    layerName = "ledge";
                    break;
                case DrawingMode.Script:
                    layerName = "script";
                    break;
            }

            if (text.DrawClickText(5, 25, "draw: " + layerName, mosX, mosY, mouseClick))
                drawType = (DrawingMode)(((int)drawType + 1) % 4);

            if (drawType == DrawingMode.Script)
            {
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                spriteBatch.Draw(nullTex,new Rectangle(400,20,400,565), new Color(new Vector4(0,0,0,.62f)));
                spriteBatch.End();

                for(int i = 0; i < scriptScroll + 28; i++)
                {
                    if (selScript == i)
                    {
                        text.Color = Color.White;
                        text.DrawText(405, 25 +(i - scriptScroll) * 20,
                            i.ToString() + ": " + map.Scripts[i] + "*");

                    }
                    else
                    {
                        if (text.DrawClickText(405, 25 +(i - scriptScroll) * 20,
                            i.ToString() + ": " + map.Scripts[i], mosX, mosY, mouseClick))
                        {
                            selScript = i;
                            editMode = EditingMode.Script;
                        }

                    }

                    if (map.Scripts[i].Length > 0)
                    {
                        String[] split = map.Scripts[i].Split(' ');

                        int c = GetCommandColor(split[0]);
                        if (c > COLOR_NONE)
                        {
                            switch (c)
                            {
                                case COLOR_GREEN:
                                    text.Color = Color.Lime;
                                    break;

                                case COLOR_YELLOW:
                                    text.Color = Color.Yellow;
                                    break;
                            }
                            text.DrawText(405, 25 +(i - scriptScroll) * 20,
                                i.ToString() + ": " + split[0]);
                        }

                        text.Color = Color.White;
                        text.DrawText(405, 25 + (i - scriptScroll) * 20,
                            i.ToString() + ": ");

                    }
                }

                bool mouseDown = (Mouse.GetState().LeftButton == ButtonState.Pressed);

                if (DrawButton(770, 20, 1, mosX, mosY, mouseDown) && scriptScroll > 0)
                    scriptScroll--;
                if (DrawButton(770, 550, 2, mosX, mosY, mouseDown) && scriptScroll < map.Scripts.Length - 28)
                    scriptScroll++;

            }

            text.Color = Color.White;
            if (editMode == EditingMode.Path)
                text.DrawText(5, 45, map.Path + "*");
            else
            {
                if (text.DrawClickText(5, 45, map.Path, mosX, mosY, mouseClick))
                    editMode = EditingMode.Path;
            }

            mouseClick = false;
        }

        private void DrawCursor()
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            spriteBatch.Draw(iconsTex,
                new Vector2(mosX, mosY),
                new Rectangle(0, 0, 32, 32),
                Color.White, 0.0f,
                new Vector2(0, 0),
                1.0f,
                SpriteEffects.None,
                0.0f);

            spriteBatch.End();
        }

        private void DrawMapSegments()
        {
            Rectangle sRect = new Rectangle();
            Rectangle dRect = new Rectangle();

            text.Size = 0.8f;

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(nullTex, new Rectangle(500, 20, 280, 550), new Color(0, 0, 0, 100));
            spriteBatch.End();

            for(int i = 0; i < 9; i++)
            {
                SegmentDefinition segDef = map.SegmentDefinitions[i];

                if (segDef == null)
                    continue;

                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

                dRect.X = 500;
                dRect.Y = 50 + i * 60;

                sRect = segDef.SourceRect;

                if (sRect.Width > sRect.Height)
                {
                    dRect.Width = 45;
                    dRect.Height = (int)(((float)sRect.Height / (float)sRect.Width) * 45.0f);
                }
                else
                {
                    dRect.Height = 45;
                    dRect.Width = (int)(((float)sRect.Width / (float)sRect.Height) * 45.0f);
                }

                spriteBatch.Draw(mapsTex[segDef.SourceIndex],
                    dRect,
                    sRect,
                    Color.White
                );

                spriteBatch.End();

                text.Color = Color.White;

                text.DrawText(dRect.X + 50, dRect.Y, segDef.Name);

                if (rightMouseDown)
                {
                    if (mosX > dRect.X && mosX < 780 && mosY > dRect.Y && mosY < dRect.Y + 45)
                    {
                        if (mouseDragSeg == -1)
                        {
                            int f = map.AddSeg(curLayer, i);

                            if (f <= -1)
                                continue;

                            float layerScalar = 0.5f;
                            if (curLayer == 0)
                                layerScalar = 0.375f;
                            else if (curLayer == 2)
                                layerScalar = 0.625f;

                            map.Segments[curLayer, f].Location.X = (mosX - sRect.Width / 4 + scroll.X * layerScalar);
                            map.Segments[curLayer, f].Location.Y = (mosY - sRect.Height / 4 + scroll.Y * layerScalar);

                            mouseDragSeg = f;
                        }
                    }
                }
            }

            Vector2 v = new Vector2((float)mosX, (float)mosY) + scroll / 2.0f;
            v *= 2f;
            text.Size = .75f;
            text.Color = Color.White;
            text.DrawText(5, 580, ((int)v.X).ToString() + ", " +
                ((int)v.Y).ToString());

        }

        private int GetCommandColor(String s)
        {
            switch (s)
            {
                case "fog":
                case "monster":
                case "makebucket":
                case "addbucket":
                case "ifnotbucketgoto":

                case "wait":

                case "setflag":
                case "iftruegoto":
                case "iffalsegoto":

                case "setglobalflag":
                case "ifglobaltruegoto":
                case "ifglobalfalsegoto":

                case "stop":
                case "setleftexit":
                case "setleftentrance":
                case "setrightexit":
                case "setrightentrance":
                case "setintroentrance":
                case "ifscrollxgreatergoto":
                case "monsterleft":
                case "monsterright":
                case "water":
                    return COLOR_GREEN;
                case "tag":
                    return COLOR_YELLOW;
            }
            return COLOR_NONE;
        }
    }
}
