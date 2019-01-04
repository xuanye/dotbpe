using System;

namespace PiggyMetrics.Common
{
    public static class Assert
    {
        public static void IsTrue(bool condition)
        {
            if (!condition)
            {
                throw new ArgumentException();
            }
        }


        public static void IsTrue(bool condition, string errorMessage)
        {
            if (!condition)
            {
                throw new ArgumentException(errorMessage);
            }
        }


        public static void IsNull<T>(T reference)
        {
            if (reference ==null)
            {
                throw new ArgumentException();
            }
        }


        public static void IsNull<T>(T reference, string errorMessage)
        {
            if (reference ==null)
            {
                throw new ArgumentException(errorMessage);
            }
        }

        public static void IsNotNull<T>(T reference, string errorMessage)
        {
            if (reference !=null)
            {
                throw new ArgumentException(errorMessage);
            }
        }
    }
}
