using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using nfl_picks_pool.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace nfl_picks_pool
{
	public class WeeklyScoreboard
	{
		static public Dictionary<string, GameScore>[] pastResults = new Dictionary<string, GameScore>[32];
		static private readonly DateTime startThursdayDate = DateTime.Parse("2022-09-08");
        static private readonly object WeeklyScoreboardLock = new object();


        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
		{
			TextWriter writer = null;
			try
			{
				var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
				writer = new StreamWriter(filePath, append);
				writer.Write(contentsToWriteToFile);
			}
			finally
			{
				if (writer != null)
					writer.Close();
			}
		}

		public static void UpdateGameScoreFinalScore(int weekNumber, GameScore finalScore)
		{
			try
			{
				if (pastResults[weekNumber] == null)
					return;
				var weekScores = pastResults[weekNumber];
				var score = weekScores[finalScore.homeTeam];
				score.awayScore = finalScore.awayScore;
				score.homeScore = finalScore.homeScore;
				score.finalScore = true;
				score.timeLeft = "Final";
				WriteToJsonFile<Dictionary<string, GameScore>>(String.Format("Files{0}scoreboard_wk{1}.json", 
					Path.DirectorySeparatorChar, weekNumber), pastResults[weekNumber]);
			}
			catch
			{
			}
			return;
		}

		public static void BuildWeeklyScoreboard(ISportsApi sportsApi, ILogger logger)
		{
			int weekNumber = 1;
			int currentWeek = nfl_picks_pool.ClassConstants.GetPickWeek();
			DateTime gameDayDate = startThursdayDate;

			for (; weekNumber <= currentWeek; weekNumber++)
			{
				string fileName = String.Format("Files{0}scoreboard_wk{1}.json", Path.DirectorySeparatorChar, weekNumber);
				if (File.Exists(fileName))
				{
					string body = File.ReadAllText(fileName);
					pastResults[weekNumber] = JsonConvert.DeserializeObject<Dictionary<string, GameScore>>(body);
				}
				else
				{
					pastResults[weekNumber] = sportsApi.GetPastScheduleGames(gameDayDate).Result;
					fileName = String.Format("Files{0}scoreboard_wk{1}.json", Path.DirectorySeparatorChar, weekNumber);
					WriteToJsonFile<Dictionary<string, GameScore>>(fileName, pastResults[weekNumber]);
				}
				gameDayDate = gameDayDate.AddDays(7);
			}
		}

		public static void CalculatePoints(Dictionary<int, Player> playerTable, ILogger logger)
		{
			lock (WeeklyScoreboardLock)
			{

				PropBets.CalculatePropPoints(playerTable);

				int currentPickWeek = ClassConstants.GetPickWeek();
				for (int weekNumber = 1; weekNumber <= currentPickWeek; weekNumber++)
				{
					Dictionary<string, GameScore> gameScores = pastResults[weekNumber];
					foreach (var gameScore in gameScores)
					{
						int numCorrectPicks = 0;
						Player soleWinner = null;
						var g = gameScore.Value;
						if (g.finalScore == false)
							continue;
						GameOfWeekLogic gameOfWeekLogic = new GameOfWeekLogic();
						var winningTeamName = g.GetWinningTeamName();
						foreach (var playerEntry in playerTable)
						{
							Player p = playerEntry.Value;
							var myPicks = p.spreadsheetPicks[weekNumber];
							if (myPicks == null)
							{
								continue;
							}

							if (g.gameOfWeek == false)
							{
								var pickmd = myPicks.Where(x => x.pickString == winningTeamName).FirstOrDefault();
								if (pickmd != null)
								{
									numCorrectPicks++;
									p.currentPlayerPoints += ClassConstants.SELECT_WINNER_POINTS;
									pickmd.winner = true;
									soleWinner = p;
									pickmd.calculatePickScoreValue(weekNumber);
								}
							}
							else if (g.gameOfWeek == true)
							{
								var myAwayTeam = myPicks.Where(x => x.pickString.StartsWith(g.awayTeam)).FirstOrDefault();
								if (myAwayTeam == null)
									continue;
								gameOfWeekLogic.checkMyScore(myAwayTeam.pickString, gameScores);
								var myHomeTeam = myPicks.Where(x => x.pickString.StartsWith(g.homeTeam)).FirstOrDefault();
								if (myHomeTeam == null)
									continue;
								gameOfWeekLogic.checkMyScore(myHomeTeam.pickString, gameScores, p.id);
							}
						}
						if (g.gameOfWeek == false && numCorrectPicks == 1)
						{
								soleWinner.currentPlayerPoints += ClassConstants.SOLE_WINNER_POINTS;
								var pickmd = soleWinner.spreadsheetPicks[weekNumber]
									.Where(x => x.pickString == winningTeamName).FirstOrDefault();
								pickmd.soleWinner = true;
								pickmd.pointDifferences = 99;
                                pickmd.calculatePickScoreValue(weekNumber);
                        }
                        else if (g.gameOfWeek)
						{
							int gameOfWeekPointValue = weekNumber < 19 ? ClassConstants.GOFW_WINNER_POINTS : ClassConstants.PLAYOFF_WINNER_POINTS;
							foreach (var pid in gameOfWeekLogic.winningPlayers)
							{
								int bonusPoints = gameOfWeekLogic.GetBonusPoints(g, winningTeamName, ClassConstants.SOLE_WINNER_POINTS, ClassConstants.PERFECT_SCORE_POINTS);
								playerTable[pid].currentPlayerPoints += (gameOfWeekPointValue + bonusPoints);
								var pickmd = playerTable[pid].spreadsheetPicks[weekNumber].Where(
									x => x.pickString.StartsWith(g.homeTeam)).FirstOrDefault();
								pickmd.GofWeek= true;
								pickmd.winner = true;
								pickmd.soleWinner = gameOfWeekLogic.SoleWinner(g, winningTeamName);
								pickmd.pointDifferences = gameOfWeekLogic.ScoreDifference();
								pickmd.calculatePickScoreValue(weekNumber);
                            }
                        }
					}
				}
			}
		}

		public static void AddCompletedGames(ISportsApi sportsApi, List<GameScore> scores, int weekNumber)
		{
			if (pastResults[weekNumber] == null)
				pastResults[weekNumber] = new Dictionary<string, GameScore>();

			Dictionary<string,GameScore> results = pastResults[weekNumber];
			foreach(var result in results)
			{
				var score = result.Value;
				if ((score.pickGame) && (!scores.Any(s=>s.homeTeam == score.homeTeam)))
				{
					if (score.finalScore)
					{
						score.timeLeft = "Final";
						scores.Add(score);
					}
					else if (score.gameStartTimeLocalTime.AddHours(3) < DateTime.Now.ToLocalTime())
					{
						GameScore finalScore = sportsApi.GetFinalGameScore(score.id).Result;
						if (finalScore != null && finalScore.finalScore)
						{
							finalScore.timeLeft = "Final";
							scores.Add(finalScore);
							UpdateGameScoreFinalScore(weekNumber, finalScore);
						}
					}
				}
			}
			return;
		}

		public static GameScore GetGameScore(int workingWeekNumber, string v)
		{
			try
			{
				var weekly = pastResults[workingWeekNumber];
				GameScore score = weekly[v];
				return weekly[v];
			}
			catch
			{
			}
			return null;
		}
	}
}
