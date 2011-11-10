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
    public class Fire : Particle
    {
        public Fire(Vector3 loc,
            Vector3 traj,
            float size,
            int icon)
        {
            this.location = loc;
            this.trajectory = traj;
            this.size = size;
            flag = icon;
            Exists = true;
            frame = 0.5f;
            Additive = true;
        }

        public override void Draw(SpriteBatch sprite, Texture2D spritesTex)
        {
            if (frame > 0.5f) return;

            Rectangle sRect = new Rectangle(flag * 64, 64, 64, 64);
            float bright = frame * 5.0f;
            float tsize;

            if (frame > 0.4)
            {
                r = 1.0f;
                g = 1.0f;
                b = (frame - 0.4f) * 10.0f;
                if (frame > 0.45f)
                    tsize = (0.5f - frame) * size * 20.0f;
                else
                    tsize = size;
            }

            else if (frame > 0.3f)
            {
                r = 1.0f;
                g = (frame - 0.3f) * 10.0f;
                b = 0.0f;
                tsize = size;
            }
            else
            {
                r = frame * 3.3f;
                g = 0.0f;
                b = 0.0f;
                tsize = (frame / 0.3f) * size;
            }

            if (flag % 2 == 0)
            {
                rotation = (frame * 7.0f + size * 20.0f);
            }
            else
                rotation = (-frame * 11.0f + size * 20.0f);

            Vector2 location_ = new Vector2(GameLocation.X, GameLocation.Y);

            sprite.Draw(spritesTex, location_, sRect, new Color(new Vector4(r, g, b, 1.0f)),
                rotation, new Vector2(32.0f, 32.0f), tsize, SpriteEffects.None, 1.0f);

        }

    }
}
