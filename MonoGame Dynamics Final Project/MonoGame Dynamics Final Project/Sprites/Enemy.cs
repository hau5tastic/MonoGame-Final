﻿#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using MonoGame_Dynamics_Final_Project;
using MonoGame_Dynamics_Final_Project.Weapons;
#endregion

namespace MonoGame_Dynamics_Final_Project.Sprites
{
    class Enemy : Player
    {
        /// Enemies
        /// Enemy logic goes in here
        /// Multiple types of enemies spawn, that logic is placed here 
        /// to control enemy movements
        /// 
        #region variables
        protected float mass;
        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }
        protected float damage;
        public float Damage
        {
            get { return damage; }
            set { damage = value; }
        }
        public float VectorSpeed;
        protected float tileWidth;
        protected float tileHeight;
        protected float windowHeight;
        public string enemyType;
        public bool offScreen;
        protected float maxSpeed;
        public float score; 
        protected Vector2 distanceBetween;
        #endregion

        public Enemy(ContentManager content, int width, int height, Texture2D textureImage, Rectangle virtualSize, int spotinFormation, string formationType, float scale)
            : base(width, height, textureImage, new Vector2(0, -500), new Vector2(0, 100f), true, scale)
        {
            Mass = mass;
            AtkSpeed = 1f;
            tileWidth = virtualSize.Width / 8f;
            tileHeight = virtualSize.Height / 5f;
            windowHeight = virtualSize.Height;
            setEnemy(spotinFormation, formationType);
            VectorSpeed = 1.0f;
            rotation = 0.0f;
            offScreen = true;
            maxSpeed = 100.0f;
            score = 0;
            level = 1;
            damage = Damage;       
        }

