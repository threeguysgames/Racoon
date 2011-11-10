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
    class Smoke : Particle
    {
        public Smoke(Vector3 location, Vector3 trajectory,
            float r, float g, float b, float a, float size, int icon)
        {
            this.location = location;
            this.trajectory = trajectory;
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = b;
            this.size = size;
            this.flag = icon;
            this.owner = -1;
            this.Exists = true;
            this.frame = 1.0f;
        }

        public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c)
        {
            if (frame < 0.5f)
            {
                if (trajectory.Y < -10.0f) trajectory.Y += gameTime * 500.0f;
                if (trajectory.X < -10.0f) trajectory.X += gameTime * 150.0f;
                if (trajectory.X > 10.0f) trajectory.X -= gameTime * 150.0f;
            }

            base.Update(gameTime, map, pMan, c);
        }

        public override void Draw(SpriteBatch sprite, Texture2D spritesTex)
        {
            Rectangle sRect = new Rectangle(flag * 64, 0, 64, 64);

            float frameAlpha;

            if (frame > 0.9f)
                frameAlpha = (1.0f - frame) * 10.0f;
            else
                frameAlpha = (frame / 0.9f);

            Vector2 location_ = new Vector2(GameLocation.X, GameLocation.Y + location.Z);

            sprite.Draw(spritesTex, location_, sRect, new Color(
                new Vector4(frame * r, frame * g, frame * b, a * frameAlpha)),
                rotation, new Vector2(32.0f, 32.0f), size + (1.0f - frame),
                SpriteEffects.None, 1.0f);

            //base.Draw(sprite, spritesTex);
        }

    }
}
