namespace Utility.PatternMatchings
{
    public record PatternMatchResult
    {
        public PatternMatchKind Kind { get; }
        public int Score { get; set; }
        public IList<TextSpan> MatchedSpans { get; }

        public PatternMatchResult(
            PatternMatchKind kind,
            int score,
            IList<TextSpan> spans)
        {
            Kind = kind;
            Score = score;
            MatchedSpans = spans;
        }
    }

}

