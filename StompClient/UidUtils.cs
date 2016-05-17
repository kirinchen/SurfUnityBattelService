using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using WebSocketSharp;	//https://github.com/sta/websocket-sharp
using Newtonsoft.Json;



namespace UnityStomp
{
	public class UidUtils
	{
		public static string getRandomString(int size){
			var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var stringChars = new char[size];
			var random = new Random();
			
			for (int i = 0; i < stringChars.Length; i++)
			{
				stringChars[i] = chars[random.Next(chars.Length)];
			}
			
			return new String(stringChars);
		}

		public static string getRandomNumber(int size){
			var chars = "0123456789";
			var stringChars = new char[size];
			var random = new Random();
			
			for (int i = 0; i < stringChars.Length; i++)
			{
				stringChars[i] = chars[random.Next(chars.Length)];
			}
			
			return new String(stringChars);
		}
		
	}
}

