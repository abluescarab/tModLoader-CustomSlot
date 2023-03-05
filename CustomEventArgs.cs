using CustomSlot.UI;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace CustomSlot {
    public delegate void ItemChangedEventHandler(CustomItemSlot slot, ItemChangedEventArgs e);
    public delegate void ItemVisiblityChangedEventHandler(CustomItemSlot slot, ItemVisibilityChangedEventArgs e);
    public delegate void PanelDragEventHandler(UIPanel sender, PanelDragEventArgs e);

    public class PanelDragEventArgs : EventArgs {
        public readonly StyleDimension X;
        public readonly StyleDimension Y;

        public PanelDragEventArgs(StyleDimension x, StyleDimension y) {
            X = x;
            Y = y;
        }
    }

    public class ItemChangedEventArgs : EventArgs {
        public readonly Item OldItem;
        public readonly Item NewItem;

        public ItemChangedEventArgs(Item oldItem, Item newItem) {
            OldItem = oldItem;
            NewItem = newItem;
        }
    }

    public class ItemVisibilityChangedEventArgs : EventArgs {
        public readonly bool Visibility;

        public ItemVisibilityChangedEventArgs(bool visibility) {
            Visibility = visibility;
        }
    }
}
