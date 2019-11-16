﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

/*
 * TODO: fix number text
 * TODO: tick correct location
 * TODO: equip/unequip
 * TODO: partner slot
 */

namespace CustomSlot {
    public class CustomItemSlot : UIElement, ICustomSlot {
        public enum ArmorType {
            HeadArmor,
            ChestArmor,
            LegArmor
        }

        internal const int TickOffsetX = 3;
        internal const int TickOffsetY = 2;

        private readonly SlotInterior interior;
        private ToggleVisibilityButton toggleButton;
        private bool forceToggleButton;

        public bool ItemVisible { get; set; }

        public int Context => interior.Context;

        public CroppedTexture2D BackgroundTexture {
            get => interior.BackgroundTexture;
            set {
                interior.BackgroundTexture = value;
                CalculateSize(this, TickOffsetX, TickOffsetY);
            }
        }

        public CroppedTexture2D EmptyTexture {
            get => interior.EmptyTexture;
            set => interior.EmptyTexture = value;
        }

        public Item Item {
            get => interior.Item;
            set => interior.Item = value;
        }

        public Func<Item, bool> IsValidItem {
            get => interior.IsValidItem;
            set => interior.IsValidItem = value;
        }

        public float Scale {
            get => interior.Scale;
            set {
                interior.Scale = value;
                CalculateSize(this, TickOffsetX, TickOffsetY);
            }
        }

        public bool ForceToggleButton {
            get => forceToggleButton;
            set {
                forceToggleButton = value;
                bool hasButton = ForceToggleButton || HasToggleButton(Context);

                if(!hasButton) {
                    if(toggleButton == null) return;

                    RemoveChild(toggleButton);
                    toggleButton = null;
                }
                else {
                    toggleButton = new ToggleVisibilityButton();
                    Append(toggleButton);
                }
            }
        }

        public CustomItemSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f,
            ArmorType defaultArmorIcon = ArmorType.HeadArmor) {
            Texture2D backgroundTexture = GetBackgroundTexture(context);

            interior = new SlotInterior(
                context,
                scale,
                null,
                new CroppedTexture2D(backgroundTexture),
                GetEmptyTexture(context, defaultArmorIcon));

            Append(interior);

            ItemVisible = true;
            ForceToggleButton = false;

            CalculateSize(this, TickOffsetX, TickOffsetY);
        }

        internal static void CalculateSize(ICustomSlot slot, int offsetX, int offsetY) {
            if(slot.BackgroundTexture == CroppedTexture2D.Empty) return;

            float width = (slot.BackgroundTexture.Texture.Width * slot.Scale) + offsetX;
            float height = (slot.BackgroundTexture.Texture.Height * slot.Scale) + offsetY;

            UIElement element = (UIElement)slot;
            element.Width.Set(width, 0f);
            element.Height.Set(height, 0f);
        }

        internal class SlotInterior : UIElement, ICustomSlot {
            private Item item;
            private CroppedTexture2D backgroundTexture;
            private float scale;

            public int Context { get; }
            public Func<Item, bool> IsValidItem { get; set; }
            public CroppedTexture2D EmptyTexture { get; set; }

            public Item Item {
                get => item;
                set => item = value;
            }

            public CroppedTexture2D BackgroundTexture {
                get => backgroundTexture;
                set {
                    backgroundTexture = value;
                    CalculateSize(this, 0, 0);
                }
            }

            public float Scale {
                get => scale;
                set {
                    scale = value;
                    CalculateSize(this, 0, 0);
                }
            }

            internal SlotInterior(int context, float scale, Func<Item, bool> isValidItem,
                CroppedTexture2D backgroundTexture, CroppedTexture2D emptyTexture) {
                Context = context;
                Scale = scale;
                IsValidItem = isValidItem;
                BackgroundTexture = backgroundTexture;
                EmptyTexture = emptyTexture;
                Item = new Item();
                Item.SetDefaults();
            }

            public override void Update(GameTime gameTime) {
                base.Update(gameTime);

                if(ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface) {
                    Main.LocalPlayer.mouseInterface = true;

                    CustomItemSlot parent = (CustomItemSlot)Parent;

                    if(parent.toggleButton != null && parent.toggleButton.ContainsPoint(Main.MouseScreen)) return;

                    if(IsValidItem == null || IsValidItem(Main.mouseItem)) {
                        ItemSlot.Handle(ref item, Context);
                    }
                }
            }

            protected override void DrawSelf(SpriteBatch spriteBatch) {
                //Rectangle rectangle = GetDimensions().ToRectangle();
                Rectangle parentRectangle = Parent.GetDimensions().ToRectangle();

                spriteBatch.Draw(
                    BackgroundTexture.Texture,
                    parentRectangle.TopLeft() + new Vector2(0, TickOffsetY),
                    BackgroundTexture.Rectangle,
                    Color.White * 0.8f,
                    0f,
                    Vector2.Zero,
                    Scale,
                    SpriteEffects.None,
                    1f);

                DrawForeground(spriteBatch);

                //spriteBatch.Draw(
                //    BackgroundTexture.Texture,
                //    rectangle.TopLeft(),
                //    BackgroundTexture.Rectangle,
                //    Color.White * 0.8f,
                //    0f,
                //    Vector2.Zero,
                //    Scale,
                //    SpriteEffects.None,
                //    1f);

                //DrawForeground(spriteBatch, rectangle);
            }

