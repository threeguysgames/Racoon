using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace TextLib
{
    public class Text
    {
        private float size = 1f;
        private Color color;
        SpriteFont font;
        SpriteBatch sprite;

        public Text(SpriteBatch _sprite, SpriteFont _font)
        {
            font = _font;
            sprite = _sprite;
        }
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }
        public float Size
        {
            get { return size; }
            set { size = value; }
        }
        public void DrawText(int x, int y, String s)
        {
            sprite.Begin(SpriteBlendMode.AlphaBlend);
            sprite.DrawString(font, s, new Vector2((float)x, (float)y), color, 0f, new Vector2(), size, SpriteEffects.None, 1f);
            sprite.End();
        }

        public bool DrawClickText(int x, int y, String s, int mosX, int mosY, bool mouseClick)
        {
            color = Color.White;

            bool r = false;

            if (mosX > x && mosY > y && mosX < x + font.MeasureString(s).X * size
                && mosY < y + font.MeasureString(s).Y * size)
            {
                color = Color.Yellow;
                if (mouseClick)
                {
                    r = true;
                }

            }

            DrawText(x, y, s);
            return r;
        }
    }
}
