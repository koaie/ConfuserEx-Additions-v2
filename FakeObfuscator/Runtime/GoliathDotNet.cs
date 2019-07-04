using System;
using System.Collections.Generic;
using System.Reflection;
#pragma warning disable 219

namespace Confuser.Runtime.FakeObfuscator
{
	public class GoliathDotNet
	{
	    public static Type[] GetTypes() => new[] {typeof(GoliathStrongNameChecker)};

        //no fields, no events, no properties
		internal class GoliathStrongNameChecker
		{
            //has throw instruction
		    public static void AntiTamper(Type t)
		    {
		        //required locals
		        var l1 = default(Assembly);
		        var l2 = default(Stack<int>);

				//this gets boring after a while, you know
				var party = new Exception();
			    throw party;
		    }

			public byte[] RequiredMethod(Assembly s) => default(byte[]);
			public string RequiredMethod(Stack<int> s) => default(string);
			public int RequiredMethod(int i, byte[] b) => default(int);
		}
	}
}
