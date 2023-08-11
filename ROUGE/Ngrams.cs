namespace ROUGE
{
    public class Ngrams
    {
        private readonly HashSet<List<string>> ngrams = new(new NgramsComparer());
        private readonly List<List<string>> ngramsList = new();
        private readonly bool exclusive = true;

        public Ngrams(IEnumerable<List<string>>? ngrams = null, bool exclusive = true)
        {
            this.exclusive = exclusive;

            if (exclusive) this.ngrams = new HashSet<List<string>>(ngrams ?? Enumerable.Empty<List<string>>(), new NgramsComparer());
            else ngramsList = new List<List<string>>(ngrams ?? Enumerable.Empty<List<string>>());
        }

        public void Add(List<string> ngram)
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
                var interSet = ngrams.Intersect(other.ngrams, new NgramsComparer()).ToHashSet();
                return new Ngrams(interSet, exclusive: true);
            }
            else
            {
                var otherList = new List<List<string>>(other.ngramsList);
                var interList = new List<List<string>>();

                var comparer = new NgramsComparer();
                foreach (var ngram in ngramsList)
                {
                    if (otherList.Contains(ngram, comparer))
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
                var unionList = new List<List<string>>(ngramsList);

                foreach (var other in others)
                    unionList.AddRange(other.ngramsList);

                return new Ngrams(unionList, exclusive);
            }
        }

        private class NgramsComparer : IEqualityComparer<List<string>>
        {
            public bool Equals(List<string>? x, List<string>? y)
            {
                // If both lists are null, they are equal
                if (x == null && y == null)
                    return true;

                // If one of the lists is null, they are not equal
                if (x == null || y == null)
                    return false;

                // Use SequenceEqual to determine if the two lists contain the same strings in the same order
                return x.SequenceEqual(y);
            }

            public int GetHashCode(List<string> obj)
            {
                if (obj == null)
                    return 0;

                // Compute a hash code for the list by combining the hash codes of its strings
                int hash = 17;
                foreach (var str in obj)
                {
                    hash = hash * 31 + (str != null ? str.GetHashCode() : 0);
                }
                return hash;
            }
        }
    }
}
