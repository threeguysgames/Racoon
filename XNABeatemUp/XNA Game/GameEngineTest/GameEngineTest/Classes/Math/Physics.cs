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
            mass = 1.2f;
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

            mass = 1.2f;
        }

        public void update(float dt)
        {
            dt *= timeCompression;

            //Linear transformations
            velocity = acceleration * dt + velocity / mass;
            position = velocity * dt + position;

            if(velocity.Length() <= 0.01f)
                velocity = Vector2.Zero;

            //Angular transition
            rotVelocity = rotAcceleration * dt + rotVelocity;
            angle = rotVelocity * dt + angle;

            acceleration = Vector2.Zero;
            rotAcceleration = 0;
            
        }
    
    }
}
