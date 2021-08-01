using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using System.IO;
using System.Collections.Generic;

namespace DirectionalMelee
{
    class DirectionalMeleeSystem : ModSystem
    {
#if DEBUG
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (DirectionalMelee.DEBUG)
            {
                Player player = Main.LocalPlayer;
                if (Main.CurrentFrameFlags.ActivePlayersCount > 1)
                    player = Main.player[(Main.myPlayer + 1) % 2];
                Vector2 playerPosScreen = player.position - Main.screenPosition;
                Vector2 playerCenter = Vector2.Transform(player.Center - Main.screenPosition, Main.GameViewMatrix.ZoomMatrix);
                Vector2 playeTopRight = Vector2.Transform(player.TopRight - Main.screenPosition, Main.GameViewMatrix.ZoomMatrix);
                Vector2 mousePosWorld = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);

                float mouseDirection = (mousePosWorld - player.itemLocation).ToRotation();
                //spriteBatch.Draw(Main.magicPixel, new Rectangle((int)playerCenter.X, (int)playerCenter.Y, 1, 1), Color.Cyan);
                //spriteBatch.Draw(Main.magicPixel, new Rectangle((int)playeTopRight.X, (int)playerCenter.Y, 1, 1), Color.Red);
                //spriteBatch.Draw(Main.magicPixel, new Rectangle((int)playerCenter.X, (int)playeTopRight.Y, 1, 1), Color.Red);
                //spriteBatch.Draw(Main.magicPixel, new Rectangle((int)playeTopRight.X, (int)playeTopRight.Y, 1, 1), Color.Red);
                //spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(player.itemLocation.X - Main.screenPosition.X), (int)(player.itemLocation.Y - Main.screenPosition.Y), 10, 10), Color.Red);

                Rectangle debugRectScreen = new Rectangle((int)(DirectionalMelee.instance.debugRect.X - Main.screenPosition.X), (int)(DirectionalMelee.instance.debugRect.Y - (int)Main.screenPosition.Y), DirectionalMelee.instance.debugRect.Width, DirectionalMelee.instance.debugRect.Height);
                spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, debugRectScreen, Color.Red);
                //ChatManager.DrawColorCodedString(spriteBatch, Main.fontDeathText, debugRect.ToString() + "/" + debugRectScreen.ToString(), new Vector2(200, 100), Color.White, 0, Vector2.Zero, Vector2.One * 0.5f);
                
                ChatManager.DrawColorCodedString(spriteBatch, Terraria.GameContent.FontAssets.DeathText.Value, player.itemRotation.ToString(), new Vector2(100, 100), Color.White, 0, Vector2.Zero, Vector2.One * 0.5f);
                ChatManager.DrawColorCodedString(spriteBatch, Terraria.GameContent.FontAssets.DeathText.Value, mouseDirection.ToString(), new Vector2(100, 120), Color.White, 0, Vector2.Zero, Vector2.One * 0.5f);
                ChatManager.DrawColorCodedString(spriteBatch, Terraria.GameContent.FontAssets.DeathText.Value, DirectionalMelee.instance.debugString, new Vector2(100, 140), Color.White, 0, Vector2.Zero, Vector2.One * 0.5f);
                ChatManager.DrawColorCodedString(spriteBatch, Terraria.GameContent.FontAssets.DeathText.Value, player.itemAnimation.ToString() + "/" + player.itemAnimationMax.ToString(), new Vector2(100, 160), Color.White, 0, Vector2.Zero, Vector2.One * 0.5f);
                ChatManager.DrawColorCodedString(spriteBatch, Terraria.GameContent.FontAssets.DeathText.Value, player.itemTime.ToString(), new Vector2(100, 180), Color.White, 0, Vector2.Zero, Vector2.One * 0.5f);
                ChatManager.DrawColorCodedString(spriteBatch, Terraria.GameContent.FontAssets.DeathText.Value, player.HeldItem.useTurn.ToString(), new Vector2(100, 200), Color.White, 0, Vector2.Zero, Vector2.One * 0.5f);
                ChatManager.DrawColorCodedString(spriteBatch, Terraria.GameContent.FontAssets.DeathText.Value, player.GetModPlayer<DirectionalMeleePlayer>().holdPlayerDirection.ToString() + "/" + player.GetModPlayer<DirectionalMeleePlayer>().holdItemRotation.ToString(), new Vector2(100, 220), Color.White, 0, Vector2.Zero, Vector2.One * 0.5f);
            }
        }
#endif
    }
}
