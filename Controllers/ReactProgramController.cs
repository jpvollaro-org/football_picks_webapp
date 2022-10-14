using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using nfl_picks_pool;
using nfl_picks_pool.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace ReactProgramNS.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReactProgramController : ControllerBase
	{
		private readonly ILogger<ReactProgramController> _logger;
		private static Dictionary<int, Player> playerTable = null;
		private static int currentWeekNumber = ClassConstants.GetPickWeek();
		private static ISportsApi _sportsApi = new SportsApiViper();

		public ReactProgramController(ILogger<ReactProgramController> logger)
		{
			_logger = logger;
			if (playerTable == null)
			{
				WeeklyScoreboard.BuildWeeklyScoreboard(_sportsApi);
				ExcelHelperClass excelHelperClass = new ExcelHelperClass();
				playerTable = excelHelperClass.ReadPredectionFile();
				WeeklyScoreboard.CalculatePoints(playerTable);
				PropBets.CalculatePropPoints(playerTable);
			}
		}

		[HttpGet]
		[Route("GetPlayerData")]
		public TempData2 GetPlayerData(int playerId)
		{
			TempData2 tempData2 = new TempData2();
			tempData2.playerPicks = playerTable[playerId].printAllPicks();
			return tempData2;
		}

		[HttpGet]
		[Route("GetPlayerStandings")]
		public List<TempPlayerData> GetPlayerStandings()
		{
			List<TempPlayerData> playerList = new List<TempPlayerData>();
			foreach(var entry in playerTable)
			{
				TempPlayerData tempPlayerData = new TempPlayerData(entry.Value, currentWeekNumber);
				playerList.Add(tempPlayerData);
			}
			playerList.Sort();
			return playerList;
		}

		[HttpGet]
		[Route("~/api/ReactProgram/getProgramData")]
		public async Task<TempData> getProgramData()
		{
			TempData result = new TempData();
			try
			{
				var scores = await _sportsApi.GetLiveGameScores();
				WeeklyScoreboard.AddCompletedGames(_sportsApi, scores, nfl_picks_pool.ClassConstants.GetPickWeek());

				Dictionary<string, GameScore> scoreBoard = new Dictionary<string, GameScore>();

				StringBuilder sb = new StringBuilder();
				var winning = new List<string>();
				var losing = new List<string>();
				foreach (GameScore score in scores)
				{
					if (score.homeScore > score.awayScore)
					{
						winning.Add(score.homeTeam);
						losing.Add(score.awayTeam);
					}
					else if (score.homeScore < score.awayScore)
					{
						winning.Add(score.awayTeam);
						losing.Add(score.homeTeam);
					}
					scoreBoard.Add(score.homeTeam, score);
					sb.Append(string.Format("{0}:{1}-{2}:{3} {4} {5}",
						score.awayTeam,
						score.awayScore,
						score.homeTeam,
						score.homeScore,
						score.timeLeft,
						System.Environment.NewLine
						));
				}
				result.scoreLine = sb.ToString();

				GameOfWeekLogic gameOfWeekLogic = new GameOfWeekLogic();
				result.players = new List<TempPlayerData>();
				foreach (var playerEntry in playerTable)
				{
					Player player = playerEntry.Value;
					TempPlayerData tempPlayerData = new TempPlayerData(player, currentWeekNumber);
					for (int idx = 0; idx < tempPlayerData.spreadsheetPicks.Count; idx++)
					{
						string p = tempPlayerData.spreadsheetPicks[idx].pickString;
						string homeTeamKey = "";

						if (p.Contains(':'))
						{
							var pickMetaData = gameOfWeekLogic.checkMyScore(p, scoreBoard);
							if (pickMetaData == null)
								continue;
							tempPlayerData.spreadsheetPicks[idx - 1].pointDifferences = pickMetaData.pointDifferences;
							tempPlayerData.spreadsheetPicks[idx - 1].GofWeek = pickMetaData.GofWeek;
							tempPlayerData.spreadsheetPicks[idx].pointDifferences = pickMetaData.pointDifferences;
							tempPlayerData.spreadsheetPicks[idx].GofWeek = pickMetaData.GofWeek;
							tempPlayerData.gameOfWeekDifference = pickMetaData.pointDifferences;
						}

						//if (scoreBoard["homeTeamKey"].GetWinningTeamName()  == p)
						//	tempPlayerData.spreadsheetPicks[idx].winning = true;
						//if (scoreBoard["homeTeamKey"].GetLosingTeamName() == p)
						//	tempPlayerData.spreadsheetPicks[idx].losing = true;

						else if (winning.Contains(p))
						{
							tempPlayerData.spreadsheetPicks[idx].winning = true;
						}
						else if (losing.Contains(p))
						{
							tempPlayerData.spreadsheetPicks[idx].losing = true;
						}
					}
					result.players.Add(tempPlayerData);
				}

				result.lowestPointDifference = gameOfWeekLogic.currentWinningPointDifference;
				result.players.Sort();
				return result;
			}
			catch(Exception excpt)
			{
				result.players = new List<TempPlayerData>();
				result.scoreLine= excpt.Message;
				return result;
			}
		}
	}
}
