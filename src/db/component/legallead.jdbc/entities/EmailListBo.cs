﻿namespace legallead.jdbc.entities
{
    public class EmailListBo
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? FromAddress { get; set; }
        public string? ToAddress { get; set; }
        public string? Subject { get; set; }
        public int? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
