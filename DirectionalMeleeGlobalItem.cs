using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DirectionalMelee
{
    class DirectionalMeleeGlobalItem : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstantiation)
        {
            var modDM = DirectionalMelee.instance;
            return lateInstantiation && modDM.IsItemIncluded(item);
        }


        public override bool CanUseItem(Item item, Player player)
        {
            DirectionalMeleePlayer modPlayer = player.GetModPlayer<DirectionalMeleePlayer>();
            if (Main.myPlayer == player.whoAmI)
            {
                Vector2 mousePosWorld = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                modPlayer.useDirection = -1;
                if (mousePosWorld.X >= player.Center.X)
                    modPlayer.useDirection = 1;
                modPlayer.useRotation = (mousePosWorld - player.MountedCenter).ToRotation() + ((modPlayer.useDirection > 0 ? 0.8f : -2.35f) * modPlayer.useDirection);

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)DirectionalMelee.MessageType.UseLocation);
                    packet.Write((sbyte)modPlayer.useDirection);
                    packet.Write(modPlayer.useRotation);
                    packet.Send();
                }
            }
            return true;
        }

        public override void UseStyle(Item item, Player player, Rectangle heldItemFrame)
        {
            DirectionalMeleePlayer modPlayer = player.GetModPlayer<DirectionalMeleePlayer>();
            //TODO: Torches have alternates, which makes them also not change dir. Seems fucky, maybe needs change?
            bool canChangeDir = true;
            if (item.createTile > -1)
            {
                TileObjectData tod = TileObjectData.GetTileData(item.createTile, 0);
                if (tod != null && tod.AlternatesCount > 0)
                    canChangeDir = false;
            }
            if (canChangeDir)
                player.ChangeDir(modPlayer.useDirection);

            if (item.useStyle == 1)
            {
                if (player.direction == 1)
                {
                    player.itemRotation = modPlayer.useRotation - DirectionalMelee.preSwingAngle + ((float)(player.itemAnimationMax - player.itemAnimation) / (float)player.itemAnimationMax * DirectionalMelee.totalSwingAngle);
                }
                else
                {
                    player.itemRotation = modPlayer.useRotation + DirectionalMelee.preSwingAngle - ((float)(player.itemAnimationMax - player.itemAnimation) / (float)player.itemAnimationMax * DirectionalMelee.totalSwingAngle);
                }
                player.itemRotation *= player.gravDir;
            }
            else
            {
                if (player.itemAnimation > player.itemAnimationMax * 0.666)
                {
                    player.itemLocation.X = -1000f;
                    player.itemLocation.Y = -1000f;
                    player.itemRotation = -1.3f * player.direction;
                }
                else
                {
                    //TODO: replace with directional offset
                    //or dont, its overwritten later by SetItemLocationHand so it's useless right now and does nothing
                    float offset = player.itemAnimation / player.itemAnimationMax * Terraria.GameContent.TextureAssets.Item[item.type].Value.Width * player.direction * item.scale * 1.2f - (10 * player.direction);
                    if (offset > -4f && player.direction == -1)
                    {
                        offset = -8f;
                    }
                    if (offset < 4f && player.direction == 1)
                    {
                        offset = 8f;
                    }
                    player.itemLocation.X = player.itemLocation.X - offset;

                    player.itemRotation = modPlayer.useRotation;
                    if (player.gravDir == -1f)
                    {
                        player.itemRotation = -player.itemRotation;
                    }
                }
            }
            if (item.useStyle != 3 || player.itemAnimation <= player.itemAnimationMax * 0.666)
                modPlayer.SetItemLocationHand();
        }

        public override void UseItemFrame(Item item, Player player)
        {
            DirectionalMeleePlayer modPlayer = player.GetModPlayer<DirectionalMeleePlayer>();

            float itemDirection = modPlayer.GetHeldItemRotation();
            if (player.itemAnimation > 0 && player.inventory[player.selectedItem].useStyle != 10)
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
            }
        }

        public override void HoldItem(Item item, Player player)
        {
            base.HoldItem(item, player);
        }

        public override void UseItemHitbox(Item item, Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            DirectionalMeleePlayer modPlayer = player.GetModPlayer<DirectionalMeleePlayer>();

            float itemLengthSqr = Vector2.DistanceSquared(Vector2.Zero, new Vector2(Terraria.GameContent.TextureAssets.Item[item.type].Value.Width, Terraria.GameContent.TextureAssets.Item[item.type].Value.Height));
            float itemLength = (float)Math.Sqrt(itemLengthSqr);
            float itemRotation = modPlayer.GetHeldItemRotation();

            hitbox = new Rectangle();

            hitbox.Width = (int)(Math.Abs(Math.Sin(itemRotation)) * itemLength) + DirectionalMelee.increaseHitboxBy;
            hitbox.Height = (int)(Math.Abs(Math.Cos(itemRotation)) * itemLength) + DirectionalMelee.increaseHitboxBy;
            hitbox.Width = (int)((float)hitbox.Width * item.scale);
            hitbox.Height = (int)((float)hitbox.Height * item.scale);

            if (item.useStyle == 1)
            {
                if (hitbox.Width < DirectionalMelee.minHitboxSizeSwing)
                    hitbox.Width = DirectionalMelee.minHitboxSizeSwing;
                if (hitbox.Height < DirectionalMelee.minHitboxSizeSwing)
                    hitbox.Height = DirectionalMelee.minHitboxSizeSwing;
            }
            else if (item.useStyle == 3)
            {
                if (hitbox.Width < DirectionalMelee.minHitboxSizeStab)
                    hitbox.Width = DirectionalMelee.minHitboxSizeStab;
                if (hitbox.Height < DirectionalMelee.minHitboxSizeStab)
                    hitbox.Height = DirectionalMelee.minHitboxSizeStab;
            }

            hitbox.X = (int)player.itemLocation.X;
            hitbox.Y = (int)player.itemLocation.Y;

            if (player.gravDir == -1)
            {
                hitbox.Y = (int)(player.position.Y + player.height + (player.position.Y - hitbox.Y));
            }
            if (player.direction == -1)
            {
                hitbox.X -= hitbox.Width;
                if (itemRotation < 0 || itemRotation > DirectionalMelee.PI)
                    hitbox.X += hitbox.Width * 2;
            }

            if (itemRotation < 0 || itemRotation > DirectionalMelee.PI)
                hitbox.X -= hitbox.Width;
            if (itemRotation < DirectionalMelee.halfPI)
                hitbox.Y -= hitbox.Height;
#if DEBUG
            DirectionalMelee.instance.debugRect = hitbox;
#endif
        }
    }
}
