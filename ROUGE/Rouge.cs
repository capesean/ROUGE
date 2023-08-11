namespace ROUGE
{
    public enum Metric { Rouge1 = 1, Rouge2 = 2, Rouge3 = 3, Rouge4 = 4, Rouge5 = 5, RougeL = 0 }
    public enum Stat { R, P, F, SystemCount, ReferenceCount, Overlap }

    public static class Rouge
    {
        // todo: allow caller to set these
        private static readonly List<Metric> Metrics = new() { Metric.Rouge1, Metric.Rouge2, Metric.Rouge3, Metric.Rouge4, Metric.Rouge5, Metric.RougeL };

        public static Dictionary<Metric, Score> GetScores(string systemSummary, string referenceSummary, bool exclusive = true)
        {
            var metricScores = new Dictionary<Metric, Score>();

            var systemSentences = systemSummary.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
                   .Select(sentence => string.Join(" ", sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)))
                   .ToList();

            var referenceSentences = referenceSummary.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
                   .Select(sentence => string.Join(" ", sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)))
                   .ToList();

            foreach (var metric in Metrics)
                if (metric != Metric.RougeL)
                    metricScores.Add(metric, GetMetricScore(systemSentences, referenceSentences, metric, exclusive));

            return metricScores;
        }

        private static Ngrams GetNgrams(int n, List<string> words, bool exclusive = true)
        {
            var ngramSet = new Ngrams(exclusive: exclusive);
            var textLength = words.Count();
            var maxIndexNgramStart = textLength - n;

            for (var i = 0; i <= maxIndexNgramStart; i++)
                ngramSet.Add(words.Skip(i).Take(n).ToList());

            return ngramSet;
        }

        private static List<string> SplitIntoWords(List<string> sentences)
        {
            return sentences.SelectMany(s => s.Split(' ')).ToList();
        }

        private static Ngrams GetWordNgrams(int n, List<string> sentences, bool exclusive = true)
        {
            if (sentences.Count == 0 || n <= 0)
                throw new ArgumentException("Number of sentences and n should be greater than 0");

            var words = SplitIntoWords(sentences);
            return GetNgrams(n, words, exclusive: exclusive);
        }

        private static Score GetMetricScore(
            List<string> systemSentences,
            List<string> referenceSentences,
            Metric metric = Metric.Rouge2,
            bool exclusive = true
            )
        {
            if (metric == Metric.RougeL) throw new NotImplementedException();
            if (!systemSentences.Any()) throw new Exception("System Summary is empty.");
            if (!referenceSentences.Any()) throw new Exception("Reference Summary is empty.");

            var systemNGrams = GetWordNgrams((int)metric, systemSentences, exclusive: exclusive);
            var referenceNGrams = GetWordNgrams((int)metric, referenceSentences, exclusive: exclusive);

            var systemCount = systemNGrams.Count();
            var referenceCount = referenceNGrams.Count();

            var overlappingNGrams = systemNGrams.Intersection(referenceNGrams);
            var overlappingCount = overlappingNGrams.Count();

            var p = systemCount == 0 ? 0 : overlappingCount / (double)systemCount;
            var r = referenceCount == 0 ? 0 : overlappingCount / (double)referenceCount;
            var f = 2.0 * (p * r / (p + r + 1e-8));

            return new Score
            {
                Hyp = systemCount,
                Ref = referenceCount,
                Overlap = overlappingCount,
                P = p,
                R = r,
                F = f
            };
        }

    }
}