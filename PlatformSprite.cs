using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PageFinder1
{
    class PlatformSprite : Sprite
    {
        public PlatformSprite(Texture2D newSpriteSheet, Texture2D newCollisionTxr, Vector2 newLocation)
            : base(newSpriteSheet, newCollisionTxr, newLocation)
        {
            spriteOrigin = new Vector2(0.5f, 0f);
            isColliding = true;
            frameTime = 1f;

            //platformsprite animations
            animations = new List<List<Rectangle>>();
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 96, 32));
        }
    }
}
