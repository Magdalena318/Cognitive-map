using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveMap
{
    public abstract class ActivationFunction
    {
        abstract public double Evalutate(double x);
        abstract public double EvaluateDerivative(double x);
    }

    public class Sigmoid : ActivationFunction
    {
        public override double EvaluateDerivative(double x)
        {
            var fx = Evalutate(x);
            return fx * (1 - fx);
        }

        public override double Evalutate(double x)
        {
            return 1.0d / (1.0d + Math.Exp(-x));
        }
    }

    public class ELU : ActivationFunction
    {
        double alpha;
        public ELU(double a)
        {
            alpha = a;
        }
        public override double EvaluateDerivative(double x)
        {
            return x >= 0 ? 1 : Evalutate(x) + alpha;
        }

        public override double Evalutate(double x)
        {
            return x >= 0 ? x : alpha * (Math.Exp(x) - 1);
        }
    }

    public class Gaussian : ActivationFunction
    {
        public override double EvaluateDerivative(double x)
        {
            return -2 * x * Math.Exp(-x * x);
        }

        public override double Evalutate(double x)
        {
            return Math.Exp(-x * x);
        }
    }
}
