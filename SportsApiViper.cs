using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.IO;
using static System.Formats.Asn1.AsnWriter;
using System.Diagnostics;
using nfl_picks_pool.Interfaces;
using Microsoft.Extensions.Logging;

namespace nfl_picks_pool
{
	public class Team
	{
		[JsonProperty("id")]
		public int id { get; set; }

		[JsonProperty("name")]
		public string name { get; set; }

		[JsonProperty("code")]
		public string code { get; set; }

		public string getDisplayName()
		{
			try
			{
				return SportsApiViper.IdToTeamNameMap[id];
			}
			catch
			{
				return "UNKNOWN";
			}
		}
	}

	public class Score
	{
		[JsonProperty("current")]
		public int current { get; set; }
	}

	public class MatchTime
	{
		[JsonProperty("played")]
		public int played { get; set; }

		[JsonProperty("periodLength")]
		public int periodLength { get; set; }

		[JsonProperty("overtimeLength")]
		public int overtimeLength { get; set; }

		[JsonProperty("totalPeriodCount")]
		public int totalPeriodCount { get; set; }
	}

	public class Competition
	{
		[JsonProperty("id")]
		public int id { get; set; }

		[JsonProperty("name")]
		public string name { get; set; }
	}

	public class Status
	{
		public int code { get; set; }
		public string type { get; set; }
		public string title { get; set; }
	}

	public class GameScoreViper
	{
		[JsonProperty("id")]
		public int id { get; set; }

		[JsonProperty("startTimestamp")]
		public long startTimestamp { get; set;}

		[JsonProperty("homeTeam")]
		public Team homeTeam { get; set; }

		[JsonProperty("awayTeam")]
		public Team awayTeam { get; set; }

		[JsonProperty("homeScore")]
		public Score homeScore { get; set; }

		[JsonProperty("awayScore")]
		public Score awayScore { get; set; }

		[JsonProperty("time")]
		public MatchTime time { get; set; }

		[JsonProperty("competition")]
		public Competition competition { get; set; }

		[JsonProperty("status")]
		public Status status { get; set; }

		public string GetTimeLeftInGame()
		{
			if (this.time.played == 0)
				return "Final";

			int quarterNumber = 1;
			int quarterTime = this.time.played;
			while ((quarterTime - this.time.periodLength) >= 0)
			{
				quarterNumber++;
				quarterTime = quarterTime - this.time.periodLength;
			}

			quarterTime = this.time.periodLength - quarterTime;
			string timeLeft = string.Format("{0}:{1} {2}", quarterTime / 60, (quarterTime % 60).ToString("D2"),
				ClassConstants.quarterNames[quarterNumber]);
			return timeLeft;
		}
	}

	public class SportsApiViper : ISportsApi
	{
		private ILogger _logger { get; set; }
		public static readonly string sportsViperApiKey = "2e4674074dmshf74e7d2da08fb82p122c91jsn22e9adfe64c7";
		public static readonly int sportsId = 63;
		public static readonly int competitionId = 9464;
		public static string testBody = @"[{""id"":10309027,""startTimestamp"":1663287300,""slug"":""los-angeles-chargers-kansas-city-chiefs"",""scoreHomeAwaySlug"":""7:10"",""scoreAwayHomeSlug"":""10:7"",""round"":2,""lastPeriod"":""period2"",""hasEventPlayerStatistics"":true,""status"":{""code"":14,""type"":""inprogress"",""title"":""2nd quarter""},""homeTeam"":{""id"":4422,""name"":""Kansas City Chiefs"",""short"":""Kansas City Chiefs"",""code"":""KCC"",""slug"":""kansas-city-chiefs"",""gender"":""M"",""teamColors"":{""primary"":""#aa0114"",""secondary"":""#ffffff""}},""awayTeam"":{""id"":4429,""name"":""Los Angeles Chargers"",""short"":""Los Angeles Chargers"",""code"":""LAC"",""slug"":""los-angeles-chargers"",""gender"":""M"",""teamColors"":{""primary"":""#3289ce"",""secondary"":""#ffc20e""}},""homeScore"":{""current"":7,""display"":7,""period1"":0,""period2"":7},""awayScore"":{""current"":10,""display"":10,""period1"":3,""period2"":7},""time"":{""played"":3600,""periodLength"":900,""overtimeLength"":600,""totalPeriodCount"":4},""competition"":{""id"":41244,""name"":""NFL, Regular Season"",""slug"":""nfl-regular-season"",""sport"":{""id"":63,""name"":""American Football"",""slug"":""american-football""},""category"":{""id"":1370,""code"":""US"",""name"":""USA"",""slug"":""usa"",""flag"":""usa""}}}]";
		public static readonly Dictionary<int, string> IdToTeamNameMap = new Dictionary<int, string>
		{
			{ 4287, "DOLPHINS" },{ 4414, "BILLS" },{ 4427, "JETS" },{ 4424, "PATRIOTS" },
			{ 4345, "STEELERS" },{ 4413, "RAVENS" },{ 4417, "BROWNS" },{ 4416, "BENGALS" },
			{ 4324, "TEXANS" },{ 4421, "COLTS" },{ 4431, "TITANS" },{ 4386, "JAGUARS" },
			{ 4422, "CHIEFS" },{ 4429, "CHARGERS" },{ 4390, "RAIDERS" },{ 4418, "BRONCOS" },

			{ 4428, "EAGLES" },{ 4392, "COWBOYS" },{ 4426, "GIANTS" },{ 4432, "COMMANDERS" },
			{ 4388, "BUCCANEERS" },{ 4393, "FALCONS" },{ 4425, "SAINTS" },{ 4415, "PANTHERS" },
			{ 4420, "PACKERS" },{ 4423, "VIKINGS" },{ 4391, "BEARS" },{ 4419, "LIONS" },
			{ 4430, "SEAHAWKS" },{ 4389, "49ERS" },{ 4412, "CARDINALS" },{ 4387, "RAMS" },
		};

