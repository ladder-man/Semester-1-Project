using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace jumpThing2
{
    class PageSprite : Sprite
{
        public PageSprite(Texture2D newSpriteSheet, Texture2D newCollisionTxr, Vector2 newLocation)
            : base(newSpriteSheet, newCollisionTxr, newLocation)
        {
            spriteOrigin = new Vector2(0.5f, 0.5f);
            isColliding = true;
            frameTime = 1f;

            animations = new List<List<Rectangle>>();
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(48, 48, 48, 48));
            animations[0].Add(new Rectangle(96, 48, 48, 48));
            animations[0].Add(new Rectangle(144, 48, 48, 48));
            animations[0].Add(new Rectangle(96, 48, 48, 48));
        }
}
}
