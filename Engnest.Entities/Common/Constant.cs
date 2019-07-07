using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engnest.Entities.Common
{
	public class Status
	{
		public const Byte ACTIVE = 1;
		public const Byte DELETE = 0;
	}
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
	public class TypePost
	{
		public const Byte USER = 0;
		public const Byte GROUP = 1;
	}

	public class TypeRequestFriend
	{
		public const Byte USER = 0;
		public const Byte GROUP = 1;
	}

	public class StatusRequestFriend
	{
		public const Byte SENDING = 0;
		public const Byte ACCEPT = 1;
		public const Byte CANCEL = 2;
		public const Byte REJECT = 4;
	}

	public class StatusMember
	{
		public const Byte SENDING = 0;
		public const Byte ACCEPT = 1;
		public const Byte REJECT = 2;
		public const Byte BLOCK = 3;
	}
	public class TypeMember
	{
		public const Byte MEMBER = 0;
		public const Byte ADMIN = 1;
	}
}