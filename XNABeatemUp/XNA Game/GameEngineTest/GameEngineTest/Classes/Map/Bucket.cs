using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;

namespace GameEngineTest
{
    public class Bucket
    {
        BucketItem[] bucketItem = new BucketItem[64];
        public int Size;
        float updateFrame = 0f;
        public bool isEmpty = false;

        public Bucket(int size)
        {
            for (int i = 0; i < bucketItem.Length; i++)
            {
                bucketItem[i] = null;
            }

            Size = size;
        }

        public void AddItem(Vector3 loc, int charDef)
        {
            for (int i = 0; i < bucketItem.Length; i++)
            {
                if (bucketItem[i] == null)
                {
                    bucketItem[i] = new BucketItem(loc, charDef);
                    return;
                }
            }
        }

        public void Update(Character[] c)
        {
            updateFrame -= Game1.frameTime;

            if(updateFrame > 0f)
                return;

            int monsters = 0;

            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] != null)
                    if (c[i].TEAM == TeamName.badGuys)
                        monsters++;
            }

            if (monsters < Size)
            {
                for (int i = 0; i < bucketItem.Length; i++)
                {
                    if (bucketItem[i] != null)
                    {
                        for (int n = 0; n < c.Length; n++)
                        {
                            if (c[n] == null)
                            {
                                c[n] = new Character(Game1.charDef[bucketItem[i].CharDef],
                                bucketItem[i].location,
                                n,
                                TeamName.badGuys);
                                
                                bucketItem[i] = null;
                                return;
                            }

                        }
                    }
                }

                if (monsters == 0)
                    isEmpty = true;

            }

            
        }
    }

    class BucketItem
    {
        public Vector3 location;
        public int CharDef;

        public BucketItem(Vector3 _loc, int _charDef)
        {
            location = _loc;
            CharDef = _charDef;
        }
    }
}