		public SportsApiViper(ILogger logger)
		{
			_logger = logger;
		}

		public async Task<List<GameScore>> GetLiveGameScores()
		{
			List<GameScoreViper> scores = new List<GameScoreViper>();
			try
			{
				var client = new HttpClient();
				var request = new HttpRequestMessage
				{
					Method = HttpMethod.Get,
					RequestUri = new Uri("https://viperscore.p.rapidapi.com/games/live?sport=american-football"),
					Headers =
					{
						{ "X-RapidAPI-Key", sportsViperApiKey },
						{ "X-RapidAPI-Host", "viperscore.p.rapidapi.com" },
					},
				};
				using (var response = await client.SendAsync(request))
				{
					response.EnsureSuccessStatusCode();
					var body = await response.Content.ReadAsStringAsync();
					scores = JsonConvert.DeserializeObject<List<GameScoreViper>>(body);
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
			}

			List <GameScore> gameScores = new List<GameScore>();
			foreach (var score in scores)
			{
				if (score.competition.name.StartsWith("NFL"))
				{
					gameScores.Add(ConvertToGameScore(score));
				}
			}

			return gameScores;
		}

		public async Task<GameScore> GetFinalGameScore(int gameId)
		{
			try
			{
				var client = new HttpClient();
				var request = new HttpRequestMessage
				{
					Method = HttpMethod.Get,
					RequestUri = new Uri(string.Format("https://viperscore.p.rapidapi.com/game/?gameId={0}",gameId)),
					Headers =
					{
						{ "X-RapidAPI-Key", sportsViperApiKey },
						{ "X-RapidAPI-Host", "viperscore.p.rapidapi.com" },
					},
				};
				using (var response = await client.SendAsync(request))
				{
					response.EnsureSuccessStatusCode();
					var body = await response.Content.ReadAsStringAsync();
					var score = JsonConvert.DeserializeObject<GameScoreViper>(body);
					return ConvertToGameScore(score);
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
			}

			return null;
		}

		public async Task<Dictionary<string,GameScore>> GetPastScheduleGames(DateTime gameDate)
		{
			Dictionary<string, GameScore> weeksGameScores = new Dictionary<string, GameScore>();

			var thurssday = await GetPastGameScores(gameDate.ToString("yyyy-MM-dd"), weeksGameScores);
			var saturday = await GetPastGameScores(gameDate.AddDays(2).ToString("yyyy-MM-dd"), weeksGameScores);
			var sunday = await GetPastGameScores(gameDate.AddDays(3).ToString("yyyy-MM-dd"), weeksGameScores);
			var monday = await GetPastGameScores(gameDate.AddDays(5).ToString("yyyy-MM-dd"), weeksGameScores);
			return weeksGameScores;
		}

		private async Task<int> GetPastGameScores(string dateOfGame, Dictionary<string, GameScore> pastScores)
		{
			List<GameScoreViper> scores = new List<GameScoreViper>();
			try
			{
				var client = new HttpClient();
				var request = new HttpRequestMessage
				{
					Method = HttpMethod.Get,
					RequestUri = new Uri(
						string.Format("https://viperscore.p.rapidapi.com/games/scheduled/date?sport=american-football&date={0}", dateOfGame)),
					Headers =
					{
						{ "X-RapidAPI-Key", sportsViperApiKey },
						{ "X-RapidAPI-Host", "viperscore.p.rapidapi.com" },
					},
				};
				using (var response = await client.SendAsync(request))
				{
					response.EnsureSuccessStatusCode();
					var body = await response.Content.ReadAsStringAsync();
					scores = JsonConvert.DeserializeObject<List<GameScoreViper>>(body);
					return ConvertAllGames(scores, pastScores);
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
			}
			return 0;
		}

		private GameScore ConvertToGameScore(GameScoreViper score)
		{
			GameScore gameScore = new GameScore();
			gameScore.id = score.id;
			gameScore.awayTeam = score.awayTeam.getDisplayName();
			gameScore.awayScore = score.awayScore.current;
			gameScore.homeTeam = score.homeTeam.getDisplayName();
			gameScore.homeScore = score.homeScore.current;
			gameScore.timeLeft = score.GetTimeLeftInGame();
			gameScore.finalScore = score.status.type == "finished" ? true : false;

			DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(score.startTimestamp);
			gameScore.gameStartTimeLocalTime = dateTimeOffset.DateTime.ToLocalTime();
			return gameScore;
		}

		private int ConvertAllGames(List<GameScoreViper> scores, Dictionary<string, GameScore> weeksGameScores)
		{
			foreach (var score in scores)
			{
				if (score.competition.name.StartsWith("NFL"))
				{
					weeksGameScores.Add(score.homeTeam.getDisplayName(), ConvertToGameScore(score));
				}
			}
			return 1;
		}
	}
}
