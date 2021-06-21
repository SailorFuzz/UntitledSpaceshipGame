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
    public class CircleEnemy : GameObject
    {
        public bool isActive = false;
        public Vector3 destination = new Vector3(1,0,1);
        public Bullet[] bulletList = new Bullet[GameConstants.NumBullets];
        public float maxHP = GameConstants.circleHealth;
        public float hits = 0;
        private float shotTime = GameConstants.timeBetweenShots;

        private TerrainRenderer Terrain;
        private float Speed = 5f; //moving speed

        public CircleEnemy(TerrainRenderer terrain, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light, float speed) : base()
        {
            Terrain = terrain;
            // add rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            //add renderer
            Texture2D texture = Content.Load<Texture2D>("Textures/Square");
            Renderer renderer = new Renderer(Content.Load<Model>("Models/p1_wedge"),
                Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            renderer.Material.Diffuse = new Vector3(GameConstants.circleRed, GameConstants.circleGreen, GameConstants.circleBlue);
            renderer.Material.Ambient = new Vector3(0.01f, 0.1f, 0.1f);
            this.Transform.LocalScale *= 0.5f;
            this.Transform.Rotate(Vector3.Down, MathHelper.Pi);
            Add<Renderer>(renderer);

            // Add collider component required for Player
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius * GameConstants.enemyBoundingSphereScale;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            Speed = speed;

            for (int i = 0; i < GameConstants.NumBullets; i++)
            {
                bulletList[i] = new Bullet(Content, camera, graphicsDevice, light, true);
                bulletList[i].Transform.Scale *= 1;
            }

            isActive = false;

        }

        public void Update(Ship player, GameTime gameTime)
        {
            if (!isActive)
                return;

            if (hits >= maxHP)
            {
                isActive = false;
                return;
            }

            if (Vector3.Distance(this.Transform.Position, player.Transform.Position) > GameConstants.circleEnemyRadius)
            {
                this.Transform.Position -= Vector3.Normalize(this.Transform.Position - player.Transform.Position) * Speed;
            }
            else if (Vector3.Distance(this.Transform.Position, player.Transform.Position) < GameConstants.circleEnemyRadius)
            {
                this.Transform.Position += Vector3.Normalize(this.Transform.Position - player.Transform.Position) * Speed;
            }

            this.Transform.Rotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateLookAt(this.Transform.Position, player.Transform.Position, Vector3.Down));
            this.Transform.Rotate(Vector3.Forward, MathHelper.Pi);

            //this.Transform.LocalPosition = new Vector3(
            //   this.Transform.LocalPosition.X,
            //   Terrain.GetAltitude(this.Transform.LocalPosition) * 2 - 200,
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
                        break;
                    }
                }



            foreach (GameObject gameObject in bulletList)
                gameObject.Update();

            float healthpercent = (float)(hits / maxHP);
            this.Renderer.Material.Diffuse = new Vector3(5f * healthpercent + GameConstants.circleRed, GameConstants.circleGreen * (1 - healthpercent), GameConstants.circleBlue * (1 - healthpercent));

            base.Update();
        }

        private bool canShoot(GameTime gameTime)
        {
            Random random = new Random();
            if ((float)gameTime.TotalGameTime.TotalSeconds > shotTime)
            {
                shotTime = (float)gameTime.TotalGameTime.TotalSeconds + GameConstants.timeBetweenShots + (float)(GameConstants.timeBetweenShots * random.NextDouble());
                return true;
            }
            else
                return false;
        }



    }
}
