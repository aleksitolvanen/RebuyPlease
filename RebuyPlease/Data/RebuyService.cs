using System;
using System.Collections.Concurrent;


namespace RebuyPlease.Data
{
    public class RebuyService
    {
        public Rebuys Rebuys { get; }

        public RebuyService()
        {
            Rebuys = new Rebuys();
            InitializeTestData();
        }

        private void InitializeTestData()
        {
            Rebuys.AddPlayer("Aleksi");
            Rebuys.AddPlayer("Aleksi2");
            Rebuys.AddPlayer("Aleksi3");
            Rebuys.AddPlayer("Aleksi4");
            Rebuys.AddTournament("Turnaus1");
            Rebuys.AddTournament("Turnaus2");
            Rebuys.AddTournament("Turnaus3");
            Rebuys.AddTournament("Turnaus4");
        }
    }

    public class Rebuys
    {
        public ConcurrentDictionary<string, Player> Players { get; }
        public ConcurrentDictionary<string, Tournament> Tournaments { get; }
        public ConcurrentStack<string> AuditLog { get; }

        public Rebuys()
        {
            Players = new ConcurrentDictionary<string, Player>();
            Tournaments = new ConcurrentDictionary<string, Tournament>();
            AuditLog = new ConcurrentStack<string>();
        }

        public void AddRebuy(string playerName, string tournament, int rebuyAmount)
        {
            if (Players.TryGetValue(playerName, out var player))
            {
                player.AddTournamentRebuy(tournament, rebuyAmount);
                AuditLog.Push($"Rebuy incremented with {rebuyAmount} to '{playerName}' in {tournament}.");
            }
        }

        public void AddPlayer(string name)
        {
            if (Players.TryAdd(name, new Player(name)))
            {
                AuditLog.Push($"Player '{name}' added");
            }
        }

        public void AddTournament(string name)
        {
            if (Tournaments.TryAdd(name, new Tournament(name)))
            {
                AuditLog.Push($"Tournament '{name}' added");
            }
        }
    }

    public class Player
    {
        public string Name { get; }

        public DateTime CreatedAt { get; }

        private ConcurrentDictionary<string, int> Rebuys { get; set; }

        public Player(string name)
        {
            Name = name;
            Rebuys = new ConcurrentDictionary<string, int>();
            CreatedAt = DateTime.UtcNow;
        }

        public int GetRebuysForTournament(string tournament)
        {
            Rebuys.TryGetValue(tournament, out int rebuys);
            return rebuys;
        }

        public ConcurrentDictionary<string, int> GetTournamentRebuys()
        {
            return Rebuys;
        }

        public void AddTournamentRebuy(string tournamentName, int rebuyAmount)
        {
            Rebuys.AddOrUpdate(tournamentName, rebuyAmount, (k, v) => v + rebuyAmount);
        }

    }
    public class Tournament
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; }

        public Tournament(string name)
        {
            Name = name;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
