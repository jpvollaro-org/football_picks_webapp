using Newtonsoft.Json;
using static nfl_picks_pool.SportsApiViper;
using System;

namespace nfl_picks_pool
{
	public class GameScore
	{
		static public Random rnd = new Random();

		public int id { get; set; }

		public DateTime gameStartTimeLocalTime { get; set; }

		public string homeTeam { get; set; }

		public string awayTeam { get; set; }

		public int homeScore { get; set; }

		public int awayScore { get; set; }

		public string timeLeft { get; set; }

		public Boolean gameOfWeek { get; set; } = false;

		public Boolean pickGame { get; set; } = false;

		public Boolean finalScore { get; set; } = false;

		public int excelRowNumber { get; set; }	

		public int selectionNumber { get; set; } = 0;

		public string label { get; set; } = "";

		public GameScore() { }

		public GameScore(string awayTeamScore, string homeTeamScore)
		{
			string[] awaySplit = awayTeamScore.Split(':', '_');
			string[] homeSplit = homeTeamScore.Split(':', '_');
			this.awayTeam = awaySplit[0];
			this.awayScore = int.Parse(awaySplit[1]);
			this.homeTeam = homeSplit[0];
			this.homeScore = int.Parse(homeSplit[1]);
		}

		public GameScore(string awayTeam, int awayScore, string homeTeam, int homeScore, Boolean isFinalScore = false)
		{
			this.awayTeam = awayTeam;
			this.awayScore = awayScore;
			this.homeTeam = homeTeam;
			this.homeScore = homeScore;
			this.finalScore = isFinalScore;
		}

		public string GetWinningTeamName()
		{
			if (awayScore > homeScore)
				return awayTeam;
			else if (homeScore > awayScore)
				return homeTeam;
			else
				return homeTeam;
		}

		public string GetLosingTeamName()
		{
			if (awayScore > homeScore)
				return homeTeam;
			else if (homeScore > awayScore)
				return awayTeam;
			else
				return awayTeam;
		}
	}
}
