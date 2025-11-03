using Afterpelago.Layout;
using Afterpelago.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace Afterpelago.Pages.Setup
{
    /// <summary>
    /// Represents the state of the Log File
    /// @todo There's probably a generic version of this enum somewhere that can be used instead
    /// </summary>
    enum LogUploadStatus
    {
        NotStarted,
        Processing,
        Completed
    }

    public partial class Setup
    {
        [Inject]
        IDialogService DialogService { get; set; }

        [Inject]
        NavigationManager NavManager { get; set; }

        /// <summary>
        /// The status of the Log File upload process
        /// </summary>
        private LogUploadStatus LogFileStatus = LogUploadStatus.NotStarted;

        /// <summary>
        /// The icon to display for the status of the log file
        /// </summary>
        private string LogUploadIcon = Icons.Material.Filled.CloudUpload;

        #region File IO Handlers

        /// <summary>
        /// Processes the Log File uploaded by the user
        /// </summary>
        /// <param name="file"></param>
        private async Task ProcessLogFile(IBrowserFile file)
        {
            // Set initial flags for processing
            LogFileStatus = LogUploadStatus.Processing;
            LogUploadIcon = "";

            await LogManager.ReadFromFile(file);

            // Set flags for completion
            LogFileStatus = LogUploadStatus.Completed;
            LogUploadIcon = Icons.Material.Filled.CheckCircle;
        }

        #endregion

        #region Wizard Validation

        /// <summary>
        /// Listens for when the user attempts to navigate between steps
        /// </summary>
        private async Task OnPreviewNavigation(StepperInteractionEventArgs e)
        {
            // Listen for clicking Step headers and for clicking "Next"
            if(e.Action == StepAction.Complete || e.Action == StepAction.Activate)
            {
                switch(e.StepIndex)
                {
                    // Step 1: Log Upload
                    case 0:
                        {
                            // If the log file has not been uploaded yet, block navigation
                            if(LogFileStatus != LogUploadStatus.Completed)
                            {
                                // Display an error
                                await DialogService.ShowMessageBox(
                                    LogFileStatus == LogUploadStatus.NotStarted ? "Log File Required" : "Please Wait",
                                    LogFileStatus == LogUploadStatus.NotStarted ? "Please upload a log file before proceeding." : "Your log file is still being processed.",
                                    "OK"
                                );

                                // Bounce the navigation request
                                e.Cancel = true;
                            }
                            break;
                        }
                    // Step 2: Obtain Game Data
                    case 1:
                        {
                            Archipelago.SetupComplete = true;
                            NavManager.NavigateTo("/report/dashboard");
                            break;
                        }
                }
            }
        }

        #endregion
    }
}
