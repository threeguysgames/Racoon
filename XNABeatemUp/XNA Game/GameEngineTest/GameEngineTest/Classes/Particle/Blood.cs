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
    public class Blood : Particle
    {
        public Blood(Vector3 loc,
            Vector3 traj,
            float r,
            float g,
            float b,
            float a,
            float size,
            int icon)
        {
            this.location = loc;
            this.trajectory = traj;
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
            this.size = size;
            this.flag = icon;
            this.owner = -1;
            this.Exists = true;
            this.rotation = GlobalFunctions.GetAngle(new Vector2(), new Vector2(traj.X, traj.Y));
            this.frame = Rand.GetRandomFloat(0.3f, 0.7f);

        }

        public override void Draw(SpriteBatch sprite, Texture2D spritesTex)
        {
            Rectangle sRect = new Rectangle(flag * 64, 0, 64, 64);

            float frameAlpha;
            if (frame > 0.9f) frameAlpha = (1.0f - frame) * 10.0f;
            else
                frameAlpha = (frame / 0.9f);

            
            sprite.Draw(spritesTex, new Vector2(GameLocation.X,GameLocation.Y + location.Z), sRect, new Color(
                new Vector4(r, g, b, a * frameAlpha)
                ),
                rotation, new Vector2(32.0f, 32.0f),
                new Vector2(size * 2f, size * 0.5f),
                SpriteEffects.None, 1.0f);

            base.Draw(sprite, spritesTex);
        }

    }
}
