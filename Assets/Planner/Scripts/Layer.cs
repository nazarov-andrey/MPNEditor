namespace Planner
{
    public static class Layer
    {
        public const int Dots = 9;

        public static class Mask
        {
            public const int Dots = 1 << Layer.Dots;
        }
    }
}