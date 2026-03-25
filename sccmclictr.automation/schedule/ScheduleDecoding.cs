// Decompiled with JetBrains decompiler
// Type: sccmclictr.automation.schedule.ScheduleDecoding
// Assembly: sccmclictr.automation, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 96476B75-C789-4A0A-9F55-EBB7DB29E9AB
// Assembly location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.dll
// XML documentation location: C:\Users\jason\Downloads\sccmclictrlib.1.0.1\lib\net48\sccmclictr.automation.xml

using System;
using System.Globalization;
using System.Linq;

#nullable disable
namespace sccmclictr.automation.schedule;

/// <summary>Class ScheduleDecoding.</summary>
public static class ScheduleDecoding
{
  /// <summary>Chech if ScheduleID is a NonRecuring Schedule</summary>
  /// <param name="ScheduleID"></param>
  /// <returns></returns>
  internal static bool isNonRecurring(string ScheduleID)
  {
    return (long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 19 & 7L) == 1L;
  }

  internal static bool isRecurInterval(string ScheduleID)
  {
    return (long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 19 & 7L) == 2L;
  }

  internal static bool isRecurWeekly(string ScheduleID)
  {
    return (long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 19 & 7L) == 3L;
  }

  internal static bool isRecurMonthlyByWeekday(string ScheduleID)
  {
    return (long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 19 & 7L) == 4L;
  }

  internal static bool isRecurMonthlyByDate(string ScheduleID)
  {
    return (long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 19 & 7L) == 5L;
  }

