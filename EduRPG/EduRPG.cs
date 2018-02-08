using Engine;
using System;
using System.Windows.Forms;

namespace EduRPG
{
    public partial class EduRPG : Form
    {
        private Player _player;

        public EduRPG()
        {
            InitializeComponent();

            Location location = new Location(1 , "Home" , "This is your house");

            // Here are a couple different ways to set the _player's values.
            //!_player = new Player(10 , 10 , 50 , 0 , 1);
            //!_player = new Player(currentHitPoints: 10 , maximumHitPoints: 10 , level: 1 , experiencePoints: 0 , gold: 50);
            //We'll go with the top one just to match the tutorial's code scheme.

            _player = new Player(10 , 10 , 50 , 0 , 1);

            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();
        }

        private void btnNorth_Click(object sender , EventArgs e)
        {
        }

        private void btnEast_Click(object sender , EventArgs e)
        {
        }

        private void btnSouth_Click(object sender , EventArgs e)
        {
        }

        private void btnWest_Click(object sender , EventArgs e)
        {
        }

        private void btnUseWeapon_Click(object sender , EventArgs e)
        {
        }

        private void btnUsePotion_Click(object sender , EventArgs e)
        {
        }

        ///This is just a test method for the "Test" button at the beginning.
        ///I'll just comment it out rather than deleting it because I like to see
        ///what I've done in the past and know what I'm able to do.
        /*
            private void btnTest_Click(object sender , EventArgs e)
            {
                Random randValue = new Random();
                lblGold.Text = randValue.Next(0 , 5000).ToString();
                //btnTest.Text = "Random Gold Amount!";
            }
       */
    }
}