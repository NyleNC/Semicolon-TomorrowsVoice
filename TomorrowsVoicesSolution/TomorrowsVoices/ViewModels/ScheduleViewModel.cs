using TomorrowsVoices.Models;

namespace TomorrowsVoices.ViewModels
{
    public class ScheduleViewModel
    {
        public List<Schedule> MorningShifts { get; set; } = new List<Schedule>();
        public List<Schedule> AfternoonShifts { get; set; } = new List<Schedule>();
        public List<Schedule> EveningShifts { get; set; } = new List<Schedule>();


        public TimeOnly MorningShiftStartTime => MorningShifts.Any() ? MorningShifts.Min(s => s.ShiftStart) : new TimeOnly(8, 0);
        public TimeOnly MorningShiftEndTime => MorningShifts.Any() ? MorningShifts.Max(s => s.ShiftEnd) : new TimeOnly(12, 0);

        public TimeOnly AfternoonShiftStartTime => AfternoonShifts.Any() ? AfternoonShifts.Min(s => s.ShiftStart) : new TimeOnly(12, 0);
        public TimeOnly AfternoonShiftEndTime => AfternoonShifts.Any() ? AfternoonShifts.Max(s => s.ShiftEnd) : new TimeOnly(17, 0);

        public TimeOnly EveningShiftStartTime => EveningShifts.Any() ? EveningShifts.Min(s => s.ShiftStart) : new TimeOnly(17, 0);
        public TimeOnly EveningShiftEndTime => EveningShifts.Any() ? EveningShifts.Max(s => s.ShiftEnd) : new TimeOnly(22, 0);
        // Dictionary to store total hours per volunteer
        public Dictionary<string, double> VolunteerTotalHours { get; set; } = new Dictionary<string, double>();
    }
}
