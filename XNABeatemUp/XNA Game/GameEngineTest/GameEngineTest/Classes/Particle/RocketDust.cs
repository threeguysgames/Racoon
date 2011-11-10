using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngineTest
{
    public class RocketDust : Particle
    {
        public RocketDust(Vector3 loc,
            Vector3 traj,
            float r, float g, float b, float a,
            float size,
            int icon)
        {
            this.location = loc;
            this.trajectory = traj;
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = b;
            this.size = size;
            this.flag = icon;
            this.Exists = true;
            this.frame = 0.5f;
            this.Additive = false;
            this.owner = -1;

        }

        public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c)
        {
            trajectory.Y = trajectory.Y + Game1.gravity * gameTime;

            base.Update(gameTime, map, pMan, c);
        }

        public override void Draw(SpriteBatch sprite, Texture2D spritesTex)
        {
            Rectangle sRect = new Rectangle(flag * 64, 0, 64, 64);
            float frameAlpha;

            if (frame > 0.9f)
                frameAlpha = (1.0f - frame) * 10.0f;
            else
                frameAlpha = (frame / 0.5f);

            Vector2 location_ = new Vector2(GameLocation.X, GameLocation.Y);
            
            sprite.Draw(spritesTex, location_, sRect, new Color(new Vector4(r,g,b,a*frameAlpha)),
                rotation, new Vector2(32.0f, 32.0f), size + (.5f - frame),
                SpriteEffects.None, 1.0f);
            
        }
    }
}
