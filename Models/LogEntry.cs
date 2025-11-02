namespace Afterpelago.Models
{
    public enum LogEntryType: byte
    {
        Unknown,
        HostStart,
        PlayerConnection,
        CheckFound,
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public required string Message { get; set; }
        public LogEntryType Category { get; set; }
    }

    public class ConnectionLogEntry : LogEntry
    {
        public required string SlotName { get; set; }
        public required string GameName { get; set; }
        public required string ClientVersion { get; set; }
        public ConnectionLogEntry()
        {
            Category = LogEntryType.PlayerConnection;
        }
    }

    public class CheckObtainedLogEntry : LogEntry
    {
        public required string SenderName { get; set; }
        public required string ReceiverName { get; set; }
        public required string ItemName { get; set; }
        public required string LocationName { get; set; }
        public bool IsSelfCollection
        {
            get
            {
                return SenderName == ReceiverName;
            }
        }

        public Slot? Sender
        {
            get
            {
                Slot? slot;
                if(Archipelago.Slots.TryGetValue(SenderName, out slot))
                {
                    return slot;
                }
                return null;
            }
        }

        public Slot? Receiver
        {
            get
            {
                Slot? slot;
                if (Archipelago.Slots.TryGetValue(ReceiverName, out slot))
                {
                    return slot;
                }
                return null;
            }
        }

        public CheckObtainedLogEntry()
        {
            Category = LogEntryType.CheckFound;
        }
    }
}
