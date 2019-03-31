using PTLive.Fetcher;
using PTLive.PoeJSONFileManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLive.StatsGenerator
{
    public static class StatsGenerator
    {
        public static TimeSpan TimeDifference(LeagueInfo a, LeagueInfo b)
        {
            var d1 = Convert.ToDateTime(a.ladder.cached_since);
            var d2 = Convert.ToDateTime(b.ladder.cached_since);
            var diff = (d1 - d2);//.ToString("hh:mm:ss");
            return diff;
        }

        public static List<AccountInfo> GetAccountsInfo(LeagueInfo lf)
        {
            Dictionary<string, AccountInfo> accInfos = new Dictionary<string, AccountInfo>();

            foreach (var entry in lf.ladder.entries)
            {
                // Create account object if does not exist.
                if (!accInfos.ContainsKey(entry.account.name)) accInfos.Add(entry.account.name, new AccountInfo(entry.account.name));

                // Update info
                CharInfo chInf = new CharInfo(entry.character, !entry.dead);
                accInfos[entry.account.name].characters.Add(chInf);
            }

            List<AccountInfo> result = accInfos.Select(d => d.Value).ToList();

            return result;
        }

        public static List<ClassInfo> GetClassesInfo(LeagueInfo lf)
        {
            Dictionary<string, ClassInfo> accInfos = new Dictionary<string, ClassInfo>() {
                {"Duelist", new ClassInfo("Duelist", false)},
                {"Champion", new ClassInfo("Champion", true)},
                {"Gladiator", new ClassInfo("Gladiator", true)},
                {"Slayer", new ClassInfo("Slayer", true)},
                {"Shadow", new ClassInfo("Shadow", false)},
                {"Trickster", new ClassInfo("Trickster", true)},
                {"Saboteur", new ClassInfo("Saboteur", true)},
                {"Assassin", new ClassInfo("Assassin", true)},
                {"Marauder", new ClassInfo("Marauder", false)},
                {"Chieftain", new ClassInfo("Chieftain", true)},
                {"Berserker", new ClassInfo("Berserker", true)},
                {"Juggernaut", new ClassInfo("Juggernaut", true)},
                {"Witch", new ClassInfo("Witch", false)},
                {"Elementalist", new ClassInfo("Elementalist", true)},
                {"Occultist", new ClassInfo("Occultist", true)},
                {"Necromancer", new ClassInfo("Necromancer", true)},
                {"Ranger", new ClassInfo("Ranger", false)},
                {"Deadeye", new ClassInfo("Deadeye", true)},
                {"Raider", new ClassInfo("Raider", true)},
                {"Pathfinder", new ClassInfo("Pathfinder", true)},
                {"Templar", new ClassInfo("Templar", false)},
                {"Inquisitor", new ClassInfo("Inquisitor", true)},
                {"Hierophant", new ClassInfo("Hierophant", true)},
                {"Guardian", new ClassInfo("Guardian", true)},
                {"Scion", new ClassInfo("Scion", false)},
                {"Ascendant", new ClassInfo("Ascendant", true)}
            };

            foreach (var entry in lf.ladder.entries)
            {
                accInfos[entry.character.charClass].totalChars += 1;
                if (!entry.dead) accInfos[entry.character.charClass].aliveChars += 1;
                if (entry.character.experience > accInfos[entry.character.charClass].highestExperienceReached && !entry.dead)
                {
                    accInfos[entry.character.charClass].highestExperienceReached = entry.character.experience;
                    accInfos[entry.character.charClass].highestLevelCurrentlyAlive = entry.character.level;
                    accInfos[entry.character.charClass].highestAliveAccountName = entry.account.name;
                }
                if (entry.character.level > accInfos[entry.character.charClass].highestLevelReached)
                {
                    accInfos[entry.character.charClass].highestLevelReached = entry.character.level;
                }
            }
            return accInfos.Select(d => d.Value).ToList<ClassInfo>();
        }

        public static List<string> ListOfAccMultipleCharsAlive(List<AccountInfo> accInfo)
        {
            var result = new List<string>();

            foreach (var account in accInfo)
            {
                if (account.AliveCharacters > 1)
                {
                    result.Add(account.accName);
                }
            }
            return result;
        }

        public static LeagueStats GenerateLeagueStats(LeagueInfo lf, int indexOfActivePlayersOldFile)
        {
            // Result
            LeagueStats result = new LeagueStats();

            // Accounts Info
            var accsInfo = GetAccountsInfo(lf);

            //*** result.AccountsParticipating
            result.AccountsParticipating = accsInfo.Count();

            //*** result.TotalCharacters
            result.TotalCharacters = lf.ladder.entries.Count();

            foreach (var entry in lf.ladder.entries)
            {
                //*** result.DeadCharacters
                if (entry.dead) result.DeadCharacters++;

                //*** result.AliveCharacters
                if (!entry.dead) result.AliveCharacters++;

                //*** result.DeadAtLevel[1-100]
                if (entry.dead) result.DeadAtLevel[entry.character.level] += 1;

                //*** result.CharsAliveAboveLvl10 & result.CharsAliveAboveLvl50
                if (!entry.dead && entry.character.level > 10) result.CharsAliveAboveLvl10++;
                if (!entry.dead && entry.character.level > 50) result.CharsAliveAboveLvl50++;

                //*** result.DeadBeforeLvl10
                if (entry.dead && entry.character.level < 10) result.DeadBeforeLvl10++;
            }

            //*** result.accountableForMostDeaths
            // Sort by death count accsInfo.
            List<AccountInfo> orderedByPopularity = accsInfo.OrderByDescending(d => d.DeadCharacters).ToList<AccountInfo>();
            // Add first 10 to the result.accountableForMostDeaths
            int count = (accsInfo.Count() >= 50) ? count = 50 : count = accsInfo.Count();
            for (int i = 0; i < count; i++)
            {
                result.accountableForMostDeaths.Add(new AccountDeathsInfo(orderedByPopularity[i].accName, orderedByPopularity[i].DeadCharacters));
            }

            //*** result.ActivePlayers
            // Check active players.
            LeagueInfo oldLInf = PoeFilesControl.GetFileByIndex(indexOfActivePlayersOldFile);
            //LeagueInfo oldLInf = PoeFilesControl.GetFileByIndex(1);
            LeagueInfo newLInfo = lf;
            int activePlayers = 0;
            foreach (var oldEntry in oldLInf.ladder.entries)
            {
                if (oldEntry.dead) continue;
                var newEntry = newLInfo.ladder.entries.Find((entr) => entr.character.name == oldEntry.character.name && entr.dead == false);
                if (newEntry != null)
                {
                    if (oldEntry.character.experience != newEntry.character.experience) activePlayers++;
                }
            }
            result.ActivePlayers = activePlayers;

            return result;
        }

    }
}
