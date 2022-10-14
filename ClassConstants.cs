using System;
using System.Collections.Generic;
using System.Globalization;

namespace nfl_picks_pool
{
	static public class ClassConstants
	{
		public static readonly List<string> quarterNames = new List<string>() { "Final", "1st", "2nd", "3rd", "4th", "OT" };

		public static Random rnd = new Random();

		public static int GetPickWeek()
		{
			DateTime now = DateTime.UtcNow.AddDays(0);
			var x = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(now, CalendarWeekRule.FirstDay, DayOfWeek.Wednesday);
			return x - 36;
		}

	}
}
