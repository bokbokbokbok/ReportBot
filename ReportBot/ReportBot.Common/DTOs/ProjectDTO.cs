﻿using McgTgBotNet.Attributes;

namespace McgTgBotNet.DTOs
{
    public class ProjectDTO
    {
        //[XMLProperty("id")]
        //public int Id { get; set; }
        [XMLProperty("name")]
        public string Name { get; set; } = string.Empty;
        [XMLProperty("description")]
        public string Description { get; set; } = string.Empty;
        [XMLProperty("status")]
        public string Status { get; set; } = string.Empty;
    }
}