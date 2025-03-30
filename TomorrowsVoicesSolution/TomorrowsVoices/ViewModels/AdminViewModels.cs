using System;
using System.Collections.Generic;
using TomorrowsVoices.Models;

namespace TomorrowsVoices.ViewModels
{
    public class AdminEventViewModel
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Notes { get; set; }
        public int VolunteerCount { get; set; }
        public List<ShiftViewModel> Shifts { get; set; }
        public int FilledShifts { get; set; }
        public int TotalShifts { get; set; }
    }

    public class ShiftViewModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int VolunteerCount { get; set; }
    }

    public class AdminVolunteerViewModel
    {
        public int VolunteerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public int TotalEvents { get; set; }
        public double TotalHours { get; set; }
        public Event LastEvent { get; set; }
    }

    public class EventDetailsViewModel
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Notes { get; set; }
        public List<ScheduleDetailsViewModel> Schedules { get; set; }
    }

    public class ScheduleDetailsViewModel
    {
        public int ScheduleId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<VolunteerDetailsViewModel> Volunteers { get; set; }
    }

    public class VolunteerDetailsViewModel
    {
        public int VolunteerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public int TotalEvents { get; set; }
        public double TotalHours { get; set; }
        public DateTime? ActualStart { get; set; }
        public DateTime? ActualEnd { get; set; }
        public List<VolunteerEventHistory> EventHistory { get; set; }
    }

    public class VolunteerEventHistory
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string EventLocation { get; set; }
        public string EventCity { get; set; }
        public List<ShiftHistory> Shifts { get; set; }
    }

    public class ShiftHistory
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public DateTime? ActualStart { get; set; }
        public DateTime? ActualEnd { get; set; }
        public double HoursWorked { get; set; }
    }
} 