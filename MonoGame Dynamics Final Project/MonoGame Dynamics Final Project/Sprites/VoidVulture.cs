﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace MonoGame_Dynamics_Final_Project.Sprites
{
    public enum VultureState
    {
        Idle,
        Expel,
        Impale
    }
    class VoidVulture : Enemy
    { 
        public VultureState vultureState;
        protected Vector2 distanceBetweenVoidVulture;

        public VoidVulture(ContentManager content, Rectangle virtualSize, int spotinFormation, string formationType)
            :base(content, 300, 300, content.Load<Texture2D>("Images/Animations/voidVulture"), virtualSize, spotinFormation, formationType, 0.5f)             
        {
            frameNum = 15;
            frameTime = 0.08f;
            vultureState = VultureState.Idle;
            collisionRange = new BoundingSphere(new Vector3(position.X + spriteOrigin.X, position.Y + spriteOrigin.Y, 0), 200f);
            velocity = new Vector2(0, 25);
            enemyType = "voidVulture";
            damage = 20f;
            score = 1000f;
            health = 2000;
            mass = 10f;
        }
       
        public override void Update(ContentManager content, GameTime gameTime, Player player)
        {
            collisionRange = new BoundingSphere(new Vector3(position.X + spriteOrigin.X, position.Y + spriteOrigin.Y, 0), 200f);
            setVulture(player);
            switch (vultureState)
            {
                case VultureState.Expel:
                    TextureImage = content.Load<Texture2D>("Images/Animations/void-vulture-expel");
                    ChasePlayer(gameTime, player);
                    frameHeight = 300;
                    frameWidth = 300;
                    break;
                case VultureState.Idle:
                    TextureImage = content.Load<Texture2D>("Images/Animations/voidVulture");
                    base.Update(gameTime, player);
                    frameHeight = 300;
                    frameWidth = 300;
                    break;
                /*case VultureState.Impale:
                    frameHeight = 600;
                    frameWidth = 600;
                    TextureImage = Content.Load<Texture2D>("Images/Animations/voidVultureImpale");
                    ChasePlayer(gameTime, player.Position);
                    break;*/
            }
        }
        public void setVulture(Player player) 
        {
            distanceBetweenVoidVulture = player.Position - position;
            float distanceLength = distanceBetweenVoidVulture.Length();
            if (collisionRange.Intersects(player.collisionRange))
            {            
                vultureState = VultureState.Expel;      
            }         
        }
    }
}
