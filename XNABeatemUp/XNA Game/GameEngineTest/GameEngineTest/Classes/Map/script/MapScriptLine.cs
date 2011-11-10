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
    public class MapScriptLine
    {
        public MapCommands Command;
        public int IParam;
        public Vector3 VParam;
        public string[] SParam;

        public MapScriptLine(string line)
        {
            if (line.Length < 1)
                return;

            SParam = line.Split(' ');
            switch (SParam[0])
            {
                case "fog":
                    Command = MapCommands.Fog;
                    break;
                case "monster":
                    Command = MapCommands.Monster;
                    VParam = new Vector3(
                        Convert.ToSingle(SParam[2]),
                        Convert.ToSingle(SParam[3]),
                        Convert.ToSingle(SParam[4]));
                    
                    break;
                case "makebucket":
                    Command = MapCommands.MakeBucket;
                    IParam = Convert.ToInt32(SParam[1]);
                    break;
                case "addbucket":
                    Command = MapCommands.AddBucket;
                    VParam = new Vector3(
                        Convert.ToSingle(SParam[2]),
                        Convert.ToSingle(SParam[3]),
                        Convert.ToSingle(SParam[4]));
                    break;
                case "ifnotbucketgoto":
                    Command = MapCommands.IfNotBucketGoto;
                    break;
                case "wait":
                    Command = MapCommands.Wait;
                    IParam = Convert.ToInt32(SParam[1]);
                    break;
                case "setflag":
                    Command = MapCommands.SetFlag;
                    break;
                case "iftruegoto":
                    Command = MapCommands.IfTrueGoto;
                    break;
                case "iffalsegoto":
                    Command = MapCommands.IfFalseGoto;
                    break;
                case "setglobalflag":
                    Command = MapCommands.SetGlobalFlag;
                    break;
                case "ifglobaltruegoto":
                    Command = MapCommands.IfGlobalTrueGoto;
                    break;
                case "ifglobalfalsegoto":
                    Command = MapCommands.IfGlobalFalseGoto;
                    break;
                case "stop":
                    Command = MapCommands.Stop;
                    break;
                case "tag":
                    Command = MapCommands.Tag;
                    break;
                case "ifscrollxgreatergoto":
                    IParam = Convert.ToInt32(SParam[1]);
                    Command = MapCommands.IfScrollXGreaterGoto;
                    break;

                case "monsterleft":
                    Command = MapCommands.MonsterLeft;
                    VParam = new Vector3(
                    0, 0, 0);
                    break;
                case "monsterright":
                    Command = MapCommands.MonsterRight;
                    VParam = new Vector3(
                    0, 0, 0);
                    break;

                case "setexit":
                    Command = MapCommands.SetExit;
                    VParam = new Vector3(Convert.ToSingle(SParam[1]),
                        Convert.ToSingle(SParam[2]), 0f);
                    break;
                case "setleftexit":
                    Command = MapCommands.SetLeftExit;
                    break;
                case "setrightexit":
                    Command = MapCommands.SetRightExit;
                    break;
                case "setleftentrance":
                    Command = MapCommands.SetLeftEntrance;
                    VParam = new Vector3(Convert.ToSingle(SParam[1]),
                        Convert.ToSingle(SParam[2]), 0f);
                    break;
                case "setrightentrance":
                    Command = MapCommands.SetRightEntrance;
                    VParam = new Vector3(Convert.ToSingle(SParam[1]),
                    Convert.ToSingle(SParam[2]), 0f);
                    break;

                case "setentrance":
                    Command = MapCommands.SetEntrance;
                    VParam = new Vector3(Convert.ToSingle(SParam[1]),
                        Convert.ToSingle(SParam[2]), 0f);
                    break;

                case "setintroentrance":
                    Command = MapCommands.SetIntroEntrance;
                    VParam = new Vector3(Convert.ToSingle(SParam[1]),
                        Convert.ToSingle(SParam[2]), 0f);
                    break;
            }
        }

    }
}
