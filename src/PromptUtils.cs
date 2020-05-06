using System;

namespace ImageUnscrambler
{
    public static class PromptUtils
    {
        public delegate bool TryParse<T>(string input, out T result);

        public static string Prompt(this string question)
        {
            Console.Write($"{question} > ");
            return Console.ReadLine();
        }

        public static T Prompt<T>(this string question, Func<string, T> parse) => parse.Invoke(Prompt(question));

        public static T Prompt<T>(this string question, TryParse<T> tryParse)
        {
            while (true)
            {
                if (tryParse(Prompt(question), out var result))
                    return result;
            }
        }

        public static T Prompt<T>(this string question, bool ignoreCase) where T : struct, Enum
        {
            while (true)
            {
                if (Enum.TryParse<T>(Prompt(question), ignoreCase, out var result))
                    return result;
            }
        }
    }
}