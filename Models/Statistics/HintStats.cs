namespace Afterpelago.Models.Statistics
{
    public class HintStats
    {
        public Dictionary<string, int> HintsByPlayer { get; set; }
        public Dictionary<string, int> HintsReferencingPlayer { get; set; }
        public Dictionary<string, TimeSpan> TimeToFulfillHints { get; set; }
        public int TotalHintCount { get; set; }
        public string PlayerWithMostHints { get; set; }
        public string PlayerMostReferencedInHints{ get; set; }
        public HintLogEntry? FastestPayoff { get; set; }
        public HintLogEntry? LongestPayoff { get; set; }
        public HintLogEntry FirstHint { get; set; }
    }
}
