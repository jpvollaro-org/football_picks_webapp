using System.Collections.Generic;

namespace nfl_picks_pool
{
	public class PropBets
	{
		static List<int> CoachFiredWinners = new List<int>() { 9, 21, 25 };

		public static int CalculatePropPoints(Player p)
		{
			int points = 0;
			if (CoachFiredWinners.Contains(p.id))
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
				if (CoachFiredWinners.Contains(p.id))
				{
					p.currentPlayerPoints = CalculatePropPoints(p);
				}
				else
				{
					p.currentPlayerPoints = 0;
				}
			}
		}
	}
}
