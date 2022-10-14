using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nfl_picks_pool.Interfaces
{
    public interface ISportsApi
    {
        Task<List<GameScore>> GetLiveGameScores();
        Task<Dictionary<string, GameScore>> GetPastScheduleGames(DateTime gameDate);
        Task<GameScore> GetFinalGameScore(int gameId);
    }
}
