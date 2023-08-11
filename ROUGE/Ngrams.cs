namespace ROUGE
{
    public class Ngrams
    {
        private readonly HashSet<string> ngrams = new();
        private readonly List<string> ngramsList = new();
        private readonly bool exclusive = true;

        public Ngrams(IEnumerable<string>? ngrams = null, bool exclusive = true)
        {
            this.exclusive = exclusive;

            if (exclusive) this.ngrams = new HashSet<string>(ngrams ?? Enumerable.Empty<string>());
            else ngramsList = new List<string>(ngrams ?? Enumerable.Empty<string>());
        }

        public void Add(string ngram)
        {
            if (exclusive) ngrams.Add(ngram);
            else ngramsList.Add(ngram);
        }

        public int Count()
        {
            return exclusive ? ngrams.Count : ngramsList.Count;
        }

        public Ngrams Intersection(Ngrams other)
        {
            if (exclusive)
            {
                var interSet = ngrams.Intersect(other.ngrams).ToHashSet();
                return new Ngrams(interSet, exclusive: true);
            }
            else
            {
                var otherList = new List<string>(other.ngramsList);
                var interList = new List<string>();

                foreach (var ngram in ngramsList)
                {
                    if (otherList.Contains(ngram))
                    {
                        otherList.Remove(ngram);
                        interList.Add(ngram);
                    }
                }
                return new Ngrams(interList, exclusive: false);
            }
        }

        public Ngrams Union(params Ngrams[] others)
        {
            if (exclusive)
            {
                var unionSet = ngrams;
                foreach (var other in others)
                    unionSet.UnionWith(other.ngrams);

                return new Ngrams(unionSet, exclusive);
            }
            else
            {
                var unionList = new List<string>(ngramsList);

                foreach (var other in others)
                    unionList.AddRange(other.ngramsList);

                return new Ngrams(unionList, exclusive);
            }
        }
    }
}
