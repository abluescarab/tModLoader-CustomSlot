using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CustomSlotter {
    public struct CroppedTexture2D {
        public Texture2D Texture { get; }
        public Rectangle Rectangle { get; set; }

        public static readonly CroppedTexture2D Empty = new CroppedTexture2D();

        public CroppedTexture2D(Texture2D texture) : this(texture, texture.Bounds) { }

        public CroppedTexture2D(Texture2D texture, Rectangle rectangle) {
            Texture = texture;
            Rectangle = rectangle;
        }

        public static bool operator ==(CroppedTexture2D ct1, CroppedTexture2D ct2) {
            return ct1.Texture == ct2.Texture && ct1.Rectangle == ct2.Rectangle;
        }

        public static bool operator !=(CroppedTexture2D ct1, CroppedTexture2D ct2) {
            return ct1.Texture != ct2.Texture || ct1.Rectangle != ct2.Rectangle;
        }
    }
}
