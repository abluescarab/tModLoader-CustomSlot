using System;
using Terraria;

namespace CustomSlot {
    public class ItemChangedEventArgs : EventArgs {
        public readonly Item Item;

        public ItemChangedEventArgs(Item item) {
            Item = item;
        }
    }

    public class ItemVisibilityChangedEventArgs : EventArgs {
        public readonly bool Visibility;

        public ItemVisibilityChangedEventArgs(bool visibility) {
            Visibility = visibility;
        }
    }
}
