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
    class DirectionalMelee : Mod
    {
        public enum MessageType : byte
        {
            /// <summary>
            /// Direction of the player and rotation towards the cursor
            /// </summary>
            UseLocation
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

        /// <summary>
        /// Don't use this. Each use style needs custom code and adding your own is currently unsupported.
        /// </summary>
        internal HashSet<int> includedUseStyles = new HashSet<int>();
        internal HashSet<int> includedItems = new HashSet<int>();
        internal HashSet<int> excludedItems = new HashSet<int>();

#if DEBUG
        public string debugString = "";
        public Rectangle debugRect = new Rectangle();
#endif


        public DirectionalMelee() { }

        public override void Load()
        {
            instance = this;
            includedUseStyles.Add(Terraria.ID.ItemUseStyleID.Swing);
        }
        public override void Unload()
        {
            instance = null;
            includedUseStyles.Clear();
            includedItems.Clear();
            excludedItems.Clear();
        }

        public override object Call(params object[] args)
        {
            string message = args[0] as string;

            if (message == "includeUseStyle")
            {
                int style = (int)args[1];
                IncludeUseStyle(style);
            }
            else if (message == "includeItem")
            {
                int item = (int)args[1];
                IncludeItem(item);
            }
            else if (message == "excludeItem")
            {
                int item = (int)args[1];
                ExcludeItem(item);
            }
            else
            {
                return "Failure";
            }
            return "Success";
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            Player player;
            MessageType msgType = (MessageType)reader.ReadByte();
            if (Main.netMode == Terraria.ID.NetmodeID.Server)
            {
                ModPacket packet;
                switch (msgType)
                {
                    case MessageType.UseLocation:
                        int useDirection = reader.ReadSByte();
                        float useRotation = reader.ReadSingle();

                        player = Main.player[whoAmI];
                        var modPlayer = player.GetModPlayer<DirectionalMeleePlayer>();

                        modPlayer.useDirection = useDirection;
                        modPlayer.useRotation = useRotation;

                        packet = GetPacket();
                        packet.Write((byte)MessageType.UseLocation);
                        packet.Write((sbyte)useDirection);
                        packet.Write(useRotation);
                        packet.Write((byte)whoAmI);

                        packet.Send(ignoreClient: whoAmI);
                        break;
                    default:
                        Logger.Debug("Unknown Message type: " + msgType);
                        break;
                }
            }
            else
            {
                int playerIndex;
                switch (msgType)
                {
                    case MessageType.UseLocation:
                        int useDirection = reader.ReadSByte();
                        float useRotation = reader.ReadSingle();
                        playerIndex = reader.ReadByte();

                        player = Main.player[playerIndex];
                        var modPlayer = player.GetModPlayer<DirectionalMeleePlayer>();

                        modPlayer.useDirection = useDirection;
                        modPlayer.useRotation = useRotation;
                        break;
                    default:
                        Logger.Debug("Unknown Message type: " + msgType);
                        break;
                }
            }
        }

        private void IncludeUseStyle(int style)
        {
            includedUseStyles.Add(style);
        }
        private void IncludeItem(int item)
        {
            includedItems.Add(item);
        }
        private void ExcludeItem(int item)
        {
            excludedItems.Add(item);
        }

        /// <summary>
        /// Returns whether the item is affected by Directional Melee or not
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool IsItemIncluded(Item item)
        {
            return (includedUseStyles.Contains(item.useStyle) || includedItems.Contains(item.type)) && !excludedItems.Contains(item.type);
        }
    }
}
