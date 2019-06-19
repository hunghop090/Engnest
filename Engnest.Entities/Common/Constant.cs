using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engnest.Entities.Common
{
	public class Constant
	{
		public const string USER_SESSION = "USER_SESSION";
        public const Byte SUCCESS = 0;
        public const Byte ERROR = 2;
    }
	public class LoginStatus
	{
		public const Byte SUCCESS = 0;
		public const Byte NOT_EXISTS_USER = 1;
		public const Byte WRONG_PASSWORD = 2;
		public const Byte BLOCKED = 3;
	}

	public class SignInStatus
	{
		public const Byte SUCCESS = 0;
		public const Byte EXISTS_USER = 1;
		public const Byte ERROR = 2;
	}

	public class TypeUpload
	{
		public const string IMAGE = "image";
		public const string VIDEO = "video";
		public const string AUDIO = "audio";
	}
	public class TypeComment
	{
		public const Byte POST = 0;
		public const Byte COMMENT = 1;
	}
}