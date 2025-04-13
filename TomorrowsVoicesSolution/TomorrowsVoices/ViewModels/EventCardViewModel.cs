using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TomorrowsVoices.ViewModels
{
    public class EventCardViewModel
    {
        public int EventId { get; set; }

        [Display(Name = "Event Name")]
        public string Title { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        [Display(Name = "Start Time")]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy h:mm tt}")]
        public DateTime Start { get; set; }

        [Display(Name = "End Time")]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy h:mm tt}")]
        public DateTime End { get; set; }

        public string Notes { get; set; }

        public List<ScheduleViewModel> Schedules { get; set; } = new List<ScheduleViewModel>();

        public bool IsRegistered { get; set; }

        // Helper properties
        public string Duration => $"{(End - Start).TotalHours:0.0} hours";

        public string EventDate => Start.ToString("dddd, MMMM d, yyyy");

        public string EventTime => $"{Start.ToString("h:mm tt")} - {End.ToString("h:mm tt")}";

        public bool IsUpcoming => Start > DateTime.Now;

        public bool IsInProgress => Start <= DateTime.Now && End >= DateTime.Now;

        public bool IsPast => End < DateTime.Now;
    }

    public class ScheduleViewModel
    {
        public int ScheduleId { get; set; }

        [DisplayFormat(DataFormatString = "{0:h:mm tt}")]
        public DateTime Start { get; set; }

        [DisplayFormat(DataFormatString = "{0:h:mm tt}")]
        public DateTime End { get; set; }

        public bool IsRegistered { get; set; }

        public string TimeSlot => $"{Start.ToString("h:mm tt")} - {End.ToString("h:mm tt")}";

        public string Duration => $"{(End - Start).TotalHours:0.0} hours";
    }

    public class MyEventViewModel
    {
        public int EventId { get; set; }

        public string Title { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy h:mm tt}")]
        public DateTime EventStart { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy h:mm tt}")]
        public DateTime EventEnd { get; set; }

        public List<MyShiftViewModel> MyShifts { get; set; } = new List<MyShiftViewModel>();

        // Helper properties
        public string EventDate => EventStart.ToString("dddd, MMMM d, yyyy");

        public string EventTime => $"{EventStart.ToString("h:mm tt")} - {EventEnd.ToString("h:mm tt")}";

        public bool IsUpcoming => EventStart > DateTime.Now;

        public bool IsInProgress => EventStart <= DateTime.Now && EventEnd >= DateTime.Now;

        public bool IsPast => EventEnd < DateTime.Now;

        public bool HasCheckableShift => MyShifts.Any(s => s.CanCheckIn);
    }

    public class MyShiftViewModel
    {
        public int AttendanceId { get; set; }

        public int ScheduleId { get; set; }

        [DisplayFormat(DataFormatString = "{0:h:mm tt}")]
        public DateTime ShiftStart { get; set; }

        [DisplayFormat(DataFormatString = "{0:h:mm tt}")]
        public DateTime ShiftEnd { get; set; }

        public DateTime? ActualStart { get; set; }

        public DateTime? ActualEnd { get; set; }

        public TimeSpan TimeUntilShift { get; set; }

        public bool CanCheckIn { get; set; }

        public bool CanCheckOut { get; set; }

        // Helper properties
        public string ShiftTime => $"{ShiftStart.ToString("h:mm tt")} - {ShiftEnd.ToString("h:mm tt")}";

        public string Duration => $"{(ShiftEnd - ShiftStart).TotalHours:0.0} hours";

        public string Status
        {
            get
            {
                if (ActualStart.HasValue && ActualEnd.HasValue)
                    return "Completed";
                else if (ActualStart.HasValue)
                    return "Checked In";
                else if (ShiftStart > DateTime.Now)
                    return "Upcoming";
                else if (ShiftStart <= DateTime.Now && ShiftEnd >= DateTime.Now)
                    return "In Progress";
                else
                    return "Missed";
            }
        }

        public string TimeUntilShiftDisplay
        {
            get
            {
                if (TimeUntilShift.TotalMinutes <= 0)
                    return "Now";

                if (TimeUntilShift.TotalDays >= 1)
                    return $"{(int)TimeUntilShift.TotalDays} day{(TimeUntilShift.TotalDays >= 2 ? "s" : "")}";

                if (TimeUntilShift.TotalHours >= 1)
                    return $"{(int)TimeUntilShift.TotalHours} hour{(TimeUntilShift.TotalHours >= 2 ? "s" : "")}";

                return $"{(int)TimeUntilShift.TotalMinutes} min";
            }
        }
    }

    public class VolunteerShiftViewModel
    {
        public int ScheduleId { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        public int VolunteerId { get; set; }
    }

    public class CheckInViewModel
    {
        public int AttendanceId { get; set; }

        public int VolunteerId { get; set; }

        public string VolunteerName { get; set; }

        public int EventId { get; set; }

        public string EventName { get; set; }

        public string EventLocation { get; set; }

        public string EventCity { get; set; }

        public int ScheduleId { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy h:mm tt}")]
        public DateTime ShiftStart { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy h:mm tt}")]
        public DateTime ShiftEnd { get; set; }

        public DateTime? ActualStart { get; set; }

        public DateTime? ActualEnd { get; set; }

        public bool CanCheckIn { get; set; }

        public bool CanCheckOut { get; set; }

        // Helper properties
        public string ShiftDate => ShiftStart.ToString("dddd, MMMM d, yyyy");

        public string ShiftTime => $"{ShiftStart.ToString("h:mm tt")} - {ShiftEnd.ToString("h:mm tt")}";

        public string Duration => $"{(ShiftEnd - ShiftStart).TotalHours:0.0} hours";

        public string Status
        {
            get
            {
                if (ActualStart.HasValue && ActualEnd.HasValue)
                    return "Completed";
                else if (ActualStart.HasValue)
                    return "Checked In";
                else if (ShiftStart > DateTime.Now)
                    return "Upcoming";
                else if (ShiftStart <= DateTime.Now && ShiftEnd >= DateTime.Now)
                    return "In Progress";
                else
                    return "Missed";
            }
        }

        public TimeSpan TimeUntilShift => ShiftStart - DateTime.Now;

        public string TimeUntilShiftDisplay
        {
            get
            {
                if (TimeUntilShift.TotalMinutes <= 0)
                    return "Now";

                if (TimeUntilShift.TotalDays >= 1)
                    return $"{(int)TimeUntilShift.TotalDays} day{(TimeUntilShift.TotalDays >= 2 ? "s" : "")}";

                if (TimeUntilShift.TotalHours >= 1)
                    return $"{(int)TimeUntilShift.TotalHours} hour{(TimeUntilShift.TotalHours >= 2 ? "s" : "")}";

                return $"{(int)TimeUntilShift.TotalMinutes} min";
            }
        }
    }
}