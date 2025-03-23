using Avalonia.Controls;
using Avalonia.Controls.Templates;
using OpenTrackerNext.Core.ViewModels;
using Splat;

namespace OpenTrackerNext.Core.DataTemplates;

/// <inheritdoc />
public sealed class ViewLocator : IDataTemplate
{
    /// <inheritdoc/>
    public Control Build(object? data)
    {
        if (data is not ViewModel viewModel)
        {
            return new TextBlock { Text = $"Not a ViewModel: {data?.GetType().FullName}" };
        }
        
        var viewType = viewModel.GetViewType();

        var instance = Locator.Current.GetService(viewType);
        
        return instance as Control ??
               new TextBlock { Text = $"View is not a valid Control: {viewType.FullName}" };
    }

    /// <inheritdoc/>
    public bool Match(object? data)
    {
        return data is ViewModel;
    }
}