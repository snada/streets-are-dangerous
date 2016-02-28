using Microsoft.Phone.Controls;
using StreetsAreDangerous.Resources;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using System.Windows;
using Microsoft.Xna.Framework;

namespace StreetsAreDangerous
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        private ContentManager content;
        private IsolatedStorageSettings settings;
        private SoundEffect bark;

        public SettingsPage()
        {
            InitializeComponent();
            PageTitle.Text = Strings.Settings;
            MusicVolumeSettingsLabel.Text = Strings.MusicVolume;
            SoundsVolumeSettingsLabel.Text = Strings.SoundEffectsVolume;
            VibrationSettingsLabel.Text = Strings.VibrationSettingsLabel;

            content = (Application.Current as App).Content;
            bark = content.Load<SoundEffect>("bark");
            FrameworkDispatcher.Update();

            settings = IsolatedStorageSettings.ApplicationSettings;

            MusicSettingsSlider.Value = (float)settings["MusicVolume"];
            SoundsSettingsSlider.Value = (float)settings["SoundsVolume"];
            VibrationCheckBox.IsChecked = (bool)settings["Vibration"];
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            settings["MusicVolume"] = (float)MusicSettingsSlider.Value;
            settings["SoundsVolume"] = (float)SoundsSettingsSlider.Value;
            settings["Vibration"] = VibrationCheckBox.IsChecked.Value;
            base.OnNavigatedFrom(e);
        }

        private void MusicSettingsSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (MediaPlayer.GameHasControl)
            {
                MediaPlayer.Volume = (float)e.NewValue;
            }
        }

        private void SoundsSettingsSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void SoundsSettingsSlider_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            bark.Play((float)SoundsSettingsSlider.Value, 0, 0);
        }
    }
}