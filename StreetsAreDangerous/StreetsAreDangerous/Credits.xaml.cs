using Microsoft.Phone.Controls;
using StreetsAreDangerous.Resources;
using Microsoft.Phone.Tasks;

namespace StreetsAreDangerous
{
    public partial class Credits : PhoneApplicationPage
    {
        public Credits()
        {
            InitializeComponent();
            this.MadeByTextBlock.Text = Strings.MadeByTitle;
            this.MusicByTextBlock.Text = Strings.MusicByTitle;
            this.ThanksToTextBlock.Text = Strings.ThanksToTitle;
            this.DedicatedToTextBlock.Text = Strings.DedicatedToTitle;
        }

        private void MailTextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask xteamsite = new WebBrowserTask();
            xteamsite.Uri = new System.Uri("http://www.xteamdimension.com");
            xteamsite.Show();
        }
    }
}