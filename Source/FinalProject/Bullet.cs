using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using GameEngine;

namespace FinalProject
{
    public class Bullet : GameObject
    {
        public bool isActive { get; set; }
        public int Level { get; set; }

        public Bullet(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light, bool enemy)
            : base()
        {
            // *** Add Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = this.Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            // *** Add Renderer
            Texture2D texture = Content.Load<Texture2D>("Textures/Square");
            Renderer renderer = new Renderer(Content.Load<Model>("Models/bullet"),
                Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            if (!enemy)
            {
                renderer.Material.Diffuse = new Vector3(1f, 0.5f, 0f);
                renderer.Material.Ambient = new Vector3(1f, 0.5f, 0f);
            }
            else
            {
                renderer.Material.Diffuse = new Vector3(1f, 0f, 0f);
                renderer.Material.Ambient = new Vector3(1f, 0f, 0f);
            }
            Add<Renderer>(renderer);

            // *** Add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius * 0.5f;
            sphereCollider.Transform = this.Transform;
            Add<Collider>(sphereCollider);



            //*** Additional Property (for Asteroid, isActive = true)
            isActive = false;
        }

        public override void Update()
        {
            if (!isActive) return;  


            if (Transform.Position.X > GameConstants.PlayfieldSizeX ||
               Transform.Position.X < -GameConstants.PlayfieldSizeX ||
               Transform.Position.Z > GameConstants.PlayfieldSizeY ||
               Transform.Position.Z < -GameConstants.PlayfieldSizeY)
            {
                isActive = false;
                Rigidbody.Velocity = Vector3.Zero; // stop moving
            }

            base.Update();

        }

        public override void Draw()
        {
            if (isActive) 
                base.Draw();
        }



    }
}
