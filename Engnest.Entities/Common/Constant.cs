using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engnest.Entities.Common
{
	public class Constant
	{
	}
	public class LoginStatus
	{
		public const Byte SUCCESS = 0;
		public const Byte NOT_EXISTS_USER = 1;
		public const Byte WRONG_PASSWORD = 2;
		public const Byte BLOCKED = 3;
	}
}