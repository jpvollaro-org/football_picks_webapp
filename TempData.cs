using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace nfl_picks_pool
{
	public class GamedayAwayHomeTeamSelection
	{
        public string value { get; set; }
        public string label { get; set; }
        public GamedayAwayHomeTeamSelection()
        {
        }

        public GamedayAwayHomeTeamSelection(string myValue)
        {
            value = myValue;
            label = myValue;
        }

        public GamedayAwayHomeTeamSelection(string myLabel, string myValue)
        {
            value = myValue;
            label = myLabel;
        }
    }

	public class SelectionClass
	{
		public List<GameScore> standardGames { get; set; }
		public List<GameScore> gofWeekGames { get; set; }
	}

	public class TempPlayerData : IComparable<TempPlayerData>
	{
		public string name { get; set; }
		public List<string> nameTeamCombo { get; set; }
		public List<PickMetaData> spreadsheetPicks { get; set; }
		public int currentPlayerPoints { get; set; }
		public int gameOfWeekDifference { get; set; }

		public TempPlayerData(Player player, int pickWeek, string pickFilter = "")
		{
			int currentWeek = ClassConstants.GetPickWeek();
			this.name = player.name;
			this.nameTeamCombo = new List<string>();
			this.nameTeamCombo.Add(player.name);
			this.nameTeamCombo.Add(player.favoriteTeam + ".png");
			this.nameTeamCombo.Add(player.id.ToString());
			this.spreadsheetPicks = new List<PickMetaData>();

			if (player.spreadsheetPicks[pickWeek] != null)
			{
				DateTime cutoffDateTime = DateTime.Now.AddHours(+4.0);
				foreach (var pick in player.spreadsheetPicks[pickWeek])
				{
					if (!String.IsNullOrEmpty(pickFilter))
					{
						int stringIndex = pick.pickString.IndexOf(':');
						if (stringIndex > 0)
						{
							if (!pickFilter.Contains(pick.pickString.Substring(0, stringIndex)))
								continue;
						}
					}
					this.spreadsheetPicks.Add(pick);
				}
			}
			this.currentPlayerPoints = player.currentPlayerPoints;
			this.gameOfWeekDifference = 300;
		}

		public int CompareTo(TempPlayerData p)
		{
			if (p == null)
			{
				return 1;
			}

			return Comparer<int>.Default.Compare(p.currentPlayerPoints, this.currentPlayerPoints);
		}
	}

	public class TempData
	{
		public string scoreLine { get; set; }
		public List<TempPlayerData> players { get; set; }
		public int lowestPointDifference { get; set; }
	}

	public class TempData2
	{
		public string playerPicks { get; set; }
	}
}
