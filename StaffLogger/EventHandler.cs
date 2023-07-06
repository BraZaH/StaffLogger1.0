namespace StaffLogger
{
    using System;
    using Exiled.Events.EventArgs;
    using Exiled.API.Features;
    using System.Collections.Generic;
    using System.Linq;

    class EventHandler
    {
        private static Dictionary<string, int> TimeStaff = new Dictionary<string, int>();
        private Dictionary<string, int> roundStaff = new Dictionary<string, int>();
        private List<string> RolesStaff = main.Singleton.Config.Roles_Staff;
        private string webhookURL = main.Singleton.Config.discordWebhook;
        private string EndRoundInfo = "";

        public void JoinStaff(VerifiedEventArgs ev)
        {
            Int32 HoraSegundo = Math.Abs(Round.ElapsedTime.Hours) * 3600;
            Int32 MinutoSegundo = Math.Abs(Round.ElapsedTime.Minutes) * 60;
            Int32 tiempoRonda = HoraSegundo + MinutoSegundo + Math.Abs(Round.ElapsedTime.Seconds);

            if (Round.IsEnded) { return; }
            if (RolesStaff.Contains(ev.Player.GroupName))
            {
                if (!TimeStaff.ContainsKey(ev.Player.UserId))
                {
                    if (roundStaff.ContainsKey(ev.Player.UserId))
                    {
                        roundStaff[ev.Player.UserId] -= tiempoRonda;
                        if (roundStaff.TryGetValue(ev.Player.UserId, out int value))
                        {
                            TimeStaff.Add(ev.Player.UserId, value);
                        }
                        roundStaff.Remove(ev.Player.UserId);
                    }
                    else
                    {
                        TimeStaff.Add(ev.Player.UserId, - tiempoRonda);
                    }
                }
                else
                {
                    TimeStaff.Add(ev.Player.UserId, -tiempoRonda);
                }

                main.sendDiscordWebhook(webhookURL, main.Singleton.Config.Avatar_Url, main.Singleton.Config.Username,
                    $"```markdown\n" +
                    $"+ 🟢 {Date} | {ev.Player.GroupName} | {ev.Player.Nickname} ({ev.Player.UserId}) ha entrado al servidor.\n" +
                    $"```");
            }
        }

        public void LeaveStaff(LeftEventArgs ev)
        {
            // Pasar el tiempo actual de la ronda a segundos
            Int32 HoraSegundo = Math.Abs(Round.ElapsedTime.Hours) * 3600;
            Int32 MinutoSegundo = Math.Abs(Round.ElapsedTime.Minutes) * 60;
            Int32 tiempoRonda = HoraSegundo + MinutoSegundo + Math.Abs(Round.ElapsedTime.Seconds);

            if (Round.IsEnded) { return; }
            if (RolesStaff.Contains(ev.Player.GroupName))
            {
                if (TimeStaff.ContainsKey(ev.Player.UserId))
                {
                    TimeStaff[ev.Player.UserId] += Math.Abs(tiempoRonda);
                    if (TimeStaff.TryGetValue(ev.Player.UserId, out int value))
                    {
                        TimeStaff.Remove(ev.Player.UserId);
                        roundStaff.Add(ev.Player.UserId, value);
                    }
                    main.sendDiscordWebhook(webhookURL, main.Singleton.Config.Avatar_Url, main.Singleton.Config.Username,
                        $"```markdown\n" +
                        $"- 🔴 {Date} | {ev.Player.Nickname} ({ev.Player.UserId}) ha salido del servidor.\n" +
                        $"```");
                }
            }
        }

        public void OnRoundStart()
        {
            main.sendDiscordWebhook(webhookURL, main.Singleton.Config.Avatar_Url, main.Singleton.Config.Username,
                $"```diff\n" +
                $"+ ronda iniciada + | {Date}\n" +
                $"```");
        }

        public void OnRoundEnd(RoundEndedEventArgs ev)
        {
            if(roundStaff.IsEmpty() && TimeStaff.IsEmpty())
            {
                main.sendDiscordWebhook(webhookURL, main.Singleton.Config.Avatar_Url, main.Singleton.Config.Username,
                    $"```fix\n" +
                    $"Staff Registrados\n" +
                    $"```\n" +
                    $"```\n" +
                    $"🔻 No se registraron staffs esta ronda 🔻" +
                    $"```\n" +
                    $"```diff\n" +
                    $"- RONDA ACABADA - | {Date}\n" +
                    $"```");
                return;
            }
            // Pasar el tiempo actual de la ronda a segundos
            Int32 HoraSegundo = Math.Abs(Round.ElapsedTime.Hours) * 3600;
            Int32 MinutoSegundo = Math.Abs(Round.ElapsedTime.Minutes) * 60;
            Int32 tiempoRonda = HoraSegundo + MinutoSegundo + Math.Abs(Round.ElapsedTime.Seconds);

            // Suma el tiempo de la ronda a los jugadores usando un diccionario auxiliar para poder traspasar y modificar los datos

            foreach (var i  in TimeStaff)
            {
                roundStaff.Add(i.Key, i.Value + Math.Abs(tiempoRonda));
            }
            TimeStaff.Clear();
            var sortedRoundStaff = from entry in roundStaff orderby entry.Value descending select entry;
            // llama y guarda los datos
            foreach (var i in sortedRoundStaff)
            {
                Int32 horas = (i.Value / 3600);
                Int32 minutos = ((i.Value - horas * 3600) / 60);
                Int32 segundos = i.Value - (horas * 3600 + minutos * 60);

                EndRoundInfo += $"📄 {i.Key} | Estuvo en el la ronda {horas} horas, {minutos} minutos, {segundos} segundos.\n";
                main.sendData(new Features.DataReference(
                    i.Key,
                    i.Value,
                    DateTime.Now
                    ));

            }
            main.sendDiscordWebhook(webhookURL, main.Singleton.Config.Avatar_Url, main.Singleton.Config.Username,
                $"```fix\n" +
                $"Staff Registrados\n" +
                $"```\n" +
                $"```\n" +
                $"{EndRoundInfo}" +
                $"```\n" +
                $"```diff\n" +
                $"- RONDA ACABADA - | {Date}\n" +
                $"```");

            EndRoundInfo = "";
            roundStaff.Clear();
        }
        public static string Date => $"{DateTime.Now:d-M-y HH:mm:ss}";
    }
}
