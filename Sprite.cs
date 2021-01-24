using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace jumpThing2
{
    class Sprite
    {

        Texture2D spriteSheet, collisionTexture;                // visible texture sheet, texture used to draw collision boxes

        public Vector2 spritePos, spriteVelocity,               // location of the sprite on-screen, velocity of the sprite in pixels per second
            spriteOrigin,                                       // where on the sprite is it's origin? (0f,0f) = top-left, (0.5f,1f) = bottom-middle, (0.5f,0.5f) = centre, etc
            spriteScale,                                        // should the sprite be resized? (1f,1f) = normal size, (2f,2f) = double size, (0.5f,1f) = half as wide, etc
            collisionInsetMin, collisionInsetMax;               // the extents of the collision box relative to the visible sprite, e.g. min = (0.25f,0.5f) would shave a quarter off on the left and half from the top

        public bool flipped,                                    // whether the sprite should appear flipped horizontally
            isDead,                                             // whether the sprite should be marked for deletion
            isColliding, drawCollision;                         // whether collision is active, whether the collision box will be drawn on screen

        int collPadding = 5;                                    // tweaks collision sensitivity for each edge, recommended = 5

        public List<List<Rectangle>> animations;                // a 2D table of animation frames, each stored as a rectangle to be selected from a sprite sheet
        public int currentAnim, currentFrame;                   // integers for tracking animation state
        public float frameTime, frameCounter;                   // floats for timing animation, frameTime is the duration of each frame in seconds


        //
        // Constructor
        //
        public Sprite(Texture2D newSpritesheet, Texture2D newCollisionTexture, Vector2 newLocation)
        {
            // assign the parameters to the member variables:
            spriteSheet = newSpritesheet;
            collisionTexture = newCollisionTexture;
            spritePos = newLocation;

            // assign some default values, will be overridden by child classes
            isColliding = false;
            drawCollision = false;
            isDead = false;
            flipped = false;
            spriteOrigin = new Vector2(0f, 0f);
            collisionInsetMin = new Vector2(0f, 0f);
            collisionInsetMax = new Vector2(0f, 0f);
            spriteScale = new Vector2(1f, 1f);
            currentAnim = 0;
            currentFrame = 0;
            frameTime = 0.5f;
            frameCounter = frameTime;

            // initialise animation lists and add a single frame a default animation
            animations = new List<List<Rectangle>>();
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 48, 48));

        }

        //
        // Update()
        // virtual method, to be overridden by child classes
        //
        public virtual void Update(GameTime gameTime) { }

        //
        // Draw()
        // advance animation and draws frame to spriteBatch
        //
        public void Draw(SpriteBatch spriteBatch, GameTime gametime)
        {
            // advance the animation timer and frames
            if (animations[currentAnim].Count > 1) // only advance animation if the current animation has multiple frames
            {
                frameCounter -= (float)gametime.ElapsedGameTime.TotalSeconds; // reduce the timer by whatever fraction of a second has passed since the last frame
                if (frameCounter <= 0)  // has the timer ran out for this frame?
                {
                    frameCounter = frameTime; // reset the timer
                    currentFrame++; // advance to the next frame
                }
                if (currentFrame >= animations[currentAnim].Count) currentFrame = 0; // if we have reached the end of the animation, start again from frame 0
            }

            // declare a SpriteEffects variable to flip the sprite if required by the flipped bool
            SpriteEffects drawEffect;
            if (flipped) drawEffect = SpriteEffects.FlipHorizontally;
            else drawEffect = SpriteEffects.None;

            // draw the visible sprite to the spriteBatch
            spriteBatch.Draw(
                spriteSheet,                            // using the spriteSheet that was assigned in the constructor
                GetRectangleForDraw(),                  // the DESTINATION rectangle is defined by the GetRectangleForDraw() method below
                animations[currentAnim][currentFrame],  // the SOURCE rectangle is the current frame of the current animation
                Color.White,                            // white = no tint
                0f,                                     // rotation of the sprite around the origin
                spriteOrigin,                           // the origin that was assigned in the constructor
                drawEffect,                             // the SpriteEffect used above to define whether flipped or not
                1f);                                    // draw-order, not used in this project

            // if required, draw a simple box over the collision area
            if (drawCollision) spriteBatch.Draw(
                collisionTexture,                       // using the collision texture that was assigned in the constructor
                GetRectangleForCollision(),             // the destination rectangle is defined by the same method that provides the shape for actual collision
                Color.Red);                             // tint red for visibility
        }

        //
        // SetAnim()
        // used to change which animation is playing
        //
        public void SetAnim(int newAnim)
        {
            if (currentAnim != newAnim && newAnim < animations.Count) // only if the new animation is not already playing
            {
                // change the animation int, and reset the counters
                currentAnim = newAnim;
                currentFrame = 0;
                frameCounter = frameTime;
            }
        }

        //
        //
        //
        // The following methods are all different ways of checking for collisions:
        // CheckCollision()
        // CheckCollisionBelow()
        // CheckCollisionAbove()
        // CheckCollisionLeft()
        // CheckCollisionRight()
        //
        //
        //

        //
        // CheckCollision()
        // compares this sprite with another and returns true if they interset
        // uses GetRectangleForCollision() to build the correct collision shape
        //
        public bool CheckCollision(Sprite otherSprite)
        {
            if (!isColliding || !otherSprite.isColliding) return false; // if either sprite is NOT set to collide, ignore collision check
            else return GetRectangleForCollision().Intersects(otherSprite.GetRectangleForCollision()); // get the GetRectangleForCollision() method of each sprite and return whether or not they intersect
        }

        //
        // CheckCollisionBelow()
        // a specific collision check that returns true if there is a collision caused by this sprite touching another one from below
        // uses GetRectangleForCollision() to build the correct collision shape, and the getEdge....ForCollision() methods to compare the edges of the collision shapes
        //
        public bool CheckCollisionBelow(Sprite otherSprite)
        {
            return CheckCollision(otherSprite) && GetEdgeBottomForCollision() < otherSprite.GetEdgeBottomForCollision() && GetEdgeBottomForCollision() > otherSprite.GetEdgeTopForCollision()
                && (GetEdgeLeftForCollision() + collPadding < otherSprite.GetEdgeRightForCollision() && GetEdgeRightForCollision() - collPadding > otherSprite.GetEdgeLeftForCollision());
        }

        //
        // CheckCollisionAbove()
        // a specific collision check that returns true if there is a collision caused by this sprite touching another one from above
        // uses GetRectangleForCollision() to build the correct collision shape, and the getEdge....ForCollision() methods to compare the edges of the collision shapes
        //
        public bool CheckCollisionAbove(Sprite otherSprite)
        {
            return CheckCollision(otherSprite) && GetEdgeTopForCollision() > otherSprite.GetEdgeTopForCollision() && GetEdgeTopForCollision() < otherSprite.GetEdgeBottomForCollision()
                && (GetEdgeLeftForCollision() + collPadding < otherSprite.GetEdgeRightForCollision() && GetEdgeRightForCollision() - collPadding > otherSprite.GetEdgeLeftForCollision());
        }

        //
        // CheckCollisionLeft()
        // a specific collision check that returns true if there is a collision caused by this sprite touching another one from the left
        // uses GetRectangleForCollision() to build the correct collision shape, and the getEdge....ForCollision() methods to compare the edges of the collision shapes
        //
        public bool CheckCollisionLeft(Sprite otherSprite)
        {
            return CheckCollision(otherSprite) && GetEdgeRightForCollision() < otherSprite.GetEdgeRightForCollision() && GetEdgeRightForCollision() > otherSprite.GetEdgeLeftForCollision()
                && (GetEdgeTopForCollision() + collPadding < otherSprite.GetEdgeBottomForCollision() && GetEdgeBottomForCollision() - collPadding > otherSprite.GetEdgeTopForCollision());
        }

        //
        // CheckCollisionRight()
        // a specific collision check that returns true if there is a collision caused by this sprite touching another one from the right
        // uses GetRectangleForCollision() to build the correct collision shape, and the getEdge....ForCollision() methods to compare the edges of the collision shapes
        //
        public bool CheckCollisionRight(Sprite otherSprite)
        {
            return CheckCollision(otherSprite) && GetEdgeLeftForCollision() > otherSprite.GetEdgeLeftForCollision() && GetEdgeLeftForCollision() < otherSprite.GetEdgeRightForCollision()
                && (GetEdgeTopForCollision() + collPadding < otherSprite.GetEdgeBottomForCollision() && GetEdgeBottomForCollision() - collPadding > otherSprite.GetEdgeTopForCollision());
        }

        //
        //
        //
        // The following methods are all used in calculating the desintation rectangle for DRAWING the visible sprite:
        // GetRectangleForDraw()
        // GetWidthForDraw()
        // GetEdgeLeftForDraw()
        // GetEdgeRightForDraw()
        // getHieghtForDraw()
        // GetEdgeTopForDraw()
        // GetEdgeBottomForDraw()
        //
        //
        //

        //
        // GetRectangleForDraw()
        // builds the rectangle that is used as the destination rectangle when drawing the sprite
        // seperates the calculations out into individual methods
        //
        public Rectangle GetRectangleForDraw()
        {
            return new Rectangle(GetEdgeLeftForDraw(), GetEdgeTopForDraw(), GetWidthForDraw(), GetHeightForDraw());
        }

        //
        // GetWidthForDraw()
        // finds the desired draw width of the visible sprite
        //
        public int GetWidthForDraw()
        {
            // draw width is the width of the current frame, multiplied by the resized scale of the sprite
            return (int)(animations[currentAnim][currentFrame].Width * spriteScale.X);
        }

        //
        // GetEdgeLeftForDraw()
        // finds the x co-ord of the left edge of the visible sprite
        //
        public int GetEdgeLeftForDraw()
        {
            // left edge is the x co-ord, minus the width scaled by the x component of the origin
            return (int)(spritePos.X - (GetWidthForDraw() * spriteOrigin.X));
        }

        //
        // GetEdgeRightForDraw()
        // finds the x co-ord of the right edge of the visible sprite
        //
        public int GetEdgeRightForDraw()
        {
            // right edge is the left edge plus the whole (scaled) width
            return GetEdgeLeftForDraw() + GetWidthForDraw();
        }

        //
        // getHieghtForDraw()
        // finds the desired draw height of the visible sprite
        //
        public int GetHeightForDraw()
        {
            // draw height is the height of the current frame, multiplied by the resized scale of the sprite
            return (int)(animations[currentAnim][currentFrame].Height * spriteScale.Y);
        }

        //
        // GetEdgeTopForDraw()
        // finds the y co-ord of the top edge of the visible sprite
        //
        public int GetEdgeTopForDraw()
        {
            // top edge is the y co-ord, minus the height scaled by the y component of the origin
            return (int)(spritePos.Y - (GetHeightForDraw() * spriteOrigin.Y));
        }

        //
        // GetEdgeBottomForDraw()
        // finds the y co-ord of the bottom edge of the visible sprite
        //
        public int GetEdgeBottomForDraw()
        {
            // bottom edge is the top edge plus the whole (scaled) height
            return GetEdgeTopForDraw() + GetHeightForDraw();
        }

        //
        //
        //
        // The following methods are all used in calculating the rectangle for COLLISION detection:
        // GetRectangleForCollision()
        // GetWidthForCollision()
        // GetEdgeLeftForCollision()
        // GetEdgeRightForCollision()
        // getHieghtForCollision()
        // GetEdgeTopForCollision()
        // GetEdgeBottomForCollision()
        // GetCentreForCollision()
        //
        //
        //

        //
        // GetRectangleForCollision()
        // builds the rectangle that is used for checking collions
        // seperates the calculations out into individual methods
        //
        public Rectangle GetRectangleForCollision()
        {
            return new Rectangle(GetEdgeLeftForCollision(), GetEdgeTopForCollision(), GetWidthForCollision(), GetHeightForCollision());
        }

        //
        // GetWidthForCollision()
        // finds the desired width of the collision rectangle
        //
        public int GetWidthForCollision()
        {
            // the width is the right edge minus the left edge
            return GetEdgeRightForCollision() - GetEdgeLeftForCollision();
        }

        //
        // GetEdgeLeftForCollision()
        // finds the x co-ord of the left edge of the collision rectangle
        //
        public int GetEdgeLeftForCollision()
        {
            // left edge is the x co-ord, plus the width scaled by the x component of the MIN inset
            return GetEdgeLeftForDraw() + (int)(collisionInsetMin.X * GetWidthForDraw());
        }

        //
        // GetEdgeRightForCollision()
        // finds the x co-ord of the right edge of the collision rectangle
        //
        public int GetEdgeRightForCollision()
        {
            // right edge is the x co-ord, minus the width scaled by the x component of the MAX inset
            return GetEdgeRightForDraw() - (int)(collisionInsetMax.X * GetWidthForDraw());
        }

        //
        // GetHeightForCollision()
        // finds the desired height of the collision rectangle
        //
        public int GetHeightForCollision()
        {
            // the height is the bottom edge minus the top edge
            return GetEdgeBottomForCollision() - GetEdgeTopForCollision();
        }

        //
        // GetEdgeTopForCollision()
        // finds the y co-ord of the top edge of the collision rectangle
        //
        public int GetEdgeTopForCollision()
        {
            // top edge is the y co-ord, plus the height scaled by the y component of the MIN inset
            return GetEdgeTopForDraw() + (int)(collisionInsetMin.Y * GetHeightForDraw());
        }

        //
        // GetEdgeBottomForCollision()
        // finds the y co-ord of the bottom edge of the collision rectangle
        //
        public int GetEdgeBottomForCollision()
        {
            // top edge is the y co-ord, minus the height scaled by the y component of the MAX inset
            return GetEdgeBottomForDraw() - (int)(collisionInsetMax.Y * GetHeightForDraw());
        }

        //
        // GetCentreForCollision()
        // finds the x and y co-ords of the centre of the collision rectangle
        //
        public Vector2 GetCentreForCollision()
        {
            // the centre is the top-left plus half of the width and height
            return new Vector2(GetEdgeLeftForCollision() + (GetWidthForCollision() * 0.5f), GetEdgeTopForCollision() + (GetHeightForCollision() * 0.5f));
        }
    }
}