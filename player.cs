﻿using System;
using System.Collections.Generic;
using System.Text;

namespace nfl_picks_pool
{
	public class WeeklyPlayerData
	{
		public int points { get; set; }
		public string picks { get; set; }
	}

	public class TabularPlayerData
	{
		public string playerName { get; set; }
		public int totalPoints { get; set; }
		public int weeklyPoints { get; set; }
		public int bonusPoints { get; set; }
		public List<WeeklyPlayerData> weeklyData { get; set; }
	}

	public class PickMetaData
	{
		public string homeTeam { get; set; } 
		public int pointDifferences { get; set; } = 300;
		public string pickString { get; set; }
		public Boolean winner { get; set; }
		public Boolean soleWinner { get; set; }
		public Boolean winning {  get; set; }
		public Boolean losing { get; set; }
		public Boolean GofWeek { get; set; }
		public int pickScoreValue { get; set; } = 0;
		public PickMetaData(string pickString, string homeTeam="")
		{
			this.pickString = pickString;
			this.homeTeam = homeTeam;
		}

        internal void calculatePickScoreValue(int weekNumber)
        {
			int gofwPointValue = 0;
			if (weekNumber < 19)
				gofwPointValue = ClassConstants.GOFW_WINNER_POINTS;
			else
				gofwPointValue = ClassConstants.PLAYOFF_WINNER_POINTS;

            if (winner == false)
			{
				pickScoreValue= 0;
				return;
			}

			if (GofWeek== false)
			{
				pickScoreValue = ClassConstants.SELECT_WINNER_POINTS;
				if (soleWinner)
					pickScoreValue+= ClassConstants.SOLE_WINNER_POINTS;
				return;
			}

			pickScoreValue= gofwPointValue;
			if (soleWinner)
				pickScoreValue += ClassConstants.SOLE_WINNER_POINTS;
			if (pointDifferences == 0)
				pickScoreValue += ClassConstants.PERFECT_SCORE_POINTS;
        }
    }

	public class Player : IComparable<Player>
	{
		public int id { get; set; }
		public string name { get; set; }
		public List<string> nameTeamCombo { get; set; }
		public int currentPlayerPoints { get; set; } = 0;
		public int bonusPlayerPoints { get; set; } = 0;
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
		}

		public TabularPlayerData GetTabularPlayerData()
		{
			TabularPlayerData _myNewNewTable = new TabularPlayerData();
			_myNewNewTable.playerName = this.name;
			_myNewNewTable.weeklyData = new List<WeeklyPlayerData>();
			_myNewNewTable.totalPoints = 0;
			_myNewNewTable.weeklyPoints = 0;
			_myNewNewTable.bonusPoints = PropBets.CalculatePropPoints(this);

			for(int idx=1; idx<=ClassConstants.GetPickWeek(); idx++)
			{
				var x = new WeeklyPlayerData()
				{
					points = 0,
					picks = ""
				};
				_myNewNewTable.weeklyData.Add(x);

				if (this.spreadsheetPicks[idx] == null)
				{
					continue;
				}

				StringBuilder sb = new StringBuilder();
				StringBuilder sbNew = new StringBuilder();
				foreach (var pickmd in this.spreadsheetPicks[idx])
				{
					sb.Append(pickmd.pickString);
					sbNew.Append(pickmd.pickString);
					x.points += pickmd.pickScoreValue;
					if (pickmd.pickScoreValue != 0)
					{
						sbNew.Append($"+{pickmd.pickScoreValue}");
					}
					sb.Append(", ");
					sbNew.Append(", ");
				}
				_myNewNewTable.weeklyPoints += x.points;
				x.picks = sbNew.ToString();
			}
			_myNewNewTable.totalPoints = _myNewNewTable.weeklyPoints + _myNewNewTable.bonusPoints;
			return _myNewNewTable;
		}
	}
}
