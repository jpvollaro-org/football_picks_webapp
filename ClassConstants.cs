﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace nfl_picks_pool
{
	static public class ClassConstants
	{
		public static readonly List<string> quarterNames = new List<string>() { "Final", "1st", "2nd", "3rd", "4th", "OT" };
		public static readonly int SELECT_WINNER_POINTS = 10;
		public static readonly int GOFW_WINNER_POINTS = 25;
		public static readonly int SOLE_WINNER_POINTS = 25;
		public static readonly int PERFECT_SCORE_POINTS = 100;
		public static readonly int PLAYOFF_WINNER_POINTS = 50;


		public static Random rnd = new Random();

		public static int GetPickWeek()
		{
			//TODO figure out year change 2022 .. 2023 .. 2024 ..
			DateTime now = DateTime.UtcNow.AddDays(0);
			var x = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(now, CalendarWeekRule.FirstDay, DayOfWeek.Wednesday);
			return x + 16;
		}

	}
}
