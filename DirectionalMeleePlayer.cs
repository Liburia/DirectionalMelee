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


        //TODO: Remove after TML fixes the call to GlobalItem.UseItemFrame
        public override void PostUpdate()
        {
            PlayerFrame();
        }
        void PlayerFrame()
        {
            Item item = player.HeldItem;
            float itemDirection = GetHeldItemRotation();
            if (player.itemAnimation > 0 && player.inventory[player.selectedItem].useStyle != 10)
            {
                if (item.useStyle == 0 || item.useStyle == 1 || item.useStyle == 3)
                {
                    if (item.useStyle == 3 && player.itemAnimation > player.itemAnimationMax * 0.666f)
                    {
                        player.bodyFrame.Y = player.bodyFrame.Height * 6;
                    }

                    if (itemDirection < DirectionalMelee.handAngleThresholds[0])
                    {
                        player.bodyFrame.Y = player.bodyFrame.Height;
                    }
                    else if (itemDirection < DirectionalMelee.handAngleThresholds[1])
                    {
                        player.bodyFrame.Y = player.bodyFrame.Height * 2;
                    }
                    else if (itemDirection < DirectionalMelee.handAngleThresholds[2])
                    {
                        player.bodyFrame.Y = player.bodyFrame.Height * 3;
                    }
                    else if (itemDirection < DirectionalMelee.handAngleThresholds[3])
                    {
                        player.bodyFrame.Y = player.bodyFrame.Height * 4;
                    }
                    else
                    {
                        player.bodyFrame.Y = player.bodyFrame.Height * 17;
                    }
                    return;
                }
            }
        }

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
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)DirectionalMelee.MessageType.HoldPlayerDirection);
                packet.Write((sbyte)holdPlayerDirection);
                packet.Send();
            }
            if (clone.holdItemRotation != holdItemRotation)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)DirectionalMelee.MessageType.HoldItemRotation);
                packet.Write(holdItemRotation);
                packet.Send();
            }
        }

        /// <summary>
        /// Value returned is in range from -0.5Pi to 1.5Pi. The min and max point towards the back of the player. 0 points up and Pi points down.
        /// </summary>
        /// <returns></returns>
        public float GetHeldItemRotation()
        {
            float itemDirection = player.itemRotation * player.direction * player.gravDir;
            if (Math.Abs(itemDirection) > DirectionalMelee.PI * 2)
                itemDirection %= DirectionalMelee.PI * 2;
            Item item = player.HeldItem;
            if (item.useStyle == 1 || item.useStyle == 3)
            {
                itemDirection += DirectionalMelee.quarterPI;
            }
            if (player.direction == -1 && itemDirection < -DirectionalMelee.halfPI)
                itemDirection += DirectionalMelee.PI * 2;


            return itemDirection;
        }
        /// <summary>
        /// Opposite of <see cref="GetHeldItemRotation"/>. Accepts rotation from -0.5Pi to 1.5Pi. Returns true <see cref="Player.itemRotation"/>.
        /// </summary>
        /// <returns></returns>
        public float SetHeldItemRotation(float itemRotation)
        {
            Item item = player.HeldItem;
            float newRotation = itemRotation;
            if (item.useStyle == 1 || item.useStyle == 3)
            {
                newRotation -= DirectionalMelee.quarterPI;
            }
            newRotation *= player.direction * player.gravDir;

            player.itemRotation = newRotation;

            return newRotation;
        }

        /// <summary>
        /// Moves the item to the correct position base on current player.itemRotation.
        /// </summary>
        public void SetItemLocationHand()
        {
            //TODO: set thresholds for weapons with sprites different from 32px.
            float num = player.mount.PlayerOffsetHitbox;
            float itemDirection = GetHeldItemRotation();
            float offset;
            Item item = player.HeldItem;
            if (itemDirection < DirectionalMelee.handAngleThresholds[0])
            {
                offset = 10f;
                if (Main.itemTexture[item.type].Width > 32)
                {
                    offset = 18f;
                }
                if (Main.itemTexture[item.type].Width >= 48)
                {
                    offset = 22f;
                }
                if (Main.itemTexture[item.type].Width >= 52)
                {
                    offset = 28f;
                }
                if (Main.itemTexture[item.type].Width >= 64)
                {
                    offset = 32f;
                }
                if (Main.itemTexture[item.type].Width >= 92)
                {
                    offset = 42f;
                }
                if (item.type == 2330 || item.type == 2320 || item.type == 2341)
                {
                    offset += 4f;
                }
                player.itemLocation.X = player.position.X + player.width * 0.5f - (Main.itemTexture[item.type].Width * 0.5f - offset) * player.direction;
                offset = 10f;
                if (Main.itemTexture[item.type].Height > 32)
                {
                    offset = 10f;
                }
                if (Main.itemTexture[item.type].Height > 52)
                {
                    offset = 12f;
                }
                if (Main.itemTexture[item.type].Height > 64)
                {
                    offset = 14f;
                }
                if (item.type == 2330 || item.type == 2320 || item.type == 2341)
                {
                    offset += 4f;
                }
                player.itemLocation.Y = player.position.Y + offset + num;
            }
            else if (itemDirection < DirectionalMelee.handAngleThresholds[1])
            {
                offset = 10f;
                if (Main.itemTexture[item.type].Width > 32)
                {
                    offset = 18f;
                }
                if (Main.itemTexture[item.type].Width >= 52)
                {
                    offset = 24f;
                }
                if (Main.itemTexture[item.type].Width >= 64)
                {
                    offset = 28f;
                }
                if (Main.itemTexture[item.type].Width >= 92)
                {
                    offset = 38f;
                }
                if (item.type == 2330 || item.type == 2320 || item.type == 2341)
                {
                    offset += 4f;
                }
                player.itemLocation.X = player.position.X + player.width * 0.5f + (Main.itemTexture[item.type].Width * 0.5f - offset) * player.direction;
                offset = 10f;
                if (Main.itemTexture[item.type].Height > 32)
                {
                    offset = 8f;
                }
                if (Main.itemTexture[item.type].Height > 52)
                {
                    offset = 12f;
                }
                if (Main.itemTexture[item.type].Height > 64)
                {
                    offset = 14f;
                }
                if (item.type == 2330 || item.type == 2320 || item.type == 2341)
                {
                    offset += 4f;
                }
                player.itemLocation.Y = player.position.Y + offset + num;
            }
            else if (itemDirection < DirectionalMelee.handAngleThresholds[2])
            {
                offset = 10f;
                if (Main.itemTexture[item.type].Width > 32)
                {
                    offset = 14f;
                }
                if (Main.itemTexture[item.type].Width >= 52)
                {
                    offset = 24f;
                }
                if (Main.itemTexture[item.type].Width >= 64)
                {
                    offset = 28f;
                }
                if (Main.itemTexture[item.type].Width >= 92)
                {
                    offset = 38f;
                }
                if (item.type == 2330 || item.type == 2320 || item.type == 2341)
                {
                    offset += 8f;
                }
                player.itemLocation.X = player.position.X + player.width * 0.5f + (Main.itemTexture[item.type].Width * 0.5f - offset) * player.direction;
                player.itemLocation.Y = player.position.Y + 24f + num;
            }
            else if (itemDirection < DirectionalMelee.handAngleThresholds[3])
            {
                offset = 12f;
                if (Main.itemTexture[item.type].Width > 32)
                {
                    offset = 20f;
                }
                if (Main.itemTexture[item.type].Width >= 52)
                {
                    offset = 26f;
                }
                if (Main.itemTexture[item.type].Width >= 64)
                {
                    offset = 30f;
                }
                if (Main.itemTexture[item.type].Width >= 92)
                {
                    offset = 40f;
                }
                if (item.type == 2330 || item.type == 2320 || item.type == 2341)
                {
                    offset += 4f;
                }
                player.itemLocation.X = player.position.X + player.width * 0.5f + (Main.itemTexture[item.type].Width * 0.5f - offset) * player.direction;
                offset = 28f;
                if (Main.itemTexture[item.type].Height > 32)
                {
                    offset = 26f;
                }
                if (Main.itemTexture[item.type].Height > 52)
                {
                    offset = 30f;
                }
                if (Main.itemTexture[item.type].Height > 64)
                {
                    offset = 32f;
                }
                if (item.type == 2330 || item.type == 2320 || item.type == 2341)
                {
                    offset -= 4f;
                }
                player.itemLocation.Y = player.position.Y + offset + num;
            }
            else
            {
                offset = 15f;
                if (Main.itemTexture[item.type].Width > 32)
                {
                    offset = 13f;
                }
                if (Main.itemTexture[item.type].Width >= 48)
                {
                    offset = 17f;
                }
                if (Main.itemTexture[item.type].Width >= 52)
                {
                    offset = 23f;
                }
                if (Main.itemTexture[item.type].Width >= 64)
                {
                    offset = 27f;
                }
                if (Main.itemTexture[item.type].Width >= 92)
                {
                    offset = 37f;
                }
                player.itemLocation.X = player.position.X + player.width * 0.5f - (Main.itemTexture[item.type].Width * 0.5f - offset) * player.direction;
                offset = 23f;
                if (Main.itemTexture[item.type].Height > 32)
                {
                    offset = 21f;
                }
                if (Main.itemTexture[item.type].Height > 52)
                {
                    offset = 25f;
                }
                if (Main.itemTexture[item.type].Height > 64)
                {
                    offset = 27f;
                }
                player.itemLocation.Y = player.position.Y + offset + num;
            }
            if (player.gravDir == -1)
                player.itemLocation.Y = player.position.Y + player.height + (player.position.Y - player.itemLocation.Y);
        }
    }
}
