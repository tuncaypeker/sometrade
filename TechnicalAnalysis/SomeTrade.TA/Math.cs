using CuttingEdge.Conditions;

namespace SomeTrade.TA
{
    public class Math
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public static double[] ABS(double[] price)
        {
            Condition.Requires(price, "price").IsNotNull().IsNotEmpty();

            var acos = new double[price.Length];
            for (int i = 0; i < price.Length; i++)
                acos[i] = System.Math.Abs(price[i]);

            return acos;
        }

        /// <summary>
        /// The arccosine function answers the question "what angle has this cosine?". 
        /// But there is no value that you can give to the cosine function to yield a result less than -1 or greater than 1. 
        /// We say that its "range is [-1, 1]".
        /// Eğer -1,1 arasında bir değer gondermezsen sonuc NaN donecektir
        /// </summary>
        public static double[] ACOS(double[] price)
        {
            Condition.Requires(price, "price").IsNotNull().IsNotEmpty();

            var acos = new double[price.Length];
            for (int i = 0; i < price.Length; i++)
                acos[i] = System.Math.Acos(price[i]);

            return acos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inReal0"></param>
        /// <param name="inReal1"></param>
        /// <returns></returns>
        public static double[] ADD(double[] inReal0, double[] inReal1)
        {
            Condition.Requires(inReal0, "inReal0").IsNotNull().IsNotEmpty();
            Condition.Requires(inReal1, "inReal1").IsNotNull().IsNotEmpty();

            var add = new double[inReal0.Length];
            for (int i = 0; i < inReal0.Length; i++)
                add[i] = inReal0[i] + inReal1[i];

            return add;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inReal"></param>
        /// <returns></returns>
        public static double[] ASIN(double[] inReal)
        {
            Condition.Requires(inReal, "inReal").IsNotNull().IsNotEmpty();

            var result = new double[inReal.Length];
            for (int i = 0; i < inReal.Length; i++)
                result[i] = System.Math.Asin(inReal[i]);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inReal"></param>
        /// <returns></returns>
        public static double[] ATAN(double[] inReal)
        {
            Condition.Requires(inReal, "inReal").IsNotNull().IsNotEmpty();

            var result = new double[inReal.Length];
            for (int i = 0; i < inReal.Length; i++)
                result[i] = System.Math.Atan(inReal[i]);

            return result;
        }

        /// <summary>
        /// Ceil: Tavana çekmek
        /// Vector Ceiling applies the ceiling function to each input in the input array. The ceiling function returns the smallest 
        /// following integer from the input. e.g. ceil(x) is the smallest integer greater than or equal to x.
        /// 
        /// 81.59	82.00
        /// 81.06   82.00
        /// 82.87   83.00
        /// </summary>
        /// <param name="inArray"></param>
        /// <returns></returns>
        public static double[] CEIL(double[] inReal)
        {
            Condition.Requires(inReal, "inReal").IsNotNull().IsNotEmpty();

            var result = new double[inReal.Length];
            for (int i = 0; i < inReal.Length; i++)
                result[i] = System.Math.Ceiling(inReal[i]);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inReal"></param>
        /// <returns></returns>
        public static double[] COS(double[] inReal)
        {
            Condition.Requires(inReal, "inReal").IsNotNull().IsNotEmpty();

            var result = new double[inReal.Length];
            for (int i = 0; i < inReal.Length; i++)
                result[i] = System.Math.Cos(inReal[i]);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inReal"></param>
        /// <returns></returns>
        public static double[] COSH(double[] inReal)
        {
            Condition.Requires(inReal, "inReal").IsNotNull().IsNotEmpty();

            var result = new double[inReal.Length];
            for (int i = 0; i < inReal.Length; i++)
                result[i] = System.Math.Cosh(inReal[i]);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inReal0"></param>
        /// <param name="inReal1"></param>
        /// <returns></returns>
        public static double[] DIV(double[] inReal0, double[] inReal1)
        {
            Condition.Requires(inReal0, "inReal0").IsNotNull().IsNotEmpty();
            Condition.Requires(inReal1, "inReal1").IsNotNull().IsNotEmpty();

            var result = new double[inReal0.Length];
            for (int i = 0; i < inReal0.Length; i++)
                result[i] = inReal0[i] / inReal1[i];

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inReal"></param>
        /// <returns></returns>
        public static double[] EXP(double[] inReal)
        {
            Condition.Requires(inReal, "inReal").IsNotNull().IsNotEmpty();

            var result = new double[inReal.Length];
            for (int i = 0; i < inReal.Length; i++)
                result[i] = System.Math.Exp(inReal[i]);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inReal"></param>
        /// <returns></returns>
        public static double[] Floor(double[] inReal)
        {
            Condition.Requires(inReal, "inReal").IsNotNull().IsNotEmpty();

            var result = new double[inReal.Length];
            for (int i = 0; i < inReal.Length; i++)
                result[i] = System.Math.Floor(inReal[i]);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inReal"></param>
        /// <returns></returns>
        public static double[] LOG(double[] inReal)
        {
            Condition.Requires(inReal, "inReal").IsNotNull().IsNotEmpty();

            var result = new double[inReal.Length];
            for (int i = 0; i < inReal.Length; i++)
                result[i] = System.Math.Log(inReal[i]);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inReal"></param>
        /// <returns></returns>
        public static double[] LOG10(double[] inReal)
        {
            Condition.Requires(inReal, "inReal").IsNotNull().IsNotEmpty();

            var result = new double[inReal.Length];
            for (int i = 0; i < inReal.Length; i++)
                result[i] = System.Math.Log10(inReal[i]);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inReal0"></param>
        /// <param name="inReal1"></param>
        /// <returns></returns>
        public static double[] MULT(double[] inReal0, double[] inReal1)
        {
            Condition.Requires(inReal0, "inReal0").IsNotNull().IsNotEmpty();
            Condition.Requires(inReal1, "inReal1").IsNotNull().IsNotEmpty();

            var result = new double[inReal0.Length];
            for (int i = 0; i < inReal0.Length; i++)
                result[i] = inReal0[i] * inReal1[i];

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inReal"></param>
        /// <returns></returns>
        public static double[] SIN(double[] inReal)
        {
            Condition.Requires(inReal, "inReal").IsNotNull().IsNotEmpty();

            var result = new double[inReal.Length];
            for (int i = 0; i < inReal.Length; i++)
                result[i] = System.Math.Sin(inReal[i]);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inReal"></param>
        /// <returns></returns>
        public static double[] SINH(double[] inReal)
        {
            Condition.Requires(inReal, "inReal").IsNotNull().IsNotEmpty();

            var result = new double[inReal.Length];
            for (int i = 0; i < inReal.Length; i++)
                result[i] = System.Math.Sinh(inReal[i]);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inReal"></param>
        /// <returns></returns>
        public static double[] SQRT(double[] inReal)
        {
            Condition.Requires(inReal, "inReal").IsNotNull().IsNotEmpty();

            var result = new double[inReal.Length];
            for (int i = 0; i < inReal.Length; i++)
                result[i] = System.Math.Sqrt(inReal[i]);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inReal0"></param>
        /// <param name="inReal1"></param>
        /// <returns></returns>
        public static double[] SUB(double[] inReal0, double[] inReal1)
        {
            Condition.Requires(inReal0, "inReal0").IsNotNull().IsNotEmpty();
            Condition.Requires(inReal1, "inReal1").IsNotNull().IsNotEmpty();

            var result = new double[inReal0.Length];
            for (int i = 0; i < inReal0.Length; i++)
                result[i] = inReal0[i] - inReal1[i];

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inReal"></param>
        /// <returns></returns>
        public static double[] TAN(double[] inReal)
        {
            Condition.Requires(inReal, "inReal").IsNotNull().IsNotEmpty();

            var result = new double[inReal.Length];
            for (int i = 0; i < inReal.Length; i++)
                result[i] = System.Math.Tan(inReal[i]);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inReal"></param>
        /// <returns></returns>
        public static double[] TANH(double[] inReal)
        {
            Condition.Requires(inReal, "inReal").IsNotNull().IsNotEmpty();

            var result = new double[inReal.Length];
            for (int i = 0; i < inReal.Length; i++)
                result[i] = System.Math.Tanh(inReal[i]);

            return result;
        }

        public static double[] AVG(double[] left, double[] right)
        {
            Condition.Requires(left, "left").IsNotNull().IsNotEmpty();
            Condition.Requires(right, "right").IsNotNull().IsNotEmpty();
            Condition.Requires(left.Length).IsEqualTo(right.Length);

            var result = new double[left.Length];
            for (int i = 0; i < left.Length; i++)
                result[i] = (left[i] + right[i]) / 2; 

            return result;
        }

        public static double[] AVG(double[] left, double[] middle, double[] right)
        {
            Condition.Requires(left, "left").IsNotNull().IsNotEmpty();
            Condition.Requires(middle, "middle").IsNotNull().IsNotEmpty();
            Condition.Requires(right, "right").IsNotNull().IsNotEmpty();
            Condition.Requires(left.Length)
                .IsEqualTo(middle.Length)
                .IsEqualTo(right.Length);

            var result = new double[left.Length];
            for (int i = 0; i < left.Length; i++)
                result[i] = (left[i] + middle[i] + right[i]) / 3;

            return result;
        }

        public static double[] AVG(double[] open, double[] high, double[] low, double[] close)
        {
            Condition.Requires(open, "inOpen").IsNotNull().IsNotEmpty();
            Condition.Requires(high, "inHigh").IsNotNull().IsNotEmpty();
            Condition.Requires(low, "inLow").IsNotNull().IsNotEmpty();
            Condition.Requires(close, "inClose").IsNotNull().IsNotEmpty();
            Condition.Requires(open.Length)
                .IsEqualTo(high.Length)
                .IsEqualTo(low.Length)
                .IsEqualTo(close.Length);

            var result = new double[open.Length];
            for (int i = 0; i <= open.Length; i++)
                result[i] = (open[i] + high[i] + low[i] + close[i]) / 4.0;

            return result;
        }

        /// <summary>
        /// Agırlıklı ortalama son dizi verilen agırlık kadar carpilarak ortalama alinir
        /// </summary>
        /// <param name="inHigh"></param>
        /// <param name="inLow"></param>
        /// <param name="inClose"></param>
        /// <returns></returns>
        public static double[] AVG_W(double[] left, double[] middle, double[] right, double weight = 2.0)
        {
            Condition.Requires(left, "left").IsNotNull().IsNotEmpty();
            Condition.Requires(middle, "middle").IsNotNull().IsNotEmpty();
            Condition.Requires(right, "right").IsNotNull().IsNotEmpty();

            var result = new double[left.Length];
            for (int i = 0; i < left.Length; i++)
                result[i] = (left[i] + middle[i] + right[i] * weight) / 4.0;

            return result;
        }
    }
}
