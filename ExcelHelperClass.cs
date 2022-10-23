using ClosedXML.Excel;
using nfl_picks_pool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ReactProgramNS
{
	// Used to handle functions that read/write EXCEL FILES
	public class ExcelHelperClass
	{
		private ILogger _logger;

		private static readonly string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;

		public static readonly string PredictionsFileXls = workingDirectory + System.IO.Path.DirectorySeparatorChar +
			"Files\\NFL PREDICTIONS 2022.xlsx";
		
		private static List<GameScore> currentWeeklyGames = new List<GameScore>();

		private int currentExcelRowNumber = 1;

		public ExcelHelperClass(ILogger logger)
		{
			_logger = logger;	
		}

		private int ReadPlayersFromFile(IXLWorksheet ws, Dictionary<int, Player> playerDictionary)
		{
			int colNumber = 1;

			// First line in spreadsheet are the players in the pool
			var excelRow = ws.Row(currentExcelRowNumber++);
			try
			{
				for (; colNumber < 100; colNumber++)
				{
					string s = excelRow.Cell(colNumber).Value.ToString();
					if (string.IsNullOrEmpty(s))
						continue;
					if (s.ToUpper() == "POINTS")
						continue;
					if (s.ToUpper().StartsWith("FINAL"))
						return (--colNumber);
					Player player = new Player(s);
					player.id = colNumber;
					playerDictionary.Add(colNumber, player);
				}
			}
			catch
			{
			}
			return colNumber;
		}

		private void ReadPlayersChoicesFromFile(IXLWorksheet ws, Dictionary<int, Player> playerDictionary, int lastColumn)
		{
			try
			{
				for (; currentExcelRowNumber <= 10; currentExcelRowNumber++)
				{
					var excelRow = ws.Row(currentExcelRowNumber);
					for (int i = 2; i <= lastColumn; i++)
					{
						string s = excelRow.Cell(i).Value.ToString();
						if (string.IsNullOrEmpty(s))
							continue;

						switch (currentExcelRowNumber)
						{
							case 2:
								if (i == 8)
									playerDictionary[i - 2].currentPlayerPoints = int.Parse(s);
								else
									playerDictionary[i - 1].currentPlayerPoints = int.Parse(s);
								break;
							case 3:
								playerDictionary[i].afcChamps = s;
								break;
							case 4:
								playerDictionary[i].nfcChamps = s;
								break;
							case 5:
								playerDictionary[i].superbowlChamp = s;
								break;
							case 6:
								playerDictionary[i].leagueMvp = s;
								break;
							case 7:
								playerDictionary[i].teamBestRecord = s;
								break;
							case 8:
								playerDictionary[i].teamWorstRecord = s;
								break;
							case 9:
								playerDictionary[i].firstCoachFired = s;
								break;
							case 10:
								string[] teamSplit = s.Split(' ', '-');
								playerDictionary[i].favoriteTeam = teamSplit[0];
								break;
						}
					}
				}
			}
			catch
			{
			}
		}

		private void ReadPlayersWeeklyPicksFromFile(IXLWorksheet ws, Dictionary<int, Player> playerDictionary, int weekNumber, int lastColumn)
		{
			int workingWeekNumber = 0;
			currentWeeklyGames = new List<GameScore>();
			List<GameScore> currentGofWeekGames = new List<GameScore>();

			try
			{
				while (workingWeekNumber <= weekNumber)
				{
					var excelRow = ws.Row(currentExcelRowNumber++);
					string s = excelRow.Cell(1).Value.ToString();
					if (string.IsNullOrEmpty(s))
						continue;
					if (s.ToUpper().StartsWith("WEEK"))
					{
						workingWeekNumber++;
						continue;
					}

					for (int i = 1; i <= lastColumn; i++)
					{
						string pick = excelRow.Cell(i).Value.ToString();
						if (string.IsNullOrEmpty(pick))
							continue;

						if (i == 1)
						{
							GameScore score = WeeklyScoreboard.GetGameScore(workingWeekNumber, excelRow.Cell(1).Value.ToString());
							if (score != null)
							{
								score.excelRowNumber = currentExcelRowNumber-1;
								score.pickGame = true;
								if (int.TryParse(excelRow.Cell(2).Value.ToString(), out int points))
								{
									score.gameOfWeek = true;
									_logger.LogError(string.Format("{0}:{1} at {2}",
										score.awayTeam, score.homeTeam, score.gameStartTimeLocalTime));
									if (workingWeekNumber == weekNumber)
										currentGofWeekGames.Add(score);
								}
								else if (workingWeekNumber == weekNumber)
								{
									currentWeeklyGames.Add(score);
								}
							}
						}

						if (playerDictionary.ContainsKey(i))
						{
							if (playerDictionary[i].spreadsheetPicks[workingWeekNumber] == null)
								playerDictionary[i].spreadsheetPicks[workingWeekNumber] = new List<PickMetaData>();

							if (int.TryParse(pick, out int score))
							{
								playerDictionary[i].spreadsheetPicks[workingWeekNumber].Add(new PickMetaData(excelRow.Cell(1).Value.ToString() + ":" + pick));
							}
							else
							{
								playerDictionary[i].spreadsheetPicks[workingWeekNumber].Add(new PickMetaData(excelRow.Cell(1).Value.ToString()));
							}
						}
					}
				}
			}
			catch
			{
			}

			foreach (var g in currentGofWeekGames)
				currentWeeklyGames.Add(g);
		}

		public Dictionary<int, Player> ReadPredectionFile()
		{
			int currentWeek = nfl_picks_pool.ClassConstants.GetPickWeek();
			Dictionary<int, Player> playerDictionary = new Dictionary<int, Player>();
			using var wbook = new XLWorkbook(PredictionsFileXls);
			var ws = wbook.Worksheet("Sheet1");
			int lastColunm = ReadPlayersFromFile(ws, playerDictionary);
			ReadPlayersChoicesFromFile(ws, playerDictionary, lastColunm);
			ReadPlayersWeeklyPicksFromFile(ws, playerDictionary, currentWeek, lastColunm);
			return playerDictionary;
		}

		public static List<GameScore> GetWeeklyGameSelections()
		{
			return currentWeeklyGames;
		}

		public string WritePicks(List<GameScore> playerScoreSelections, Player playerEntry, int currentPickWeek)
		{
			if (playerEntry == null)
			{
				return "UNKNOWN PLAYER KEY DETECTED";
			}
			
			using var wbook = new XLWorkbook(PredictionsFileXls);
			var ws = wbook.Worksheet("Sheet1");
			foreach (var game in playerScoreSelections)
			{
				if (game.awayTeam == null && game.homeTeam == null)
					continue;

				GameScore scoreBoardGame = WeeklyScoreboard.GetGameScore(currentPickWeek, game.homeTeam);
				if (scoreBoardGame == null)
					continue;

				if (scoreBoardGame.gameOfWeek)
				{
					ws.Cell(scoreBoardGame.excelRowNumber-1, playerEntry.id).Value = game.awayScore;
					ws.Cell(scoreBoardGame.excelRowNumber, playerEntry.id).Value = game.homeScore;
				}
				else
				{
					if (game.awayScore > game.homeScore)
					   ws.Cell(scoreBoardGame.excelRowNumber-1, playerEntry.id).Value = game.awayTeam;
					else
					   ws.Cell(scoreBoardGame.excelRowNumber, playerEntry.id).Value = game.homeTeam;
				}
			}
			wbook.Save();

			return "SUCCESS";
		}
	}
}
