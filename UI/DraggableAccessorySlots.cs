using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.UI;

namespace CustomSlot.UI {
    [Autoload(false)]
    public abstract class DraggableAccessorySlots : ModAccessorySlot {
        /// <summary>
        /// The UI to draw the slots on top of.
        /// </summary>
        public abstract AccessorySlotsUI UI { get; }
        /// <summary>
        /// Whether to use the <see cref="CustomLocation"/> property.
        /// </summary>
        public abstract bool UseCustomLocation { get; }

        public override Vector2? CustomLocation {
            get {
                if(UI != null && UseCustomLocation) {
                    CalculatedStyle innerDims = UI.Panel.GetInnerDimensions();

                    return new Vector2(
                        innerDims.X + (innerDims.Width * 0.66f),
                        innerDims.Y);
                }

                return base.CustomLocation;
            }
        }
    }
}
