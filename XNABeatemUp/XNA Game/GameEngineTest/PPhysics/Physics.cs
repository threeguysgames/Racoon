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

namespace PPhysics
{
    public class Physics
    {
        public Vector2 worldAcceleration;

        public Vector2 position;
        public Vector2 velocity;
        public Vector2 acceleration;

        public float angle;
        public float rotVelocity;
        public float rotAcceleration;

        public float maxVelocity;
        public float maxRotVelocity;

        public float timeCompression;

        public float mass;

        public Physics ()
        {
            position = Vector2.Zero;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
            angle = 0.0f;
            rotVelocity = 0.0f;
            rotAcceleration = 0.0f;
            timeCompression = 1.0f;

            mass = 10;
        }
        public Physics(Vector2 _position)
        {
            position = _position;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
            
            angle = 0.0f;
            rotVelocity = 0.0f;
            rotAcceleration = 0.0f;
            
            timeCompression = 1.0f;
            mass = 10;
        }

        public void update(float dt)
        {
            dt *= timeCompression;

            acceleration = acceleration / mass;
            //Linear transformations
            velocity = acceleration * dt + velocity;
            position = velocity * dt + position;

            Console.WriteLine(acceleration.X + " " + velocity.X + " " + position.X);

            //Angular transition
            rotVelocity = rotAcceleration * dt + rotVelocity;
            angle = rotVelocity * dt + angle;

            acceleration = Vector2.Zero;
            rotAcceleration = 0;
            
        }
    
    }
}
