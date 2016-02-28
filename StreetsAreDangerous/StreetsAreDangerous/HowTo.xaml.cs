using Microsoft.Phone.Controls;
using StreetsAreDangerous.Resources;
using System.Windows.Controls;

namespace StreetsAreDangerous
{
    public partial class HowTo : PhoneApplicationPage
    {
        public HowTo()
        {
            InitializeComponent();
            ((PivotItem)HowToPivot.Items[0]).Header = Strings.MovementTitle; ;
            ((PivotItem)HowToPivot.Items[1]).Header = "PowerUps";
            ((PivotItem)HowToPivot.Items[2]).Header = Strings.ObstaclesTitle;
            ((PivotItem)HowToPivot.Items[3]).Header = Strings.ScoresTitle;
            HowToPivot.Title = Strings.HowToTitle;
            MovementTextBlock.Text = Strings.MovementText;
            PowerUpsTextBlock.Text = Strings.PowerUpsText;
            ObstaclesTextBlock.Text = Strings.ObstaclesText;
            ScoresTextBlock.Text = Strings.ScoresText;
        }
    }
}