using System;

namespace Niverobot.Domain.EfModels
{
    public class Reminder
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string SenderUserName { get; set; }
        public DateTime TriggerDate { get; set; }
        public int SenderId { get; set; }
        public long ReceiverId { get; set; }
    }
}
