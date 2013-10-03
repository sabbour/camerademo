using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CameraDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // To store the captured image
        StorageFile image;

        public MainPage()
        {
            this.InitializeComponent();

            // Handle the DataRequested event for the Share Charm
            DataTransferManager.GetForCurrentView().DataRequested += MainPage_DataRequested;
        }

        /// <summary>
        /// Event handler for the Share event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void MainPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            // Check that we have an image captured first
            if (image != null)
            {
                // Set the parameters for the share request
                // The more parameters you set, the more apps will be able to accept your Share request
                var request = args.Request;
                request.Data.Properties.Title = "My photo";
                request.Data.Properties.Description = "Check out this amazing photo!";
                request.Data.SetText("The photo title");

                // Add the captured photo to a List of StorageFiles
                // Then set it in the data. This is basically the "attachment"
                var storageFiles = new List<StorageFile>();
                storageFiles.Add(image);
                request.Data.SetStorageItems(storageFiles);

                // Get a stream reference for the image to pass it as a bitmap for thumbnail and bitmap
                var imageReference = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(image);
                request.Data.SetBitmap(imageReference);
                request.Data.Properties.Thumbnail = imageReference;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// Permissions needed: Webcam, Microphone, PicturesLibrary, VideosLibrary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // Capture photo
            var camera = new CameraCaptureUI();
            image = await camera.CaptureFileAsync(CameraCaptureUIMode.Photo);

            DisplayNotification("Photo captured", "Hey good lookin'");

            // Save image to Pictures Library
            var saveImageFile = await KnownFolders.PicturesLibrary.CreateFileAsync("test.jpg", CreationCollisionOption.ReplaceExisting);
            await image.CopyAndReplaceAsync(saveImageFile);

            DisplayNotification("Photo saved", "test.jpg");
        }

        /// <summary>
        /// Display a push notification
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        private void DisplayNotification(string title, string description)
        {

            //<toast>
            //    <visual>
            //        <binding template="ToastText02">
            //            <text id="1">title</text>
            //            <text id="2">description</text>
            //        </binding>
            //    </visual>
            //</toast>

            var template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            var headlineElement = template.GetElementsByTagName("text")[0];
            var bodyElement = template.GetElementsByTagName("text")[1];

            headlineElement.AppendChild(template.CreateTextNode(title));
            bodyElement.AppendChild(template.CreateTextNode(description));

            var scheduledNotification = new ScheduledToastNotification(template, DateTimeOffset.Now.AddSeconds(1));
            var notifier = ToastNotificationManager.CreateToastNotifier();
            notifier.AddToSchedule(scheduledNotification);
        }
    }
}
