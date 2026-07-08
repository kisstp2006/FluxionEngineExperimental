namespace FluxionEditor.Foundation.Utilities
{
    /// <summary>
    /// Simple ID generator / validator. Used for lightweight identity checks.
    /// </summary>
    public static class ID
    {
        /// <summary>Sentinel value for an invalid / uninitialized ID.</summary>
        public const int INVALID_ID = -1;

        /// <summary>Returns <c>true</c> if the ID is valid (not <see cref="INVALID_ID"/>).</summary>
        public static bool IsValid(int id) => id != INVALID_ID;
    }
}
