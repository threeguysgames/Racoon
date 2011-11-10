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

using CharacterEditor.Character;
using TextLib;

namespace CharacterEditor
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;
        Text text;
        CharDef charDef;

        Texture2D nullTex;
        Texture2D iconsTex;
        Texture2D[] legsTex = new Texture2D[3];
        Texture2D[] torsoTex = new Texture2D[3];
        Texture2D[] headTex = new Texture2D[3];
        Texture2D[] weaponTex = new Texture2D[1];

        const int FACE_LEFT = 0;
        const int FACE_RIGHT = 1;

        const int AUX_SCRIPT = 0;
        const int AUX_TRIGS = 1;
        const int AUX_TEXTURES = 2;

        int auxMode = AUX_SCRIPT;
        int trigScroll = 0;
        int textScroll = 0;

        const int TRIG_ROCKET_ACROSS = 0;
        const int TRIG_ROCKET_DOWN = 1;
        const int TRIG_ROCKET_UP = 2;
        const int TRIG_MELEE_DOWN = 3;
        const int TRIG_MELEE_UP = 4;
        const int TRIG_MELEE_DIAG_UP = 5;
        const int TRIG_MELEE_DIAG_DOWN = 6;
        const int TRIG_MELEE_UPPERCUT = 7;
        const int TRIG_MELEE_SMACKDOWN = 8;
        const int TRIG_ROCKET_RAIN = 9;

        int selPart = 0;
        int selFrame = 0;
        int selAnim = 0;
        int selKeyFrame = 0;
        int selScriptLine = 0;

        int curKey = 0;
        bool playing = false;
        float curFrame = 0;

        int frameScroll;
        int animScroll;
        int keyFrameScroll;

        MouseState mouseState;
        MouseState preState;

        KeyboardState keyState;
        KeyboardState oldKeyState;

        bool mouseClick;

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

            charDef = new CharDef();
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
            iconsTex = Content.Load<Texture2D>(@"gfx/icons");

            LoadTextures(legsTex, @"gfx/legs");
            LoadTextures(torsoTex, @"gfx/torso");
            LoadTextures(headTex, @"gfx/head");
            LoadTextures(weaponTex, @"gfx/weapon");
        }

        private void LoadTextures(Texture2D[] textures, string path)
        {
            for (int i = 0; i < textures.Length; i++)
                textures[i] = Content.Load<Texture2D>(path + (i + 1).ToString());
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void CopyFrame(int src, int dest)
        {
            Frame keySrc = charDef.Frames[src];
            Frame keyDest = charDef.Frames[dest];

            keyDest.Name = keySrc.Name;

            for (int i = 0; i < keyDest.Parts.Length; i++)
            {
                Part srcPart = keySrc.Parts[i];
                Part destPart = keyDest.Parts[i];

                destPart.Index = srcPart.Index;
                destPart.Location = srcPart.Location;
                destPart.Rotation = srcPart.Rotation;
                destPart.Scaling = srcPart.Scaling;
            }
        }

        private void SwapParts(int idx1, int idx2)
        {
            if (idx1 < 0 || idx2 < 0 ||
                idx1 >= charDef.Frames[selFrame].Parts.Length ||
                idx2 >= charDef.Frames[selFrame].Parts.Length)
                return;

            Part t = new Part();

            Part i = charDef.Frames[selFrame].Parts[idx1];
            Part j = charDef.Frames[selFrame].Parts[idx2];

            charDef.Frames[selFrame].Parts[idx1] = j;
            charDef.Frames[selFrame].Parts[idx2] = i;
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
            mouseState = Mouse.GetState();

            int xM = mouseState.X - preState.X;
            int yM = mouseState.Y - preState.Y;

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (preState.LeftButton == ButtonState.Pressed)
                {
                    charDef.Frames[selFrame].Parts[selPart].Location +=
                        new Vector2((float)xM / 2.0f, (float)yM / 2.0f);
                }
            }
            else
            {
                if (preState.LeftButton == ButtonState.Pressed)
                {
                    mouseClick = true;
                }
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                if (preState.RightButton == ButtonState.Pressed)
                {
                    charDef.Frames[selFrame].Parts[selPart].Rotation += (float)yM / 100.0f;
                }
            }

            if (mouseState.MiddleButton == ButtonState.Pressed)
            {
                if (preState.MiddleButton == ButtonState.Pressed)
                {
                    charDef.Frames[selFrame].Parts[selPart].Scaling +=
                        new Vector2((float)xM * 0.01f, (float)yM * 0.01f);
                }
            }

            preState = mouseState;

            UpdateKeys();

            Animation animation = charDef.Animations[selAnim];
            KeyFrame keyframe = animation.KeyFrames[curKey];

            if (playing)
            {
                curFrame += (float)gameTime.ElapsedGameTime.TotalSeconds * 30.0f;

                if (curFrame > keyframe.Duration)
                {
                    curFrame -= keyframe.Duration;
                    curKey++;

                    if (curKey >= animation.KeyFrames.Length)
                        curKey = 0;

                    keyframe = animation.KeyFrames[curKey];
                }
            }
            else
                curKey = selKeyFrame;

            if (keyframe.FrameRef < 0)
                curKey = 0;

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

        private void PressKey(Keys key)
        {
            string t = String.Empty;

            switch (editMode)
            {
                case EditingMode.FrameName:
                    t = charDef.Frames[selFrame].Name;
                    break;
                case EditingMode.AnimationName:
                    t = charDef.Animations[selAnim].Name;
                    break;
                case EditingMode.PathName:
                    t = charDef.Path;
                    break;
                case EditingMode.Script:
                    t = charDef.Animations[selAnim].KeyFrames[selKeyFrame].Scripts[selScriptLine];
                    break;
            }

            if (key == Keys.Back)
            {
                if (t.Length > 0) t = t.Substring(0, t.Length - 1);
            }
            else if (key == Keys.Enter)
            {
                editMode = EditingMode.None;
            }
            else
            {
                t = (t + (char)key).ToLower();
            }

            switch (editMode)
            {
                case EditingMode.FrameName:
                    charDef.Frames[selFrame].Name = t;
                    break;
                case EditingMode.AnimationName:
                    charDef.Animations[selAnim].Name = t;
                    break;
                case EditingMode.PathName:
                    charDef.Path = t;
                    break;
                case EditingMode.Script:
                    charDef.Animations[selAnim].KeyFrames[selKeyFrame].Scripts[selScriptLine] = t;
                    break;
            }
        }

        private string GetTrigName(int idx)
        {
            switch (idx)
            {
                case TRIG_ROCKET_ACROSS:
                    return "rocket across";
                case TRIG_ROCKET_DOWN:
                    return "rocket down";
                case TRIG_ROCKET_UP:
                    return "rocket up";
                case TRIG_MELEE_DOWN:
                    return "melee down";
                case TRIG_MELEE_UP:
                    return "melee up";
                case TRIG_MELEE_DIAG_UP:
                    return "melee diag up";
                case TRIG_MELEE_DIAG_DOWN:
                    return "melee diag down";
                case TRIG_MELEE_UPPERCUT:
                    return "melee uppercut";
                case TRIG_MELEE_SMACKDOWN:
                    return "melee smackdown";
                case TRIG_ROCKET_RAIN:
                    return "rocket rain";
            }
            return "";
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(nullTex, new Rectangle(300, 450, 200, 5), new Color(new
                Vector4(1.0f, 0.0f, 0.0f, 0.5f)));
            spriteBatch.Draw(nullTex, new Rectangle(0, 0, 200, 450), new Color(new
                Vector4(0.0f, 0.0f, 0.0f, 0.5f)));
            spriteBatch.Draw(nullTex, new Rectangle(590, 0, 300, 600), new Color(new
                Vector4(0.0f, 0.0f, 0.0f, 0.5f)));
            spriteBatch.Draw(nullTex, new Rectangle(200, 0, 150, 110), new Color(new
                Vector4(0.0f, 0.0f, 0.0f, 0.5f)));
            spriteBatch.End();

            // TODO: Add your drawing code here
            if (selFrame > 0)
                DrawCharacter(new Vector2(400f, 450f), 2f, FACE_RIGHT, selFrame - 1, false, 0.2f);
            if (selFrame < charDef.Frames.Length - 1)
                DrawCharacter(new Vector2(400f, 450f), 2f, FACE_RIGHT, selFrame + 1, false, 0.2f);

            DrawCharacter(new Vector2(400f, 450f), 2f, FACE_RIGHT, selFrame, false, 1.0f);

            DrawPalette();
            DrawPartsList();
            DrawFramesList();
            DrawAnimationList();
            DrawKeyFramesList();

            int fref = charDef.Animations[selAnim].KeyFrames[curKey].FrameRef;
            if (fref < 0)
                fref = 0;

            DrawCharacter(new Vector2(500f, 100f), 0.5f, FACE_LEFT, fref, true, 1.0f);
            if (playing)
            {
                if (text.DrawClickText(480, 100, "stop", mouseState.X, mouseState.Y, mouseClick))
                    playing = false;
            }
            else
            {
                if (text.DrawClickText(480, 100, "play", mouseState.X, mouseState.Y, mouseClick))
                    playing = true;
            }

            if (DrawButton(200, 5, 3,mouseClick))
                charDef.Write();
            if (DrawButton(230, 5, 4, mouseClick))
                charDef.Read();

            if (editMode == EditingMode.PathName)
            {
                text.Color = Color.Lime;

                text.DrawText(270, 15, charDef.Path + "*");
            }
            else
            {
                if (text.DrawClickText(270, 15, charDef.Path, mouseState.X, mouseState.Y, mouseClick))
                    editMode = EditingMode.PathName;
            }

            if (auxMode == AUX_SCRIPT)
            {
                text.Color = Color.Lime;
                text.DrawText(210, 110, "script");
            }
            else
            {
                if (text.DrawClickText(210, 110, "script", mouseState.X,
                    mouseState.Y, mouseClick))
                    auxMode = AUX_SCRIPT;
            }
            if (auxMode == AUX_TRIGS)
            {
                text.Color = Color.Lime;
                text.DrawText(260, 110, "trigs");
            }
            else
            {
                if (text.DrawClickText(260, 110, "trigs", mouseState.X,
                    mouseState.Y, mouseClick))
                    auxMode = AUX_TRIGS;
            }
            if (auxMode == AUX_TEXTURES)
            {
                text.Color = Color.Lime;
                text.DrawText(300, 110, "textures");
            }
            else
            {
                if (text.DrawClickText(300, 110, "textures", mouseState.X,
                    mouseState.Y, mouseClick))
                    auxMode = AUX_TEXTURES;
            }
            #region Script
            if (auxMode == AUX_SCRIPT)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (editMode == EditingMode.Script && selScriptLine == i)
                    {
                        text.Color = Color.Lime;
                        text.DrawText(210, 42 + i * 16, i.ToString() + ": " + charDef.Animations[selAnim].KeyFrames[selKeyFrame].Scripts[i] + "*");
                    }
                    else
                    {
                        if (text.DrawClickText(210, 42 + i * 16, i.ToString() + ": " + charDef.Animations[selAnim].KeyFrames[selKeyFrame].Scripts[i], mouseState.X, mouseState.Y, mouseClick))
                        {
                            selScriptLine = i;
                            editMode = EditingMode.Script;
                        }
                    }
                }
            }
            #endregion

            #region Trigs

            if (auxMode == AUX_TRIGS)
            {
                if (DrawButton(330, 41, 1,mouseClick))
                {
                    if (trigScroll > 0) trigScroll--;
                }

                if (DrawButton(330, 92, 2,mouseClick))
                    if (trigScroll < 100) trigScroll++;

                for (int i = 0; i < 4; i++)
                {
                    int t = i + trigScroll;
                    if (text.DrawClickText(210, 42 + i* 16,
                        GetTrigName(t), mouseState.X, mouseState.Y, mouseClick))
                    {
                        charDef.Frames[selFrame].Parts[selPart].Index = t + 1000;
                    }
                }
            }

            #endregion

            #region Textures


            if (auxMode == AUX_TEXTURES)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (DrawButton(210 + i * 21, 40, 1, mouseClick,0.45f))
                    {
                        switch (i)
                        {
                            case 0:
                                if (charDef.HeadIndex < headTex.Length - 1)
                                    charDef.HeadIndex++;
                                break;
                            case 1:
                                if (charDef.TorsoIndex < torsoTex.Length - 1)
                                    charDef.TorsoIndex++;
                                break;
                            case 2:
                                if (charDef.LegsIndex < legsTex.Length - 1)
                                    charDef.LegsIndex++;
                                break;
                            case 3:
                                if (charDef.WeaponIndex < weaponTex.Length - 1)
                                    charDef.WeaponIndex++;
                                break;
                        }

                    }
                    string t = charDef.HeadIndex.ToString();
                    switch (i)
                    {
                        case 1:
                            t = charDef.TorsoIndex.ToString();
                            break;
                        case 2:
                            t = charDef.LegsIndex.ToString();
                            break;
                        case 3:
                            t = charDef.WeaponIndex.ToString();
                            break;
                    }
                    text.Color = (Color.White);
                    text.DrawText(212 + i * 21, 60, t);
                    if (DrawButton(210 + i * 21, 85, 2, mouseClick, 0.45f))
                    {
                        switch (i)
                        {
                            case 0:
                                if (charDef.HeadIndex > 0) charDef.HeadIndex--;
                                break;
                            case 1:
                                if (charDef.TorsoIndex > 0) charDef.TorsoIndex--;
                                break;
                            case 2:
                                if (charDef.LegsIndex > 0) charDef.LegsIndex--;
                                break;
                            case 3:
                                if (charDef.WeaponIndex > 0) charDef.WeaponIndex--;
                                break;
                        }
                    }
                }
            }


            #endregion

            DrawCursor();

            mouseClick = false;

            base.Draw(gameTime);
        }

        private void DrawCursor()
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            spriteBatch.Draw(iconsTex,
                new Vector2(mouseState.X, mouseState.Y),
                new Rectangle(0, 0, 32, 32),
                Color.White, 0.0f,
                new Vector2(0, 0),
                1.0f,
                SpriteEffects.None,
                0.0f);

            spriteBatch.End();
        }

        private void DrawKeyFramesList()
        {
            for (int i = keyFrameScroll; i < keyFrameScroll + 13; i++)
            {
                Animation animation = charDef.Animations[selAnim];

                if (i < animation.KeyFrames.Length)
                {
                    int y = (i - keyFrameScroll) * 15 + 250;
                    int frameRef = animation.KeyFrames[i].FrameRef;

                    string name = "";

                    if (frameRef > -1)
                        name = charDef.Frames[frameRef].Name;

                    if (i == selKeyFrame)
                    {
                        text.Color = Color.Lime;
                        text.DrawText(5, y, i.ToString() + ": " + name);
                    }
                    else
                    {
                        if (text.DrawClickText(5, y, i.ToString() + ": " + name, mouseState.X, mouseState.Y, mouseClick))
                            selKeyFrame = i;
                    }

                    if (frameRef > -1)
                    {
                        if (text.DrawClickText(110, y, "-", mouseState.X, mouseState.Y, mouseClick))
                        {
                            animation.KeyFrames[i].Duration--;
                            if (animation.KeyFrames[i].Duration <= 0)
                            {
                                for (int j = i; j < animation.KeyFrames.Length - 1; j++)
                                {
                                    KeyFrame keyframe = animation.KeyFrames[j];

                                    keyframe.FrameRef = animation.KeyFrames[j + 1].FrameRef;
                                    keyframe.Duration = animation.KeyFrames[j + 1].Duration;
                                }

                                animation.KeyFrames[animation.KeyFrames.Length - 1].FrameRef = -1;
                            }
                        }

                        text.DrawText(125, y, animation.KeyFrames[i].Duration.ToString());

                        if (text.DrawClickText(140, y, "+", mouseState.X, mouseState.Y, mouseClick))
                            animation.KeyFrames[i].Duration++;
                    }
                }
            }

            if (DrawButton(170, 250, 1,  (mouseState.LeftButton == ButtonState.Pressed)) && keyFrameScroll > 0)
                keyFrameScroll--;

            if (DrawButton(170, 410, 2,  (mouseState.LeftButton == ButtonState.Pressed)) && keyFrameScroll < charDef.Animations[selAnim].KeyFrames.Length - 13)
                keyFrameScroll++;
        }

        private void DrawAnimationList()
        {
            for (int i = animScroll; i < animScroll + 15; i++)
            {
                if (i < charDef.Animations.Length)
                {
                    int y = (i - animScroll) * 15 + 5;
                    if (i == selAnim)
                    {
                        text.Color = Color.Lime;

                        text.DrawText(5, y, i.ToString() + ": " + charDef.Animations[i].Name + ((editMode == EditingMode.AnimationName) ? "*" : ""));
                    }
                    else
                    {
                        if (text.DrawClickText(5, y, i.ToString() + ": " + charDef.Animations[i].Name, mouseState.X, mouseState.Y, mouseClick))
                        {
                            selAnim = i;
                            editMode = EditingMode.AnimationName;
                        }
                    }
                }
            }

            if (DrawButton(170, 5, 1,  (mouseState.LeftButton == ButtonState.Pressed)) && animScroll > 0)
                animScroll--;

            if (DrawButton(170, 200, 2,  (mouseState.LeftButton == ButtonState.Pressed)) && animScroll < charDef.Animations.Length - 15)
                animScroll++;
        }

        private void DrawFramesList()
        {
            for (int i = frameScroll; i < frameScroll + 20; i++)
            {
                if (i < charDef.Frames.Length)
                {
                    int y = (i - frameScroll) * 15 + 280;

                    if (i == selFrame)
                    {
                        text.Color = Color.Lime;

                        text.DrawText(600, y, i.ToString() + ": " + charDef.Frames[i].Name + ((editMode == EditingMode.FrameName) ? "*" : ""));

                        if (text.DrawClickText(720, y, "(a)", mouseState.X, mouseState.Y, mouseClick))
                        {
                            Animation animation = charDef.Animations[selAnim];

                            for (int j = 0; j < animation.KeyFrames.Length; j++)
                            {
                                KeyFrame keyFrame = animation.KeyFrames[j];

                                if (keyFrame.FrameRef == -1)
                                {
                                    keyFrame.FrameRef = i;
                                    keyFrame.Duration = 1;

                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (text.DrawClickText(600, y, i.ToString() + ": " + charDef.Frames[i].Name, mouseState.X, mouseState.Y, mouseClick))
                        {
                            if (selFrame != i)
                            {
                                if (String.IsNullOrEmpty(charDef.Frames[i].Name))
                                    CopyFrame(selFrame, i);

                                selFrame = i;
                                editMode = EditingMode.FrameName;
                            }
                        }
                    }
                }
            }

            if (DrawButton(770, 280, 1, (mouseState.LeftButton == ButtonState.Pressed)) && frameScroll > 0) frameScroll--;
            if (DrawButton(770, 570, 2,  (mouseState.LeftButton == ButtonState.Pressed)) && frameScroll < charDef.Frames.Length - 20) frameScroll++;
        }

        private void DrawPartsList()
        {
            for (int i = 0; i < charDef.Frames[selFrame].Parts.Length; i++)
            {
                int y = 5 + i * 15;

                text.Size = 0.75f;

                string line = "";

                int index = charDef.Frames[selFrame].Parts[i].Index;
                if (index < 0)
                    line = "";
                else if (index < 64)
                    line = "head" + index.ToString();
                else if (index < 74)
                    line = "torso" + index.ToString();
                else if (index < 128)
                    line = "arms" + index.ToString();
                else if (index < 192)
                    line = "legs" + index.ToString();
                else
                    line = "weapon" + index.ToString();

                if (selPart == i)
                {
                    text.Color = Color.Lime;
                    text.DrawText(600, y, i.ToString() + ": " + line);

                    if (DrawButton(700, y, 1,  mouseClick,0.8f))
                    {
                        SwapParts(selPart, selPart - 1);
                        if (selPart > 0) selPart--;
                    }

                    if (DrawButton(720, y, 2, mouseClick, 0.8f))
                    {
                        SwapParts(selPart, selPart + 1);
                        if (selPart < charDef.Frames[selFrame].Parts.Length - 1)
                            selPart++;
                    }

                    Part part = charDef.Frames[selFrame].Parts[selPart];
                    if (text.DrawClickText(740, y, (part.Flip == 0 ? "(n)" : "(m)"),
                        mouseState.X, mouseState.Y, mouseClick))
                    {
                        part.Flip = 1 - part.Flip;
                    }

                    if (text.DrawClickText(762, y, "(r)", mouseState.X, mouseState.Y, mouseClick))
                        part.Scaling = new Vector2(1.0f, 1.0f);

                    if (text.DrawClickText(780, y, "(x)", mouseState.X, mouseState.Y, mouseClick))
                        part.Index = -1;
                }
                else
                {
                    if (text.DrawClickText(600, y, i.ToString() + ": " + line, mouseState.X, mouseState.Y, mouseClick))
                        selPart = i;
                }
            }
        }

        private bool DrawButton(int x, int y, int idx, bool mouseClick)
        {
            return DrawButton(x, y, idx, mouseClick, 1.0f);
        }

        private bool DrawButton(int x, int y, int idx, bool mouseClick, float scale)
        {
            bool r = false;

            Rectangle sRect = new Rectangle(32 * (idx % 8),
                32 * (idx / 8), 32, 32);
            Rectangle dRect = new Rectangle(x, y,
                (int)(32f * scale),
                (int)(32f * scale));

            if (dRect.Contains(mouseState.X, mouseState.Y))
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
        private void DrawPalette()
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            for (int l = 0; l < 4; l++)
            {
                Texture2D texture = null;
                switch (l)
                {
                    case 0: 
                        texture = headTex[charDef.HeadIndex];
                        break;
                    case 1:
                        texture = torsoTex[charDef.TorsoIndex];
                        break;
                    case 2:
                        texture = legsTex[charDef.LegsIndex];
                        break;
                    case 3:
                        texture = weaponTex[charDef.WeaponIndex];
                        break;
                }

                if (texture != null)
                {
                    for (int i = 0; i < 25; i++)
                    {
                        Rectangle sRect = new Rectangle((i % 5) * 64,
                            (i / 5) * 64, 64, 64);

                        Rectangle dRect = new Rectangle(i * 23, 467 + l * 32, 23, 32);

                        spriteBatch.Draw(nullTex, dRect, new Color(0, 0, 0, 25));

                        if (l == 3)
                        {
                            sRect.X = (i % 4) * 80;
                            sRect.Y = (i / 4) * 64;
                            sRect.Width = 80;

                            if (i < 15)
                            {
                                dRect.X = i * 30;
                                dRect.Width = 30;
                            }
                        }

                        spriteBatch.Draw(texture, dRect, sRect, Color.White);

                        if (dRect.Contains(mouseState.X, mouseState.Y))
                        {
                            if (mouseClick)
                            {
                                charDef.Frames[selFrame].Parts[selPart].Index = i + 64 * l;
                            }
                        }
                    }
                }
            }

            spriteBatch.End();
        }

        private void DrawCharacter(Vector2 loc, float scale, 
            int face, int frameIndex, bool preview, float alpha)
        {
            Rectangle sRect = new Rectangle();

            Frame frame = charDef.Frames[frameIndex];

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            for (int i = 0; i < frame.Parts.Length; i++)
            {
                Part part = frame.Parts[i];
                if (part.Index > -1)
                {
                    sRect.X = ((part.Index % 64) % 5) * 64;
                    sRect.Y = ((part.Index % 64) / 5) * 64;
                    sRect.Width = 64;
                    sRect.Height = 64;

                    if (part.Index >= 192)
                    {
                        sRect.X = ((part.Index % 64) % 3) * 80;
                        sRect.Width = 80;
                    }

                    float rotation = part.Rotation;

                    Vector2 location = part.Location * scale + loc;
                    Vector2 scaling = part.Scaling * scale;
                    if (part.Index >= 128) scaling *= 1.35f;

                    if (face == FACE_LEFT)
                    {
                        rotation = -rotation;
                        location.X -= part.Location.X * scale * 2.0f;
                    }

                    if ((part.Index >= 1000) && alpha >= 1f)
                    {
                        spriteBatch.End();
                        text.Color = Color.Lime;
                        if (preview)
                        {
                            //text.Size = 0.45f;
                            text.DrawText((int)location.X, (int)location.Y, "*");
                        }
                        else
                        {
                            text.Size = 1f;
                            text.DrawText((int)location.X, (int)location.Y, "*" +
                                GetTrigName(part.Index - 1000));
                        }
                        spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

                    }

                    else
                    {
                        Texture2D texture = null;

                        int t = part.Index / 64;
                        switch (t)
                        {
                            case 0:
                                texture = headTex[charDef.HeadIndex];
                                break;
                            case 1:
                                texture = torsoTex[charDef.TorsoIndex];
                                break;
                            case 2:
                                texture = legsTex[charDef.LegsIndex];
                                break;
                            case 3:
                                texture = weaponTex[charDef.WeaponIndex];
                                break;
                        }

                        Color color = new Color(255, 255, 255, (byte)(alpha * 255));

                        if (!preview && selPart == i)
                            color = new Color(255, 0, 0, (byte)(alpha * 255));

                        bool flip = false;

                        if ((face == FACE_RIGHT && part.Flip == 0) ||
                            (face == FACE_LEFT && part.Flip == 1))
                            flip = true;

                        if (texture != null)
                        {
                            spriteBatch.Draw(
                                texture,
                                location,
                                sRect,
                                color,
                                rotation,
                                new Vector2((float)sRect.Width / 2f, 32f),
                                scaling,
                                (flip ? SpriteEffects.None : SpriteEffects.FlipHorizontally),
                                1.0f
                                );
                        }
                    }
                   
                }
            }
            spriteBatch.End();
        }
    }
}
