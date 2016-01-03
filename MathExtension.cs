using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class MathExtension
    {
        /// <summary>
        /// Caclulates the best fit strainght line to a set of points using Linear least squares algorithm
        /// http://stackoverflow.com/questions/12946341/algorithm-for-scatter-plot-best-fit-line
        /// </summary>
        /// <param name="x">X points</param>
        /// <param name="y">Y points</param>
        /// <param name="slope">clculated slop</param>
        /// <param name="intercept">calculated intercept point</param>
        public static void GenerateLinearBestFit(double[] x, double[] y, out double slope, out double intercept)
        {
            if (x.Length != y.Length)
                throw new ApplicationException("MathExtension.GenerateLinearBestFit array of x and y points must be same length");
            if (x.Length <= 1)
                throw new ApplicationException("Need at least two points to do a best fit line");

            int numPoints = x.Length;
            double meanX = x.Average();
            double meanY = y.Average();

            double sumXSquared = 0d, sumXY = 0d;
            for (int c = 0; c < numPoints; c++)
            {
                sumXSquared += (x[c] * x[c]);
                sumXY += (x[c] * y[c]);
            }

            slope     = (sumXY / numPoints - meanX * meanY) / (sumXSquared / numPoints - meanX * meanX);
            intercept = meanY - (slope * meanX);
        }
    }
}
