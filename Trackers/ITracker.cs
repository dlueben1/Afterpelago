using Afterpelago.Models;

namespace Afterpelago.Trackers
{
    public interface ITracker
    {
        void ParseLine(LogEntry entry);
        void Save();
    }
}