            protected void DrawForeground(SpriteBatch spriteBatch) {
                Rectangle rectangle = GetDimensions().ToRectangle();

                if(Item.stack <= 0) {
                    ModContent.GetInstance<WingSlot.WingSlot>().Logger.Info(rectangle.Size());
                    // TODO: fix empty icon drawing
                    spriteBatch.Draw(
                        EmptyTexture.Texture,
                        rectangle.Center(),
                        EmptyTexture.Rectangle,
                        Color.White * 0.35f,
                        0f,
                        Vector2.Zero,
                        Scale,
                        SpriteEffects.None,
                        0f);
                }
                else {
                    Texture2D itemTexture = Main.itemTexture[Item.type];
                    Rectangle itemRectangle = Main.itemAnimations[Item.type] != null
                        ? Main.itemAnimations[Item.type].GetFrame(itemTexture) : itemTexture.Frame();

                    spriteBatch.Draw(
                        itemTexture,
                        rectangle.Center(),
                        itemRectangle,
                        Color.White,
                        0f,
                        itemRectangle.Center(),
                        Scale,
                        SpriteEffects.None,
                        0f);
                }
            }
        }

        internal class ToggleVisibilityButton : UIElement {
            internal ToggleVisibilityButton() {
                Width.Set(Main.inventoryTickOnTexture.Width, 0f);
                Height.Set(Main.inventoryTickOnTexture.Height, 0f);

                OnClick += (evt, element) => {
                    if(!(element.Parent is CustomItemSlot slot)) return;

                    slot.ItemVisible = !slot.ItemVisible;
                };
            }

            protected override void DrawSelf(SpriteBatch spriteBatch) {
                if(!(Parent is CustomItemSlot slot)) return;

                Rectangle parentRectangle = Parent.GetDimensions().ToRectangle();
                Texture2D tickTexture =
                    slot.ItemVisible ? Main.inventoryTickOnTexture : Main.inventoryTickOffTexture;

                Left.Set(parentRectangle.Width - Width.Pixels + TickOffsetX, 0f);

                spriteBatch.Draw(
                    tickTexture,
                    new Vector2(parentRectangle.Right - tickTexture.Width + TickOffsetX, parentRectangle.Top),
                    //tickRectangle,
                    Color.White * 0.7f);
            }

            public override void Update(GameTime gameTime) {
                base.Update(gameTime);

                if(ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface) {
                    Main.LocalPlayer.mouseInterface = true;
                }
            }
        }

        /// <summary>
        /// Get the background texture of a slot based on its context.
        /// </summary>
        /// <param name="context">slot context</param>
        /// <returns>background texture of the slot</returns>
        public static Texture2D GetBackgroundTexture(int context) {
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
        public static CroppedTexture2D GetEmptyTexture(int context, CustomItemSlot.ArmorType armorType = CustomItemSlot.ArmorType.HeadArmor) {
            int frame = -1;

            switch(context) {
                case ItemSlot.Context.EquipArmor:
                    switch(armorType) {
                        case CustomItemSlot.ArmorType.HeadArmor:
                            frame = 0;
                            break;
                        case CustomItemSlot.ArmorType.ChestArmor:
                            frame = 6;
                            break;
                        case CustomItemSlot.ArmorType.LegArmor:
                            frame = 12;
                            break;
                    }
                    break;
                case ItemSlot.Context.EquipArmorVanity:
                    switch(armorType) {
                        case CustomItemSlot.ArmorType.HeadArmor:
                            frame = 3;
                            break;
                        case CustomItemSlot.ArmorType.ChestArmor:
                            frame = 9;
                            break;
                        case CustomItemSlot.ArmorType.LegArmor:
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
            Rectangle rectangle = extraTextures.Frame(3, 6, frame % 3, frame / 3);
            rectangle.Width -= 2;
            rectangle.Height -= 2;

            return new CroppedTexture2D(extraTextures, rectangle);
        }

        /// <summary>
        /// Whether the slot has a visibility toggle button.
        /// </summary>
        public static bool HasToggleButton(int context) {
            return context == ItemSlot.Context.EquipAccessory ||
                   context == ItemSlot.Context.EquipLight ||
                   context == ItemSlot.Context.EquipPet;
        }
    }
}

//private void Draw(SpriteBatch spriteBatch, Rectangle rectangle) {
//    spriteBatch.Draw(
//        _backTexture,
//        rectangle.TopLeft(),
//        null,
//        Color.White * 0.8f,
//        0f,
//        Vector2.Zero,
//        Scale,
//        SpriteEffects.None,
//        1f);

//    if(Item.stack > 0) {
//        Texture2D itemTex = Main.itemTexture[Item.type];
//        Rectangle itemRect = Main.itemAnimations[Item.type] != null ?
//            Main.itemAnimations[Item.type].GetFrame(itemTex) :
//            itemTex.Frame(1, 1, 0, 0);

//        Vector2 position = new Rectangle(itemRect.X, itemRect.Y, itemRect.Width, itemRect.Height).Center.ToVector2();

//        spriteBatch.Draw(
//            itemTex,
//            position,
//            itemRect,
//            Color.White,
//            0f,
//            itemRect.Size() / 2f,
//            Scale,
//            SpriteEffects.None,
//            0f);

//        // TODO: fix number text
//        ChatManager.DrawColorCodedStringWithShadow(
//            spriteBatch,
//            Main.fontItemStack,
//            Item.stack.ToString(),
//            position + new Vector2(10f, 26f) * Scale,
//            Color.White,
//            0f,
//            Vector2.Zero,
//            new Vector2(Scale),
//            -1f,
//            Scale);
//    }
//    else if(Item.stack == 0 && ValidItemTexture != null) {
//        spriteBatch.Draw(
//            ValidItemTexture,
//            rectangle.TopLeft() + (rectangle.Size() / 2f),
//            null,
//            Color.White * 0.35f,
//            0f,
//            ValidItemTexture.Size() / 2f,
//            Scale,
//            SpriteEffects.None,
//            0f); // layer depth 0 = front
//    }
//}