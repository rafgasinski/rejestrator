namespace rejestrator.Utils
{
    using System;
    public static class Error
    {
        public const string BAD_REQUEST = "BAD REQUEST";

        public const string NOT_FOUND = "NOT FOUND";

        public const string NO_CONTENT = "NO CONTENT";

        public const string OTHER = "OTHER";

        public static bool IsResponseError(string response)
        {
            if (response == BAD_REQUEST)
                // Implement handling
                return true;

            else if (response == NOT_FOUND)
                // Implement handling
                return true;

            else if (response == NO_CONTENT)
                // Implement handling
                return true;

            else if (response == OTHER)
                // Implement handling
                return true;
            else
                return false;
        }
    }
}
