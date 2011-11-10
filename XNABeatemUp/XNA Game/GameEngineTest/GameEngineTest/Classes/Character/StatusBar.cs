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
    public class StatusBar
    {
        String Type;
        public Color Color;
        Vector2 Location;

        public StatusBar(String type, Vector2 location)
        {
            Type = type;

            switch (Type)
            {
                case "HP":
                    Color = Color.Red;
                    break;
                default:
                    Color = Color.Green;
                    break;
            }

            Location = location;
        }
    }
}
