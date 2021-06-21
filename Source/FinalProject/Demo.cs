using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

using GameEngine;

namespace FinalProject
{
    public class Demo : GameObject
    {
        public bool isActive = true;
        public bool shoots = false;
        public bool reverse = false;
        public bool follow = false;
        public Bullet[] bulletList = new Bullet[GameConstants.NumBullets];
        public float shotTime = GameConstants.timeBetweenShots/2;

        public Demo(Model model, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light)
            : base()
        {
            isActive = true;
            // *** Add Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            // *** Add Renderer
            Texture2D texture = Content.Load<Texture2D>("Textures/Square");
            Renderer renderer = new Renderer(model,
                Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            renderer.Material.Diffuse = new Vector3(GameConstants.playerRed, GameConstants.playerGreen, GameConstants.playerBlue);
            renderer.Material.Ambient = new Vector3(0.01f, 0.1f, 0.1f);
            this.Transform.LocalScale *= 0.4f;
            Add<Renderer>(renderer);

            // *** Add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius * GameConstants.ShipBoundingSphereScale;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

        }

        public void Update(GameTime gameTime)
        {
            base.Update();

        }


    }
}
