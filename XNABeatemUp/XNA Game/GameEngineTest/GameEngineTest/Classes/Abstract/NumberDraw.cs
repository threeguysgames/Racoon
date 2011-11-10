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
    public class NumberDraw
    {
        public NumberDraw()
        {
        }

        public void Draw(SpriteBatch sB, Texture2D tex, long number, Vector2 loc, Color color)
        {
            
            int place = 0;

            long n = number;

            while (true)
            {
                long digit = number % 10;
                number = number / 10;

                sB.Draw(tex, loc + new Vector2((float)place * -17f, 0f),
                    new Rectangle((int)digit * 16, 224, 16, 32),
                    color);

                place++;
                if (number <= 0)
                {
                    return;
                }
            }

            
        }
    }
}
