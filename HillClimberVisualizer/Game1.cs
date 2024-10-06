using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

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

        private Perceptron HillClimber;
        double[][] Inputs;
        double[] DesiredOutputs;
        double CurrentError;

        public Game1()
        {
            gfx = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        static double Error(double output, double desiredOutput) => desiredOutput - output;

        protected override void Initialize()
        {
            HillClimber = new Perceptron(1, 0.01, 0.1, Error, 0, 300, 0, 1);
            // instead of 300 the best solution would be finding the intercept of the secant line for smallest point and largest point
            // OR using compute for the slope data instead of direct refrence
            HillClimber.Randomize(new Random(), 0, 2);
            Inputs = [[100], [200], [300]];
            DesiredOutputs = [300, 200, 100];

            CurrentError = HillClimber.GetError(Inputs, DesiredOutputs);

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

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            CurrentError = HillClimber.TrainHillClimber(Inputs, DesiredOutputs, CurrentError, out CurrentSlope);
            CurrentBestSlope = new(HillClimber.Weights[0], HillClimber.Unnormalize(HillClimber.Bias));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
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
