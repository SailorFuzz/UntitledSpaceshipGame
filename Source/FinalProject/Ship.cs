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
    public class Ship : GameObject
    {
        public bool isActive = true;
        public bool spread = false;
        public bool big = false;
        private TerrainRenderer Terrain;
        public float maxHP = GameConstants.PlayerMaxHealth;
        public float hits = 0;
        public float powerup = GameConstants.powerUpTime;

        public Ship(TerrainRenderer terrain, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light)
            : base()
        {
            isActive = true;
            Terrain = terrain;
            // *** Add Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            // *** Add Renderer
            Texture2D texture = Content.Load<Texture2D>("Textures/Square");
            Renderer renderer = new Renderer(Content.Load<Model>("Models/p1_wedge"),
                Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            renderer.Material.Diffuse = new Vector3(GameConstants.playerRed, GameConstants.playerGreen, GameConstants.playerBlue);
            renderer.Material.Ambient = new Vector3(0.01f, 0.1f, 0.1f);
            this.Transform.LocalScale *= 0.4f;
            Add<Renderer>(renderer);

            // *** Add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius * GameConstants.ShipBoundingSphereScale ;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);
        }


        public void Update(GameTime gameTime)
        {
            if (!isActive)
                return;

            if(hits >= maxHP)
            {
                isActive = false;
                return;
            }

            if (InputManager.IsKeyDown(Keys.W))
                 this.Rigidbody.Acceleration += this.Transform.Forward * GameConstants.ShipSpeed;
            else if (InputManager.IsKeyDown(Keys.S))
                this.Rigidbody.Acceleration -= this.Transform.Forward * GameConstants.ShipSpeed;
            else
                this.Rigidbody.Acceleration *= 0;

            //if (InputManager.IsKeyDown(Keys.LeftShift))
            //    this.Transform.Rotate(Vector3.Forward, Time.ElapsedGameTime * 60);
            //else
            //{
            //    this.Transform.Position = new Vector3(this.Transform.Position.X, 0, this.Transform.Position.Z);
            //}


            if (InputManager.IsKeyDown(Keys.A))
                this.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime * 4);
            if (InputManager.IsKeyDown(Keys.D))
                this.Transform.Rotate(Vector3.Down, Time.ElapsedGameTime * 4);

            if (this.Rigidbody.Velocity.Length() > GameConstants.MaxSpeed)
                this.Rigidbody.Velocity = Vector3.Normalize(this.Rigidbody.Velocity) * GameConstants.MaxSpeed;


            if (big)
                big = powerTime(gameTime);
            if (spread)
                spread = powerTime(gameTime);




            float healthpercent = (float)(hits / maxHP);
            this.Renderer.Material.Diffuse = new Vector3(3f * healthpercent + GameConstants.playerRed, GameConstants.playerGreen * (1 - healthpercent), GameConstants.playerBlue * (1 - healthpercent));


            base.Update();

        }

        private bool powerTime(GameTime gameTime)
        {
            if ((float)gameTime.TotalGameTime.TotalSeconds > powerup)
                return false;
            else
                return true;
        }

    }
}
