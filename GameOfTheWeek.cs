using System.Collections.Generic;
using System;

namespace nfl_picks_pool
{
	public class GameOfWeekLogic
	{
		public int currentWinningPointDifference = 9999;
		public List<int> winningPlayers = new List<int>();
		string firstTeamScore = string.Empty;
		int HomeTeamSelections = 0;
		int AwayTeamSelections = 0;

		public PickMetaData checkMyScore(string scoreString, Dictionary<string, GameScore> scoreBoard, int playerNumber = 0)
		{
			try
			{
				if (firstTeamScore == String.Empty)
				{
					firstTeamScore = scoreString;
					return null;
				}

				PickMetaData pickMetaData = new PickMetaData(scoreString);
				GameScore checkScore = new GameScore(firstTeamScore, scoreString);
				GameScore score = scoreBoard[checkScore.homeTeam];
				pickMetaData.pointDifferences = Math.Abs(checkScore.awayScore - score.awayScore) +
					Math.Abs(checkScore.homeScore - score.homeScore);

				var myTeamWinning = checkScore.GetWinningTeamName();
				var teamWinning = score.GetWinningTeamName();
				if (myTeamWinning == teamWinning)
				{
					if (checkScore.homeTeam == teamWinning)
						HomeTeamSelections++;
					else if (checkScore.awayTeam == teamWinning)	
						AwayTeamSelections++;	
					pickMetaData.GofWeek = true;
					if (pickMetaData.pointDifferences <= currentWinningPointDifference)
					{
						if (pickMetaData.pointDifferences != currentWinningPointDifference)
						{
							winningPlayers.Clear();
						}
						currentWinningPointDifference = pickMetaData.pointDifferences;
						winningPlayers.Add(playerNumber);
					}
				}

				firstTeamScore = String.Empty;
				return pickMetaData;
			}
			catch
			{
				return null;
			}
		}

		public List<int> GetWinningPlayers()
		{
			return this.winningPlayers;
		}

		public int GetBonusPoints(GameScore score, string winningTeamName)
		{
			if (score.homeTeam == winningTeamName && HomeTeamSelections == 1)
				return 25;
			if (score.awayTeam == winningTeamName && AwayTeamSelections == 1)
				return 25;
			else
				return 0;
		}
	}
}
