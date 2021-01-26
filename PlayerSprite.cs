using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace PageFinder1
{
    class PlayerSprite : Sprite
    {
        //initialize variables
        bool jumping, walking, falling, jumpIsPressed;
        const float jumpSpeed = 6.5f;
        const float walkSpeed = 200f;
        public int lives = 3;
        SoundEffect jumpSound, bumpSound;

        public PlayerSprite(Texture2D newSpriteSheet, Texture2D newCollisionTxr,
            Vector2 newLocation, SoundEffect newJumpSound, SoundEffect newBumpSound)
            : base(newSpriteSheet, newCollisionTxr, newLocation)
        {
            jumpSound = newJumpSound;
            bumpSound = newBumpSound;

            spriteOrigin = new Vector2(0.5f, 0f);
            isColliding = true;

            collisionInsetMin = new Vector2(0.25f, 0.3f);
            collisionInsetMax = new Vector2(0.25f, 0.0f);

            //playersprite animations
            frameTime = 0.1f;
            animations = new List<List<Rectangle>>();

            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 48, 48));
            animations[0].Add(new Rectangle(0, 0, 48, 48));
            animations[0].Add(new Rectangle(0, 0, 48, 48));
            animations[0].Add(new Rectangle(48, 0, 48, 48));
            animations[0].Add(new Rectangle(48, 0, 48, 48));
            animations[0].Add(new Rectangle(48, 0, 48, 48));

            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(48, 0, 48, 48));
            animations[1].Add(new Rectangle(96, 0, 48, 48));
            animations[1].Add(new Rectangle(48, 0, 48, 48));
            animations[1].Add(new Rectangle(144, 0, 48, 48));

            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(96, 0, 48, 48));

            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(0, 48, 48, 48));

            jumping = false;
            walking = false;
            falling = true;
            jumpIsPressed = false;
        }

        //playerSprite controls
        public void Update(GameTime gametime, List<PlatformSprite> platforms)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            //jump controls
            if (!jumpIsPressed && !jumping && !falling &&
                (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up)
                || gamePadState.IsButtonDown(Buttons.A)))
            {
                jumpIsPressed = true;
                jumping = true;
                walking = false;
                falling = false;
                spriteVelocity.Y -= jumpSpeed;
                jumpSound.Play();
            }
            else if (jumpIsPressed && !jumping && !falling &&
                !(keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Space)
                || gamePadState.IsButtonDown(Buttons.A)))
            {
                jumpIsPressed = false;
            }

            //movement left controls
            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left)
                || gamePadState.IsButtonDown(Buttons.DPadLeft))
            {
                walking = true;
                spriteVelocity.X = -walkSpeed * (float)gametime.ElapsedGameTime.TotalSeconds;
                flipped = true;
            }
            //movement right controls
            else if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right)
                || gamePadState.IsButtonDown(Buttons.DPadRight))
            {
                walking = true;
                spriteVelocity.X = walkSpeed * (float)gametime.ElapsedGameTime.TotalSeconds;
                flipped = false;
            }
            else
            {
                walking = false;
                spriteVelocity.X = 0;
            }
            //fall speed and platform collision check
            if ((falling || jumping) && spriteVelocity.Y < 500f)
                spriteVelocity.Y += 10f * (float)gametime.ElapsedGameTime.TotalSeconds;
            spritePos += spriteVelocity;

            bool hasCollided = false;

            foreach (PlatformSprite platform in platforms)
            {
                if (CheckCollisionBelow(platform))
                {
                    bumpSound.Play();
                    hasCollided = true;
                    while (CheckCollision(platform)) spritePos.Y--;
                    spriteVelocity.Y = 0;
                    jumping = false;
                    falling = false;
                }
                else if (CheckCollisionAbove(platform))
                {
                    hasCollided = true;
                    while (CheckCollision(platform)) spritePos.Y++;
                    spriteVelocity.Y = 0;
                    jumping = false;
                    falling = true;
                }

                if (CheckCollisionLeft(platform))
                {
                    hasCollided = true;
                    while (CheckCollision(platform)) spritePos.X--;
                    spriteVelocity.X = 0;
                }
                else if (CheckCollisionRight(platform))
                {
                    hasCollided = true;
                    while (CheckCollision(platform)) spritePos.X++;
                    spriteVelocity.X = 0;
                }

                if (!hasCollided && walking) falling = true;
                if (jumping && spriteVelocity.Y > 0)
                {
                    jumping = false;
                    falling = true;
                }
                //setting animations for walking, falling, jumping
                if (walking) SetAnim(1);
                else if (falling) SetAnim(3);
                else if (jumping) SetAnim(2);
                else SetAnim(0);
            }

        }
        //reset player function
        public void ResetPlayer(Vector2 newPos)
        {
            spritePos = newPos;
            spriteVelocity = new Vector2();
            jumping = false;
            walking = false;
            falling = true;
        }
    }
}
