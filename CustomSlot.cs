using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;

/*
 * TODO: fix number text
 * TODO: tick correct location
 * TODO: equip/unequip
 * TODO: partner slot
 */

namespace CustomSlotter {
    public class CustomSlot : UIElement {
        public enum ArmorType {
            HeadArmor,
            ChestArmor,
            LegArmor
        }

        private readonly SlotInterior _interior;
        private ToggleButton _toggleButton;
        private bool _forceToggleButton;

        public bool ItemVisible { get; set; }

        public CroppedTexture2D BackgroundTexture => _interior.BackgroundTexture;
        public CroppedTexture2D EmptyTexture => _interior.EmptyTexture;
        public int Context => _interior.Context;

        public Item Item {
            get => _interior.Item;
            set => _interior.Item = value;
        }

        public Func<Item, bool> IsValidItem {
            get => _interior.IsValidItem;
            set => _interior.IsValidItem = value;
        }

        public float Scale {
            get => _interior.Scale;
            set => _interior.Scale = value;
        }

        public bool ForceToggleButton {
            get => _forceToggleButton;
            set {
                _forceToggleButton = value;
                bool hasButton = ForceToggleButton || UIUtils.HasToggleButton(Context);

                if(!hasButton) {
                    if(_toggleButton == null) return;

                    RemoveChild(_toggleButton);
                    _toggleButton = null;
                }
                else {
                    _toggleButton = new ToggleButton();
                    Append(_toggleButton);
                }
            }
        }

        public CustomSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f,
            ArmorType defaultArmorIcon = ArmorType.HeadArmor) {
            _interior = new SlotInterior(
                scale,
                context,
                null,
                new CroppedTexture2D(UIUtils.GetBackgroundTexture(context)),
                UIUtils.GetEmptyTexture(context, defaultArmorIcon));
        }

        internal class SlotInterior : UIElement {
            private Item _item;
            private float _scale;

            internal int Context { get; }
            internal Func<Item, bool> IsValidItem { get; set; }
            internal CroppedTexture2D BackgroundTexture { get; }
            internal CroppedTexture2D EmptyTexture { get; }

            internal Item Item {
                get => _item;
                set => _item = value;
            }

            internal float Scale {
                get => _scale;
                set {
                    _scale = value;
                    CalculateSize();
                }
            }

            internal SlotInterior(float scale, int context, Func<Item, bool> isValidItem,
                CroppedTexture2D backgroundTexture, CroppedTexture2D emptyTexture) {
                Scale = scale;
                Context = context;
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

                    CustomSlot parent = (CustomSlot)Parent;

                    if(IsValidItem == null || IsValidItem(Main.mouseItem)) {
                        ItemSlot.Handle(ref _item, Context);
                    }
                }
            }

            protected override void DrawSelf(SpriteBatch spriteBatch) {
                Rectangle rectangle = GetDimensions().ToRectangle();

                spriteBatch.Draw(
                    BackgroundTexture.Texture,
                    rectangle.TopLeft(),
                    BackgroundTexture.Rectangle,
                    Color.White * 0.8f,
                    0f,
                    Vector2.Zero,
                    Scale,
                    SpriteEffects.None,
                    1f);

                DrawForeground(spriteBatch, rectangle);
            }