        public void ChasePlayer(GameTime gameTime, Player player)
        {
            float timeLapse = (float)(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            source = animatedSprite(frameNum, frameTime, frameWidth, frameHeight, TextureImage, timeLapse);
            distanceBetween = ((player.Position + player.SpriteOrigin) - (position + spriteOrigin));
            if (distanceBetween.Length() > 40.0f)
            {
                distanceBetween.Normalize();
                rotation = (float)Math.Atan2(distanceBetween.Y, distanceBetween.X) - MathHelper.PiOver2;
                position += distanceBetween * VectorSpeed; // set speed here
            }            
        }

        public Vector2 getDirectionVector()
        {
            if (distanceBetween != null)
            {
                return distanceBetween;
            }
            return new Vector2(0, 100);
        }
       
        public virtual void UpdateWeapon(GameTime gameTime, Player player, Vector2 directionShot, ContentManager content)
        {
            directionShot = velocity;

            foreach (Weapon weapon in primary)
            {
                weapon.Update(gameTime);
            }
        }

        public void Update(GameTime gameTime, Rectangle virtualSize)
        {
            if (velocity.X > 100)
            {
                velocity.X = 100;
            }
            if (velocity.Y > 100)
            {
                velocity.Y = 100;
            }

            float timeLapse = (float)(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            position += Velocity * timeLapse;
            source = animatedSprite(frameNum, frameTime, frameWidth, frameHeight, TextureImage, timeLapse);

            if (Position.Y > 0.0f)
            {
                offScreen = false;
            } 
            if (!offScreen)
            {
                if (Position.X >= virtualSize.Width)
                {
                    velocity.X *= -1;
                }
                else if (Position.X <= 0)
                {
                    velocity.X *= -1;
                }

                if (Position.Y >= virtualSize.Height)
                {
                    velocity.Y *= -1;
                }
                else if (Position.Y <= 0)
                {
                    velocity.Y *= -1;
                } 
            }          
        }

        public virtual void Update(ContentManager content, GameTime gameTime, Player player){ }

        public virtual void Update(GameTime gameTime, Player player)
        {
            float timeLapse = (float)(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            position += Velocity * timeLapse;
            source = animatedSprite(frameNum, frameTime, frameWidth, frameHeight, TextureImage, timeLapse);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, Color color)
        {
            foreach(EnemyWeapon shot in primary)
            {
                shot.Draw(spriteBatch);
            }
            spriteBatch.Draw(TextureImage,
                            position,
                            source,
                            color,
                            rotation,
                            spriteOrigin,
                            Scale * 2,
                            Spriteeffect,
                            0.0f);
        }

        public virtual void shootPrimary(ContentManager content) { }

        #region hardcoded formations
        // This method sets the enemy position depending on it's spot in the formation 
        private void setEnemy(int spotinFormation, string formationType)
        {        
            switch(formationType)
            {
                case "delta": // formation size: 10
                    switch(spotinFormation)
                    {
                        case 1:
                            position = getGridPos(4, 5);
                            break;
                        case 2:
                            position = getGridPos(3, 4);
                            break;
                        case 3:
                            position = getGridPos(5, 4);
                            break;
                        case 4:
                            position = getGridPos(2, 3);
                            break;
                        case 5:
                            position = getGridPos(4, 3);
                            break;
                        case 6:
                            position = getGridPos(6, 3);
                            break;
                        case 7:
                            position = getGridPos(1, 2);
                            break;
                        case 8:
                            position = getGridPos(3, 2);
                            break;
                        case 9:
                            position = getGridPos(5, 2);
                            break;
                        case 10:
                            position = getGridPos(7, 2);
                            break;
                        default:
                            Console.WriteLine("Could not set position {0} in {1} formation", spotinFormation, formationType);
                            break;
                    }
                    break;

                case "v": // formation size: 7
                    switch(spotinFormation)
                    {
                        case 1:
                            position = getGridPos(4, 5);
                            break;
                        case 2:
                            position = getGridPos(3, 4);
                            break;
                        case 3:
                            position = getGridPos(5, 4);
                            break;
                        case 4:
                            position = getGridPos(2, 3);
                            break;
                        case 5:
                            position = getGridPos(6, 3);
                            break;
                        case 6:
                            position = getGridPos(1, 2);
                            break;
                        case 7:
                            position = getGridPos(7, 2);
                            break;
                        default:
                            Console.WriteLine("Could not set position {0} in {1} formation", spotinFormation, formationType);
                            break;
                    }
                    break;

                case "line": // formation size: 5
                    switch (spotinFormation)
                    {
                        case 1:
                            position = getGridPos(2, 5);
                            break;
                        case 2:
                            position = getGridPos(3, 5);
                            break;
                        case 3:
                            position = getGridPos(4, 5);
                            break;
                        case 4:
                            position = getGridPos(5, 5);
                            break;
                        case 5:
                            position = getGridPos(6, 5);
                            break;
                        default:
                            Console.WriteLine("Could not set position {0} in {1} formation", spotinFormation, formationType);
                            break;
                    }
                    break;

                case "diamond": // formation size: 9
                    switch (spotinFormation)
                    {
                        case 1:
                            position = getGridPos(4, 5);
                            break;
                        case 2:
                            position = getGridPos(3, 4);
                            break;
                        case 3:
                            position = getGridPos(5, 4);
                            break;
                        case 4:
                            position = getGridPos(2, 3);
                            break;
                        case 5:
                            position = getGridPos(4, 3);
                            break;
                        case 6:
                            position = getGridPos(6, 3);
                            break;
                        case 7:
                            position = getGridPos(3, 2);
                            break;
                        case 8:
                            position = getGridPos(5, 2);
                            break;
                        case 9:
                            position = getGridPos(4, 1);
                            break;
                        default:
                            Console.WriteLine("Could not set position {0} in {1} formation", spotinFormation, formationType);
                            break;
                    }
                    break;

                case "shockwave": // formation size: 9
                    switch (spotinFormation)
                    {
                        case 1:
                            position = getGridPos(4, 5);
                            break;
                        case 2:
                            position = getGridPos(3, 4);
                            break;
                        case 3:
                            position = getGridPos(5, 4);
                            break;
                        case 4:
                            position = getGridPos(2, 3);
                            break;
                        case 5:
                            position = getGridPos(4, 3);
                            break;
                        case 6:
                            position = getGridPos(6, 3);
                            break;
                        case 7:
                            position = getGridPos(4, 2);
                            break;
                        case 8:
                            position = getGridPos(3, 1);
                            break;
                        case 9:
                            position = getGridPos(5, 1);
                            break;
                        default:
                            Console.WriteLine("Could not set position {0} in {1} formation", spotinFormation, formationType);
                            break;
                    }
                    break;

                default:
                    Console.WriteLine("Could not load formation type");
                    break;
            }
        }

        // grid helper method
        private Vector2 getGridPos(int x, int y)
        {
            return new Vector2(x * tileWidth, (y * tileHeight) - windowHeight);
        }
        #endregion
    }
}