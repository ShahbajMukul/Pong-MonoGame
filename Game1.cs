using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pong;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Project1
{
    public class Game1 : Game
    {
        SpriteFont scoreFont;
        int score;

        Texture2D ballTexture;
        Vector2 ballPosition;
        float ballSpeed;
        float jumpPower;

        Vector2 velocity;
        float gravity;

        bool grounded;

        bool GameOver;



        Texture2D smallSpikeTexture;
        List<Vector2> smallSpikes;

        Random random;
        double timer;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

    

        protected override void Initialize()
        {
            score = 0;  

            ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

            ballSpeed = 500f;
            GameOver = false;
            velocity = Vector2.Zero;
            gravity = 600f; 
            jumpPower = -800f;

            random =new Random();
            timer = 0;
            smallSpikes = new List<Vector2>();
            /*smallSpikePosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
            _graphics.PreferredBackBufferHeight -100)*/
            ;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            scoreFont = Content.Load<SpriteFont>("ScoreFont");
            ballTexture = Content.Load<Texture2D>("ball");
            smallSpikeTexture = Content.Load<Texture2D>("small_metal_spike");
        }

        protected override void Update(GameTime gameTime)
        {

            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ballPosition.Y += 5f;


            // hitbox
            Rectangle ballRectangle = new Rectangle(
                (int)ballPosition.X,
                (int)ballPosition.Y,
                ballTexture.Width,
                ballTexture.Height);

            // collision with the spike -> game over
            for (int i = 0; i < smallSpikes.Count; i++)
            {
                Rectangle spikeRectangle = new Rectangle(
                    (int)smallSpikes[i].X,
                    (int)smallSpikes[i].Y,
                    smallSpikeTexture.Width,
                    smallSpikeTexture.Height);

                if (ballRectangle.Intersects(spikeRectangle))
                {
                    GameOver = true;
                }
            }


            if (ballPosition.Y + ballTexture.Height >= GraphicsDevice.Viewport.Height)
            {
                grounded = true;
                // ball is grounded, reset Y position and Y velocity
                ballPosition.Y = GraphicsDevice.Viewport.Height - ballTexture.Height;
                velocity.Y = 0;
            }
            else
            {
                grounded = false;
            }

            for (int i = 0; i < smallSpikes.Count; i++)
            {
                smallSpikes[i] = new Vector2(smallSpikes[i].X - 1, smallSpikes[i].Y);

                //Remove spike if it's off the screen
                if (smallSpikes[i].X < -smallSpikeTexture.Width)
                {
                    smallSpikes.RemoveAt(i);
                    score++;
                    i--;
                }
            }

            //Add new spikes every 1-3 seconds
            timer += gameTime.ElapsedGameTime.TotalSeconds;
            if (timer >= random.Next(4, 7))
            {
                smallSpikes.Add(new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight - smallSpikeTexture.Height));
                timer = 0;
            }


            var kstate = Keyboard.GetState();

            // jump
            if ((kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.W) || kstate.IsKeyDown(Keys.Space)) && grounded)
            {
                velocity.Y = jumpPower;
                grounded = false;
            }

            velocity.Y += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            ballPosition += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;


            // restart game
            if (GameOver)
            {
                smallSpikes.Clear();
                

                if (kstate.IsKeyDown(Keys.R))
                {
                    GameOver = false;
                    score = 0;
                    ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
                }
            }

 


            // horizontal speed when on ground
            if ((kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.A)) &&  grounded)
            {
                ballPosition.X -= ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if ((kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D)) && grounded)
            {
                ballPosition.X += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            // end

            // horizontal speed when in air
            if ((kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.A)) && !grounded)
            {
                ballPosition.X -= (ballSpeed / 2) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if ((kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D)) && !grounded)
            {
                ballPosition.X += (ballSpeed/2) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            // end


            // keep the ball on screen
            if (ballPosition.X > _graphics.PreferredBackBufferWidth - ballTexture.Width)
            {
                ballPosition.X = _graphics.PreferredBackBufferWidth - ballTexture.Width;
            }
            else if (ballPosition.X < ballTexture.Width)
            {
                ballPosition.X =  ballTexture.Width; 
            }

            if (ballPosition.Y > _graphics.PreferredBackBufferHeight - ballTexture.Height)
            {
                ballPosition.Y = _graphics.PreferredBackBufferHeight - ballTexture.Height;
            }
            else if (ballPosition.Y < ballTexture.Height)
            {   
                ballPosition.Y = ballTexture.Height;
            }



            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // show score at the end of the game
            if (GameOver)
            {
                string gameOverText = "Game over\nScore: " + score.ToString() + "\nClick 'R' to Restart";
                Vector2 gameOverSize = scoreFont.MeasureString(gameOverText);
                _spriteBatch.DrawString(scoreFont, gameOverText, new Vector2(_graphics.PreferredBackBufferWidth / 2 - gameOverSize.X , _graphics.PreferredBackBufferHeight / 2 - gameOverSize.Y / 2 ), Color.White);
            }
            else
            {
                _spriteBatch.DrawString(scoreFont, "Score: " + score.ToString(), new Vector2(15, 15), Color.Black);
            }

            // draw ball
            _spriteBatch.Draw(ballTexture, ballPosition, Color.White);

            // multiple spikes
            foreach (Vector2 position in smallSpikes)
            {
                _spriteBatch.Draw(smallSpikeTexture, position, Color.White);
            }

            _spriteBatch.End();



            base.Draw(gameTime);
        }


    }
}