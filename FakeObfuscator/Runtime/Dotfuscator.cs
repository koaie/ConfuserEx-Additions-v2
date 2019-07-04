using System;

namespace Confuser.Runtime.FakeObfuscator
{
	public class Dotfuscator
	{
	    public static Type[] GetTypes() => new[] { typeof(DotfuscatorStringDecrypter) };

        internal class DotfuscatorStringDecrypter
        {
            //no exception handlers, static
	        private static string Decrypt(string s, int i)
	        {
                //required method
	            string.Intern(s);
                
                //stloc
                //ldci4
	            char[] o = s.ToCharArray(); //ldarg0, callvirt tochararray
	            string s2 = 5.ToString();   //stloc x, ldci4
	            return s2 + o;              //prevent deobfuscator from adding a pop due to vars being unused
	        }
	    }
	}
}
