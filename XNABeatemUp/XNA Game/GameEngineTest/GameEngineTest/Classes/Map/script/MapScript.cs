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
    public enum TransitionDirection : int
    {
        None = -1,
        Left = 0,
        Right = 1,
        Intro = 2,
        Exit = 3
    }

    public class MapScript
    {
        Map map;

        public MapScriptLine[] Lines;

        int curLine;
        float waiting;
        public bool IsReading;

        public MapFlags Flags;

        public MapScript(Map _map)
        {
            map = _map;
            Flags = new MapFlags(32);
            Lines = new MapScriptLine[128];
        }

        public void DoScript(Character[] c)
        {
            if (waiting > 0f)
            {
                waiting -= Game1.frameTime;
            }
            else
            {
                bool done = false;
                while (!done)
                {
                    curLine++;

                    if (Lines[curLine] != null)
                    {
                        switch (Lines[curLine].Command)
                        {
                            case MapCommands.Fog:
                                map.fog = true;
                                break;

                            case MapCommands.Monster:
                                for (int i = 0; i < c.Length; i++)
                                {
                                    if (c[i] == null)
                                    {
                                        c[i] = new Character(
                                            Game1.charDef[(int)GetMonsterFromString(Lines[curLine].SParam[1])],
                                            Lines[curLine].VParam,
                                            i,                                            
                                            TeamName.badGuys);
                                        c[i].setMeleeAI();
                                        //c[i].PManager.MakeSpawnSmoke(c[i].position);
                                        if (Lines[curLine].SParam.Length > 5)
                                            c[i].Name = Lines[curLine].SParam[5];
                                        break;
                                    }
                                }

                                break;

                            case MapCommands.MonsterLeft:
                                for (int i = 0; i < c.Length; i++)
                                {
                                    if (c[i] == null)
                                    {
                                         Console.WriteLine(Game1.scroll.X + " " + Game1.scroll.Y);
                                        Vector3 spawn = new Vector3(Game1.scroll.X - 150, Game1.scroll.Y + 400, 0);
                                        Console.WriteLine(spawn.X + " " + spawn.Y);
                                        c[i] = new Character(
                                            Game1.charDef[(int)GetMonsterFromString(Lines[curLine].SParam[1])],
                                            Lines[curLine].VParam + spawn,
                                            i,
                                            TeamName.badGuys);
                                        c[i].setMeleeAI();
                                        //c[i].PManager.MakeSpawnSmoke(c[i].position);
                                        if (Lines[curLine].SParam.Length > 1)
                                            c[i].Name = Lines[curLine].SParam[1];
                                        break;
                                    }
                                }
                                break;

                            case MapCommands.MonsterRight:
                                for (int i = 0; i < c.Length; i++)
                                {
                                   
                                    if (c[i] == null)
                                    {
                                         Console.WriteLine(Game1.scroll.X + " " + Game1.scroll.Y);
                                        Vector3 spawn = new Vector3(Game1.scroll.X + 950, Game1.scroll.Y + 400, 0);
                                        Console.WriteLine(spawn.X + " " + spawn.Y);
                                        c[i] = new Character(
                                            Game1.charDef[(int)GetMonsterFromString(Lines[curLine].SParam[1])],
                                            Lines[curLine].VParam + spawn,
                                            i,
                                            TeamName.badGuys);
                                        c[i].setMeleeAI();
                                        //c[i].PManager.MakeSpawnSmoke(c[i].position);

                                        if (Lines[curLine].SParam.Length > 1)
                                            c[i].Name = Lines[curLine].SParam[1];
                                        break;
                                    }
                                }
                                break;

                            case MapCommands.MakeBucket:
                                map.bucket = new Bucket(Lines[curLine].IParam);
                                break;

                            case MapCommands.AddBucket:
                                map.bucket.AddItem(Lines[curLine].VParam,
                                    (int)GetMonsterFromString(Lines[curLine].SParam[1]));
                                break;

                            case MapCommands.IfNotBucketGoto:
                                if (map.bucket.isEmpty)
                                    GotoTag(Lines[curLine].SParam[1]);
                                break;

                            case MapCommands.Wait:
                                waiting = (float)Lines[curLine].IParam / 100f;
                                done = true;
                                break;

                            case MapCommands.SetFlag:
                                Flags.SetFlag(Lines[curLine].SParam[1]);
                                break;

                            case MapCommands.IfTrueGoto:
                                if (Flags.GetFlag(Lines[curLine].SParam[1]))
                                    GotoTag(Lines[curLine].SParam[2]);
                                break;

                            case MapCommands.IfFalseGoto:
                                if (!Flags.GetFlag(Lines[curLine].SParam[1]))
                                    GotoTag(Lines[curLine].SParam[2]);
                                break;

                            case MapCommands.SetGlobalFlag:
                                map.GlobalFlags.SetFlag(Lines[curLine].SParam[1]);
                                break;

                            case MapCommands.IfGlobalTrueGoto:
                                if (map.GlobalFlags.GetFlag(Lines[curLine].SParam[1]))
                                    GotoTag(Lines[curLine].SParam[2]);
                                break;

                            case MapCommands.IfGlobalFalseGoto:
                                if (!map.GlobalFlags.GetFlag(Lines[curLine].SParam[1]))
                                    GotoTag(Lines[curLine].SParam[2]);
                                break;

                            case MapCommands.Stop:
                                IsReading = false;
                                done = true;
                                break;

                            case MapCommands.Tag:
                                //
                                break;

                            case MapCommands.IfScrollXGreaterGoto:
                                if (Game1.scroll.X > Lines[curLine].IParam)
                                    GotoTag(Lines[curLine].SParam[2]);
                                break;

                            case MapCommands.SetExit:
                                break;

                            case MapCommands.SetLeftExit:
                                map.transitionDest[(int)TransitionDirection.Left] =
                                    Lines[curLine].SParam[1];
                                break;

                            case MapCommands.SetRightExit:
                                map.transitionDest[(int)TransitionDirection.Right] =
                                    Lines[curLine].SParam[1];
                                break;

                            case MapCommands.SetLeftEntrance:
                                if (map.transDir == TransitionDirection.Right)
                                {
                                    c[0].position = Lines[curLine].VParam;
                                    c[0].facing = CharDir.right;
                                    c[0].setAnim("idle");
                                    c[0].state = CharState.ground;
                                    c[0].trajectory = new Vector3(200f, 0f, 0f);
                                    map.transDir = TransitionDirection.None;
                                }
                                break;
                            case MapCommands.SetRightEntrance:
                                if (map.transDir == TransitionDirection.Left)
                                {
                                    c[0].position = Lines[curLine].VParam;
                                    c[0].facing = CharDir.left;
                                    c[0].setAnim("idle");
                                    c[0].state = CharState.ground;
                                    c[0].trajectory = new Vector3(-200f, 0f, 0f);
                                    map.transDir = TransitionDirection.None;
                                }
                                break;
                            case MapCommands.SetEntrance:
                                break;

                            case MapCommands.SetIntroEntrance:
                                if (map.transDir == TransitionDirection.Intro)
                                {
                                    c[0].position = Lines[curLine].VParam;
                                    c[0].facing = CharDir.right;
                                    c[0].setAnim("idle");
                                    c[0].state = CharState.ground;
                                    c[0].trajectory = Vector3.Zero;
                                    map.transDir = TransitionDirection.None;
                                }

                                break;
                        }
                    }
                }
            }
        }

        public bool GotoTag(String tag)
        {
            for (int i = 0; i < Lines.Length; i++)
            {
                if(Lines[i] != null)
                {
                    if (Lines[i].Command == MapCommands.Tag)
                    {
                        if (Lines[i].SParam[1] == tag)
                        {
                            curLine = i;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static CharacterDefinitions GetMonsterFromString(String m)
        {
            switch (m)
            {
                case "orc":
                    return CharacterDefinitions.orc;
                    
                case "lizard":
                    return CharacterDefinitions.lizard;
            }

            return CharacterDefinitions.orc;
        }


    }
}
