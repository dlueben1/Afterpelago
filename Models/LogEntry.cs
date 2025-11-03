using System.Runtime.Serialization;

namespace Afterpelago.Models
{
    public enum LogEntryType: byte
    {
        Unknown,
        HostStart,
        PlayerConnection,
        PlayerDisconnect,
        CheckFound,
        Release,
        ServerShutdown
    }

    /// <summary>
    /// Represents an entry (line) in a Log File.
    /// 
    /// `[DataContract]` is explicitly required for serialization over Web Workers.
    /// </summary>
    [KnownType(typeof(CheckObtainedLogEntry))]
    [KnownType(typeof(CheckObtainedLogEntry[]))]
    [DataContract]
    public class LogEntry
    {
        [DataMember]
        public DateTime Timestamp { get; set; }
        [DataMember]
        public required string Message { get; set; }
        [DataMember]
        public LogEntryType Category { get; set; }
    }

    [DataContract]
    public class ReleaseLogEntry : LogEntry
    {
        /// <summary>
        /// The Checks that were completed by releasing the game
        /// </summary>
        [DataMember]
        public Check[] ChecksFromRelease { get; set; }

        [DataMember]
        public required string SlotName { get; set; }

        [IgnoreDataMember]
        public Slot? Slot
        {
            get
            {
                Slot? slot;
                if (Archipelago.Slots.TryGetValue(SlotName, out slot))
                {
                    return slot;
                }
                return null;
            }
        }

        public ReleaseLogEntry()
        {
            Category = LogEntryType.Release;
            ChecksFromRelease = new Check[0];
        }
    }

    [DataContract]
    public class ConnectionLogEntry : LogEntry
    {
        [DataMember]
        public required string SlotName { get; set; }

        [IgnoreDataMember]
        public Slot? Slot
        {
            get
            {
                Slot? slot;
                if (Archipelago.Slots.TryGetValue(SlotName, out slot))
                {
                    return slot;
                }
                return null;
            }
        }

        [DataMember]
        public required string GameName { get; set; }
        [DataMember]
        public required string ClientVersion { get; set; }
        public ConnectionLogEntry()
        {
            Category = LogEntryType.PlayerConnection;
        }
    }

    [DataContract]
    public class DisconnectLogEntry : LogEntry
    {
        [DataMember]
        public required string SlotName { get; set; }
        public DisconnectLogEntry()
        {
            Category = LogEntryType.PlayerDisconnect;
        }
    }

    [DataContract]
    public class CheckObtainedLogEntry : LogEntry
    {
        [DataMember]
        public required string SenderName { get; set; }
        [DataMember]
        public required string ReceiverName { get; set; }
        [DataMember]
        public required string ItemName { get; set; }
        [DataMember]
        public required string LocationName { get; set; }

        [DataMember]
        public int ObtainedOrder { get; set; }

        [IgnoreDataMember]
        public bool IsSelfCollection
        {
            get
            {
                return SenderName == ReceiverName;
            }
        }
        [IgnoreDataMember]
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
        [IgnoreDataMember]
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
