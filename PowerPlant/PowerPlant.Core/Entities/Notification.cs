using System;

namespace PowerPlant.Core.Entities
{
    public class Notification : Entity
    {
        public DateTime Date { get; set; }
        public string Message { get; set; }
    }
}
