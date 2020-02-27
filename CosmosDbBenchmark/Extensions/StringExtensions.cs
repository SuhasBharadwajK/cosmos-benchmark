using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDbBenchmark
{
    public static class StringExtensions
    {
        public static string Scramble(this string source)
        {
            StringBuilder jumbledStringBuilder = new StringBuilder();
            Random rand = new Random();
            jumbledStringBuilder.Append(source);
            int lengthSB = jumbledStringBuilder.Length;
            for (int i = 0; i < lengthSB; ++i)
            {
                int index1 = (rand.Next() % lengthSB);
                int index2 = (rand.Next() % lengthSB);

                char temp = jumbledStringBuilder[index1];
                jumbledStringBuilder[index1] = jumbledStringBuilder[index2];
                jumbledStringBuilder[index2] = temp;

            }

            return jumbledStringBuilder.ToString();
        }
    }
}
