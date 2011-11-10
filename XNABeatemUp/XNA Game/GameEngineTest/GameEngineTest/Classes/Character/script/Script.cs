using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngineTest
{
    public class Script
    {
        Character character;

        public Script(Character _character)
        {
            this.character = _character;
        }

        public void DoScript(int animIdx, int keyFrameIdx)
        {
            CharDef charDef = character.charDef;
            Animation animation = charDef.Animations[animIdx];
            KeyFrame keyFrame = animation.keyFrames[keyFrameIdx];

            bool done = false;

            for (int i = 0; i < keyFrame.Scripts.Length; i++)
            {
                if (done)
                    break;

                else
                {
                    ScriptLine line = keyFrame.Scripts[i];

                    if (line != null)
                    {
                        switch(line.Command)
                        {
                                //Sets animation
                            case Commands.SetAnim:
                                character.setAnim(line.SParam);
                                break;

                                //Goto frame N
                            case Commands.Goto:
                                character.animFrame = line.IParam;
                                done = true;
                                break;

                                //If up is pressed, Goto N
                            case Commands.IfUpGoto:
                                if (character.keyUp)
                                {
                                    character.animFrame = line.IParam;
                                    done = true;
                                }
                                break;

                                //if down is pressed Goto N
                            case Commands.IfDownGoto:
                                if (character.keyDown)
                                {
                                    character.animFrame = line.IParam;
                                    done = true;
                                }
                                break;

                                //Float the character in the air
                            case Commands.Float:
                                character.floating = true;
                                break;

                                //Remove the float status
                            case Commands.UnFloat:
                                character.floating = false;
                                break;

                                //Slide with facing direction force X
                            case Commands.Slide:
                                character.Slide(line.IParam);
                                break;

                                //Back the character up with force X
                            case Commands.Backup:
                                character.Slide(-line.IParam);
                                break;

                                //Make the character jump
                            case Commands.SetJump:
                                character.SetJump(line.IParam);
                                break;

                                //Emulate a joystick move
                            case Commands.JoyMove:
                                if (character.keyLeft)
                                    character.trajectory.X = -character.Speed;
                                else if (character.keyRight)
                                    character.trajectory.X = character.Speed; 
                                else if (character.keyUp)
                                    character.trajectory.Y = -character.Speed;
                                else if (character.keyDown)
                                    character.trajectory.Y = character.Speed;
                                
                                break;

                                //Clear keys
                            case Commands.ClearKeys:
                                character.pressedKeys = PressedKeys.None;
                                break;

                                //If uppercut key pressed, set animframe to N
                            case Commands.SetUpperGoto:
                                character.GotoGoal[(int)PressedKeys.Upper] =
                                    line.IParam;
                                break;

                            //If lower attack set, goto animFrame N
                            case Commands.SetLowerGoto:
                                character.GotoGoal[(int)PressedKeys.Lower] =
                                     line.IParam;
                                break;

                                //If attack button pressed goto animFrame N
                            case Commands.SetAtkGoto:
                                character.GotoGoal[(int)PressedKeys.Attack] =
                                     line.IParam;
                                break;

                                //If any button is pressed goto animFrame N
                            case Commands.SetAnyGoto:
                                character.GotoGoal[(int)PressedKeys.Upper] =
                                     line.IParam;
                                character.GotoGoal[(int)PressedKeys.Lower] =
                                     line.IParam;
                                character.GotoGoal[(int)PressedKeys.Attack] =
                                     line.IParam;

                                break;

                                //If secondary attack is set, goto animFrame N
                            case Commands.SetSecondaryGoto:
                                character.GotoGoal[(int)PressedKeys.Secondary] =
                                     line.IParam;
                                character.GotoGoal[(int)PressedKeys.SecUp] =
                                     line.IParam;
                                character.GotoGoal[(int)PressedKeys.SecDown] =
                                     line.IParam;

                                break;

                                //If secondary button and up are pressed goto animFrame N
                            case Commands.SetSecUpGoto:
                                character.GotoGoal[(int)PressedKeys.SecUp] =
                                     line.IParam;

                                break;

                                //If secondary button and down are pressed goto animFrame N
                            case Commands.SetSecDownGoto:
                                character.GotoGoal[(int)PressedKeys.SecDown] =
                                     line.IParam;

                                break;

                                //Set gravity of character to N
                            case Commands.SetGravity:
                                character.gravity = line.IParam;
                                break;

                                //Set friction of character to N
                            case Commands.SetFriction:
                                character.friction = line.IParam;
                                break;

                                //Set gravity to world gravity
                            case Commands.DefaultGravity:
                                character.gravity = Game1.gravity;
                                break;

                                //Set friction to world friction
                            case Commands.DefualtFriction:
                                character.friction = Game1.friction;
                                break;

                            case Commands.Ethereal:
                                character.Ethereal = true;
                                break;

                            case Commands.Solid:
                                character.Ethereal = false;
                                break;

                            case Commands.Speed:
                                character.Speed = (float)line.IParam;
                                break;

                            case Commands.HP:
                                character.HP = character.MHP = (float)line.IParam;
                                break;

                            case Commands.DeathCheck:
                                if (character.HP < 0)
                                {
                                    character.killMe();
                                }
                                break;

                            case Commands.IfDyingGoto:
                                if (character.HP < 0)
                                {
                                    character.SetFrame(line.IParam);
                                    done = true;
                                }
                                break;

                            case Commands.KillMe:
                                character.killMe();
                                break;



                                
                        }

                    }
                }
            }


        }
    }
}
