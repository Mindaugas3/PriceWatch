namespace ASP.NETCoreWebApplication.Utils
{
    public struct Range
    {

        public Range(int min, int max)
        {
            this.Max = max;
            this.Min = min;
        }

        public int Min {
            get;
        }

        public int Max
        {
            get;
        }

    }
}