using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using DialogHostAvalonia;
using OpenTrackerNext.Core.Results;
using OpenTrackerNext.Core.ViewModels.Dialogs;
using OpenTrackerNext.Editor.ViewModels;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Views;

/// <inheritdoc />
[SplatIgnoreReactiveView]
public sealed partial class MainWindow : ReactiveWindow<MainViewModel>
{
    private const string NewFolderDialogMessage = "OpenTrackerNext Editor - Select New Pack Folder";
    private const string OpenFolderDialogMessage = "OpenTrackerNext Editor - Select Existing Pack Folder";

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        
        this.WhenActivated(
            disposables =>
            {
                this.OneWayBind(
                        ViewModel,
                        vm => vm.Title,
                        v => v.Title)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.TopMenuItems,
                        v => v.TopMenu.ItemsSource)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.ToolLayout,
                        v => v.ContentControl.Content)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.ToolLayoutIsVisible,
                        v => v.ContentControl.IsVisible)
                    .DisposeWith(disposables);

                if (ViewModel is null)
                {
                    return;
                }

                ViewModel.NewPackDialog
                    .RegisterHandler(
                        async context =>
                            context.SetOutput(await FolderDialogAsync(NewFolderDialogMessage).ConfigureAwait(true)))
                    .DisposeWith(disposables);
                ViewModel.OpenPackDialog
                    .RegisterHandler(
                        async context =>
                            context.SetOutput(await FolderDialogAsync(OpenFolderDialogMessage).ConfigureAwait(true)))
                    .DisposeWith(disposables);
                ViewModel.AddImagesDialog
                    .RegisterHandler(
                        async context =>
                        {
                            var result = await OpenAddImagesDialogAsync().ConfigureAwait(true);
                            context.SetOutput(result);
                        })
                    .DisposeWith(disposables);
                ViewModel.TextBoxDialog
                    .RegisterHandler(
                        async context =>
                        {
                            var result = await OpenTextBoxDialogAsync(context.Input).ConfigureAwait(true);
                            context.SetOutput(result);
                        })
                    .DisposeWith(disposables);
                ViewModel.YesNoDialog
                    .RegisterHandler(
                        async context =>
                        {
                            var result = await OpenYesNoDialogAsync(context.Input).ConfigureAwait(true);
                            context.SetOutput(result);
                        })
                    .DisposeWith(disposables);
                ViewModel.YesNoCancelDialog
                    .RegisterHandler(
                        async context =>
                        {
                            var result = await OpenYesNoCancelDialogAsync(context.Input).ConfigureAwait(true);
                            context.SetOutput(result);
                        })
                    .DisposeWith(disposables);
                ViewModel.ExceptionDialog
                    .RegisterHandler(
                        async context =>
                        {
                            await OpenExceptionDialogAsync(context.Input).ConfigureAwait(true);
                            context.SetOutput(Unit.Default);
                        })
                    .DisposeWith(disposables);
            });
    }

    private async Task<IStorageFolder?> FolderDialogAsync(string dialogTitle)
    {
        var options = new FolderPickerOpenOptions
        {
            Title = dialogTitle,
            AllowMultiple = false
        };

        var result = await StorageProvider
            .OpenFolderPickerAsync(options)
            .ConfigureAwait(true);

        return result.Count > 0 ? result[0] : null;
    }

    private async Task<IReadOnlyList<IStorageFile>?> OpenAddImagesDialogAsync()
    {
        var options = new FilePickerOpenOptions
        {
            Title = "OpenTrackerNext Editor - Add Image File(s)",
            AllowMultiple = true,
            FileTypeFilter = new List<FilePickerFileType> { FilePickerFileTypes.ImageAll }
        };

        var results = await StorageProvider
            .OpenFilePickerAsync(options)
            .ConfigureAwait(true);

        return results.Count > 0 ? results : null;
    }

    private async Task<OperationResult> OpenTextBoxDialogAsync(TextBoxDialogViewModel viewModel)
    {
        var result = await DialogHost
            .Show(viewModel, MainDialogHost)
            .ConfigureAwait(true);

        if (result is not OperationResult operationResult)
        {
            throw new InvalidDataException("Invalid dialog result.");
        }

        return operationResult;
    }

    private async Task<YesNoDialogResult> OpenYesNoDialogAsync(YesNoDialogViewModel viewModel)
    {
        var result = await DialogHost
            .Show(viewModel, MainDialogHost)
            .ConfigureAwait(true);

        if (result is not YesNoDialogResult yesNoResult)
        {
            throw new InvalidDataException("Invalid dialog result.");
        }

        return yesNoResult;
    }

    private async Task<YesNoCancelDialogResult> OpenYesNoCancelDialogAsync(YesNoCancelDialogViewModel viewModel)
    {
        var result = await DialogHost
            .Show(viewModel, MainDialogHost)
            .ConfigureAwait(true);

        if (result is not YesNoCancelDialogResult yesNoCancelResult)
        {
            throw new InvalidDataException("Invalid dialog result.");
        }

        return yesNoCancelResult;
    }

    private async Task OpenExceptionDialogAsync(ExceptionDialogViewModel viewModel)
    {
        await DialogHost
            .Show(viewModel, MainDialogHost)
            .ConfigureAwait(true);
    }
}