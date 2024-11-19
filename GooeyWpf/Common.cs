namespace GooeyWpf
{
    public class Common
    {
        public static readonly string[] PositiveStrings =
        {
            "yes", "yeah", "yup", "sure", "okay", "uh huh", "alright", "go", "go for it", "do it", "do", "mm hmm"
        };

        public static readonly string[] NegativeStrings =
        {
            "no", "nah", "nope", "don't", "stop", "nuh uh", "uh uh", "cancel"
        };

        public static Uri Resource(string path) => new Uri("pack://application:,,," + path);

        public static string RemovePunctuation(string text) =>
            text.Replace(".", "")
                .Replace(",", "")
                .Replace("-", "")
                .Replace(";", "")
                .Replace("'", "")
                .Replace("!", "")
                .Replace("?", "");

        public static bool IsPositive(string text)
        {
            return SimilarOrMatch(text, PositiveStrings, 0.75f);
        }

        public static bool IsNegative(string text)
        {
            return SimilarOrMatch(text, NegativeStrings, 0.75f);
        }

        public static bool SimilarOrMatch(string text, string[] strings, float threshold)
        {
            text = text.ToLower();
            bool ret = Similarities(text, strings) > threshold;
            if (!ret)
            {
                for (int i = 0; i < strings.Length; i++)
                {
                    if (text.StartsWith(strings[i]) || text.EndsWith(strings[i]))
                        return true;
                }
            }
            return ret;
        }

        public static float Similarities(string s0, string[] s1)
        {
            float largest = 0f;
            for (int i = 0; i < s1.Length; i++)
            {
                float current = Similarity(s0, s1[i]);
                if (current > largest)
                    largest = current;
            }
            return largest;
        }

        public static int MostSimilar(string s0, string[] s1)
        {
            float largest = 0f;
            int largestIndex = 0;
            for (int i = 0; i < s1.Length; i++)
            {
                float current = Similarity(s0, s1[i]);
                if (current > largest)
                {
                    largest = current;
                    largestIndex = i;
                }
            }
            return largestIndex;
        }
        
        // Levenshtein distance algorithm for comparing similarity between two strings
        public static float Similarity(string s0, string s1)
        {
            int len0 = s0.Length + 1;
            int len1 = s1.Length + 1;

            int[] col = new int[len1];
            int[] prevCol = new int[len1];

            for (int i = 0; i < len1; i++)
            {
                prevCol[i] = i;
            }

            for (int i = 0; i < len0; i++)
            {
                col[0] = i;
                for (int j = 1; j < len1; j++)
                {
                    col[j] = Min(
                        Min(
                            1 + col[j - 1],
                            1 + prevCol[j]),
                        prevCol[j - 1] + (i > 0 && s0[i - 1] == s1[j - 1] ? 0 : 1));
                }

                (prevCol, col) = (col, prevCol);
            }

            float dist = prevCol[len1 - 1];

            return 1.0f - dist / Max(s0.Length, s1.Length);
        }

        public static int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }
    }
}