using CsvHelper;
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

namespace ReactProgramNS
{
	// Used to handle functions that read/write EXCEL FILES
	public class ExcelHelperClass
	{
		private static readonly string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
		public static readonly string PredictionsFile = workingDirectory + System.IO.Path.DirectorySeparatorChar +
			"Files\\NFL PREDICTIONS 2022.csv";

		public ExcelHelperClass()
		{
		}

		private int ReadPlayersFromFile(CsvReader csv, Dictionary<int, Player> playerDictionary)
		{
			int colNumber = 1;

			// First line in spreadsheet are the players in the pool
			csv.Read();
			try
			{
				for (; colNumber < 100; colNumber++)
				{
					string s = csv.GetField(colNumber);
					if (string.IsNullOrEmpty(s))
						continue;
					if (s.ToUpper() == "POINTS")
						continue;
					if (s.ToUpper().StartsWith("FINAL"))
						return(--colNumber);
					Player player = new Player(csv.GetField(colNumber));
					player.id = colNumber;
					playerDictionary.Add(colNumber,player);
				}
			}
			catch
			{
			}
			return colNumber;
		}

		private void ReadPlayersChoicesFromFile(CsvReader csv, Dictionary<int, Player> playerDictionary, int lastColumn)
		{
			try
			{
				for (int rowNumber = 2; rowNumber <= 10; rowNumber++)
				{
					csv.Read();
					for (int i = 1; i <= lastColumn; i++)
					{
						string s = csv.GetField(i);
						if (string.IsNullOrEmpty(s))
							continue;
						
						switch (rowNumber)
						{
							case 2:
								if (i == 7)
									playerDictionary[i-2].currentPlayerPoints = int.Parse(s);
								else
									playerDictionary[i-1].currentPlayerPoints = int.Parse(s);
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

		private void ReadPlayersWeeklyPicksFromFile(CsvReader csv, Dictionary<int, Player> playerDictionary, int weekNumber, int lastColumn)
		{
			int workingWeekNumber = 0;
			try
			{
				while(workingWeekNumber <= weekNumber)
				{ 
					csv.Read();
					string s = csv.GetField(0);
					if (string.IsNullOrEmpty(s))
						continue;
					if (s.ToUpper().StartsWith("WEEK"))
					{
						workingWeekNumber++;
						continue;
					}
				
					for (int i = 0; i <= lastColumn; i++)
					{
						string pick = csv.GetField(i);
						if (string.IsNullOrEmpty(pick))
							continue;

						if ( i == 0)
						{
							//TODO find a better way than relying on Kerry making a score prediction
							WeeklyScoreboard.UpdateGameScoreFields(csv.GetField(0), workingWeekNumber, csv.GetField(1));
						}

						if (playerDictionary.ContainsKey(i))
						{
							if (playerDictionary[i].spreadsheetPicks[workingWeekNumber] == null)
								playerDictionary[i].spreadsheetPicks[workingWeekNumber] = new List<PickMetaData>();

							if (int.TryParse(pick, out int score))
							{
								playerDictionary[i].spreadsheetPicks[workingWeekNumber].Add(new PickMetaData(csv.GetField(0) + ":" + pick));
							}
							else
							{
								playerDictionary[i].spreadsheetPicks[workingWeekNumber].Add(new PickMetaData(csv.GetField(0)));
							}
						}
					}
				}
			}
			catch
			{
			}
		}

		private void ReadPlayersPointsFromFile(CsvReader csv, Dictionary<int, Player> playerDictionary, int lastColumn)
		{
			string s = string.Empty;
			while (!s.ToUpper().StartsWith("TOTAL"))
			{
				csv.Read();
				s = csv.GetField(0);
				if (string.IsNullOrEmpty(s))
					continue;
				if (!s.ToUpper().StartsWith("TOTAL"))
					continue;

				for (int i = 1; i <= lastColumn; i++)
				{
					s = csv.GetField(i);
					if (string.IsNullOrEmpty(s))
						continue;

					if (i == 7)
						playerDictionary[i - 2].currentPlayerPoints = int.Parse(s);
					else
						playerDictionary[i - 1].currentPlayerPoints = int.Parse(s);
				}
				return;
			}
		}

		public Dictionary<int, Player> ReadPredectionFile()
		{
			int currentWeek = nfl_picks_pool.ClassConstants.GetPickWeek();
			Dictionary<int, Player> playerDictionary = new Dictionary<int, Player>(); 
			using (var reader = new StreamReader(PredictionsFile))
			{
				using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
				{
					int lastColunm = ReadPlayersFromFile(csv, playerDictionary);
					ReadPlayersChoicesFromFile(csv, playerDictionary, lastColunm);
					ReadPlayersWeeklyPicksFromFile(csv, playerDictionary, currentWeek, lastColunm);
					//ReadPlayersPointsFromFile(csv, playerDictionary, lastColunm);
				}
			}
			return playerDictionary;
		}
	}
}
