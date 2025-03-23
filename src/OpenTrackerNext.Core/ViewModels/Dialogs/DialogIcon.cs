using Ardalis.SmartEnum;

namespace OpenTrackerNext.Core.ViewModels.Dialogs;

/// <summary>
/// Represents the type of dialog icon to display.
/// </summary>
public sealed class DialogIcon : SmartEnum<DialogIcon>
{
    /// <summary>
    /// A <see cref="DialogIcon"/> representing the error icon.
    /// </summary>
    public static readonly DialogIcon Error;
    
    /// <summary>
    /// A <see cref="DialogIcon"/> representing the info icon.
    /// </summary>
    public static readonly DialogIcon Info;
    
    /// <summary>
    /// A <see cref="DialogIcon"/> representing the question icon.
    /// </summary>
    public static readonly DialogIcon Question;
    
    /// <summary>
    /// A <see cref="DialogIcon"/> representing the stop icon.
    /// </summary>
    public static readonly DialogIcon Stop;
    
    /// <summary>
    /// A <see cref="DialogIcon"/> representing the success icon.
    /// </summary>
    public static readonly DialogIcon Success;
    
    /// <summary>
    /// A <see cref="DialogIcon"/> representing the warning icon.
    /// </summary>
    public static readonly DialogIcon Warning;

    /// <summary>
    /// Initializes static members of the <see cref="DialogIcon"/> class.
    /// </summary>
    static DialogIcon()
    {
        Error = new DialogIcon(nameof(Error), 0) { IconValue = "mdi-close-circle" };
        Info = new DialogIcon(nameof(Info), 1) { IconValue = "mdi-information" };
        Question = new DialogIcon(nameof(Question), 2) { IconValue = "mdi-help-circle" };
        Stop = new DialogIcon(nameof(Stop), 3) { IconValue = "mdi-close-octagon" };
        Success = new DialogIcon(nameof(Success), 4) { IconValue = "mdi-check-circle" };
        Warning = new DialogIcon(nameof(Warning), 5) { IconValue = "mdi-alert" };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogIcon"/> class.
    /// </summary>
    /// <param name="name">
    /// A <see cref="string"/> representing the name of the dialog icon.
    /// </param>
    /// <param name="value">
    /// An <see cref="int"/> representing the value of the dialog icon.
    /// </param>
    private DialogIcon(string name, int value)
        : base(name, value)
    {
    }

    /// <summary>
    /// Gets a <see cref="string"/> representing the icon value.
    /// </summary>
    public required string IconValue { get; init; }
}