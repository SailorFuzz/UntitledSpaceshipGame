using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace FinalProject
{
    public class GameConstants
    {
        //Camera Constants
        public const float CameraHeight = 9000.0f;
        public const float PlayfieldSizeX = 8500f;
        public const float PlayfieldSizeY = 10000f;

        //Ship Constantsa
        public const float ShipSpeed = 3000f;
        public const float MaxSpeed = 5000f;
        public const float boostSpeedMod = 20f;
        public const float WarpBuffer = 3000f;
        public const float PlayerMaxHealth = 10.0f;
        public const float playerRed = 0f;
        public const float playerGreen = 2f;
        public const float playerBlue = 5f;

        //Enemy Constants
        public const float timeBetweenShots = 1.5f;
        public const float circleEnemyRadius = 5000f;
        public const float circleSpread = 2500f;
        public const float basicRed = 0f;
        public const float basicGreen = 3f;
        public const float basicBlue = 0.2f;
        public const float circleRed = 2f;
        public const float circleGreen = 3f;
        public const float circleBlue = 0.2f;


        //Enemy Health
        public const float basicHealth = 5.0f;
        public const float circleHealth = 3.0f;

        //PickUps
        public const float pickupSpeed = 1500f;
        public const float healthChance = 50f;
        public const float bigChance = 70f;
        public const float spreadChance = 85f;
        public const float powerUpTime = 5f;

        //Collision Constants
        public const float enemyBoundingSphereScale = 0.3f; //95% size
        public const float basicBoundingSphereScale = 0.086f; 
        public const float ShipBoundingSphereScale = 0.3f; //50% size

        //Bullet Constants
        public const int NumBullets = 100;
        public const float BulletSpeedAdjustment = 20000.0f;
        public const float BulletLevelAdujustment = 500f;
        public const float enemyBulletSpeed = 3000.0f;

        //Scoring Constants
        public const int ShotPenalty = 1;
        public const int HitBonus = 10;
        public const int DeathPenalty = 100;
        public const int KillBonus = 100;

        

    }
}
