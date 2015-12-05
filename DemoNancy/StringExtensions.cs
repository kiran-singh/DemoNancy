namespace DemoNancy
{
    public static class StringExtensions
    {
        public static bool ToBool(this string input)
        {
            bool parsed;
            bool.TryParse(input, out parsed);

            return parsed;
        }

        public static int ToInt(this string input)
        {
            int parsed;
            int.TryParse(input, out parsed);

            return parsed;
        }

        public static long ToLong(this string input)
        {
            long parsed;
            long.TryParse(input, out parsed);

            return parsed;
        }
    }
}