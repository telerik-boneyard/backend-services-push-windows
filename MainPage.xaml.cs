using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Telerik.Everlive.Sdk.Core;
using Telerik.Everlive.Sdk.Core.Model.System;
using Telerik.Everlive.Sdk.Core.Model.System.Push;
using Telerik.Everlive.Sdk.Core.Transport;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WindowsPushSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private bool isDeviceRegistered;
        public bool IsDeviceRegistered
        {
            get
            {
                return this.isDeviceRegistered;
            }
            set
            {
                if (this.isDeviceRegistered != value)
                {
                    this.isDeviceRegistered = value;
                    this.OnPropertyChanged("IsDeviceRegistered");
                    this.UpdateShowRegisterSecondTileButton();
                }
            }
        }

        private bool isSecondaryTileRegistered;
        public bool IsSecondaryTileRegistered
        {
            get
            {
                return this.isSecondaryTileRegistered;
            }
            set
            {
                if (this.isSecondaryTileRegistered != value)
                {
                    this.isSecondaryTileRegistered = value;
                    this.OnPropertyChanged("IsSecondaryTileRegistered");
                    this.UpdateShowRegisterSecondTileButton();
                }
            }
        }

        private bool isSecondaryTilePinned;
        public bool IsSecondaryTilePinned
        {
            get
            {
                return this.isSecondaryTilePinned;
            }
            set
            {
                if (this.isSecondaryTilePinned != value)
                {
                    this.isSecondaryTilePinned = value;
                    this.OnPropertyChanged("IsSecondaryTilePinned");
                    this.UpdateShowRegisterSecondTileButton();
                }
            }
        }

        private bool showRegisterSecondTileButton;
        public bool ShowRegisterSecondTileButton
        {
            get
            {
                return this.showRegisterSecondTileButton;
            }
            set
            {
                if (this.showRegisterSecondTileButton != value)
                {
                    this.showRegisterSecondTileButton = value;
                    this.OnPropertyChanged("ShowRegisterSecondTileButton");
                }
            }
        }

        private void UpdateShowRegisterSecondTileButton()
        {
            if (this.IsSecondaryTilePinned && this.IsDeviceRegistered)
            {
                if (this.IsSecondaryTileRegistered)
                {
                    this.ShowRegisterSecondTileButton = false;
                }
                else
                {
                    this.ShowRegisterSecondTileButton = true;
                }
            }
            else
            {
                this.ShowRegisterSecondTileButton = false;
            }
        }
		
        private const String SecondaryTileId = "SecondaryTileId";

        EverliveApp everliveApp;

        private const String BackendServicesApiKey = "your-api-key-here";

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
            if (MainPage.BackendServicesApiKey.StartsWith("your"))
            {
                MessageDialog msgDialog = new MessageDialog("You need to set a valid Telerik Backend Services API Key!", "Windows Push Sample");
                UICommand okBtn = new UICommand("OK");
                okBtn.Invoked = OkBtnClick;
                msgDialog.Commands.Add(okBtn);
                msgDialog.ShowAsync();
            }
            else
            {
                everliveApp = new EverliveApp(MainPage.BackendServicesApiKey);
                everliveApp.AppSettings.ServiceUrl = "localhost:3000";
                this.CheckRegistration();
                this.CheckSecondaryTiles();
                this.CheckSecondaryTileRegistration();
            }
        }

        private void OkBtnClick(IUICommand command)
        {
            Application.Current.Exit();
        }

        private void CheckRegistration()
        {
            var result = everliveApp.WorkWith().Push().CurrentDevice.GetRegistration().ExecuteSync(5000);

            if (result != null)
            {
                this.IsDeviceRegistered = true;
            }
            else
            {
                this.IsDeviceRegistered = false;
            }
        }

        private void CheckSecondaryTileRegistration()
        {
            if (this.IsDeviceRegistered)
            {
                var result = everliveApp.WorkWith().Push().CurrentDevice.GetRegistration().ExecuteSync(5000);
                if (result != null && result.SecondaryPushTokens != null)
                {
                    Dictionary<string, PushTokenDetails> secondaryTiles = result.SecondaryPushTokens;
                    string base64TileId = Convert.ToBase64String(Encoding.UTF8.GetBytes(MainPage.SecondaryTileId));
                    this.IsSecondaryTileRegistered = secondaryTiles.ContainsKey(base64TileId);
                }
            }
            else
            {
                this.IsSecondaryTileRegistered = false;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached. The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void OnRegisterClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = everliveApp.WorkWith().Push().CurrentDevice
                        .Register(new DeviceParameters())
                        .ExecuteSync(5000);

                if (result.CreatedAt != null)
                {
                    this.SetSuccessStatus("Device registered successfully!");
                    this.IsDeviceRegistered = true;
                }
                else
                {
                    this.SetErrorMessage("Cannot register device!");
                }
            }
            catch (EverliveException ex)
            {
                this.SetErrorMessage("Error on registering device: " + ex.Message);
            }
            catch (AggregateException aggEx)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var exception in aggEx.InnerExceptions)
                {
                    sb.AppendLine(string.Format("{0}: {1}", exception.ToString(), exception.Message));
                }
                this.SetErrorMessage("Error on registering device: " + sb.ToString());
            }
        }

        private void SetSuccessStatus(String statusMessage)
        {
            this.statusText.Text = statusMessage;
            this.statusText.Foreground = new SolidColorBrush(Colors.Green);
        }

        private void SetErrorMessage(String errorMessage)
        {
            this.statusText.Text = errorMessage;
            this.statusText.Foreground = new SolidColorBrush(Colors.Red);
        }

        private void OnUnregisterClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = everliveApp.WorkWith().Push().CurrentDevice.Unregister().ExecuteSync(5000);

                if (result)
                {
                    this.SetSuccessStatus("Device unregistered successfully!");
                    this.IsDeviceRegistered = false;
                    this.IsSecondaryTileRegistered = false;
                }
                else
                {
                    this.SetErrorMessage("Error on unregistering device!");
                }
            }
            catch (Exception ex)
            {
                this.SetErrorMessage("Error on unregistering device! - ErrorMessage: " + ex.Message);
            }
        }

        private void OnSendClick(object sender, RoutedEventArgs e)
        {
            PushNotification pushNotification = new PushNotification();
            pushNotification.Message = this.pushNotificationMessage.Text;
            // sends a toast notification with template "TextToast01"
            var result = everliveApp.WorkWith().Push().Notifications().Create(pushNotification).ExecuteSync();

            if (result.CreatedAt != null)
            {
                this.SetSuccessStatus("Message: \"" + pushNotification.Message + "\" sent!");
            }
            else
            {
                this.SetErrorMessage("Failed to send message: " + pushNotification.Message);
            }
        }

        private async void OnPinSecondTileClick(object sender, RoutedEventArgs e)
        {
            Uri logo = new Uri("ms-appx:///Assets/tile_telerik_platform_150x150.png");
            Uri smallLogo = new Uri("ms-appx:///Assets/tile_telerik_platform_50x50.png");

            // During creation of secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
            // These arguments should be meaningful to the application. In this sample, we'll pass in the date and time the secondary tile was pinned.
            string tileActivationArguments = MainPage.SecondaryTileId + " WasPinnedAt=" + DateTime.Now.ToLocalTime().ToString();

            // Create a 1x1 Secondary tile
            SecondaryTile secondaryTile = new SecondaryTile(MainPage.SecondaryTileId,
                                                            "SecondaryTile",
                                                            "Secondary Tile",
                                                            tileActivationArguments,
                                                            TileOptions.ShowNameOnLogo,
                                                            logo);

            // Specify a foreground text value.
            // The tile background color is inherited from the parent unless a separate value is specified.
            secondaryTile.ForegroundText = ForegroundText.Light;

            // Like the background color, the small tile logo is inherited from the parent application tile by default. Let's override it, just to see how that's done.
            secondaryTile.SmallLogo = smallLogo;

            // OK, the tile is created and we can now attempt to pin the tile.
            // Note that the status message is updated when the async operation to pin the tile completes.
            bool isPinned = await secondaryTile.RequestCreateForSelectionAsync(MainPage.GetElementRect((FrameworkElement)sender), Windows.UI.Popups.Placement.Below);

            if (isPinned)
            {
                this.SetSuccessStatus("Secondary tile was successfully pinned.");
                this.IsSecondaryTilePinned = true;
            }
            else
            {
                this.SetErrorMessage("Secondary tile is not pinned.");
            }
        }

        private async void OnUnpinSecondTileClick(object sender, RoutedEventArgs e)
        {
            SecondaryTile secondaryTile = new SecondaryTile(MainPage.SecondaryTileId);
            bool isUnpinned = await secondaryTile.RequestDeleteForSelectionAsync(MainPage.GetElementRect((FrameworkElement)sender), Windows.UI.Popups.Placement.Below);
            if (isUnpinned)
            {
                this.SetSuccessStatus("Secondary tile was successfully unpinned.");
                this.IsSecondaryTilePinned = false;
            }
            else
            {
                this.SetErrorMessage("Secondary tile is not unpinned.");
            }
        }

        private void OnSendPushToSecondTileClick(object sender, RoutedEventArgs e)
        {
            // Windows: {
            //      "Tile": {
            //          "template": "TileSquare150x150PeekImageAndText01",
            //          "fallback": "TileSquarePeekImageAndText01"
            //          "text": ["WindowsPushSample", "Text2", "Text3", "Text4"],
            //          "image": [".\\Assets\\tile_telerik_platform_150x150.png"]
            //      },
            //      "SecondaryTileId": MainPage.SecondaryTileId
            //}
            JObject windows = new JObject();
            JObject tile = new JObject();
            tile["template"] = "TileSquarePeekImageAndText01";
            tile["text"] = new JArray(new String[] { this.text1.Text, this.text2.Text, this.text3.Text, this.text4.Text});
            tile["image"] = new JArray(new String[] { this.image1.Text });
            windows["Tile"] = tile;
            windows["SecondaryTileId"] = MainPage.SecondaryTileId;

            PushNotification pushNotification = new PushNotification();
            pushNotification.Windows = windows;

            var result = everliveApp.WorkWith().Push().Notifications().Create(pushNotification).ExecuteSync(5000);

            if (result.CreatedAt != null)
            {
                this.SetSuccessStatus("Successfully sent a secondary tile notification!");
            }
            else
            {
                this.SetErrorMessage("Failed to send a secondary tile notification!");
            }
        }

        private void OnRegisterSecondTileClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = everliveApp.WorkWith().Push().CurrentDevice
                                .UpdateRegistration(MainPage.SecondaryTileId)
                                .ExecuteSync(5000);
                if (result)
                {
                    this.SetSuccessStatus("SecondaryTile with Id: \"" + MainPage.SecondaryTileId + "\" registered successfully!");
                    this.IsSecondaryTileRegistered = true;
                }
                else
                {
                    this.SetErrorMessage("Error on registering secondary tile!");
                }
            }
            catch (EverliveException ex)
            {
                this.SetErrorMessage("Error on registering device: " + ex.Message);
            }
        }

        // Gets the rectangle of the element
        public static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

        private void OnUnregisterSecondTileClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = everliveApp.WorkWith().Push().CurrentDevice
                    .UnregisterSecondaryTile(MainPage.SecondaryTileId)
                    .ExecuteSync(5000);
                if (result)
                {
                    this.SetSuccessStatus("Secondary tile unregistered successfully!");
                    this.IsSecondaryTileRegistered = false;
                }
                else
                {
                    this.SetErrorMessage("Error on unregistering secondary tile!");
                }
            }
            catch (Exception ex)
            {
                this.SetErrorMessage("Error on unregistering secondary tile! - ErrorMessage: " + ex.Message);
            }
        }

        private async void CheckSecondaryTiles()
        {
            IReadOnlyList<SecondaryTile> tilelist = await Windows.UI.StartScreen.SecondaryTile.FindAllAsync();
            if (tilelist.Count > 0)
            {
                this.IsSecondaryTilePinned = true;
            }
            else
            {
                this.IsSecondaryTilePinned = false;
            }
        }
    }
}
