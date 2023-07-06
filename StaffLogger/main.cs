namespace StaffLogger
{
    using Exiled.API.Features;
    using System.Collections.Specialized;
    using System.Collections.Generic;
    using System;
    using System.Net;
    using System.IO;
    using StaffLogger.Features;
    using Utf8Json;

    using Player = Exiled.Events.Handlers.Player;
    using server = Exiled.Events.Handlers.Server;

    public class main : Plugin<config>
    {
        public static main Singleton;
        private EventHandler Event;

        public override void OnEnabled()
        {
            Singleton = this;
            Event = new EventHandler();

            Player.Verified += Event.JoinStaff;
            Player.Left += Event.LeaveStaff;
            server.RoundStarted += Event.OnRoundStart;
            server.RoundEnded += Event.OnRoundEnd;

        }
        public override void OnDisabled()
        {
            Player.Verified -= Event.JoinStaff;
            Player.Left -= Event.LeaveStaff;
            server.RoundStarted -= Event.OnRoundStart;
            server.RoundEnded -= Event.OnRoundEnd;

            Singleton = null;
        }

        public static void sendDiscordWebhook(string URL, string profilepic, string username, string message)
        {
            NameValueCollection discordValues = new NameValueCollection();
            discordValues.Add("username", username);
            discordValues.Add("avatar_url", profilepic);
            discordValues.Add("content", message);
            new WebClient().UploadValues(URL, discordValues);
        }

        public static void sendData(DataReference reference)
        {
            WebRequest wb = WebRequest.Create(Singleton.Config.dbLink);
            wb.Method = "POST";
            wb.ContentType = "application/json";
            using (StreamWriter sex = new StreamWriter(wb.GetRequestStream()))
            {
                string data = JsonSerializer.ToJsonString(new DataReference(reference.UserID, reference.Time, reference.Date));
                sex.Write(data);
                sex.Close();
            }
            wb.GetResponse();
        }

    }
}
