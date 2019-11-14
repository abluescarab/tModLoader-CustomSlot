using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace CustomSlot {
    internal class UIUtils {
        /// <summary>
        /// Get the background texture of a slot based on its context.
        /// </summary>
        /// <param name="context">slot context</param>
        /// <returns>background texture of the slot</returns>
        internal static Texture2D GetBackgroundTexture(int context) {
            switch(context) {
                case ItemSlot.Context.EquipAccessory:
                case ItemSlot.Context.EquipArmor:
                case ItemSlot.Context.EquipGrapple:
                case ItemSlot.Context.EquipMount:
                case ItemSlot.Context.EquipMinecart:
                case ItemSlot.Context.EquipPet:
                case ItemSlot.Context.EquipLight:
                    return Main.inventoryBack3Texture;
                case ItemSlot.Context.EquipArmorVanity:
                case ItemSlot.Context.EquipAccessoryVanity:
                    return Main.inventoryBack8Texture;
                case ItemSlot.Context.EquipDye:
                    return Main.inventoryBack12Texture;
                case ItemSlot.Context.ChestItem:
                    return Main.inventoryBack5Texture;
                case ItemSlot.Context.BankItem:
                    return Main.inventoryBack2Texture;
                case ItemSlot.Context.GuideItem:
                case ItemSlot.Context.PrefixItem:
                case ItemSlot.Context.CraftingMaterial:
                    return Main.inventoryBack4Texture;
                case ItemSlot.Context.TrashItem:
                    return Main.inventoryBack7Texture;
                case ItemSlot.Context.ShopItem:
                    return Main.inventoryBack6Texture;
                default:
                    return Main.inventoryBackTexture;
            }
        }

        /// <summary>
        /// Get the empty texture of a slot based on its context.
        /// </summary>
        /// <param name="context">slot context</param>
        /// <param name="armorType">type of equipment in the slot</param>
        /// <returns>empty texture of the slot</returns>
        internal static CroppedTexture2D GetEmptyTexture(int context, CustomSlot.ArmorType armorType = CustomSlot.ArmorType.HeadArmor) {
            int frame = -1;

            switch(context) {
                case ItemSlot.Context.EquipArmor:
                    switch(armorType) {
                        case CustomSlot.ArmorType.HeadArmor:
                            frame = 0;
                            break;
                        case CustomSlot.ArmorType.ChestArmor:
                            frame = 6;
                            break;
                        case CustomSlot.ArmorType.LegArmor:
                            frame = 12;
                            break;
                    }
                    break;
                case ItemSlot.Context.EquipArmorVanity:
                    switch(armorType) {
                        case CustomSlot.ArmorType.HeadArmor:
                            frame = 3;
                            break;
                        case CustomSlot.ArmorType.ChestArmor:
                            frame = 9;
                            break;
                        case CustomSlot.ArmorType.LegArmor:
                            frame = 15;
                            break;
                    }
                    break;
                case ItemSlot.Context.EquipAccessory:
                    frame = 11;
                    break;
                case ItemSlot.Context.EquipAccessoryVanity:
                    frame = 2;
                    break;
                case ItemSlot.Context.EquipDye:
                    frame = 1;
                    break;
                case ItemSlot.Context.EquipGrapple:
                    frame = 4;
                    break;
                case ItemSlot.Context.EquipMount:
                    frame = 13;
                    break;
                case ItemSlot.Context.EquipMinecart:
                    frame = 7;
                    break;
                case ItemSlot.Context.EquipPet:
                    frame = 10;
                    break;
                case ItemSlot.Context.EquipLight:
                    frame = 17;
                    break;
            }

            if(frame == -1) return new CroppedTexture2D();

            Texture2D extraTextures = Main.extraTexture[54];
            Rectangle rectangle = extraTextures.Frame(3, 6, frame % 3, frame % 3);
            rectangle.Width -= 2;
            rectangle.Height -= 2;

            return new CroppedTexture2D(extraTextures, rectangle);
        }

        /// <summary>
        /// Whether the slot has a visibility toggle button.
        /// </summary>
        internal static bool HasToggleButton(int context) {
            return context == ItemSlot.Context.EquipAccessory ||
                   context == ItemSlot.Context.EquipLight ||
                   context == ItemSlot.Context.EquipPet;
        }
    }
}
