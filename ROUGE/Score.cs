namespace ROUGE
{
    public class Score
    {
        public int? Hyp { get; set; }
        public int? Ref { get; set; }
        public int? Overlap { get; set; }
        public double? R { get; set; }
        public double? P { get; set; }
        public double? F { get; set; }
    }
}