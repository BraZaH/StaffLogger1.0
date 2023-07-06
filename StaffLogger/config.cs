namespace StaffLogger
{
    using Exiled.API.Interfaces;
    using System.Collections.Generic;

    public sealed class config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public List<string> Roles_Staff { get; set; } = new List<string>() { "owner", "CoOwner", "highStaff", "admin", "moderator", "staffEventos", "helper"};
        public string discordWebhook { get; set; } = "sexhook";
        public string dbLink { get; set; } = "Acá pone el link bien epikkkkkkkkkkkk";
        public string Avatar_Url { get; set; } = "";
        public string Username { get; set; } = "Logs Staff";
    }
}
