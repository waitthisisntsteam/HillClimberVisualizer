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

        private KeyValuePair<double, double> CurrentBestSlope;
        private KeyValuePair<double, double> CurrentSlope;

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

            CurrentBestSlope = new(0, 0);

            CurrentSlope = new();

            base.Initialize();
        }

        protected double CalculateY(double m, double x, double b) => m * x + b;
        protected KeyValuePair<double, double> GenerateSlope() => new KeyValuePair<double, double>(Random.Next(0, GraphicsDevice.Viewport.Height), Random.Next(0, GraphicsDevice.Viewport.Height));

        protected KeyValuePair<double, double> Mutate(KeyValuePair<double, double> input)
        {
            int valAlteration = Random.Next(0, 2) == 0 ? -1 : 1;
            int inputSelector = Random.Next(0, 2);

            double m = input.Key;
            double b = input.Value;

            if (inputSelector == 0) m = input.Key + Random.NextDouble() * valAlteration;
            else b = input.Value + Random.NextDouble() * valAlteration;

            return new KeyValuePair<double, double>(m, b);
        }

        protected double Error(List<Point> desiredOutput, KeyValuePair<double, double> slope)
        {
            double sum = 0;
            for (int i = 0; i < desiredOutput.Count; i++) sum += Math.Pow(desiredOutput[i].Y - CalculateY(slope.Key, desiredOutput[i].X, slope.Value), 2);
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

            double error = Error(Points, CurrentBestSlope);

            CurrentSlope = Mutate(CurrentBestSlope);
            double newError = Error(Points, CurrentSlope);
            
            if (newError < error)
            {
                CurrentBestSlope = CurrentSlope;
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

            double y1 = CalculateY(CurrentBestSlope.Key, 0, CurrentBestSlope.Value);
            double y2 = CalculateY(CurrentBestSlope.Key, GraphicsDevice.Viewport.Width, CurrentBestSlope.Value);
            spriteBatch.DrawLine(0, (float)y1, GraphicsDevice.Viewport.Width, (float)y2, Color.Black, 2);

            double ny1 = CalculateY(CurrentSlope.Key, 0, CurrentSlope.Value);
            double ny2 = CalculateY(CurrentSlope.Key, GraphicsDevice.Viewport.Width, CurrentSlope.Value);
            spriteBatch.DrawLine(0, (float)ny1, GraphicsDevice.Viewport.Width, (float)ny2, Color.Blue, 2);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
