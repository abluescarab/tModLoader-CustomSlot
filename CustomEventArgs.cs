using System;
using Terraria;

namespace CustomSlot {
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
