using Avalonia;
using Avalonia.Markup.Xaml;

namespace OpenTrackerNext.Editor.UnitTests;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}