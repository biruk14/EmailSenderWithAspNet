﻿namespace EmailSender.DTOs;

public class EmailDTO
{
    public string To { get; set; }
    public string? CC { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public IFormFile? Attachment { get; set; }
}
