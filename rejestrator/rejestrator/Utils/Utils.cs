namespace rejestrator.Utils
{
    using System;
    public static class Utils
    {
        public static bool Between(int value, int a, int b)
        {
            if (value > a && value <= b)
                return true;
            return false;
        }

        public static bool Between(string value, int a, int b)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            if (!int.TryParse(value, out int val))
                throw new Exception("Value isn't int");
            if (val > a && val <= b)
                return true;
            return false;
        }
    }
}
