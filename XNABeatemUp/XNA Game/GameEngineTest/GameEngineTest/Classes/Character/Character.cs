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
    public enum CharDir
    {
        left = 0,
        right = 1
    }
    public enum CharState
    {
        ground = 0,
        air = 1
    }
    public enum JumpState
    {
        up = 0,
        down = 1
    }

    public enum moveState
    {
        running = 0,
        walking = 1,
        sliding = 2
    }

    public enum TeamName
    {
        goodGuys = 0,
        badGuys = 1,
        NPC = 2,
        none = 3
    }

    public class Character : Entity
    {
 

        #region CONSTANTS

        #region TRIGGERS
        public const int TRIG_ROCKET_ACROSS = 0;
        public const int TRIG_ROCKET_DOWN = 1;
        public const int TRIG_ROCKET_UP = 2;
        
        public const int TRIG_MELEE_DOWN = 3;
        public const int TRIG_MELEE_UP = 4;
        public const int TRIG_MELEE_DIAG_UP = 5;
        public const int TRIG_MELEE_DIAG_DOWN = 6;
        public const int TRIG_MELEE_UPPERCUT = 7;
        public const int TRIG_MELEE_SMACKDOWN = 8;
        public const int TRIG_ROCKET_RAIN = 9;
        #endregion

        #endregion


        KeyboardState lastKeyState ; 
        KeyboardState keyState;

        public PressedKeys pressedKeys;



        #region status levels
        public float HP;
        public float MHP;
        public float MP;
        public String Name;
        public float Speed;
        #endregion

        #region character properties

        public CharDir facing;
        public int ledgeAttach = -1;
        public CharState state;
        public JumpState jumpState;
        public AI Ai = null;
        public int ID = 0;
        public TeamName TEAM = 0;
        public bool Ethereal = false;
        public float DyingFrame = -1f;
        
        #endregion

        #region key press booleans

            public bool keyLeft;
            public bool keyRight;
            public bool keyUp;
            public bool keyDown;
            public bool meleeDown;
            public bool secondDown;

            public bool noKeysDown;
        
        #endregion

        public bool jumpDown;

        public float friction;
        public float gravity;

        public bool walk = true;
        public int count = 0;

        public bool useSlowTime = false;

        public bool floating = false;
        public int[] GotoGoal = { -1, -1, -1, -1, -1, -1, -1, -1 };
        Script script;
        public ParticleManager PManager;

        public StatusBar healthBar;

        #region graphics animation

        public int      animFrame;
        public String   animName;
        public int      anim;

        public float rotationDiff = 0;
        public float sclaeDiff = 0;
        public float posDiff = 0;
        public float interpolation = 0;

        public CharDef charDef;

        float frame = 0f;

        public static Texture2D[] headTex = new Texture2D[3];
        public static Texture2D[] torsoTex = new Texture2D[3];
        public static Texture2D[] legsTex = new Texture2D[3];
        public static Texture2D[] weaponTex = new Texture2D[1];
        public static Texture2D shadowTex;
        public static Texture2D statusBarTex;

        #region Shadow properties        
        public Vector2 shadowPosition;
        public Vector2 prevShadowPosition;
        #endregion

        #endregion


        public Character(CharDef newCharDef, Vector3 _pos, int _ID, TeamName team)
        {
            script = new Script(this);

            charDef = newCharDef;
            HP = 100;
            MHP = HP;
            MP = 300;
            
            position = _pos;
            hitBox = new Rectangle(0, 0, 120, 40);
            centerOfObject = new Vector2((position.X + hitBox.Width) / 2,
                (position.Y + hitBox.Height) / 2);

            trajectory = new Vector3();

            facing = CharDir.right;
            scale = 1.0f;

            state = CharState.ground;
            jumpState = JumpState.up;

            if (!(team == TeamName.goodGuys))
                Speed = 100f;
            else
                Speed = 150f;

            friction = 1300f;
            gravity = 900f;
            jumpY = 0f;

            Ai = null;
            ID = _ID;

            TEAM = team;

            InitScript();
            Ethereal = false;

            PManager = Game1.pManager;
            healthBar = new StatusBar("HP", new Vector2(shadowPosition.X, shadowPosition.Y + 40));

            Name = "";
            animName = "";
            setAnim("idle");

        }

        public void setMeleeAI()
        {
            Ai = new MeleeAI();
        }
        public void setRocketAI()
        {
            Ai = new RocketAI();
        }
        private void InitScript()
        {
            setAnim("init");
            if (animName == "init")
            {
                for (int i = 0; i < charDef.Animations[anim].keyFrames.Length; i++)
                {
                    if (charDef.Animations[anim].keyFrames[i].frameRef > -1)
                        script.DoScript(anim, i);
                }
            }
        }

        public void processInput()
        {
            lastKeyState = keyState;
            keyState = Keyboard.GetState();
            GamePadState gpState = GamePad.GetState(PlayerIndex.One);

            keyLeft = false;
            keyRight = false;
            keyUp = false;
            keyDown = false;
            jumpDown = false;
            meleeDown = false;
            secondDown = false;
            noKeysDown = true;

            if(keyState.IsKeyDown(Keys.Left))
            { keyLeft = true; facing = CharDir.left; noKeysDown = false; }
            if(keyState.IsKeyDown(Keys.Right))
            { keyRight = true; facing = CharDir.right; noKeysDown = false;}

            if (keyState.IsKeyDown(Keys.Up))
            { keyUp = true; noKeysDown = false; }
            if (keyState.IsKeyDown(Keys.Down))
            { keyDown = true; noKeysDown = false; }

            if(gpState.DPad.Up == ButtonState.Pressed)
            { keyUp = true; noKeysDown = false; }

            if (lastKeyState != null)
            {
                if (lastKeyState.IsKeyUp(Keys.Space) && keyState.IsKeyDown(Keys.Space))
                {
                    jumpDown = true;
                    noKeysDown = false;
                }
                if (lastKeyState.IsKeyUp(Keys.LeftShift) && keyState.IsKeyDown(Keys.LeftShift))
                { meleeDown = true; noKeysDown = false; }

                if (lastKeyState.IsKeyUp(Keys.F) && keyState.IsKeyDown(Keys.F))
                { secondDown = true; noKeysDown = false; }
            }

            if (keyState.IsKeyDown(Keys.Escape) &&
                lastKeyState.IsKeyUp(Keys.Escape))
            {
                Game1.Menu.Pause();
            }

        }

        public void update(Map map, ParticleManager pMan, Character[] c)
        {

            if (Ai != null)
                Ai.Update(c, ID, map);

            pressedKeys = PressedKeys.None;

            if (meleeDown)
            {
                pressedKeys = PressedKeys.Attack;

                if (keyUp) pressedKeys = PressedKeys.Lower;
                if (keyDown) pressedKeys = PressedKeys.Upper;
            }

            if (secondDown)
            {
                pressedKeys = PressedKeys.Secondary;

            }
            if (pressedKeys > PressedKeys.None)
            {
                if (GotoGoal[(int)pressedKeys] > -1)
                {
                    SetFrame(GotoGoal[(int)pressedKeys]);

                    if (keyLeft)
                        facing = CharDir.left;
                    if (keyRight)
                        facing = CharDir.right;


                    pressedKeys = PressedKeys.None;

                    for (int i = 0; i < GotoGoal.Length; i++)
                        GotoGoal[i] = -1;

                    frame = 0f;
                }
            }

            //float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float dt = 0;

            if (!useSlowTime)
                dt = Game1.realTime;
            else
            {
                dt = Game1.frameTime;
                if (Game1.realTime == Game1.frameTime)
                    useSlowTime = false;
            }

            prevPos = new Vector3(position.X, position.Y, position.Z);
            prevShadowPosition = new Vector2(shadowPosition.X, shadowPosition.Y);


            if (DyingFrame > -1f)
                DyingFrame += Game1.frameTime;

            if (DyingFrame < 0f)
            {

                #region Update Animation

                Animation animation = charDef.Animations[anim];
                KeyFrame keyFrame = animation.keyFrames[animFrame];
                KeyFrame nextFrame;

                if(animFrame + 1 >= animation.keyFrames.Length)
                    nextFrame = animation.keyFrames[0];
                else
                    nextFrame = animation.keyFrames[animFrame + 1];

                frame += dt * 30.0f; //30fps

                

                //How much until the next frame
                interpolation = 1 - (keyFrame.duration - frame) / keyFrame.duration; ;
                if (interpolation > 1)
                    interpolation = 1;
                if (interpolation < 0)
                    interpolation = 0;

                //Console.WriteLine("interpoloation " + interpolation);

                //Then we should move to the next frame
                if (frame > (float)keyFrame.duration)
                {
                    int pFrame = animFrame;
                    script.DoScript(anim, animFrame);
                    CheckTrig(pMan);

                    //Subtract the frame duration to reset the frame
                    frame -= (float)keyFrame.duration;

                    if (animFrame == pFrame)
                        animFrame++;

                    keyFrame = animation.keyFrames[animFrame];

                    if (animFrame >= animation.keyFrames.Length)
                        animFrame = 0;

                }

                if (keyFrame.frameRef < 0)
                    animFrame = 0;
                #endregion

                #region update on ground physics
                if (state == CharState.ground)
                {
                    if (trajectory.X > 0f)
                    {
                        trajectory.X -= friction * dt;
                        if (trajectory.X < 0f) trajectory.X = 0f;
                    }

                    if (trajectory.X < 0f)
                    {
                        trajectory.X += friction * dt;
                        if (trajectory.X > 0f) trajectory.X = 0f;
                    }

                    if (trajectory.Y > 0f)
                    {
                        trajectory.Y -= friction * dt;
                        if (trajectory.Y < 0f) trajectory.Y = 0f;
                    }

                    if (trajectory.Y < 0f)
                    {
                        trajectory.Y += friction * dt;
                        if (trajectory.Y > 0f) trajectory.Y = 0f;
                    }

                }

                if (state == CharState.air)
                {
                    trajectory.Z += gravity * dt;
                    if (position.Z > 0f) { position.Z = 0f; trajectory.Z = 0f; state = CharState.ground; Land(); }
                    else { position.Z += trajectory.Z * dt; }
                }

                position.X += trajectory.X * dt;
                position.Y += trajectory.Y * dt;


                // Console.WriteLine(trajectory.Z + " " + position.Z);

                if (state == CharState.air)
                    shadowPosition = new Vector2(position.X, position.Y);

                if (state == CharState.ground)
                    shadowPosition = new Vector2(position.X, position.Y);

                #endregion

                #region char input

                if (animName == "idle" || animName == "run" || state == CharState.ground)
                {
                    if (animName == "idle" || animName == "run")
                    {
                        if (keyLeft || keyRight || keyUp || keyDown)
                        {
                            setAnim("run");
                        }
                        else
                            setAnim("idle");

                        if (keyLeft)
                        {

                            trajectory.X = -Speed;
                        }

                        if (keyRight)
                        {
                            trajectory.X = Speed;
                        }

                        if (keyUp)
                        {
                            trajectory.Y = -Speed;
                        }
                        if (keyDown)
                        {
                            trajectory.Y = Speed;
                        }

                    }

                    if (jumpDown)
                    {
                        setAnim("jump");
                        trajectory.Z = -300;
                        state = CharState.air;
                        shadowPosition.Y = position.Y;
                        jumpState = JumpState.up;
                    }

                    //Check melee collision
                    if (meleeDown)
                    {
                        setAnim("attack");
                    }

                    if (secondDown)
                    {
                        setAnim("airatk");
                    }

                }

                if (animName == "jump")
                {
                    if (secondDown)
                    {
                        setAnim("melee");
                    }
                }


                #endregion

            }

        }

        public void draw(SpriteBatch spriteBatch)
        {
            SpriteEffects effects;
            Rectangle sRect = new Rectangle();

            int frameIdx = charDef.GetAnimation(anim).GetKeyFrame(animFrame).frameRef;
            int nexFrameId = 0;

            if (charDef.GetAnimation(anim).GetKeyFrame(animFrame + 1).frameRef == -1 )
                nexFrameId = charDef.GetAnimation(anim).GetKeyFrame(0).frameRef;
            else
                nexFrameId = charDef.GetAnimation(anim).GetKeyFrame(animFrame  + 1).frameRef;
            
            Frame frame = charDef.GetFrame(frameIdx);
            Frame nextFrame = charDef.GetFrame(nexFrameId);

            //Flip the character based on facing
            if(facing == CharDir.left)
                effects = SpriteEffects.FlipHorizontally;
            else
                effects = SpriteEffects.None;

            Vector2 shadowLocation = shadowPosition - Game1.scroll;

            sRect = new Rectangle(0,0,120,120);

            Color color = new Color(new Vector4(1.0f, 1.0f, 1.0f, 1f));

            if (DyingFrame > 0f)
                color = new Color(new Vector4(1f - DyingFrame, 1f - DyingFrame,
                    1f - DyingFrame, 1f - DyingFrame));

            //Draw the shadow
            spriteBatch.Draw(shadowTex, shadowLocation,
                sRect, color, 0f,
                new Vector2((float)sRect.Width / 2.0f, (float)sRect.Height - 20), .9f, SpriteEffects.None, 0f);


            for (int i = 0; i < frame.Parts.Length; i++)
            {
                Part part = frame.Parts[i];
                if (part.index > -1 && part.index < 1000)
                {
                    /*grab the source rectangle from sprite sheet
                     * 0-63 is head
                     * 64-127 is torso
                     * 128-191 is legs
                     * 192 - 256 is weapons
                     */

                    sRect.X = ((part.index % 64) % 5) * 64;
                    sRect.Y = ((part.index % 64) / 5) * 64;
                    sRect.Width = 64;
                    sRect.Height = 64;

                    /*weapon textures are 3 columns wide*/
                    if (part.index >= 192)
                    {
                        sRect.X = ((part.index % 64) % 3) * 80;
                        sRect.Width = 80;
                    }

                    /*Try to write interpolation code here for location, scaling, rotation*/

                    float rotDiff = nextFrame.Parts[i].rotation - part.rotation;
                    float rotation = part.rotation;

                    Vector2 locDiff = nextFrame.Parts[i].location - part.location;
                    Vector2 partLoc = part.location;

                    Vector2 scaleDiff = nextFrame.Parts[i].scaling - part.scaling;
                    Vector2 scale2 = part.scaling;

                    

                    //Console.WriteLine(rotDiff + "r" + frame.Parts[i].rotation + "nr" + nextFrame.Parts[i].rotation + "angle " + rotation);

                    Vector2 charPos = new Vector2(position.X, position.Y + position.Z);

                    Vector2 location = partLoc * scale + charPos - Game1.scroll;
                    Vector2 scaling = scale2 * scale;

                    /*
                    Vector2 location = part.location * scale + charPos - Game1.scroll;
                    Vector2 scaling = part.scaling * scale;

                    if (part.index >= 128) scaling *= 1.35f;

                    if (facing == CharDir.left)
                    {
                        rotation = -rotation;
                        location.X -= part.location.X * scale * 2.0f;
                    }
                     */

                    if (part.index >= 128) scaling *= 1.35f;

                    if (facing == CharDir.left)
                    {
                        rotation = -rotation;
                        location.X -= partLoc.X * scale * 2.0f;
                    }

                    Texture2D texture = null;

                    int t = part.index / 64;
                    switch (t)
                    {
                        case 0:
                            texture = headTex[charDef.headIndex];
                            break;
                        case 1:
                            texture = torsoTex[charDef.torsoIndex];
                            break;

                        case 2:
                            texture = legsTex[charDef.legsIndex];
                            break;

                        case 3:
                            texture = weaponTex[charDef.weaponIndex];
                            break;
                    }

                    if (texture != null)
                    {
                        spriteBatch.Draw(texture, location, sRect, color, rotation, 
                            new Vector2((float)sRect.Width / 2.0f, 32.0f), scaling, effects, (float)(shadowLocation.Y + i)/(Game1.scroll.Y + 800 + frame.Parts.Length));
                    }
                }
            }

            if (TEAM == TeamName.goodGuys)
            {
                spriteBatch.Draw(statusBarTex, new Rectangle((int)shadowLocation.X - 64, (int)shadowLocation.Y - 119, (int)(128f * (HP / MHP)), (int)16),
                     new Rectangle(0, 16, 128, 16),
                healthBar.Color, 0f, new Vector2(0, 16), SpriteEffects.None, 0.02f);

                spriteBatch.Draw(statusBarTex, new Rectangle((int)shadowLocation.X - 64, (int)shadowLocation.Y - 120, 128, (int)16),
                     new Rectangle(0, 0, 128, 16),
                    Color.White, 0f, new Vector2(0, 16), SpriteEffects.None, 0.01f);
            }

           


            
        }

        public void setAnim(string newAnim)
        {
            //If the animation is already the current animation do nothing
            if (animName != newAnim)
            {
                for (int i = 0; i < charDef.Animations.Length; i++)
                {
                    if (charDef.Animations[i].name == newAnim)
                    {
                        for (int t = 0; t < GotoGoal.Length; t++)
                            GotoGoal[t] = -1;

                        anim = i;
                        animFrame = 0;
                        frame = 0;
                        animName = newAnim;

                        break;
                    }
                }
            }
        }

        internal static void loadTextures(ContentManager content)
        {
            for (int i = 0; i < headTex.Length; i++)
                headTex[i] = content.Load<Texture2D>(@"gfx/head" +
                    (i + 1).ToString());

            for (int i = 0; i < torsoTex.Length; i++)
                torsoTex[i] = content.Load<Texture2D>(@"gfx/torso" +
                    (i + 1).ToString());

            for (int i = 0; i < legsTex.Length; i++)
                legsTex[i] = content.Load<Texture2D>(@"gfx/legs" +
                    (i + 1).ToString());

            for (int i = 0; i < weaponTex.Length; i++)
                weaponTex[i] = content.Load<Texture2D>(@"gfx/weapon" +
                    (i + 1).ToString());

            shadowTex = content.Load<Texture2D>(@"gfx/shadow");

            statusBarTex = content.Load <Texture2D>(@"gfx/hud/statusbar");
        }

        public void Slide(float x)
        {
            trajectory.X = (float)facing * 2f * x - x;
        }

        public void SetJump(float _jump)
        {
            this.trajectory.Z = -_jump;
            shadowPosition.Y = position.Y;
            state = CharState.air;
            jumpState = JumpState.up;
        }

        public void Land()
        {
            state = CharState.ground;
            switch (animName)
            {
                case "jhit":
                    setAnim("land");
                    break;
                default:
                    setAnim("idle");
                    break;

            }

        }

        private void CheckTrig(ParticleManager pMan)
        {
            int frameIndex = charDef.Animations[anim].keyFrames[animFrame].frameRef;

            Frame frame = charDef.Frames[frameIndex];

            for (int i = 0; i < frame.Parts.Length; i++)
            {
                Part part = frame.Parts[i];

                //If the part is a trigger
                if (part.index >= 1000)
                {
                    
                    Vector3 charPos = new Vector3(position.X, position.Y,0.0f);

                    Vector3 loc = new Vector3(part.location.X * scale + charPos.X,
                        charPos.Y,part.location.Y*scale);

                    if (facing == CharDir.left)
                    {
                        loc.X -= part.location.X * scale * 2.0f;
                    }

                    FireTrig(part.index - 1000, loc, pMan);
                }
            }
        }

        public bool InHitBounds(Vector3 hitLoc)
        {
           // Console.WriteLine(hitLoc.X + " " + hitLoc.Y + " " + hitLoc.Z);

            if (hitLoc.X > (position.X - hitBox.Width / 2) &&
                hitLoc.X < (position.X + hitBox.Width / 2) &&
                hitLoc.Y > (position.Y - hitBox.Height / 2) &&
                hitLoc.Y < (position.Y + hitBox.Height / 2) &&
                hitLoc.Z > (position.Z - hitBox.Width) &&
                hitLoc.Z < (position.Z))
                   
                return true;

            return false;
        }

        private void FireTrig(int trig, Vector3 loc, ParticleManager pMan)
        {
            Vector3 loc_ = new Vector3(loc.X, loc.Y, loc.Z);

            switch (trig)
            {
                case TRIG_ROCKET_ACROSS:
                    pMan.MakeRocket(loc_, new Vector3(1300f, 0f,0f), facing, ID);
                    QuakeManager.SetQuake(.5f);
                    QuakeManager.SetBlast(.5f, new Vector2(loc_.X, loc_.Y));
                    break;
                case TRIG_ROCKET_UP:
                    pMan.MakeRocket(loc_, new Vector3(700f, -700f,0f), facing, ID);
                    break;
                case TRIG_ROCKET_DOWN:
                    if (state == CharState.air)
                    {
                        loc_.Y += position.Z;
                        pMan.MakeRocket(loc_, new Vector3(700f, 700f,400f), facing, ID);
                    }
                    else
                    {
                        pMan.MakeRocket(loc_, new Vector3(700f, 700f, 400f), facing, ID);
                    }
                    break;

                case TRIG_ROCKET_RAIN:
                    for (int i = 0; i < 10; i++)
                    {
                        
                        pMan.MakeRocket(loc_ + new Vector3(0.0f, 0.0f, -1000.0f) +
                        Rand.GetRandomVector3(
                            -200f, 200f, 0f, 0f), new Vector3(0f, 0f, 700.0f), facing, ID);
                    }
                    break;

                default:

                    if (facing == CharDir.left)
                        pMan.AddParticle(new Hit(loc_, new Vector3(-100f, 0f, 0f), ID, trig));
                    else
                        pMan.AddParticle(new Hit(loc_, new Vector3(100f, 0f, 0f), ID, trig));
                break;
            }
        }
        public void SetFrame(int newFrame)
        {
            animFrame = newFrame;
            frame = 0f;
            for (int i = 0; i < GotoGoal.Length; i++)
                GotoGoal[i] = -1;
            
        }

        public void killMe()
        {
            if (DyingFrame < 0f)
                DyingFrame = 0f;
        }
    }
}
