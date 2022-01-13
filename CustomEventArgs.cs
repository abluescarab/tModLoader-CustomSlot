using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
