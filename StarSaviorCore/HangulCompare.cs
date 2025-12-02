using System.Text;

namespace EndoAshu.StarSavior.Core
{
    public static class HangulCompare
    {
        public static double GetHangulSimilarity(string source, string target)
        {
            if (string.IsNullOrEmpty(target))
                return -1;
            string s1 = source.Normalize(NormalizationForm.FormD);
            string s2 = target.Normalize(NormalizationForm.FormD);

            return GetSimilarity(s1, s2);
        }

        public static double GetSimilarity(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
                return (source == target) ? 1.0 : 0.0;

            int stepsToSame = ComputeLevenshteinDistance(source, target);
            int maxLength = Math.Max(source.Length, target.Length);

            return 1.0 - ((double)stepsToSame / maxLength);
        }

        private static int ComputeLevenshteinDistance(string source, string target)
        {
            int n = source.Length;
            int m = target.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }
    }
}