using System.ComponentModel.DataAnnotations;

namespace Cloud9_2.Models
{
    /// <summary>
    /// Közös logikai csoport az egész Task rendszerben
    /// DB mező: DisplayType (TaskStatusPM, TaskTypePM)
    /// </summary>
    public enum TaskDisplayType
    {
        [Display(Name = "Bejelentés")]
        Bejelentes = 1,

        [Display(Name = "Intézkedés")]
        Intezkedes = 2

        // később:
        // [Display(Name = "Egyéb")]
        // Egyeb = 3
    }

    /// <summary>
    /// Státusz logikai kategória
    /// DB: TaskStatusPM.DisplayType
    /// </summary>
    public enum TaskStatusDisplayType
    {
        [Display(Name = "Bejelentés")]
        Bejelentes = 1,

        [Display(Name = "Intézkedés")]
        Intezkedes = 2
    }

    /// <summary>
    /// Típus logikai kategória
    /// DB: TaskTypePM.DisplayType
    /// </summary>
    public enum TaskTypeDisplayType
    {
        [Display(Name = "Bejelentés")]
        Bejelentes = 1,

        [Display(Name = "Intézkedés")]
        Intezkedes = 2
    }
}
