using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace CognitiveMap
{
    class Adam
    {
        Matrix<double> m;
        Matrix<double> v;
        const double beta_1 = 0.9d;
        const double beta_2 = 0.999d;
        const double epsilon = 0.1d;
        double stepSize;
        public Adam(int size, double _stepSize)
        {
            m = Matrix<double>.Build.Dense(size, size);
            v = Matrix<double>.Build.Dense(size, size);
            stepSize = _stepSize;
        }
        public void updateWeights(ref Matrix<double> weights, Matrix<double> gradient, int iteration)
        {
            m = beta_1 * m + (1 - beta_1) * gradient;
            v = beta_2 * v + (1 - beta_2) * gradient.PointwisePower(2);
            var m_hat = m / (1 - Math.Pow(beta_1, iteration));
            var v_hat = v / (1 - Math.Pow(beta_2, iteration));
            weights -= stepSize * m_hat.PointwiseDivide(v_hat.PointwiseSqrt().Add(epsilon));
        }
    }
}
