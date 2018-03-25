using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using System.IO;

namespace DirectionalMelee
{
	class DirectionalMelee : Mod
    {
        public enum MessageType : byte
        {
            HoldPlayerDirection,
            HoldItemRotation,
        }

#if DEBUG
        public const bool DEBUG = false;
#endif

        public static DirectionalMelee instance;

        public static float PI = (float)Math.PI;
        public static float halfPI = PI / 2f;
        public static float quarterPI = PI / 4f;

        public static float[] handAngleThresholds = new float[] { 0.3f, PI / 5f * 2f, PI / 5f * 3f, 2.65f };

        public static float preSwingAngle = halfPI;
        public static float totalSwingAngle = preSwingAngle + quarterPI;

        public static int minHitboxSizeSwing = 20;
        public static int minHitboxSizeStab = 10;

        public static int increaseHitboxBy = 3;

#if DEBUG
        public string debugString = "";
        public Rectangle debugRect = new Rectangle();
#endif

        public DirectionalMelee()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
        }

        public override void Load()
        {
            instance = this;
        }
        public override void Unload()
        {
            instance = null;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            Player player;
            MessageType msgType = (MessageType)reader.ReadByte();
            if (Main.netMode == 2)
            {
                ModPacket packet;
                switch (msgType)
                {
                    case MessageType.HoldPlayerDirection:
                        int holdPlayerDirection = reader.ReadSByte();
                        player = Main.player[whoAmI];
                        player.GetModPlayer<DirectionalMeleePlayer>().holdPlayerDirection = holdPlayerDirection;

                        packet = GetPacket();
                        packet.Write((byte)MessageType.HoldPlayerDirection);
                        packet.Write((sbyte)holdPlayerDirection);
                        packet.Write((byte)whoAmI);

                        for (int i = 0; i < Main.ActivePlayersCount; i++)
                        {
                            if (i != whoAmI)
                                packet.Send(i);
                        }
                        break;
                    case MessageType.HoldItemRotation:
                        float holdItemRotation = reader.ReadSingle();
                        player = Main.player[whoAmI];
                        player.GetModPlayer<DirectionalMeleePlayer>().holdItemRotation = holdItemRotation;

                        packet = GetPacket();
                        packet.Write((byte)MessageType.HoldItemRotation);
                        packet.Write(holdItemRotation);
                        packet.Write((byte)whoAmI);

                        for (int i = 0; i < Main.ActivePlayersCount; i++)
                        {
                            if (i != whoAmI)
                                packet.Send(i);
                        }
                        break;
                    default:
                        ErrorLogger.Log("TestMod: Unknown Message type: " + msgType);
                        break;
                }
            }
            else
            {
                int playerIndex;
                switch (msgType)
                {
                    case MessageType.HoldPlayerDirection:
                        int holdPlayerDirection = reader.ReadSByte();
                        playerIndex = reader.ReadByte();
                        player = Main.player[playerIndex];
                        player.GetModPlayer<DirectionalMeleePlayer>().holdPlayerDirection = holdPlayerDirection;
                        break;
                    case MessageType.HoldItemRotation:
                        float holdItemRotation = reader.ReadSingle();
                        playerIndex = reader.ReadByte();
                        player = Main.player[playerIndex];
                        player.GetModPlayer<DirectionalMeleePlayer>().holdItemRotation = holdItemRotation;
                        break;
                    default:
                        ErrorLogger.Log("TestMod: Unknown Message type: " + msgType);
                        break;
                }
            }
        }

#if DEBUG
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (DEBUG)
            {
                Player player = Main.LocalPlayer;
                if (Main.ActivePlayersCount > 1)
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

                Rectangle debugRectScreen = new Rectangle((int)(debugRect.X - Main.screenPosition.X), (int)(debugRect.Y - (int)Main.screenPosition.Y), debugRect.Width, debugRect.Height);
                spriteBatch.Draw(Main.magicPixel, debugRectScreen, Color.Red);
                //ChatManager.DrawColorCodedString(spriteBatch, Main.fontDeathText, debugRect.ToString() + "/" + debugRectScreen.ToString(), new Vector2(200, 100), Color.White, 0, Vector2.Zero, Vector2.One * 0.5f);

                ChatManager.DrawColorCodedString(spriteBatch, Main.fontDeathText, player.itemRotation.ToString(), new Vector2(100, 100), Color.White, 0, Vector2.Zero, Vector2.One * 0.5f);
                ChatManager.DrawColorCodedString(spriteBatch, Main.fontDeathText, mouseDirection.ToString(), new Vector2(100, 120), Color.White, 0, Vector2.Zero, Vector2.One * 0.5f);
                ChatManager.DrawColorCodedString(spriteBatch, Main.fontDeathText, debugString, new Vector2(100, 140), Color.White, 0, Vector2.Zero, Vector2.One * 0.5f);
                ChatManager.DrawColorCodedString(spriteBatch, Main.fontDeathText, player.itemAnimation.ToString() + "/" + player.itemAnimationMax.ToString(), new Vector2(100, 160), Color.White, 0, Vector2.Zero, Vector2.One * 0.5f);
                ChatManager.DrawColorCodedString(spriteBatch, Main.fontDeathText, player.itemTime.ToString(), new Vector2(100, 180), Color.White, 0, Vector2.Zero, Vector2.One * 0.5f);
                ChatManager.DrawColorCodedString(spriteBatch, Main.fontDeathText, player.HeldItem.useTurn.ToString(), new Vector2(100, 200), Color.White, 0, Vector2.Zero, Vector2.One * 0.5f);
                ChatManager.DrawColorCodedString(spriteBatch, Main.fontDeathText, player.GetModPlayer<DirectionalMeleePlayer>().holdPlayerDirection.ToString() + "/" + player.GetModPlayer<DirectionalMeleePlayer>().holdItemRotation.ToString(), new Vector2(100, 220), Color.White, 0, Vector2.Zero, Vector2.One * 0.5f);
            }
        }
#endif
    }
}
