using System;
using System.Runtime.CompilerServices;

namespace FluxionEditor.Foundation.Utilities
{
    public static class MathUtilities
    {
        /// <summary>Absolute tolerance, used for values near zero.</summary>
        public const float Epsilon = 1e-5f;

        /// <summary>Relative tolerance, used for large magnitude values.</summary>
        public const float RelativeEpsilon = 1e-6f;

        /// <summary>
        /// Near-equality check combining absolute and relative tolerance,
        /// so it stays accurate both near zero and at large magnitudes.
        /// NaN is never equal to anything.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTheSameAs(this float value, float other)
        {
            // Bit-exact match (also covers infinity == infinity).
            if (value == other) return true;

            float diff = MathF.Abs(value - other);

            // Absolute tolerance handles values near zero, where the
            // relative check below would be overly strict.
            if (diff <= Epsilon) return true;

            // Relative tolerance scales with the larger operand.
            return diff <= MathF.Max(MathF.Abs(value), MathF.Abs(other)) * RelativeEpsilon;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTheSameAs(this float? value, float? other)
        {
            if (!value.HasValue || !other.HasValue) return false;
            return value.GetValueOrDefault().IsTheSameAs(other.GetValueOrDefault());
        }
    }
}
