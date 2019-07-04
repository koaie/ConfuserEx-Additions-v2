using System;

namespace Confuser.Runtime.FakeObfuscator
{
	public class Xenocode
	{
	    public static Type[] GetTypes() => new[] {typeof(XenocodeStringDecrypter)};

        //no fields, 1, 2 or 3 methods, no properties, no events
	    internal class XenocodeStringDecrypter
	    {
            //has to contain int 1789
	        public string Decrypt(string x, int y) => 1789.ToString();
	    }
    }
}
