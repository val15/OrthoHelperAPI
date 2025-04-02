using OrthoHelper.Domain.Features.Auth.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Infrastructure.Features.TextProcessing.Entities
{
       public class Message
    {
        public int Id { get; set; }
        public string InputText { get; set; }
        public string OutputText { get; set; }
        public string Diff { get; set; }
        public TimeSpan ProcessingTime { get; set; }

        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
