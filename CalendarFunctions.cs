using System;

// Token: 0x020000EF RID: 239
public class CalendarFunctions
{
	// Token: 0x06000654 RID: 1620 RVA: 0x0002D30C File Offset: 0x0002B50C
	public static string GetSeason(string month, string hemisphere)
	{
		string text = month.ToUpper();
		int num2;
		switch (text)
		{
		case "JAN":
			num2 = 3;
			goto IL_162;
		case "FEB":
			num2 = 3;
			goto IL_162;
		case "MAR":
			num2 = 3;
			goto IL_162;
		case "APR":
			num2 = 1;
			goto IL_162;
		case "MAY":
			num2 = 1;
			goto IL_162;
		case "JUN":
			num2 = 1;
			goto IL_162;
		case "JUL":
			num2 = 1;
			goto IL_162;
		case "AUG":
			num2 = 1;
			goto IL_162;
		case "SEP":
			num2 = 1;
			goto IL_162;
		case "OCT":
			num2 = 1;
			goto IL_162;
		case "NOV":
			num2 = 3;
			goto IL_162;
		case "DEC":
			num2 = 3;
			goto IL_162;
		}
		num2 = 1;
		IL_162:
		if (hemisphere.ToUpper() == "SOUTH")
		{
			num2 += 4;
		}
		return CalendarFunctions.seasons[num2];
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x0002D49C File Offset: 0x0002B69C
	public static long ToJulian(DateTime dateTime)
	{
		int day = dateTime.Day;
		int num = dateTime.Month;
		int num2 = dateTime.Year;
		if (num < 3)
		{
			num += 12;
			num2--;
		}
		return (long)(day + (153 * num - 457) / 5 + 365 * num2 + num2 / 4 - num2 / 100 + num2 / 400 + 1721119);
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x0002D504 File Offset: 0x0002B704
	public static string FromJulian(long julianDate, string format)
	{
		long num = julianDate + 68569L;
		long num2 = 4L * num / 146097L;
		num -= (146097L * num2 + 3L) / 4L;
		long num3 = 4000L * (num + 1L) / 1461001L;
		num = num - 1461L * num3 / 4L + 31L;
		long num4 = 80L * num / 2447L;
		int day = (int)(num - 2447L * num4 / 80L);
		num = num4 / 11L;
		int month = (int)(num4 + 2L - 12L * num);
		int year = (int)(100L * (num2 - 49L) + num3 + num);
		DateTime dateTime = new DateTime(year, month, day);
		return dateTime.ToString(format);
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x0002D5B4 File Offset: 0x0002B7B4
	public static string GetFullMonth(int month, bool abbreviate, bool upperCase)
	{
		string text = "January";
		switch (month)
		{
		case 1:
			text = "January";
			break;
		case 2:
			text = "February";
			break;
		case 3:
			text = "March";
			break;
		case 4:
			text = "April";
			break;
		case 5:
			text = "May";
			break;
		case 6:
			text = "June";
			break;
		case 7:
			text = "July";
			break;
		case 8:
			text = "August";
			break;
		case 9:
			text = "September";
			break;
		case 10:
			text = "October";
			break;
		case 11:
			text = "November";
			break;
		case 12:
			text = "December";
			break;
		}
		if (abbreviate && text.Length > 3)
		{
			text = text.Substring(0, 3);
		}
		if (upperCase)
		{
			text = text.ToUpper();
		}
		return text;
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x0002D6B4 File Offset: 0x0002B8B4
	public static int GetMoonPhase()
	{
		float num;
		if (GameDataManager.missionMode || GameDataManager.trainingMode)
		{
			num = (float)CalendarFunctions.ToJulian(Convert.ToDateTime(UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.date));
		}
		else
		{
			num = UIFunctions.globaluifunctions.campaignmanager.julianStartDay + UIFunctions.globaluifunctions.campaignmanager.hoursDurationOfCampaign / 24f;
		}
		float num2 = 2415035.5f;
		float num3 = 29.530588f;
		float num4 = num - num2;
		num4 %= num3;
		num4 /= num3;
		int result = 0;
		if (num4 < 0.0625f)
		{
			result = 0;
		}
		else if (num4 < 0.1875f)
		{
			result = 1;
		}
		else if (num4 < 0.3125f)
		{
			result = 2;
		}
		else if (num4 < 0.4375f)
		{
			result = 3;
		}
		else if (num4 < 0.5625f)
		{
			result = 4;
		}
		else if (num4 < 0.6875f)
		{
			result = 5;
		}
		else if (num4 < 0.8125f)
		{
			result = 6;
		}
		else if (num4 < 0.9375f)
		{
			result = 7;
		}
		return result;
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x0002D7D8 File Offset: 0x0002B9D8
	public static int MaxDaysInMonth(int month, int year)
	{
		int num = 30;
		switch (month)
		{
		case 1:
			num = 31;
			break;
		case 2:
			num = 28;
			if (year % 4 == 0)
			{
				num++;
			}
			break;
		case 3:
			num = 31;
			break;
		case 4:
			num = 30;
			break;
		case 5:
			num = 31;
			break;
		case 6:
			num = 30;
			break;
		case 7:
			num = 31;
			break;
		case 8:
			num = 31;
			break;
		case 9:
			num = 30;
			break;
		case 10:
			num = 31;
			break;
		case 11:
			num = 30;
			break;
		case 12:
			num = 31;
			break;
		}
		return num;
	}

	// Token: 0x040006C0 RID: 1728
	public static string[] seasons = new string[]
	{
		"summer",
		"summer",
		"summer",
		"winter",
		"summer",
		"winter",
		"summer",
		"summer"
	};
}
