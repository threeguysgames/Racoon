using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace GameEngineTest
{
    public class HitHP : Particle
    {
        private long val;
        private NumberDraw numDraw;

        public HitHP(long value, Vector3 loc)
        {
            val = value;
            location = loc;
            Exists = true;
            frame = 0.3f;
            trajectory = new Vector3(0, -70, 0);
            numDraw = new NumberDraw();
            a = 8f;
        }

        public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c)
        {
            base.Update(gameTime, map, pMan, c);
        }

        public override void Draw(SpriteBatch sprite, Texture2D spritesTex)
        {
            float frameAlpha;

            if (frame > 0.9f)
                frameAlpha = (1.0f - frame) * 10.0f;
            else
                frameAlpha = (frame / 0.9f);

            Vector2 location_ = new Vector2(GameLocation.X, GameLocation.Y + location.Z);

            numDraw.Draw(sprite, spritesTex, val, location_, new Color(
                new Vector4(1, 1, 1, a * frameAlpha)));
        }

    }
}
