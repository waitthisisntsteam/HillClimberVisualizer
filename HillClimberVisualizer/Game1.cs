using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Threading;
using MonoGame.Extended;

namespace HillClimberVisualizer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager gfx;
        private SpriteBatch spriteBatch;

        private Random Random;
        private List<Point> Points;

        private KeyValuePair<int, int> CurrentSlope;

        public Game1()
        {
            gfx = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Random = new Random();
            Points = new List<Point>();

            Points.Add(new Point(100, 300));
            Points.Add(new Point(200, 200));
            Points.Add(new Point(300, 100));

            CurrentSlope = new();
            CurrentSlope = GenerateSlope();

            base.Initialize();
        }

        protected int CalculateY(int m, int x, int b) => m * x + b;
        protected KeyValuePair<int, int> GenerateSlope() => new KeyValuePair<int, int>(Random.Next(0, GraphicsDevice.Viewport.Height), Random.Next(0, GraphicsDevice.Viewport.Height));

        protected KeyValuePair<int, int> Mutate(KeyValuePair<int, int> input)
        {
            int valAlteration = Random.Next(0, 2) == 0 ? -1 : 1;

            int m = input.Key + Random.Next(0, 100) * valAlteration;
            int b = input.Value + Random.Next(0, 100) * valAlteration;

            return new KeyValuePair<int, int>(m, b);
        }

        protected double Error(List<Point> desiredOutput, KeyValuePair<int, int> slope)
        {
            double sum = 0;
            for (int i = 0; i < desiredOutput.Count; i++) sum += Math.Abs(desiredOutput[i].Y - CalculateY(slope.Key, desiredOutput[i].X, slope.Value));
            return sum / desiredOutput.Count;
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            double error = Error(Points, CurrentSlope);

            var newSlope = Mutate(CurrentSlope);
            double newError = Error(Points, newSlope);
            
            if (newError < error)
            {
                CurrentSlope = newSlope;
                error = newError;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            foreach (Point p in Points) spriteBatch.DrawPoint(p.X, p.Y, Color.Blue, 10);

            int y1 = CalculateY(CurrentSlope.Key, 0, CurrentSlope.Value);
            int y2 = CalculateY(CurrentSlope.Key, GraphicsDevice.Viewport.Width, CurrentSlope.Value);
            spriteBatch.DrawLine(0, y2, GraphicsDevice.Viewport.Width, y1, Color.Black, 2);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
