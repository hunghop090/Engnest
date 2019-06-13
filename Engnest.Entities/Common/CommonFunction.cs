using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engnest.Entities.Common
{
	public class CommonFunction
	{
		public static String GetTimestamp(DateTime value)
		{
			return value.ToString("yyyyMMddHHmmssffff");
		}
		public static int RandomNumber(int min, int max)
		{
			Random random = new Random();
			return random.Next(min, max);
		}
	}
}