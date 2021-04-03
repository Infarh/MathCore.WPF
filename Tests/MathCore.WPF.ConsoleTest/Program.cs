using System;

namespace MathCore.WPF.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var s1 = string.Format("{0}{1}", "abc", "cba");
            var s2 = "abc" + "cba";
            var s3 = "abccba";

            var h1 = s1.GetHashCode();
            var h2 = s2.GetHashCode();
            var h3 = s3.GetHashCode();

            var s1_refeq_s2 = ReferenceEquals(s1, s2);
            var s2_refeq_s3 = ReferenceEquals(s2, s3);
            var s3_refeq_s1 = ReferenceEquals(s3, s1);

            var s1_eq_s2 = s1 == s2;
            var s2_eq_s3 = s2 == s3;
            var s3_eq_s1 = s3 == s1;

            var s1_objeq_s2 = (object)s1 == (object)s2;
            var s2_objeq_s3 = (object)s2 == (object)s3;
            var s3_objeq_s1 = (object)s3 == (object)s1;
        }
    }
}
