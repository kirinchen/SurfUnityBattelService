using System;



namespace UnityStomp
{
	public class UidUtils
	{
		public static string getRandomString(int size){
			var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var stringChars = new char[size];
			
			for (int i = 0; i < stringChars.Length; i++)
			{
				stringChars[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
			}
			
			return new String(stringChars);
		}

		public static string getRandomNumber(int size){
			var chars = "0123456789";
			var stringChars = new char[size];
			
			for (int i = 0; i < stringChars.Length; i++)
			{
				stringChars[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
			}
			
			return new String(stringChars);
		}
		
	}
}

