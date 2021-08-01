using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using System.IO;

namespace DirectionalMelee
{
    class DirectionalMeleePlayer : ModPlayer
    {
        public int holdPlayerDirection = 1;
        public float holdItemRotation = 0;

        public override void clientClone(ModPlayer clientClone)
        {
            DirectionalMeleePlayer clone = clientClone as DirectionalMeleePlayer;
            clone.holdPlayerDirection = holdPlayerDirection;
            clone.holdItemRotation = holdItemRotation;
        }
        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            DirectionalMeleePlayer clone = clientPlayer as DirectionalMeleePlayer;
            if (clone.holdPlayerDirection != holdPlayerDirection)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)DirectionalMelee.MessageType.HoldPlayerDirection);
                packet.Write((sbyte)holdPlayerDirection);
                packet.Send();
            }
            if (clone.holdItemRotation != holdItemRotation)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)DirectionalMelee.MessageType.HoldItemRotation);
                packet.Write(holdItemRotation);
                packet.Send();
            }
        }

        /// <summary>
        /// Value returned is in range from -0.5Pi to 1.5Pi. The min and max point towards the back of the Player. 0 points up and Pi points down.
        /// </summary>
        /// <returns></returns>
        public float GetHeldItemRotation()
        {
            Item item = Player.HeldItem;
            float itemDirection = Player.itemRotation * Player.direction * Player.gravDir;
            if (Math.Abs(itemDirection) > DirectionalMelee.PI * 2)
                itemDirection %= DirectionalMelee.PI * 2;
            if (item.useStyle == 1 || item.useStyle == 3)
            {
                itemDirection += DirectionalMelee.quarterPI;
            }
            if (Player.direction == -1 && itemDirection < -DirectionalMelee.halfPI)
                itemDirection += DirectionalMelee.PI * 2;


            return itemDirection;
        }
        /// <summary>
        /// Opposite of <see cref="GetHeldItemRotation"/>. Accepts rotation from -0.5Pi to 1.5Pi. Returns true <see cref="Player.itemRotation"/>.
        /// </summary>
        /// <returns></returns>
        public float SetHeldItemRotation(float itemRotation)
        {
            Item item = Player.HeldItem;
            float newRotation = itemRotation;
            if (item.useStyle == 1 || item.useStyle == 3)
            {
                newRotation -= DirectionalMelee.quarterPI;
            }
            newRotation *= Player.direction * Player.gravDir;

            Player.itemRotation = newRotation;

            return newRotation;
        }

        /// <summary>
        /// Moves the item to the correct position base on current Player.itemRotation.
        /// </summary>
        public void SetItemLocationHand()
        {
            //TODO: set thresholds for weapons with sprites different from 32px.
            float num = Player.mount.PlayerOffsetHitbox;
            float itemDirection = GetHeldItemRotation();
            float offset;
            Item item = Player.HeldItem;
            if (itemDirection < DirectionalMelee.handAngleThresholds[0])
            {
                offset = 10f;
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width > 32)
                {
                    offset = 18f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width >= 48)
                {
                    offset = 22f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width >= 52)
                {
                    offset = 28f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width >= 64)
                {
                    offset = 32f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width >= 92)
                {
                    offset = 42f;
                }
                if (item.type == 2330 || item.type == 2320 || item.type == 2341)
                {
                    offset += 4f;
                }
                Player.itemLocation.X = Player.position.X + Player.width * 0.5f - (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width * 0.5f - offset) * Player.direction;
                offset = 10f;
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Height > 32)
                {
                    offset = 10f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Height > 52)
                {
                    offset = 12f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Height > 64)
                {
                    offset = 14f;
                }
                if (item.type == 2330 || item.type == 2320 || item.type == 2341)
                {
                    offset += 4f;
                }
                Player.itemLocation.Y = Player.position.Y + offset + num;
            }
            else if (itemDirection < DirectionalMelee.handAngleThresholds[1])
            {
                offset = 10f;
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width > 32)
                {
                    offset = 18f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width >= 52)
                {
                    offset = 24f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width >= 64)
                {
                    offset = 28f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width >= 92)
                {
                    offset = 38f;
                }
                if (item.type == 2330 || item.type == 2320 || item.type == 2341)
                {
                    offset += 4f;
                }
                Player.itemLocation.X = Player.position.X + Player.width * 0.5f + (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width * 0.5f - offset) * Player.direction;
                offset = 10f;
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Height > 32)
                {
                    offset = 8f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Height > 52)
                {
                    offset = 12f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Height > 64)
                {
                    offset = 14f;
                }
                if (item.type == 2330 || item.type == 2320 || item.type == 2341)
                {
                    offset += 4f;
                }
                Player.itemLocation.Y = Player.position.Y + offset + num;
            }
            else if (itemDirection < DirectionalMelee.handAngleThresholds[2])
            {
                offset = 10f;
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width > 32)
                {
                    offset = 14f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width >= 52)
                {
                    offset = 24f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width >= 64)
                {
                    offset = 28f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width >= 92)
                {
                    offset = 38f;
                }
                if (item.type == 2330 || item.type == 2320 || item.type == 2341)
                {
                    offset += 8f;
                }
                Player.itemLocation.X = Player.position.X + Player.width * 0.5f + (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width * 0.5f - offset) * Player.direction;
                Player.itemLocation.Y = Player.position.Y + 24f + num;
            }
            else if (itemDirection < DirectionalMelee.handAngleThresholds[3])
            {
                offset = 12f;
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width > 32)
                {
                    offset = 20f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width >= 52)
                {
                    offset = 26f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width >= 64)
                {
                    offset = 30f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width >= 92)
                {
                    offset = 40f;
                }
                if (item.type == 2330 || item.type == 2320 || item.type == 2341)
                {
                    offset += 4f;
                }
                Player.itemLocation.X = Player.position.X + Player.width * 0.5f + (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width * 0.5f - offset) * Player.direction;
                offset = 28f;
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Height > 32)
                {
                    offset = 26f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Height > 52)
                {
                    offset = 30f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Height > 64)
                {
                    offset = 32f;
                }
                if (item.type == 2330 || item.type == 2320 || item.type == 2341)
                {
                    offset -= 4f;
                }
                Player.itemLocation.Y = Player.position.Y + offset + num;
            }
            else
            {
                offset = 15f;
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width > 32)
                {
                    offset = 13f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width >= 48)
                {
                    offset = 17f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width >= 52)
                {
                    offset = 23f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width >= 64)
                {
                    offset = 27f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width >= 92)
                {
                    offset = 37f;
                }
                Player.itemLocation.X = Player.position.X + Player.width * 0.5f - (Terraria.GameContent.TextureAssets.Item[item.type].Value.Width * 0.5f - offset) * Player.direction;
                offset = 23f;
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Height > 32)
                {
                    offset = 21f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Height > 52)
                {
                    offset = 25f;
                }
                if (Terraria.GameContent.TextureAssets.Item[item.type].Value.Height > 64)
                {
                    offset = 27f;
                }
                Player.itemLocation.Y = Player.position.Y + offset + num;
            }
            if (Player.gravDir == -1)
                Player.itemLocation.Y = Player.position.Y + Player.height + (Player.position.Y - Player.itemLocation.Y);
        }
    }
}
