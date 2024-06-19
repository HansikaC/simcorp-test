using System;
///Problem summary 
/// <summary>
/// EXERCISE INSTRUCTIONS:
///
/// Change the ThresholdChecker class to support two comparisons:
///     1) relative (percentage-based) comparisons, and
///     2) the absolute difference comparisons as currently is implemented
///
/// Update the tests in Program.Main() to confirm the new behavior.
///
///
/// NOTE:
///
/// Feel free to rename classes/methods for what is most clear and natural.
///
/// We expect that we will need to extend the types of comparisons the ThresholdChecker class needs to
/// support in the future. Please make the changes with that consideration in mind.
/// </summary>
/// 

namespace Questions
{
    public static class Program
    {
        public static void Main()
        {
            // Absolute comparison
            var absoluteChecker = new ThresholdChecker(10, new AbsoluteComparisonType());

            // Test out the existing absolute comparison code
            Console.WriteLine("Absolute Comparison Tests:");
            TestAbsoluteComparisons(absoluteChecker);

            Console.WriteLine("-----------------------------------------------------------------");

            // Relative comparison
            absoluteChecker.SetComparisonType(new RelativeComparisonType());

            // Test out the new relative based comparisons
            Console.WriteLine("Relative Comparison Tests:");
            TestRelativeComparisons(absoluteChecker);
        }

        private static void TestAbsoluteComparisons(ThresholdChecker checker)
        {
            var tests = new (double value1, double value2, bool expected)[]
            {
                (100, 110, false),  // difference is equal to threshold
                (100, 111, true),   // difference exceeds threshold
                (100, 90, false),   // difference is less than threshold
                (100, 89, true),    // difference exceeds threshold
                (-10, 0, false),    // difference is equal to threshold
                (0, 0, false),      // zero values
                (1e10, 1e10 + 5, false), // large values, difference less than threshold
                (1e10, 1e10 + 15, true),  // large values, difference exceeds threshold
                (0.1, 0.2, false),  // small rational numbers, difference less than threshold
                (0.1, 0.11, false), // very small difference
                (-0.1, 0.2, false),  // one positive, one negative rational number, not exceeding threshold
                (0.5, -0.5, false),  // both rational numbers, opposite signs, not exceeding threshold
            };

            foreach (var (value1, value2, expected) in tests)
            {
                bool result = checker.CheckThreshold(value1, value2);
                Console.WriteLine($"CheckThreshold({value1}, {value2}) = {result}, Expected: {expected}");
            }
        }

        private static void TestRelativeComparisons(ThresholdChecker checker)
        {
            var tests = new (double value1, double value2, bool expected)[]
            {
                (108.9999999999, 108, false),  // very small relative difference
                (-10, -20, true),   // both are negative
                (0, 100, true),     // zero value1
                (100, 0, true),     // zero value2, expected true due to handling
                (0, 0, false),      // both zero
                (1e10, 1e10 + 5, false), // large values, relative difference less than threshold
                (1e10, 1e10 + 1e9, false),  // large values, relative difference is 10%
                (90, 100, false),   // relative difference where value2 > value1
                (100, 90, true),    // relative difference where value1 > value2
                (-100, 100, true),  // value1 is negative, value2 is positive
                (100, -100, true),  // value1 is positive, value2 is negative
                (0.1, 0.2, true),   // small rational numbers
                (0.1, 0.11, false), // very small rational difference
                (-0.1, 0.2, true),  // one positive, one negative rational number
                (0.5, -0.5, true),  // both rational numbers, opposite signs
            };

            foreach (var (value1, value2, expected) in tests)
            {
                bool result = checker.CheckThreshold(value1, value2);
                Console.WriteLine($"CheckThreshold({value1}, {value2}) = {result}, Expected: {expected}");
            }
        }
    }

    public interface IComparisonType
    {
        bool IsThresholdExceeded(double value1, double value2, double threshold);
    }

    public class AbsoluteComparisonType : IComparisonType
    {
        // Returns true if delta is over the threshold
        public bool IsThresholdExceeded(double value1, double value2, double threshold)
        {
            return Math.Abs(value2 - value1) > threshold;
        }
    }

    public class RelativeComparisonType : IComparisonType
    {
        public bool IsThresholdExceeded(double value1, double value2, double threshold)
        {
            // using formula PercentageDifference = [(n2-n1)/n2] * 100
            if (value2 == 0)
            {
                // Handle divide-by-zero case
                return value1 != 0;
            }
            double relativeDifference = Math.Abs((value2 - value1) / value2) * 100;
            return relativeDifference > threshold;
        }
    }


    public class ThresholdChecker
    {
        public double Threshold { get; set; }
        private IComparisonType _comparisonType;

        public ThresholdChecker(double threshold, IComparisonType comparisonType)
        {
            Threshold = threshold;
            _comparisonType = comparisonType;
        }

        public bool CheckThreshold(double value1, double value2)
        {
            return _comparisonType.IsThresholdExceeded(value1, value2, Threshold);
        }

        public void SetComparisonType(IComparisonType comparisonType)
        {
            _comparisonType = comparisonType;
        }
    }
}
