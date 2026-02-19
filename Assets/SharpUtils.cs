namespace NathanBrown
{
    // series of short utility functions for use in various projects
    class SharpUtils
    {        public static float Remap(this float value, (float min, float max) from, (float min, float max) to)
        => (value - from.min) / (from.max - from.min) * (to.max - to.min) + to.min;
    }
}
