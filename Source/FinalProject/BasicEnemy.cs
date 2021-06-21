using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using GameEngine;

namespace FinalProject
{
    public class BasicEnemy : GameObject
    {
        public bool isActive = false;
        public bool isBoss = false;
        public Vector3 destination = new Vector3(1,0,1);
        public Bullet[] bulletList = new Bullet[GameConstants.NumBullets];
        public float maxHP = GameConstants.basicHealth;
        public float hits = 0;
        public float shotTime = GameConstants.timeBetweenShots;
        public float shotMod = 1f;

        private TerrainRenderer Terrain;
        private float Speed = 5f; //moving speed

        public BasicEnemy(TerrainRenderer terrain, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light, float speed) : base()
        {
            Terrain = terrain;
            // add rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            //add renderer
            Texture2D texture = Content.Load<Texture2D>("Textures/Square");
            Renderer renderer = new Renderer(Content.Load<Model>("Models/Spaceships_3"),
                Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            renderer.Material.Diffuse = new Vector3(GameConstants.basicRed, GameConstants.basicGreen, GameConstants.basicBlue);
            renderer.Material.Ambient = new Vector3(0.01f, 0.1f, 0.1f);
            this.Transform.LocalScale *= 0.1f;
            //this.Transform.Rotate(Vector3.Down, MathHelper.Pi);
            Add<Renderer>(renderer);

            // Add collider component required for Player
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius * GameConstants.basicBoundingSphereScale;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            Speed = speed;

            for (int i = 0; i < GameConstants.NumBullets; i++)
            {
                bulletList[i] = new Bullet(Content, camera, graphicsDevice, light, true);
                bulletList[i].Transform.Scale *= 1;
            }

            isActive = false;
            isBoss = false;

        }

        public void Update(Ship player, GameTime gameTime)
        {

            if (!isActive)
                return;

            if(hits >= maxHP)
            {
                isActive = false;
                return;
            }

            
            if (Vector3.Distance(this.Transform.Position, destination) > 10f)
            {
                this.Transform.Position -= Vector3.Normalize(this.Transform.Position - destination) * Speed;
            }

            //this.Transform.LocalPosition = new Vector3(
            //   this.Transform.LocalPosition.X,
            //   Terrain.GetAltitude(this.Transform.LocalPosition)*2-200,
            //   this.Transform.LocalPosition.Z) + Vector3.Up;

            if (player.isActive && canShoot(gameTime))
                for (int i = 0; i < GameConstants.NumBullets; i++)
                {
                    if (!bulletList[i].isActive)
                    {
                        bulletList[i].Rigidbody.Velocity = -Vector3.Normalize(this.Transform.Position - player.Transform.Position) * (GameConstants.enemyBulletSpeed);
                        bulletList[i].Transform.LocalPosition = this.Transform.Position;
                        bulletList[i].Level = 1;
                        bulletList[i].isActive = true;
                        if (isBoss)
                        {
                            int temp = i;
                            bool left = false;
                            bool right = false;
                            do
                            {
                                temp++;
                                if (temp > bulletList.Length)
                                    temp = 0;
                                if (!left && !bulletList[temp].isActive)
                                {
                                    bulletList[temp].Rigidbody.Velocity = (-Vector3.Normalize(this.Transform.Position - player.Transform.Position) + this.Transform.Left * 2) * (GameConstants.enemyBulletSpeed);
                                    bulletList[temp].Transform.LocalPosition = this.Transform.Position;
                                    bulletList[temp].isActive = true;
                                    left = true;
                                }
                                temp++;
                                if (temp > bulletList.Length)
                                    temp = 0;
                                if (!right && !bulletList[temp].isActive)
                                {
                                    bulletList[temp].Rigidbody.Velocity = (-Vector3.Normalize(this.Transform.Position - player.Transform.Position) + this.Transform.Right * 2) * (GameConstants.enemyBulletSpeed);
                                    bulletList[temp].Transform.LocalPosition = this.Transform.Position;
                                    bulletList[temp].isActive = true;
                                    right = true;
                                }

                            } while (!left && !right);

                        }
                        break;
                    }
                }

            this.Transform.Rotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateLookAt(this.Transform.Position, player.Transform.Position, Vector3.Down));
            this.Transform.Rotate(Vector3.Left, MathHelper.Pi);

            foreach (GameObject gameObject in bulletList)
                gameObject.Update();

            float healthpercent = (float)(hits / maxHP);
            this.Renderer.Material.Diffuse = new Vector3(5f *healthpercent + GameConstants.basicRed, GameConstants.basicGreen * (1-healthpercent), GameConstants.basicBlue * (1 - healthpercent));
            //this.Renderer.Material.Ambient = Vector3.One * (float)random.NextDouble();

            base.Update();
        }

        private bool canShoot(GameTime gameTime)
        {
            Random random = new Random();
            if ((float)gameTime.TotalGameTime.TotalSeconds > shotTime)
            {
                shotTime = (float)gameTime.TotalGameTime.TotalSeconds + (GameConstants.timeBetweenShots + (float)(GameConstants.timeBetweenShots * random.NextDouble())) * shotMod;
                return true;
            }
            else
                return false;
        }



    }
}
