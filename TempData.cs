using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace nfl_picks_pool
{
	public class TempPlayerData : IComparable<TempPlayerData>
	{
		public string name { get; set; }
		public List<string> nameTeamCombo { get; set; }
		public List<PickMetaData> spreadsheetPicks { get; set; }
		public int currentPlayerPoints { get; set; }
		public int gameOfWeekDifference { get; set; }

		public TempPlayerData(Player player, int pickWeek)
		{
			this.name = player.name;
			this.nameTeamCombo = new List<string>();
			this.nameTeamCombo.Add(player.name);
			this.nameTeamCombo.Add(player.favoriteTeam + ".png");
			this.nameTeamCombo.Add(player.id.ToString());
			if (player.spreadsheetPicks[pickWeek] == null)
			{
				this.spreadsheetPicks = new List<PickMetaData>();
			}
			else
			{
				this.spreadsheetPicks = player.spreadsheetPicks[pickWeek];
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
