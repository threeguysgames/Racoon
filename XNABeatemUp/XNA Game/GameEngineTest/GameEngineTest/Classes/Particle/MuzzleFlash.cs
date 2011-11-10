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
    class MuzzleFlash : Particle
    {
        public MuzzleFlash(Vector3 _loc, Vector3 _traj, float _size)
        {
            location = _loc;
            trajectory = _traj;
            this.size = _size;
            rotation = Rand.GetRandomFloat(0f, 6.28f);
            Exists = true;
            frame = 0.05f;
            Additive = true;
        }

        public override void Draw(SpriteBatch sprite, Texture2D spritesTex)
        {
            Vector2 location_ = new Vector2(GameLocation.X, GameLocation.Y + location.Z);

            sprite.Draw(spritesTex, location_,
                new Rectangle(64, 138, 64, 64),
                new Color(new Vector4(1f, 0.8f, 0.6f, frame * 8f)),
                rotation, new Vector2(32.0f, 32.0f),
                size - frame,
                SpriteEffects.None, 1.0f);
        }
    }
}
