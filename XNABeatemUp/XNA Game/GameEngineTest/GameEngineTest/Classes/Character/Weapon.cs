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
    public class Weapon
    {
        public float ATK;
        public Texture2D weaponTex;
        public Vector2 position;
        public float angle;
    }

    class Projectile : Weapon
    {
        public Vector2 trajectory;
        public Vector2 shadow;
        public Rectangle hitbox;
    }

    class ProjectileWeapon : Weapon
    {
        public Projectile[] projectiles;
    }

    class MeleeWeapon : Weapon
    {
        public Rectangle hitbox;
    }
}
