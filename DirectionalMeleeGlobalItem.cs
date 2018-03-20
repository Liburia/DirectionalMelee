using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DirectionalMelee
{
    class DirectionalMeleeGlobalItem : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            DirectionalMeleePlayer modPlayer = player.GetModPlayer<DirectionalMeleePlayer>();
            if (Main.myPlayer == player.whoAmI && (item.useStyle == 1 || item.useStyle == 3))
            {
                Vector2 mousePosWorld = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                modPlayer.holdPlayerDirection = -1;
                if (mousePosWorld.X >= player.Center.X)
                    modPlayer.holdPlayerDirection = 1;
                modPlayer.holdItemRotation = (mousePosWorld - player.MountedCenter).ToRotation() + ((modPlayer.holdPlayerDirection > 0 ? 0.8f : -2.35f) * modPlayer.holdPlayerDirection);
            }
            return true;
        }

        public override void UseStyle(Item item, Player player)
        {
            if (item.useStyle == 1 || item.useStyle == 3)
            {
                DirectionalMeleePlayer modPlayer = player.GetModPlayer<DirectionalMeleePlayer>();
                player.ChangeDir(modPlayer.holdPlayerDirection);
                if (item.useStyle == 1)
                {
                    if (player.direction == 1)
                    {
                        player.itemRotation = modPlayer.holdItemRotation - DirectionalMelee.preSwingAngle + ((float)(player.itemAnimationMax - player.itemAnimation) / (float)player.itemAnimationMax * DirectionalMelee.totalSwingAngle);
                    }
                    else
                    {
                        player.itemRotation = modPlayer.holdItemRotation + DirectionalMelee.preSwingAngle - ((float)(player.itemAnimationMax - player.itemAnimation) / (float)player.itemAnimationMax * DirectionalMelee.totalSwingAngle);
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
                        float offset = player.itemAnimation / player.itemAnimationMax * Main.itemTexture[item.type].Width * player.direction * item.scale * 1.2f - (10 * player.direction);
                        if (offset > -4f && player.direction == -1)
                        {
                            offset = -8f;
                        }
                        if (offset < 4f && player.direction == 1)
                        {
                            offset = 8f;
                        }
                        player.itemLocation.X = player.itemLocation.X - offset;

                        player.itemRotation = modPlayer.holdItemRotation;
                        if (player.gravDir == -1f)
                        {
                            player.itemRotation = -player.itemRotation;
                        }
                    }
                }
                if (item.useStyle != 3 || player.itemAnimation <= player.itemAnimationMax * 0.666)
                    modPlayer.SetItemLocationHand();
            }
        }

        //TODO: Doesn't work with TML 0.10.1.3 https://github.com/blushiemagic/tModLoader/issues/319
        public override bool UseItemFrame(Item item, Player player)
        {
            if (item.useStyle == 1 || item.useStyle == 3)
            {
                DirectionalMeleePlayer modPlayer = player.GetModPlayer<DirectionalMeleePlayer>();
                float itemDirection = modPlayer.GetHeldItemRotation();
                if (item.useStyle == 3 && player.itemAnimation > player.itemAnimationMax * 0.666f)
                {
                    player.bodyFrame.Y = player.bodyFrame.Height * 6;
                }
                else if (itemDirection < DirectionalMelee.handAngleThresholds[0])
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
                return true;
            }
            return false;
        }

        public override void UseItemHitbox(Item item, Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            DirectionalMeleePlayer modPlayer = player.GetModPlayer<DirectionalMeleePlayer>();
            float itemLengthSqr = Vector2.DistanceSquared(Vector2.Zero, new Vector2(Main.itemTexture[item.type].Width, Main.itemTexture[item.type].Height));
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
