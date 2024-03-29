﻿using System;

namespace Mail2Gotify.Models
{
    public class GotifyMessage
    {
        public int? AppId { get; set; }

        public DateTime? Date { get; set; }

        public int? Id { get; set; }

        public string Message { get; set; }

        public int? Priority { get; set; }

        public string Title { get; set; }
    }
}
