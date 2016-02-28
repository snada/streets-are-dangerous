using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Threading;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using StreetsAreDangerous.Resources;
using StreetsAreDangerous.Scores;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;

namespace StreetsAreDangerous
{
    public partial class MainPage : PhoneApplicationPage
    {
        ContentManager content;

        Score[] scores;
        bool showScores;

        public MainPage()
        {
            InitializeComponent();

            content = (Application.Current as App).Content;
            FrameworkDispatcher.Update();

            Strings.Culture = Thread.CurrentThread.CurrentCulture;
            PlayButton.Content = Strings.PlayButton;
            ScoresButton.Content = Strings.ScoresButton;
            HowToButton.Content = Strings.HowToButton;
            SettingsButton.Content = Strings.Settings;
            CreditsButton.Content = Strings.CreditsButton;
            ScoresBackButton.Content = Strings.ScoresBackButton;
            ScoresTitleTextBox.Text = Strings.ScoresButton;
            NoScoresTextBlock.Text = Strings.NoScores;

            ResetStoryBoard.Completed += new EventHandler(ResetStoryBoard_Completed);
            listBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(listBox_SelectionChanged);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (showScores)
            {
                e.Cancel = true;
                showScores = !showScores;
                ScoresOutStoryBoard.Begin();
                OpeningStoryBoard.Begin();
            }
            base.OnBackKeyPress(e);
        }

        void listBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            listBox.SelectedIndex = -1;
        }

        void ResetStoryBoard_Completed(object sender, EventArgs e)
        {
            OpeningStoryBoard.Begin();
        }

        /// <summary>
        /// Metodo di gestione di quando si naviga VIA da questa pagina.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Metodo di gestione dell'evento di navigazione su QUESTA pagina.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

            //Se non c'è alcun Default Name salvato
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains("DefaultName"))
                settings.Add("DefaultName", Strings.DefaultName);

            if (!settings.Contains("SoundsVolume"))
                settings.Add("SoundsVolume", 1.0f);

            //PRIMA VOLTA CHE GIOCHI. IL FILE NON ESISTE: GLIELO CREO CON UN ARRAY VUOTO.
            if (!storage.FileExists("scores.xml"))
            {
                using (IsolatedStorageFileStream stream = storage.OpenFile("scores.xml", FileMode.OpenOrCreate))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(Score[]));
                    xml.Serialize(stream, new Score[0]);
                    stream.Close();
                    stream.Dispose();
                }
            }

            //CARICO I PUNTEGGI SALVATI.
            using (IsolatedStorageFileStream stream = storage.OpenFile("scores.xml", FileMode.Open))
            {
                XmlSerializer xml = new XmlSerializer(typeof(Score[]));
                scores = xml.Deserialize(stream) as Score[];
                stream.Close();
                stream.Dispose();
            }
            storage.Dispose();

            if (scores.Length < 1)
                listBox.Items.Clear(); //Lista dei punteggi vuota
            else
            {
                //La lista dei punteggi è piena.
                NoScoresTextBlock.Visibility = System.Windows.Visibility.Collapsed;
                listBox.DataContext = scores;
                listBox.UpdateLayout();
            }

            ResetStoryBoard.Begin();
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Gestione del click del bottone di gioco.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Gestione del click del bottone di visualizzazione dei punteggi.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScoresButton_Click(object sender, RoutedEventArgs e)
        {
            showScores = true;
            ScoresInStoryBoard.Begin();
        }

        /// <summary>
        /// Gestione del click per tornare indietro dalla visualizzazione dei punteggi.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScoresBackButton_Click(object sender, RoutedEventArgs e)
        {
            showScores = !showScores;
            ScoresOutStoryBoard.Begin();
            OpeningStoryBoard.Begin();
        }

        private void HowToButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/HowTo.xaml", UriKind.Relative));
        }

        private void CreditsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Credits.xaml", UriKind.Relative));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }
    }
}
