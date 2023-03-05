using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace CustomSlot.UI {
    public class AccessorySlotsUI : UIState {
        /// <summary>
        /// The horizontal margin between slots.
        /// Default: 3 (from game source code)
        /// </summary>
        protected const int HorizontalSlotMargin = 3;
        /// <summary>
        /// The current panel coordinates.
        /// </summary>
        public Vector2 PanelCoordinates => new Vector2(Panel.Left.Pixels, Panel.Top.Pixels);
        /// <summary>
        /// The default location of the panel.
        /// </summary>
        public Vector2 DefaultCoordinates => GetDefaultPosition();
        /// <summary>
        /// The panel holding the item slots.
        /// </summary>
        public DraggableUIPanel Panel { get; protected set; }
        /// <summary>
        /// Whether the UI is visible or not.
        /// </summary>
        public virtual bool IsVisible => Main.playerInventory;

        public override void OnInitialize() {
            float slotSize = new CustomItemSlot().Width.Pixels;

            Panel = new DraggableUIPanel();
            Panel.Width.Set((slotSize * 3) + (HorizontalSlotMargin * 2), 0);
            Panel.Height.Set(slotSize + Panel.PaddingBottom + HorizontalSlotMargin, 0);

            Append(Panel);
        }

        protected virtual Vector2 GetDefaultPosition() {
            return new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
        }

        public virtual void ResetPosition() {
            Vector2 pos = GetDefaultPosition();

            Panel.Left.Set(pos.X, 0);
            Panel.Top.Set(pos.Y, 0);
        }
    }
}
