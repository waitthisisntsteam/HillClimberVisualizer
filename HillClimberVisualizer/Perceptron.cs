using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillClimberVisualizer
{
    public class Perceptron
    {
        public double[] Weights;
        public double Bias;
        private double WeightMutationAmount;
        private double BiasMutationAmount;
        private Func<double, double, double> ErrorFunc;

        private double Min;
        private double Max;
        private double NMin;
        private double NMax;

        public Perceptron(double[] weights, double bias, double mutationAmount, double biasAmount, Func<double, double, double> errorFunc, double min, double max, double nMin, double nMax)
        {
            Weights = weights;
            Bias = bias;
            WeightMutationAmount = mutationAmount;
            BiasMutationAmount = biasAmount;
            ErrorFunc = errorFunc;

            Min = min;
            Max = max;
            NMin = nMin;
            NMax = nMax;
        }
        public Perceptron(int inputAmount, double mutationAmount, double biasAmount, Func<double, double, double> errorFunc, double min, double max, double nMin, double nMax)
        {
            Weights = new double[inputAmount];
            Bias = 0;

            BiasMutationAmount = biasAmount;
            WeightMutationAmount = mutationAmount;
            ErrorFunc = errorFunc;

            Min = min;
            Max = max;
            NMin = nMin;
            NMax = nMax;
        }

        private double Random(Random random, double min, double max) => (random.NextDouble() * (max - min)) + min;
        public void Randomize(Random random, double min, double max)
        {
            for (int i = 0; i < Weights.Length; i++) Weights[i] = Random(random, min, max);
            Bias = Random(random, min, max);
        }

        public double[] Compute(double[][] inputs)
        {
            double[] output = new double[inputs.Length];
            for (int i = 0; i < inputs.Length; i++) output[i] = Compute(inputs[i]);
            return output;
        }

        private double Compute(double[] inputs)
        {
            double output = Bias;
            for (int i = 0; i < inputs.Length; i++) output += inputs[i] * Weights[i];
            return output;
        }

        public double GetError(double[][] inputs, double[] desiredOutputs)
        {
            double[] outputs = Compute(inputs);

            double errorSum = 0;
            for (int i = 0; i < outputs.Length; i++) errorSum += Math.Pow(ErrorFunc(outputs[i], desiredOutputs[i]), 2);
            return errorSum / outputs.Length;
        }

        public double Normalize(double num) => (num - Min) / (Max - Min) * (NMax - NMin) + NMin;
        public double Unnormalize(double num) => (num - NMin) / (NMax - NMin) * (Max - Min) + Min;
        public double TrainHillClimber(double[][] inputs, double[] desiredOutputs, double currentError, out KeyValuePair<double, double> testedSlope)
        {
            Random rand = new Random();
            int chosenIndex = rand.Next(0, Weights.Length + 1);
            int valAlteration = rand.Next(0, 2) == 1 ? -1 : 1;
            double originalWeight = chosenIndex < Weights.Length ? Weights[chosenIndex] : 0;
            double originalBias = Bias;

            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < inputs[i].Length; j++) inputs[i][j] = Normalize(inputs[i][j]);
            }
            for (int i = 0; i < desiredOutputs.Length; i++) desiredOutputs[i] = Normalize(desiredOutputs[i]);

            if (chosenIndex < Weights.Length) Weights[chosenIndex] += WeightMutationAmount * valAlteration;
            else Bias += BiasMutationAmount * valAlteration;

            double error = GetError(inputs, desiredOutputs);

            testedSlope = new KeyValuePair<double, double>(Weights[0], Unnormalize(Bias));

            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < inputs[i].Length; j++) inputs[i][j] = Unnormalize(inputs[i][j]);
            }
            for (int i = 0; i < desiredOutputs.Length; i++) desiredOutputs[i] = Unnormalize(desiredOutputs[i]);

            if (error < currentError) return error;
            if (chosenIndex < Weights.Length) Weights[chosenIndex] = originalWeight;
            else Bias = originalBias;

            return currentError;
        }
    }
}
