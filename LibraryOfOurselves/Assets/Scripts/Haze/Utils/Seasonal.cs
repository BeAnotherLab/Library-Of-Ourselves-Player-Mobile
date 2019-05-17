using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Haze{
	public static class Seasonal {
		static int January = 1, February = 2, March = 3, April = 4, May = 5, June = 6/*, July = 7, August = 8*/, September = 9, October = 10, /*November = 11, */December = 12;
		
		//To be updated every few years!
		static bool isChineseNewYear(DateTime date){
			if(date.Year == 2019 && date.Month == February && date.Day == 5) return true;
			if(date.Year == 2020 && date.Month == January && date.Day == 25) return true;
			return false;
		}
		static bool isEid(DateTime date){
			if(date.Year == 2019 && date.Month == June && date.Day == 4) return true;
			return false;
		}
		static bool isMothersDay(DateTime date){
			if(date.Year == 2019 && date.Month == May && date.Day == 12) return true;
			if(date.Year == 2020 && date.Month == May && date.Day == 10) return true;
			return false;
		}
		static bool isYomKippur(DateTime date){
			if(date.Year == 2018 && date.Month == September && date.Day == 19) return true;
			if(date.Year == 2019 && date.Month == October && date.Day == 9) return true;
			if(date.Year == 2020 && date.Month == September && date.Day == 28) return true;
			return false;
		}
		
		public enum Season{
			None,
			AprilFoolsDay,
			ChineseNewYear,
			Christmas,
			Easter,
			Eid,
			FathersDay,
			Halloween,
			MothersDay,
			MayTheFourth,
			NewYear,
			PiDay,
			SaintPatricksDay,
			SummerSolstice,
			ValentinesDay,
			WinterSolstice,
			YomKippur
		}
		
		public static Season CurrentSeason{
			get{
				DateTime date = DateTime.Now;
			//	ChineseLunisolarCalendar chinese = new ChineseLunisolarCalendar();
			//	GregorianCalendar gregorian = new GregorianCalendar();
				
				if(date.Month == April && date.Day == 1)
					return Season.AprilFoolsDay;
				/*DateTime chineseNewYear = chinese.ToDateTime(DateTime.UtcNow.Year, 1, 1, 0, 0, 0, 0);
				if(date.Month == chineseNewYear.Month && date.Day == chineseNewYear.Day)
					return Season.ChineseNewYear;*/
				if(isChineseNewYear(date))
					return Season.ChineseNewYear;
				if(date.Month == December && (date.Day == 24 || date.Day == 25))
					return Season.Christmas;
				DateTime easter = Easter(date.Year);
				if(date.Month == easter.Month && date.Day == easter.Day)
					return Season.Easter;
				if(date.Month == March && date.Day == 19)
					return Season.FathersDay;
				if(date.Month == October && date.Day == 31)
					return Season.Halloween;
				if(date.Month == May && date.Day == 4)
					return Season.MayTheFourth;
				if((date.Month == January && date.Day == 1) || (date.Month == December && date.Day == 31))
					return Season.NewYear;
				if(date.Month == March && date.Day == 14)
					return Season.PiDay;
				if(date.Month == March && date.Day == 17)
					return Season.SaintPatricksDay;
				if(date.Month == June && date.Day == 21)
					return Season.SummerSolstice;
				if(date.Month == February && date.Day == 14)
					return Season.ValentinesDay;
				if(date.Month == December && date.Day == 21)
					return Season.WinterSolstice;
				if(isEid(date))
					return Season.Eid;
				if(isMothersDay(date))
					return Season.MothersDay;
				if(isYomKippur(date))
					return Season.YomKippur;
				
				return Season.None;
			}
		}
		
		//Gauss' Easter algorithm
		private static DateTime Easter(int year){
			int a = year%19;
			int b = year/100;
			int c = (b - b/4 - (8*b + 13)/25 + 19*a + 15)%30;
			int d = c - (c/28)*(1-(c/28)*(29/(c+1))*((21-a)/11));
			int e = d - ((year + year/4 + d + 2 - b + b/4)%7);
			int month = 3+(e+40)/44;
			return new DateTime(year, month, e+28-31*month/4);
		}
		
	}
}