            protected void DrawForeground(SpriteBatch spriteBatch, Rectangle rectangle) {
                if(Item.stack <= 0) {
                    spriteBatch.Draw(
                        EmptyTexture.Texture,
                        rectangle.Center(),
                        EmptyTexture.Rectangle,
                        Color.White * 0.35f,
                        0f,
                        EmptyTexture.Rectangle.Center(),
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

            /// <summary>
            /// Calculate the size of the slot based on its background texture and scale.
            /// </summary>
            internal void CalculateSize() {
                if(BackgroundTexture == CroppedTexture2D.Empty) return;

                Width.Set(BackgroundTexture.Texture.Width * Scale, 0f);
                Height.Set(BackgroundTexture.Texture.Height * Scale, 0f);
            }
        }

        internal class ToggleButton : UIElement {
            internal ToggleButton() {
                Width.Set(Main.inventoryTickOnTexture.Width, 0f);
                Height.Set(Main.inventoryTickOnTexture.Height, 0f);

                OnClick += (evt, element) => {
                    if(!(element.Parent is CustomSlot slot)) return;

                    slot.ItemVisible = !slot.ItemVisible;
                };
            }

            protected override void DrawSelf(SpriteBatch spriteBatch) {
                if(!(Parent is CustomSlot slot)) return;

                Rectangle parentRectangle = Parent.GetDimensions().ToRectangle();
                Texture2D tickTexture =
                    slot.ItemVisible ? Main.inventoryTickOnTexture : Main.inventoryTickOffTexture;
                //int x = (int)(parentRectangle.Right - 2 - (tickTexture.Width / 2f * slot.Scale));

                Left.Set(parentRectangle.Width * slot.Scale, 0f);
                //Left.Set(parentRectangle.Width - 2 - (tickTexture.Width / 2f * slot.Scale), 0f);
                //Rectangle tickRectangle = new Rectangle(
                //    (int)(parentRectangle.Right - 2 - (tickTexture.Width / 2f * slot.Scale)),
                //    parentRectangle.Top - 2,
                //    tickTexture.Width,
                //    tickTexture.Height);

                //Left.Set(parentRectangle.Width + 2, 0f);
                //spriteBatch.Draw(tickTexture, tickRectangle, Color.White * 0.7f);
                spriteBatch.Draw(
                    tickTexture,
                    new Vector2(
                        parentRectangle.Right - 2 - (tickTexture.Width / 2f * slot.Scale),
                        parentRectangle.Top - 2),
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
    }
}




//using System;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;
//using Terraria.GameInput;
//using Terraria.UI;


//namespace CustomSlotter {
//    public class CustomSlot : UIElement {
//        public enum ArmorType {
//            HeadArmor,
//            ChestArmor,
//            LegArmor
//        }

//        private SlotInterior _interior;
//        private ToggleButton _toggleButton;
//        private float _scale;
//        private CroppedTexture2D _backgroundTexture;
//        private CroppedTexture2D _emptyTexture;
//        private bool _forceToggleButton;
//        private Item _item;
//        private int _context;
//        private Func<Item, bool> _isValidItem;
//        private bool _itemVisible;

//        public CustomSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f, ArmorType defaultArmorIcon = ArmorType.HeadArmor) {
//            _interior = new SlotInterior();

//            _context = context;
//            _scale = scale;
//            _forceToggleButton = false;
//            _isValidItem = null;
//            _itemVisible = true;
//            _backgroundTexture = new CroppedTexture2D(UIUtils.GetBackgroundTexture(context));
//            _emptyTexture = UIUtils.GetEmptyTexture(context, defaultArmorIcon);
//            _item = new Item();

//            _item.SetDefaults();
//        }

//        internal class SlotInterior : UIElement {
//            public override void Update(GameTime gameTime) {
//                base.Update(gameTime);

//                if(ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface) {
//                    Main.LocalPlayer.mouseInterface = true;

//                    CustomSlot parent = (CustomSlot)Parent;

//                    if(parent._isValidItem == null || IsValidItem(Main.mouseItem)) {
//                        ItemSlot.Handle(ref _item, Context);
//                    }
//                }
//            }

//            protected override void DrawSelf(SpriteBatch spriteBatch) {
//                Rectangle rectangle = GetDimensions().ToRectangle();

//                spriteBatch.Draw(
//                    BackgroundTexture.Texture,
//                    rectangle.TopLeft(),
//                    BackgroundTexture.Rectangle,
//                    Color.White * 0.8f,
//                    0f,
//                    Vector2.Zero,
//                    Scale,
//                    SpriteEffects.None,
//                    1f);

//                DrawForeground(spriteBatch, rectangle);
//            }

//            protected void DrawForeground(SpriteBatch spriteBatch, Rectangle rectangle) {
//                if(Item.stack <= 0) {
//                    spriteBatch.Draw(
//                        EmptyTexture.Texture,
//                        rectangle.Center(),
//                        EmptyTexture.Rectangle,
//                        Color.White * 0.35f,
//                        0f,
//                        EmptyTexture.Rectangle.Center(),
//                        Scale,
//                        SpriteEffects.None,
//                        0f);
//                }
//                else {
//                    Texture2D itemTexture = Main.itemTexture[Item.type];
//                    Rectangle itemRectangle = Main.itemAnimations[Item.type] != null
//                        ? Main.itemAnimations[Item.type].GetFrame(itemTexture) : itemTexture.Frame();

//                    spriteBatch.Draw(
//                        itemTexture,
//                        rectangle.Center(),
//                        itemRectangle,
//                        Color.White,
//                        0f,
//                        itemRectangle.Center(),
//                        Scale,
//                        SpriteEffects.None,
//                        0f);
//                }
//            }

//            /// <summary>
//            /// Calculate the size of the slot based on its background texture and scale.
//            /// </summary>
//            internal void CalculateSize() {
//                if((Parent as CustomSlot).BackgroundTexture == CroppedTexture2D.Empty) return;

//                Width.Set(BackgroundTexture.Texture.Width * Scale, 0f);
//                Height.Set(BackgroundTexture.Texture.Height * Scale, 0f);
//            }
//        }

//        internal class ToggleButton : UIElement {

//        }

//        //public bool ForceToggleButton {
//        //    get => _forceToggleButton;
//        //    set {
//        //        _forceToggleButton = value;
//        //        bool hasButton = ForceToggleButton || UIUtils.HasToggleButton(_slotInterior.Context);

//        //        if(!hasButton) {
//        //            if(_toggleButton == null) return;

//        //            RemoveChild(_toggleButton);
//        //            _toggleButton = null;
//        //        }
//        //        else {
//        //            _toggleButton = new ToggleButton();
//        //            Append(_toggleButton);
//        //        }
//        //    }
//        //}

//        //internal CustomSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f, ArmorType armorIcon = ArmorType.HeadArmor) {
//        //    _slotInterior = new SlotInterior(context, new CroppedTexture2D(UIUtils.GetBackgroundTexture(context)), UIUtils.GetEmptyTexture(context, armorIcon));
//        //}

//        //internal class SlotInterior : UIElement {
//        //    private Item _item;

//        //    internal Item Item => _item;
//        //    internal int Context { get; }
//        //    internal CroppedTexture2D BackgroundTexture { get; }
//        //    internal CroppedTexture2D EmptyTexture { get; }
//        //    internal Func<Item, bool> IsValidItem { get; set; }

//        //    internal SlotInterior(int context, CroppedTexture2D backgroundTexture, CroppedTexture2D emptyTexture, Func<Item, bool> isValidItem) {
//        //        Context = context;
//        //        BackgroundTexture = backgroundTexture;
//        //        EmptyTexture = emptyTexture;
//        //        IsValidItem = isValidItem;
//        //    }
//        //}

//        //internal class ToggleButton : UIElement {
//        //    private bool _visible;
//        //}

//        //private ToggleButton _toggleButton;
//        //private SlotInterior _interior;
//        //private Item _item;
//        //private bool _forceToggleButton;
//        //private float _scale;

//        ///// <summary>
//        ///// If the slot is for equipment, whether the item in the slot is visible.
//        ///// </summary>
//        //public bool ItemVisible { get; set; }
//        ///// <summary>
//        ///// The item in the slot.
//        ///// </summary>
//        //public Item Item => _item;
//        ///// <summary>
//        ///// The <see cref="ItemSlot"/>.Context of the slot.
//        ///// </summary>
//        //public int Context { get; }
//        ///// <summary>
//        ///// The scale of the slot.
//        ///// </summary>
//        //public float Scale {
//        //    get => _scale;
//        //    set {
//        //        _scale = value;
//        //        _interior.CalculateSize();
//        //    }
//        //}
//        ///// <summary>
//        ///// The function to check if the item is valid before placing it in the slot.
//        ///// </summary>
//        //public Func<Item, bool> IsValidItem { get; }
//        ///// <summary>
//        ///// Whether to force a visibility toggle button on the slot.
//        ///// </summary>
//        //public bool ForceToggleButton {
//        //    get => _forceToggleButton;
//        //    set {
//        //        _forceToggleButton = value;
//        //        bool hasButton = ForceToggleButton || UIUtils.HasToggleButton(Context);

//        //        if(!hasButton) {
//        //            if(_toggleButton == null) return;

//        //            RemoveChild(_toggleButton);
//        //            _toggleButton = null;
//        //        }
//        //        else {
//        //            _toggleButton = new ToggleButton();
//        //            Append(_toggleButton);
//        //        }
//        //    }
//        //}

//        //public CustomSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f,
//        //    ArmorType armorIcon = ArmorType.HeadArmor) {
//        //    _interior = new SlotInterior() {
//        //        BackgroundTexture = new CroppedTexture2D(UIUtils.GetBackgroundTexture(context)),
//        //        EmptyTexture = UIUtils.GetEmptyTexture(context, armorIcon)
//        //    };

//        //    ItemVisible = true;
//        //    ForceToggleButton = false;
//        //    _scale = scale;
//        //    _item = new Item();
//        //    _item.SetDefaults();
//        //    _interior.CalculateSize();
//        //}

//        //internal class SlotInterior : UIElement {
//        //    private CroppedTexture2D _backgroundTexture;

//        //    /// <summary>
//        //    /// The grayed-out texture displayed in the empty slot.
//        //    /// </summary>
//        //    public CroppedTexture2D EmptyTexture { get; set; }
//        //    /// <summary>
//        //    /// The background texture of the slot, determined by its context by default.
//        //    /// </summary>
//        //    public CroppedTexture2D BackgroundTexture {
//        //        get => _backgroundTexture;
//        //        set {
//        //            _backgroundTexture = value;
//        //            CalculateSize();
//        //        }
//        //    }

//        //    //            public override void Update(GameTime gameTime) {
//        //    //                base.Update(gameTime);

//        //    //                if(ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface) {
//        //    //                    Main.LocalPlayer.mouseInterface = true;

//        //    //                    if(IsValidItem == null || IsValidItem(Main.mouseItem)) {
//        //    //                        ItemSlot.Handle(ref _item, Context);
//        //    //                    }
//        //    //                }
//        //    //            }

//        //    //            protected override void DrawSelf(SpriteBatch spriteBatch) {
//        //    //                Rectangle rectangle = GetDimensions().ToRectangle();

//        //    //                spriteBatch.Draw(
//        //    //                    BackgroundTexture.Texture,
//        //    //                    rectangle.TopLeft(),
//        //    //                    BackgroundTexture.Rectangle,
//        //    //                    Color.White * 0.8f,
//        //    //                    0f,
//        //    //                    Vector2.Zero,
//        //    //                    Scale,
//        //    //                    SpriteEffects.None,
//        //    //                    1f);

//        //    //                DrawForeground(spriteBatch, rectangle);
//        //    //            }

//        //    //            protected void DrawForeground(SpriteBatch spriteBatch, Rectangle rectangle) {
//        //    //                if(Item.stack <= 0) {
//        //    //                    spriteBatch.Draw(
//        //    //                        EmptyTexture.Texture,
//        //    //                        rectangle.Center(),
//        //    //                        EmptyTexture.Rectangle,
//        //    //                        Color.White * 0.35f,
//        //    //                        0f,
//        //    //                        EmptyTexture.Rectangle.Center(),
//        //    //                        Scale,
//        //    //                        SpriteEffects.None,
//        //    //                        0f);
//        //    //                }
//        //    //                else {
//        //    //                    Texture2D itemTexture = Main.itemTexture[Item.type];
//        //    //                    Rectangle itemRectangle = Main.itemAnimations[Item.type] != null
//        //    //                        ? Main.itemAnimations[Item.type].GetFrame(itemTexture) : itemTexture.Frame();

//        //    //                    spriteBatch.Draw(
//        //    //                        itemTexture,
//        //    //                        rectangle.Center(),
//        //    //                        itemRectangle,
//        //    //                        Color.White,
//        //    //                        0f,
//        //    //                        itemRectangle.Center(),
//        //    //                        Scale,
//        //    //                        SpriteEffects.None,
//        //    //                        0f);
//        //    //                }
//        //    //            }

//        //    /// <summary>
//        //    /// Calculate the size of the slot based on its background texture and scale.
//        //    /// </summary>
//        //    internal void CalculateSize() {
//        //        if((Parent as CustomSlot).BackgroundTexture == CroppedTexture2D.Empty) return;

//        //        Width.Set(BackgroundTexture.Texture.Width * Scale, 0f);
//        //        Height.Set(BackgroundTexture.Texture.Height * Scale, 0f);
//        //    }
//        //}

//        //internal class ToggleButton : UIElement {
//        //    internal ToggleButton() {
//        //        Width.Set(Main.inventoryTickOnTexture.Width, 0f);
//        //        Height.Set(Main.inventoryTickOnTexture.Height, 0f);
//        //        //Top.Set(-2, 0f);

//        //        //ModContent.GetInstance<Mod>().Logger.Info(Main.inventoryTickOnTexture.Width);

//        //        OnClick += (evt, element) => {
//        //            if(!(element.Parent is CustomSlot slot)) return;

//        //            slot.ItemVisible = !slot.ItemVisible;
//        //        };
//        //    }

//        //    protected override void DrawSelf(SpriteBatch spriteBatch) {
//        //        if(!(Parent is CustomSlot slot)) return;

//        //        Rectangle parentRectangle = Parent.GetDimensions().ToRectangle();
//        //        Texture2D tickTexture =
//        //            slot.ItemVisible ? Main.inventoryTickOnTexture : Main.inventoryTickOffTexture;
//        //        //int x = (int)(parentRectangle.Right - 2 - (tickTexture.Width / 2f * slot.Scale));

//        //        Left.Set(parentRectangle.Width * slot.Scale, 0f);
//        //        //Left.Set(parentRectangle.Width - 2 - (tickTexture.Width / 2f * slot.Scale), 0f);
//        //        //Rectangle tickRectangle = new Rectangle(
//        //        //    (int)(parentRectangle.Right - 2 - (tickTexture.Width / 2f * slot.Scale)),
//        //        //    parentRectangle.Top - 2,
//        //        //    tickTexture.Width,
//        //        //    tickTexture.Height);

//        //        //Left.Set(parentRectangle.Width + 2, 0f);
//        //        //spriteBatch.Draw(tickTexture, tickRectangle, Color.White * 0.7f);
//        //        spriteBatch.Draw(
//        //            tickTexture,
//        //            new Vector2(
//        //                parentRectangle.Right - 2 - (tickTexture.Width / 2f * slot.Scale),
//        //                parentRectangle.Top - 2),
//        //            //tickRectangle,
//        //            Color.White * 0.7f);
//        //    }

//        //    public override void Update(GameTime gameTime) {
//        //        base.Update(gameTime);

//        //        if(ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface) {
//        //            Main.LocalPlayer.mouseInterface = true;
//        //        }
//        //    }
//        //}
//        //    }

//        //    //        private void Draw(SpriteBatch spriteBatch, Rectangle rectangle) {
//        //    //            spriteBatch.Draw(
//        //    //                _backTexture,
//        //    //                rectangle.TopLeft(),
//        //    //                null,
//        //    //                Color.White * 0.8f,
//        //    //                0f,
//        //    //                Vector2.Zero,
//        //    //                Scale,
//        //    //                SpriteEffects.None,
//        //    //                1f);

//        //    //            if(Item.stack > 0) {
//        //    //                Texture2D itemTex = Main.itemTexture[Item.type];
//        //    //                Rectangle itemRect = Main.itemAnimations[Item.type] != null ?
//        //    //                    Main.itemAnimations[Item.type].GetFrame(itemTex) :
//        //    //                    itemTex.Frame(1, 1, 0, 0);

//        //    //                Vector2 position = new Rectangle(itemRect.X, itemRect.Y, itemRect.Width, itemRect.Height).Center.ToVector2();

//        //    //                spriteBatch.Draw(
//        //    //                    itemTex,
//        //    //                    position,
//        //    //                    itemRect,
//        //    //                    Color.White,
//        //    //                    0f,
//        //    //                    itemRect.Size() / 2f,
//        //    //                    Scale,
//        //    //                    SpriteEffects.None,
//        //    //                    0f);

//        //    //                // TODO: fix number text
//        //    //                ChatManager.DrawColorCodedStringWithShadow(
//        //    //                    spriteBatch,
//        //    //                    Main.fontItemStack,
//        //    //                    Item.stack.ToString(),
//        //    //                    position + new Vector2(10f, 26f) * Scale,
//        //    //                    Color.White,
//        //    //                    0f,
//        //    //                    Vector2.Zero,
//        //    //                    new Vector2(Scale),
//        //    //                    -1f,
//        //    //                    Scale);
//        //    //            }
//        //    //            else if(Item.stack == 0 && ValidItemTexture != null) {
//        //    //                spriteBatch.Draw(
//        //    //                    ValidItemTexture,
//        //    //                    rectangle.TopLeft() + (rectangle.Size() / 2f),
//        //    //                    null,
//        //    //                    Color.White * 0.35f,
//        //    //                    0f,
//        //    //                    ValidItemTexture.Size() / 2f,
//        //    //                    Scale,
//        //    //                    SpriteEffects.None,
//        //    //                    0f); // layer depth 0 = front
//        //    //            }
//        //    //        }

//        //    //        private bool HasTick() {
//        //    //            return _context == ItemSlot.Context.EquipAccessory ||
//        //    //                   _context == ItemSlot.Context.EquipLight ||
//        //    //                   _context == ItemSlot.Context.EquipPet;
//        //    //        }
//    }
//}
