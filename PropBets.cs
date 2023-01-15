using System.Collections.Generic;

namespace nfl_picks_pool
{
	public class PropBets
	{
		static List<int> CoachFiredWinners = new List<int>() { 9, 21, 25 };
        static List<int> CorrectRecordWinners = new List<int>() { 6, 11 };
        static List<int> WorstRecord = new List<int>() { 17, 19, 21 };

        public static int CalculatePropPoints(Player p)
		{
			int points = 0;
			if (CoachFiredWinners.Contains(p.id))
			{
				points += 100;
			}
			if (CorrectRecordWinners.Contains(p.id))
			{
				points += 100;
			}
			if (WorstRecord.Contains(p.id))
			{
				points += 100;
			}
			return points;
		}

		public static void CalculatePropPoints(Dictionary<int, Player> playerTable)
		{
			foreach (var playerEntry in playerTable)
			{
				Player p = playerEntry.Value;
				p.currentPlayerPoints = CalculatePropPoints(p) + p.bonusPlayerPoints;
			}
		}
	}
}
