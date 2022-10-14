using System;
using System.Collections.Generic;
using System.Text;

namespace nfl_picks_pool
{
	public class PickMetaData
	{
		public int pointDifferences { get; set; } = 300;
		public string pickString { get; set; }
		public Boolean winner { get; set; }
		public Boolean soleWinner { get; set; }
		public Boolean winning {  get; set; }
		public Boolean losing { get; set; }
		public Boolean GofWeek { get; set; }
		public PickMetaData(string pickString)
		{
			this.pickString = pickString;
		}
	}

	public class Player : IComparable<Player>
	{
		public int id { get; set; }
		public string name { get; set; }
		public List<string> nameTeamCombo { get; set; }
		public int currentPlayerPoints { get; set; } = 0;
		public int futurePlayerPoints { get; set; }
		public int gameOfWeekDifference { get; set; }
		public string afcChamps { get; set; }
		public string nfcChamps { get; set; }
		public string superbowlChamp { get; set; }
		public string leagueMvp { get; set; }
		public string teamBestRecord { get; set; }
		public string teamWorstRecord { get; set; }
		public string firstCoachFired { get; set; }
		public string favoriteTeam { get; set; } 
		public int favoriteTeamWins { get; set; }
		public int favoriteTeamLosses { get; set; }
		public List<PickMetaData>[] spreadsheetPicks { get; set; }

		public Player(string playerName)
		{
			this.name = playerName;
			this.spreadsheetPicks = new List<PickMetaData>[32];
		}

		public int CompareTo(Player p)
		{
			if (p == null)
			{
				return 1;
			}

			return Comparer<int>.Default.Compare(p.currentPlayerPoints, this.currentPlayerPoints);
			//return Comparer<int>.Default.Compare(p.futurePlayerPoints, this.futurePlayerPoints);
			//return Comparer<int>.Default.Compare(this.gameOfWeekDifference, p.gameOfWeekDifference);

		}

		public string printAllPicks()
		{
			int totalPoints = 0;
			int weekPoints = 0;
			StringBuilder sb = new StringBuilder();

			sb.AppendLine(this.name);
			for(int idx=1; idx<32; idx++)
			{
				if (this.spreadsheetPicks[idx] == null)
					break;
				weekPoints = 0;
				sb.Append("PLAYERPOINTS");
				foreach (var pickmd in this.spreadsheetPicks[idx])
				{
					sb.Append(pickmd.pickString);
					if (pickmd.pickString.Contains(':'))
					{
						if (pickmd.winner)
						{
							sb.Append("+25");
							weekPoints += 25;
						}
					}
					else
					{
						if (pickmd.soleWinner)
						{
							sb.Append("+35");
							weekPoints += 35;
						}
						else if (pickmd.winner)
						{
							sb.Append("+10");
							weekPoints += 10;
						}
					}
					sb.Append(",");
				}
				sb.Replace("PLAYERPOINTS", String.Format("[{0}]", weekPoints));
				sb.AppendLine();
				totalPoints+=(weekPoints);
			}


			sb.Append("TotalPoints=" + (totalPoints + PropBets.CalculatePropPoints(this)));
			return sb.ToString();	
		}
	}
}
