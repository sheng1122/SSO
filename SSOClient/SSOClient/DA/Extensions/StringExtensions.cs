using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DA.Extensions
{
    public static class StringExtensions
    {
        public static string Normalize(this string value)
        {
            value = value.Replace(" ", "_");

            string patern = "[^A-Za-z0-9_]";
            Regex regex = new Regex(patern);

            return regex.Replace(value, "");
        }
    }
}
