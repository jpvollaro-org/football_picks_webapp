using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using nfl_picks_pool.Interfaces;

namespace nfl_picks_pool
{
	public class SportsTeamStubTest : ISportsApi
	{
		public ILogger Logger { get; set; }

		public SportsTeamStubTest(ILogger logger)
		{
			Logger = logger;
		}

		public List<GameScore> GetFakeData()
		{
			List<GameScore> gameScores = new List<GameScore>();
			gameScores.Add(new GameScore("COWBOYS", 7, "RAMS", 10, false));
			return gameScores;
		}

		public Task<GameScore> GetFinalGameScore(int gameId)
		{
			throw new NotImplementedException();
		}

		public async Task<List<GameScore>> GetLiveGameScores()
		{
			System.Threading.Thread.Sleep(300);
			return await Task.FromResult(GetFakeData());
		}

		public Task<Dictionary<string,GameScore>> GetPastScheduleGames(DateTime gameDate)
		{
			throw new NotImplementedException();
		}
	}
}
