using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nopipeline.Sample
{
  public class LawnItem
  {
    public readonly Texture2D Texture;
    public readonly Vector2 Position;
    public readonly SpriteEffects FlipFlags;

    public LawnItem(Texture2D texture, Vector2 position, SpriteEffects flipFlags)
    {
	 Texture = texture;
	 Position = position;
	 FlipFlags = flipFlags;
    }

    public void Draw()
    {
	 Game1.SpriteBatch.Draw(
		 Texture,
		 Position,
		 null,
		 Color.White,
		 0,
		 Vector2.Zero,
		 1,
		 FlipFlags,
		 (Position.Y + Texture.Height) / 500f
	 );
    }
  }
}
