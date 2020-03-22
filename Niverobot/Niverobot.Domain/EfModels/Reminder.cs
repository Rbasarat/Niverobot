using System;
using System.Collections.Generic;
using System.Text;

namespace Niverobot.Domain.EfModels
{
    class Reminder
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
    }
}
