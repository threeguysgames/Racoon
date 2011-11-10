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
    public class Menu
    {
        public enum Trans : int
        {
            Buttons = 0,
            All = 1
        }

        public enum Level : int
        {
            Main = 0,
            Options = 1,
            Quit = 2,
            NewGame = 3,
            ResumeGame = 4,
            EndGame = 5,
            Dead = 6
        }

        public enum Option : int
        {
            None = -1,
            NewGame = 0,
            Continue = 1,
            Options = 2,
            Quit = 3,
            Back = 4,
            ResumeGame = 5,
            EndGame = 6
        }

        public enum MenuMode : int
        {
            Main = 0,
            Pause = 1,
            Dead = 2
        }

        public float transFrame = 1f;
        public Trans transType = Trans.All;
        public Level transGoal;
        public Level level = Level.Main;
        public int sellItem;

        Texture2D tex;
        Texture2D foreTex;
        Texture2D optionsTex;
        Texture2D backTex;
        Texture2D spritesTex;

        SpriteBatch sprite;

        int[] levelSel = new int[32];

        Option[] option = new Option[10];
        float[] optionFrame = new float[10];

        int totalOptions = 0;
        float frame;

        GamePadState[] oldState = new GamePadState[4];
        KeyboardState[] keyOldState = new KeyboardState[1];

        public MenuMode menuMode = MenuMode.Main;

        public Menu(Texture2D _tex,
            Texture2D _foreTex,
            Texture2D _optionsTex,
            Texture2D _backTex,
            Texture2D _spritesTex,
            SpriteBatch _sprite)
        {
            tex = _tex;
            foreTex = _foreTex;
            optionsTex = _optionsTex;
            backTex = _backTex;
            spritesTex = _spritesTex;
            sprite = _sprite;
            

        }

        public void Update(Game1 game)
        {
            frame += Game1.frameTime / 2f;
            if (frame > 6.28) frame -= 6.28f;

            if (transFrame < 2f)
            {
                float pFrame = transFrame;
                transFrame += Game1.frameTime;
                if (transType == Trans.Buttons)
                {
                    transFrame += Game1.frameTime;
                }

                if (pFrame < 1f && transFrame >= 1f)
                {
                    levelSel[(int)level] = sellItem;
                    level = transGoal;
                    sellItem = levelSel[(int)level];
                    switch (level)
                    {
                        case Level.NewGame:
                            game.NewGame();
                            break;

                        case Level.ResumeGame:
                            Game1.GameMode = Game1.GameModes.Playing;
                            break;

                        case Level.Quit:
                            
                            break;

                        case Level.EndGame:
                            Main();
                            break;
                    }
                }
            }

            for (int i = 0; i < optionFrame.Length; i++)
            {
                if (sellItem == i)
                {
                    if (optionFrame[i] < 1f)
                    {
                        optionFrame[i] += Game1.frameTime * 7f;
                        if (optionFrame[i] > 1f) optionFrame[i] = 1f;
                    }
                }
                else
                {
                    if (optionFrame[i] > 0f)
                    {
                        optionFrame[i] -= Game1.frameTime * 4f;
                        if(optionFrame[i] < 0f) optionFrame[i] = 0f;
                    }
                }
            }
            
            PopulateOptions();

            for (int i = 0; i < 1; i++)
            {
                KeyboardState ks = Keyboard.GetState();

                if (totalOptions > 0)
                {
                    if ((ks.IsKeyDown(Keys.Left) && keyOldState[i].IsKeyUp(Keys.Left)) ||
                        (ks.IsKeyDown(Keys.Up) && keyOldState[i].IsKeyUp(Keys.Up)))
                    {
                        sellItem = (sellItem + (totalOptions - 1)) % totalOptions;
                    }

                    if ((ks.IsKeyDown(Keys.Right) && keyOldState[i].IsKeyUp(Keys.Right)) ||
                        (ks.IsKeyDown(Keys.Down) && keyOldState[i].IsKeyUp(Keys.Down)))
                    {
                        sellItem = (sellItem + 1) % totalOptions;
                    }

                }

                bool ok = false;
                if (transFrame > 1.9f)
                {
                    if (ks.IsKeyDown(Keys.Enter) && keyOldState[i].IsKeyUp(Keys.Enter))
                    {
                        ok = true;
                    }

                    if (ks.IsKeyDown(Keys.Escape) && keyOldState[i].IsKeyUp(Keys.Escape))
                    {
                        if (menuMode == MenuMode.Main || menuMode == MenuMode.Dead)
                            ok = true;
                        else
                        {
                            Transition(Level.ResumeGame, true);
                        }
                    }

                    if (ok)
                    {
                        switch (level)
                        {
                            case Level.Main:
                                switch (option[sellItem])
                                {
                                    case Option.NewGame:
                                        Transition(Level.NewGame, true);

                                        break;
                                    case Option.ResumeGame:
                                        Transition(Level.ResumeGame, true);
                                        break;
                                    case Option.EndGame:
                                        Transition(Level.EndGame, true);
                                        break;
                                    case Option.Continue:

                                        break;
                                }
                                break;
                            case Level.Dead:
                                switch (option[sellItem])
                                {
                                    case Option.EndGame:
                                        Transition(Level.EndGame, true);
                                        break;
                                    case Option.Quit:
                                        Transition(Level.Quit, true);
                                        break;
                                }
                                break;
                            case Level.Options:
                                switch (option[sellItem])
                                {
                                    case Option.Back:
                                        Transition(Level.Main);
                                        
                                        break;
                                }
                                break;

                        }
                    }
                }

                keyOldState[i] = ks;

            }

        }

        private void Transition(Level goal)
        {
            Transition(goal, false);
        }

        private void Transition(Level goal, bool all)
        {
            transGoal = goal;
            transFrame = 0f;
            if (all)
                transType = Trans.All;
            else
                transType = Trans.Buttons;
        }

        private float GetAlpha(bool buttons)
        {
            if (!buttons && transType == Trans.Buttons)
                return 1f;
            if (transFrame < 2f)
            {
                if (transFrame < 1f)
                    return 1f - transFrame;
                else
                    return transFrame - 1f;
            }
            return 1f;
        }

        public void Draw()
        {
            sprite.Begin(SpriteBlendMode.AlphaBlend);

            if (menuMode == MenuMode.Main)
            {
                sprite.Draw(backTex, new Rectangle(0, 0, 1280, 720), new Color(new Vector4(GetAlpha(false),
                    GetAlpha(false), GetAlpha(false), 1f)));
            }
            else if(menuMode == MenuMode.Pause)
            {
                sprite.Draw(backTex, new Rectangle(0, 0, 1280, 720), new Rectangle(600, 400, 200, 200),
                    new Color(new Vector4(1f, 1f, 1f, .5f)));
            }

            sprite.End();
            sprite.Begin(SpriteBlendMode.Additive);

            float texAlpha = GetAlpha(false);
            if (menuMode != MenuMode.Dead)
            {
                if (menuMode != MenuMode.Main)
                    texAlpha = 0f;

                //Draw the menu texture
            }

            PopulateOptions();

            for (int i = 0; i < totalOptions; i++)
            {
                sprite.Draw(optionsTex,
                    new Vector2(190f + (float)i * 5f + optionFrame[i] * 10f + GetAlpha(true) * 50f,
                        300f + (float)i * 64f - (float)totalOptions * 32f),
                        new Rectangle(0, (int)option[i] * 64, 320, 64),
                        new Color(new Vector4(1f, 1f - optionFrame[i], 1f - optionFrame[i], GetAlpha(true))),
                        (1f - optionFrame[i]) * -.1f,
                        new Vector2(160f, 32f), 1f, SpriteEffects.None, 1f);
            }

            sprite.End();

            if (menuMode != MenuMode.Dead)
            {
                //Draw graphics for main menu
            }
        }

        private void PopulateOptions()
        {
            for (int i = 0; i < option.Length; i++)
            {
                option[i] = Option.None;
            }

            switch (level)
            {
                case Level.Main:
                    if (menuMode == MenuMode.Pause)
                    {
                        option[0] = Option.ResumeGame;
                        option[1] = Option.EndGame;
                        option[2] = Option.Options;
                        option[3] = Option.Quit;
                        totalOptions = 4;
                    }
                    else
                    {
                        option[0] = Option.NewGame;
                        option[1] = Option.Continue;
                        option[2] = Option.Options;
                        option[3] = Option.Quit;
                        totalOptions = 4;
                    }
                    break;

                case Level.Options:
                    option[0] = Option.Back;
                    totalOptions = 1;
                    break;

                case Level.Dead:
                    option[0] = Option.EndGame;
                    option[1] = Option.Quit;
                    totalOptions = 2;
                    break;

                default:
                    totalOptions = 0;
                    break;
            }
        }

        public void Pause()
        {
            menuMode = MenuMode.Pause;
            Game1.GameMode = Game1.GameModes.Menu;

            transFrame = 1f;
            level = Level.Main;
            transType = Trans.All;
        }

        public void Die()
        {
            menuMode = MenuMode.Dead;
            Game1.GameMode = Game1.GameModes.Menu;

            transFrame = 1;
            level = Level.Dead;
            transType = Trans.All;
        }

        public void Main()
        {
            Transition(Level.Main);
            Game1.GameMode = Game1.GameModes.Menu;
            menuMode = MenuMode.Main;
        }
    }
}