  internal static bool isgmt(string ScheduleID)
  {
    return Convert.ToBoolean(long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) & 1L);
  }

  internal static int dayspan(string ScheduleID)
  {
    return (int) Convert.ToInt16(long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 3 & 31L /*0x1F*/);
  }

  internal static int hourpan(string ScheduleID)
  {
    return (int) Convert.ToInt16(long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 8 & 31L /*0x1F*/);
  }

  internal static int minutespan(string ScheduleID)
  {
    return (int) Convert.ToInt16(long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 13 & 63L /*0x3F*/);
  }

  internal static int weekorder(string ScheduleID)
  {
    return (int) Convert.ToInt16(long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 9 & 7L);
  }

  internal static int fornumberofweeks(string ScheduleID)
  {
    return (int) Convert.ToInt16(long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 13 & 7L);
  }

  internal static int fornumberofmonths(string ScheduleID)
  {
    return (int) Convert.ToInt16(long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 12 & 15L);
  }

  internal static int fornumberofmonths2(string ScheduleID)
  {
    return (int) Convert.ToInt16(long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 10 & 15L);
  }

  internal static int iDay(string ScheduleID)
  {
    return (int) Convert.ToInt16(long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 16 /*0x10*/ & 7L);
  }

  internal static int monthday(string ScheduleID)
  {
    return (int) Convert.ToInt16(long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 14 & 31L /*0x1F*/);
  }

  internal static int dayduration(string ScheduleID)
  {
    return (int) Convert.ToInt16(long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 22 & 31L /*0x1F*/);
  }

  internal static int hourduration(string ScheduleID)
  {
    return (int) Convert.ToInt16(long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 27 & 31L /*0x1F*/);
  }

  internal static int minuteduration(string ScheduleID)
  {
    return (int) Convert.ToInt16(long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 32 /*0x20*/ & 63L /*0x3F*/);
  }

  internal static int startyear(string ScheduleID)
  {
    return (int) Convert.ToInt16((long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 38 & 63L /*0x3F*/) + 1970L);
  }

  internal static int startmonth(string ScheduleID)
  {
    return (int) Convert.ToInt16(long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 44 & 15L);
  }

  internal static int startday(string ScheduleID)
  {
    return (int) Convert.ToInt16(long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 48 /*0x30*/ & 31L /*0x1F*/);
  }

  internal static int starthour(string ScheduleID)
  {
    return (int) Convert.ToInt16(long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 53 & 31L /*0x1F*/);
  }

  internal static int startminute(string ScheduleID)
  {
    return (int) Convert.ToInt16(long.Parse(ScheduleID, NumberStyles.AllowHexSpecifier) >> 58 & 63L /*0x3F*/);
  }

  internal static string encodeID(object Schedule)
  {
    int flagsWord = 0;
    int startTimeWord = 0;
    ScheduleDecoding.SMS_ST_NonRecurring smsStNonRecurring = Schedule as ScheduleDecoding.SMS_ST_NonRecurring;
    if (smsStNonRecurring.IsGMT)
      flagsWord |= 1;
    DateTime startTime;
    int startTimeWithYear;
    if (smsStNonRecurring.StartTime.Year > 1970 & smsStNonRecurring.StartTime.Year < 2033)
    {
      int startTimeBeforeYear = startTimeWord;
      startTime = smsStNonRecurring.StartTime;
      int yearBitsShifted = startTime.Year - 1970 << 6;
      startTimeWithYear = startTimeBeforeYear | yearBitsShifted;
    }
    else
      startTimeWithYear = startTimeWord | 4032;
    int startTimeAfterYear = startTimeWithYear;
    startTime = smsStNonRecurring.StartTime;
    int monthBitsShifted = startTime.Month << 12;
    int startTimeAfterMonth = startTimeAfterYear | monthBitsShifted;
    startTime = smsStNonRecurring.StartTime;
    int dayBitsShifted = startTime.Day << 16 /*0x10*/;
    int startTimeAfterDay = startTimeAfterMonth | dayBitsShifted;
    startTime = smsStNonRecurring.StartTime;
    int hourBitsShifted = startTime.Hour << 21;
    int startTimeAfterHour = startTimeAfterDay | hourBitsShifted;
    startTime = smsStNonRecurring.StartTime;
    int minuteBitsShifted = startTime.Minute << 26;
    int startTimePacked = startTimeAfterHour | minuteBitsShifted;
    int flagsWithDuration = flagsWord | smsStNonRecurring.DayDuration << 22 | smsStNonRecurring.HourDuration << 27;
    int startTimeWithMinuteDuration = startTimePacked | smsStNonRecurring.MinuteDuration;
    switch (Schedule.GetType().Name)
    {
      case "SMS_ST_NonRecurring":
        flagsWithDuration |= 524288 /*0x080000*/;
        break;
      case "SMS_ST_RecurInterval":
        ScheduleDecoding.SMS_ST_RecurInterval smsStRecurInterval = smsStNonRecurring as ScheduleDecoding.SMS_ST_RecurInterval;
        flagsWithDuration = flagsWithDuration | smsStRecurInterval.DaySpan << 3 | smsStRecurInterval.HourSpan << 8 | smsStRecurInterval.MinuteSpan << 13 | 1048576 /*0x100000*/;
        break;
    }
    string hexResult = ((long) startTimeWithMinuteDuration << 32 /*0x20*/ | (long) (uint) flagsWithDuration).ToString("X");
    while (hexResult.Length < 16 /*0x10*/)
      hexResult = "0" + hexResult;
    return hexResult;
  }

  /// <summary>Decode an SMS ScheduleID string</summary>
  /// <param name="ScheduleID">SMS encoded 64bit ScheduleID string</param>
  /// <returns>object of type: SMS_ST_NonRecurring, SMS_ST_RecurInterval, SMS_ST_RecurWeekly, SMS_ST_RecurMonthlyByWeekday or SMS_ST_RecurMonthlyByDate</returns>
  public static object DecodeScheduleID(string ScheduleID)
  {
    try
    {
      int year = ScheduleDecoding.startyear(ScheduleID);
      int month = ScheduleDecoding.startmonth(ScheduleID);
      int day = ScheduleDecoding.startday(ScheduleID);
      int hour = ScheduleDecoding.starthour(ScheduleID);
      int minute = ScheduleDecoding.startminute(ScheduleID);
      if (ScheduleDecoding.isNonRecurring(ScheduleID))
        return (object) new ScheduleDecoding.SMS_ST_NonRecurring()
        {
          IsGMT = ScheduleDecoding.isgmt(ScheduleID),
          StartTime = new DateTime(year, month, day, hour, minute, 0),
          DayDuration = ScheduleDecoding.dayduration(ScheduleID),
          HourDuration = ScheduleDecoding.hourduration(ScheduleID),
          MinuteDuration = ScheduleDecoding.minuteduration(ScheduleID)
        };
      if (ScheduleDecoding.isRecurInterval(ScheduleID))
      {
        ScheduleDecoding.SMS_ST_RecurInterval smsStRecurInterval = new ScheduleDecoding.SMS_ST_RecurInterval();
        smsStRecurInterval.IsGMT = ScheduleDecoding.isgmt(ScheduleID);
        smsStRecurInterval.StartTime = new DateTime(year, month, day, hour, minute, 0);
        smsStRecurInterval.DayDuration = ScheduleDecoding.dayduration(ScheduleID);
        smsStRecurInterval.DaySpan = ScheduleDecoding.dayspan(ScheduleID);
        smsStRecurInterval.HourDuration = ScheduleDecoding.hourduration(ScheduleID);
        smsStRecurInterval.HourSpan = ScheduleDecoding.hourpan(ScheduleID);
        smsStRecurInterval.MinuteDuration = ScheduleDecoding.minuteduration(ScheduleID);
        smsStRecurInterval.MinuteSpan = ScheduleDecoding.minutespan(ScheduleID);
        return (object) smsStRecurInterval;
      }
      if (ScheduleDecoding.isRecurWeekly(ScheduleID))
      {
        ScheduleDecoding.SMS_ST_RecurWeekly smsStRecurWeekly = new ScheduleDecoding.SMS_ST_RecurWeekly();
        smsStRecurWeekly.IsGMT = ScheduleDecoding.isgmt(ScheduleID);
        smsStRecurWeekly.StartTime = new DateTime(year, month, day, hour, minute, 0);
        smsStRecurWeekly.Day = ScheduleDecoding.iDay(ScheduleID);
        smsStRecurWeekly.ForNumberOfWeeks = ScheduleDecoding.fornumberofweeks(ScheduleID);
        smsStRecurWeekly.DayDuration = ScheduleDecoding.dayduration(ScheduleID);
        smsStRecurWeekly.HourDuration = ScheduleDecoding.hourduration(ScheduleID);
        smsStRecurWeekly.MinuteDuration = ScheduleDecoding.minuteduration(ScheduleID);
        return (object) smsStRecurWeekly;
      }
      if (ScheduleDecoding.isRecurMonthlyByWeekday(ScheduleID))
      {
        ScheduleDecoding.SMS_ST_RecurMonthlyByWeekday monthlyByWeekday = new ScheduleDecoding.SMS_ST_RecurMonthlyByWeekday();
        monthlyByWeekday.IsGMT = ScheduleDecoding.isgmt(ScheduleID);
        monthlyByWeekday.StartTime = new DateTime(year, month, day, hour, minute, 0);
        monthlyByWeekday.WeekOrder = ScheduleDecoding.weekorder(ScheduleID);
        monthlyByWeekday.Day = ScheduleDecoding.iDay(ScheduleID);
        monthlyByWeekday.ForNumberOfMonths = ScheduleDecoding.fornumberofmonths(ScheduleID);
        monthlyByWeekday.DayDuration = ScheduleDecoding.dayduration(ScheduleID);
        monthlyByWeekday.HourDuration = ScheduleDecoding.hourduration(ScheduleID);
        monthlyByWeekday.MinuteDuration = ScheduleDecoding.minuteduration(ScheduleID);
        return (object) monthlyByWeekday;
      }
      if (ScheduleDecoding.isRecurMonthlyByDate(ScheduleID))
      {
        ScheduleDecoding.SMS_ST_RecurMonthlyByDate recurMonthlyByDate = new ScheduleDecoding.SMS_ST_RecurMonthlyByDate();
        recurMonthlyByDate.IsGMT = ScheduleDecoding.isgmt(ScheduleID);
        recurMonthlyByDate.StartTime = new DateTime(year, month, day, hour, minute, 0);
        recurMonthlyByDate.ForNumberOfMonths = ScheduleDecoding.fornumberofmonths2(ScheduleID);
        recurMonthlyByDate.MonthDay = ScheduleDecoding.monthday(ScheduleID);
        recurMonthlyByDate.DayDuration = ScheduleDecoding.dayduration(ScheduleID);
        recurMonthlyByDate.HourDuration = ScheduleDecoding.hourduration(ScheduleID);
        recurMonthlyByDate.MinuteDuration = ScheduleDecoding.minuteduration(ScheduleID);
        return (object) recurMonthlyByDate;
      }
    }
    catch
    {
    }
    return (object) null;
  }

  public static DateTime GetNthWeekdayOfMonth(DateTime dt, int n, DayOfWeek weekday)
  {
    IOrderedEnumerable<DateTime> source = Enumerable.Range(1, DateTime.DaysInMonth(dt.Year, dt.Month)).Select<int, DateTime>((Func<int, DateTime>) (day => new DateTime(dt.Year, dt.Month, day))).Where<DateTime>((Func<DateTime, bool>) (day => day.DayOfWeek == weekday)).OrderBy<DateTime, int>((Func<DateTime, int>) (day => day.Day));
    int zeroBasedWeekIndex = n - 1;
    if (zeroBasedWeekIndex >= 0 && zeroBasedWeekIndex < source.Count<DateTime>())
      return source.ElementAt<DateTime>(zeroBasedWeekIndex);
    if (n == 5)
    {
      int lastWeekFallbackIndex = 3;
      if (lastWeekFallbackIndex >= 0 && lastWeekFallbackIndex < source.Count<DateTime>())
        return source.ElementAt<DateTime>(lastWeekFallbackIndex);
    }
    throw new InvalidOperationException("The specified day does not exist in this month!");
  }

  /// <summary>split the scheduleID string into 16char substrings</summary>
  /// <param name="ScheduleID"></param>
  /// <returns>16char ScheduleIDs</returns>
  public static string[] GetScheduleIDs(string ScheduleID)
  {
    if (ScheduleID.Length < 16 /*0x10*/)
      return new string[1]{ ScheduleID };
    string[] scheduleIds = new string[ScheduleID.Length / 16 /*0x10*/];
    for (int index = 0; (index + 1) * 16 /*0x10*/ <= ScheduleID.Length; ++index)
      scheduleIds[index] = ScheduleID.Substring(index * 16 /*0x10*/, 16 /*0x10*/);
    return scheduleIds;
  }

  /// <summary>Non recuring schedule</summary>
  public class SMS_ST_NonRecurring
  {
    /// <summary>duration in Days</summary>
    public int DayDuration { get; set; }

    /// <summary>duration in hours</summary>
    public int HourDuration { get; set; }

    /// <summary>Time is GMT Time</summary>
    public bool IsGMT { get; set; }

    /// <summary>duration in minutes</summary>
    public int MinuteDuration { get; set; }

    /// <summary>Get or set the start time</summary>
    public DateTime StartTime { get; set; }

    /// <summary>Get the next start time</summary>
    public DateTime NextStartTime => this.StartTime;

    /// <summary>get the ScheduleID</summary>
    public string ScheduleID => ScheduleDecoding.encodeID((object) this);
  }

  /// <summary>Interval Schedule (day, hour, minute)</summary>
  public class SMS_ST_RecurInterval : ScheduleDecoding.SMS_ST_NonRecurring
  {
    /// <summary>Interval span in days</summary>
    public int DaySpan { get; set; }

    /// <summary>Interval span in hours</summary>
    public int HourSpan { get; set; }

    /// <summary>Interval span in minutes</summary>
    public int MinuteSpan { get; set; }

    /// <summary>get the next start time</summary>
    public new DateTime NextStartTime
    {
      get
      {
        DateTime unusedDefault = new DateTime();
        DateTime nextStartTime = this.StartTime.Subtract(new TimeSpan(this.DaySpan, this.HourSpan, this.MinuteSpan, 0));
        DateTime durationEndTime = nextStartTime + new TimeSpan(this.DayDuration, this.HourDuration, this.MinuteDuration, 0);
        while (durationEndTime < this.StartTime)
        {
          durationEndTime += new TimeSpan(this.DaySpan, this.HourSpan, this.MinuteSpan, 0);
          nextStartTime += new TimeSpan(this.DaySpan, this.HourSpan, this.MinuteSpan, 0);
        }
        return nextStartTime;
      }
    }

    /// <summary>The last Start Time in the past...</summary>
    public DateTime PreviousStartTime
    {
      get
      {
        DateTime candidateTime = this.StartTime.Subtract(new TimeSpan(this.DaySpan, this.HourSpan, this.MinuteSpan, 0));
        DateTime previousCandidate = candidateTime;
        while (candidateTime < DateTime.Now)
        {
          previousCandidate = candidateTime;
          candidateTime += new TimeSpan(this.DaySpan, this.HourSpan, this.MinuteSpan, 0);
        }
        return candidateTime > DateTime.Now ? previousCandidate : candidateTime;
      }
    }
  }

  /// <summary>Weekly Interval</summary>
  public class SMS_ST_RecurWeekly : ScheduleDecoding.SMS_ST_NonRecurring
  {
    /// <summary>Day of the Week</summary>
    public int Day { get; set; }

    /// <summary>interval in weeks</summary>
    public int ForNumberOfWeeks { get; set; }

    /// <summary>Get the next start time</summary>
    public new DateTime NextStartTime
    {
      get
      {
        DateTime nextStartTime = new DateTime(
          this.StartTime.Year, this.StartTime.Month, this.StartTime.Day,
          this.StartTime.Hour, this.StartTime.Minute, 0);
        while (nextStartTime.DayOfWeek + 1 != (DayOfWeek) this.Day)
          nextStartTime += new TimeSpan(1, 0, 0, 0);
        return nextStartTime;
      }
    }

    /// <summary>The last Start Time in the past...</summary>
    public DateTime PreviousStartTime
    {
      get
      {
        if (this.StartTime > DateTime.Now)
          return this.StartTime;
        DateTime nextMatchingWeekday = new DateTime(
          DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
          this.StartTime.Hour, this.StartTime.Minute, 0);
        while (nextMatchingWeekday.DayOfWeek + 1 != (DayOfWeek) this.Day | nextMatchingWeekday < DateTime.Now)
          nextMatchingWeekday += new TimeSpan(1, 0, 0, 0);
        DateTime rewindedByWeekInterval = nextMatchingWeekday.Subtract(new TimeSpan(this.ForNumberOfWeeks * 7, 0, 0, 0));
        return rewindedByWeekInterval < this.StartTime ? nextMatchingWeekday : rewindedByWeekInterval;
      }
    }
  }

  /// <summary>Monthly interval (by date)</summary>
  public class SMS_ST_RecurMonthlyByDate : ScheduleDecoding.SMS_ST_NonRecurring
  {
    /// <summary>interval in months</summary>
    public int ForNumberOfMonths { get; set; }

    /// <summary>Day of the month</summary>
    public int MonthDay { get; set; }

    /// <summary>get next start time</summary>
    public new DateTime NextStartTime
    {
      get
      {
        if (this.MonthDay == 0)
        {
          DateTime nextStartTime = new DateTime(this.StartTime.Year, this.StartTime.Month, DateTime.DaysInMonth(this.StartTime.Year, this.StartTime.Month), this.StartTime.Hour, this.StartTime.Minute, 0);
          while (nextStartTime < this.StartTime)
          {
            nextStartTime = nextStartTime.AddMonths(this.ForNumberOfMonths);
            nextStartTime = new DateTime(
              nextStartTime.Year, nextStartTime.Month,
              DateTime.DaysInMonth(nextStartTime.Year, nextStartTime.Month),
              this.StartTime.Hour, this.StartTime.Minute, 0);
          }
          return nextStartTime;
        }
        DateTime nextStartTime1 = new DateTime(
          this.StartTime.Year, this.StartTime.Month, this.MonthDay,
          this.StartTime.Hour, this.StartTime.Minute, 0);
        while (nextStartTime1 < this.StartTime || DateTime.DaysInMonth(nextStartTime1.Year, nextStartTime1.Month) < this.MonthDay)
          nextStartTime1 = nextStartTime1.AddMonths(this.ForNumberOfMonths);
        return nextStartTime1;
      }
    }
  }

  /// <summary>Monthly interval (by weekday)</summary>
  public class SMS_ST_RecurMonthlyByWeekday : ScheduleDecoding.SMS_ST_NonRecurring
  {
    /// <summary>Week day</summary>
    public int Day { get; set; }

    /// <summary>interval in months</summary>
    public int ForNumberOfMonths { get; set; }

    /// <summary>WeekOrder</summary>
    public int WeekOrder { get; set; }

    /// <summary>Get next start time</summary>
    public new DateTime NextStartTime
    {
      get
      {
        DateTime dt = ScheduleDecoding.GetNthWeekdayOfMonth(this.StartTime, this.WeekOrder, (DayOfWeek) ((this.Day - 1) % 7));
        dt = new DateTime(dt.Year, dt.Month, dt.Day, this.StartTime.Hour, this.StartTime.Minute, 0);
        while (dt < this.StartTime)
        {
          dt = dt.AddMonths(this.ForNumberOfMonths);
          dt = ScheduleDecoding.GetNthWeekdayOfMonth(dt, this.WeekOrder, (DayOfWeek) ((this.Day - 1) % 7));
          dt = new DateTime(dt.Year, dt.Month, dt.Day, this.StartTime.Hour, this.StartTime.Minute, 0);
        }
        return dt;
      }
    }
  }
}
