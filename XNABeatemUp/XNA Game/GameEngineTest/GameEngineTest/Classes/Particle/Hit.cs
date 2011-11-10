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
    public class Hit :Particle
    {
        public Hit(Vector3 loc, Vector3 traj, int owner, int flag)
        {
            location = loc;
            trajectory = traj;
            this.owner = owner;
            this.flag = flag;

            Exists = true;
            frame = 0.5f;
        }

        public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c)
        {
            HitManager.CheckHit(this, c, pMan);

            KillMe();
            
        }

        public override void Draw(SpriteBatch sprite, Texture2D spritesTex)
        {
            //
        }
    }
}
