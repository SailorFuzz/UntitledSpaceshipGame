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
    public class Health : GameObject
    {
        public bool isActive = false;
        private TerrainRenderer Terrain;

        public Health(TerrainRenderer terrain, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light, float speed) : base()
        {
            Terrain = terrain;
            // add rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            rigidbody.Velocity = new Vector3(0, 0, speed);
            Add<Rigidbody>(rigidbody);

            //add renderer
            Texture2D texture = Content.Load<Texture2D>("Textures/fire");
            Renderer renderer = new Renderer(Content.Load<Model>("Models/Sphere"),
                Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            renderer.Material.Diffuse = new Vector3(3f, 0.5f, 0.5f);
            renderer.Material.Ambient = new Vector3(0.3f, 0.1f,0.1f);
            this.Transform.LocalScale *= 60f;
            this.Transform.Rotate(Vector3.Down, MathHelper.Pi);
            Add<Renderer>(renderer);

            // Add collider component required for Player
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius * GameConstants.enemyBoundingSphereScale;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            isActive = false;

        }

        public override void Update()
        {
            if (!isActive)
                return;
            this.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime);

            if (this.Transform.Position.Y > GameConstants.PlayfieldSizeY)
                isActive = false;
            base.Update();
        }



    }
}
