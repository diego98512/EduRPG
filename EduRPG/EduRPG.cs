using Engine;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EduRPG
{
    public partial class EduRPG : Form
    {
        private Player _player;
        private Monster _currentMonster;

        public EduRPG()
        {
            InitializeComponent();

            //Location location = new Location(1 , "Home" , "This is your house");
            // Here are a couple different ways to set the _player's values.
            //!_player = new Player(10 , 10 , 50 , 0 , 1);
            //!_player = new Player(currentHitPoints: 10 , maximumHitPoints: 10 , level: 1 , experiencePoints: 0 , gold: 50);
            //We'll go with the top one just to match the tutorial's code scheme.

            _player = new Player( currentHitPoints: 10 , maximumHitPoints: 10 , currentMana: 20 , maximumMana: 20 , gold: 50 , experiencePoints: 0 , level: 1 , statusEffect: 0 );
            MoveTo( World.LocationByID( World.LOCATION_ID_HOME ) );
            _player.Inventory.Add( new InventoryItem( World.ItemByID( World.ITEM_ID_RUSTY_SWORD ) , 1 ) );
            _player.BigSpellBook.Add( new SpellBook( World.SpellByID( World.SPELL_ID_FIRE ) , 1 ) );

            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();
            lblMana.Text = _player.CurrentMana.ToString();
        }

        private void btnNorth_Click( object sender , EventArgs e )
        {
            MoveTo( _player.CurrentLocation.LocationToNorth );
        }

        private void btnEast_Click( object sender , EventArgs e )
        {
            MoveTo( _player.CurrentLocation.LocationToEast );
        }

        private void btnSouth_Click( object sender , EventArgs e )
        {
            MoveTo( _player.CurrentLocation.LocationToSouth );
        }

        private void btnWest_Click( object sender , EventArgs e )
        {
            MoveTo( _player.CurrentLocation.LocationToWest );
        }

        private void MoveTo( Location newLocation )
        {
            //Does the location have any required items?
            if ( !_player.HasRequiredItemToEnterThisLocation( newLocation ) )
            {
                rtbMessages.Text += "You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine;
                return;
            }

            //Update the player's current location.
            _player.CurrentLocation = newLocation;

            //Show/hide available movement buttons based on location available.
            btnNorth.Visible = ( newLocation.LocationToNorth != null );
            btnEast.Visible = ( newLocation.LocationToEast != null );
            btnSouth.Visible = ( newLocation.LocationToSouth != null );
            btnWest.Visible = ( newLocation.LocationToWest != null );

            //Display current location name and description.
            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;

            //Completely heal the player.
            _player.CurrentHitPoints = _player.MaximumHitPoints;

            //Update hit points in UI.
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();

            //Does the location have a quest?
            if ( newLocation.QuestAvailableHere != null )
            {
                //See if the player already has the quest and if they've completed it.
                bool playerAlreadyHasQuest = _player.HasThisQuest( newLocation.QuestAvailableHere );
                bool playerAlreadyCompletedQuest = _player.CompletedThisQuest( newLocation.QuestAvailableHere );

                foreach ( PlayerQuest playerQuest in _player.Quests )
                {
                    if ( playerQuest.Details.ID == newLocation.QuestAvailableHere.ID )
                    {
                        playerAlreadyHasQuest = true;

                        if ( playerQuest.IsCompleted )
                        {
                            playerAlreadyCompletedQuest = true;
                        }
                    }
                }

                //See if the player already has the quest.
                if ( playerAlreadyHasQuest )
                {
                    //If player has not already completed the quest yet.
                    if ( !playerAlreadyCompletedQuest )
                    {
                        //See if player has all the items needed to complete the quest
                        bool playerHasAllItemsToCompleteQuest = _player.HasAllQuestCompletionItems( newLocation.QuestAvailableHere );

                        //The player has all items required to complete quest.
                        if ( playerHasAllItemsToCompleteQuest )
                        {
                            //Display message.
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "You have completed the " + newLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;

                            // Remove quest items from inventory
                            _player.RemoveQuestCompletionItems( newLocation.QuestAvailableHere );

                            //Give quest rewards.
                            rtbMessages.Text += "You've received " + Environment.NewLine;
                            rtbMessages.Text +=
                           newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() +
                           " experience points and " + Environment.NewLine;
                            rtbMessages.Text +=
                           newLocation.QuestAvailableHere.RewardGold.ToString() +
                           " gold!" + Environment.NewLine;
                            rtbMessages.Text +=
                           newLocation.QuestAvailableHere.RewardItem.Name +
                           Environment.NewLine;
                            rtbMessages.Text += Environment.NewLine;

                            _player.ExperiencePoints +=
                           newLocation.QuestAvailableHere.RewardExperiencePoints;
                            _player.Gold += newLocation.QuestAvailableHere.RewardGold;

                            // Add the reward item to the player's inventory
                            _player.AddItemToInventory( newLocation.QuestAvailableHere.RewardItem );

                            // Mark the quest as completed
                            _player.MarkQuestCompleted( newLocation.QuestAvailableHere );
                        }
                    }
                }
                else
                {
                    // The player does not already have the quest

                    // Display the messages
                    rtbMessages.Text += "You've received the " +
                   newLocation.QuestAvailableHere.Name +
                   " quest." + Environment.NewLine;
                    rtbMessages.Text += newLocation.QuestAvailableHere.Description +
                   Environment.NewLine;
                    rtbMessages.Text += "To complete it, return with:" +
                   Environment.NewLine;
                    foreach ( QuestCompletionItem qci in
                   newLocation.QuestAvailableHere.QuestCompletionItems )
                    {
                        if ( qci.Quantity == 1 )
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " +
                           qci.Details.Name + Environment.NewLine;
                        }
                        else
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " +
                           qci.Details.NamePlural + Environment.NewLine;
                        }
                    }
                    rtbMessages.Text += Environment.NewLine;

                    // Add the quest to the player's quest list
                    _player.Quests.Add( new PlayerQuest( newLocation.QuestAvailableHere ) );
                }
            }

            //Does the location have a monster?
            if ( newLocation.MonsterLivingHere != null )
            {
                rtbMessages.Text += "You see a " + newLocation.MonsterLivingHere.Name +
               Environment.NewLine;

                // Make a new monster, using the values from the standard monster in the World.Monster list
                Monster standardMonster = World.MonsterByID(
               newLocation.MonsterLivingHere.ID );

                _currentMonster = new Monster( standardMonster.ID , standardMonster.Name ,
                standardMonster.MaximumDamage , standardMonster.RewardExperiencePoints ,
               standardMonster.RewardGold , standardMonster.CurrentHitPoints ,
               standardMonster.MaximumHitPoints , standardMonster.CurrentMana , standardMonster.MaximumMana , standardMonster.StatusEffect );

                foreach ( LootItem lootItem in standardMonster.LootTable )
                {
                    _currentMonster.LootTable.Add( lootItem );
                }

                cboWeapons.Visible = true;
                cboPotions.Visible = true;
                cboSpells.Visible = true;
                btnUseWeapon.Visible = true;
                btnUsePotion.Visible = true;
                btnCastSpell.Visible = true;
            }
            else
            {
                _currentMonster = null;

                cboWeapons.Visible = false;
                cboPotions.Visible = false;
                cboSpells.Visible = false;
                btnUseWeapon.Visible = false;
                btnUsePotion.Visible = false;
                btnCastSpell.Visible = false;
            }

            //Refresh player's inventory list
            UpdateInventoryListInUI();

            //Refresh player's quest list
            UpdateQuestListInUI();

            //Refresh player's weapon list
            UpdateWeaponListInUI();

            //Refresh player's potion list
            UpdatePotionListInUI();

            //Refresh player's spell list
            UpdateSpellsListInUI();

        }

        private void UpdateInventoryListInUI()
        {
            dgvInventory.RowHeadersVisible = false;
            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[ 0 ].Name = "Name";
            dgvInventory.Columns[ 0 ].Width = 197;
            dgvInventory.Columns[ 1 ].Name = "Quantity";
            dgvInventory.Rows.Clear();

            foreach ( InventoryItem inventoryItem in _player.Inventory )
            {
                if ( inventoryItem.Quantity > 0 )
                {
                    dgvInventory.Rows.Add( new[] {
                    inventoryItem.Details.Name,
                    inventoryItem.Quantity.ToString() } );
                }
            }
        }

        private void UpdateQuestListInUI()
        {
            dgvQuests.RowHeadersVisible = false;
            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[ 0 ].Name = "Name";
            dgvQuests.Columns[ 0 ].Width = 197;
            dgvQuests.Columns[ 1 ].Name = "Done?";
            dgvQuests.Rows.Clear();

            foreach ( PlayerQuest playerQuest in _player.Quests )
            {
                dgvQuests.Rows.Add( new[] {
                playerQuest.Details.Name,
                playerQuest.IsCompleted.ToString() } );
            }
        }

        private void UpdateWeaponListInUI()
        {
            List<Weapon> weapons = new List<Weapon>();

            foreach ( InventoryItem inventoryItem in _player.Inventory )
            {
                if ( inventoryItem.Details is Weapon )
                {
                    if ( inventoryItem.Quantity > 0 )
                    {
                        weapons.Add( (Weapon)inventoryItem.Details );
                    }
                }
            }
            if ( weapons.Count == 0 )
            {
                // The player doesn't have any weapons, so hide the weapon combobox and "Use" button
                cboWeapons.Visible = false;
                btnUseWeapon.Visible = false;
            }
            else
            {
                cboWeapons.DataSource = weapons;
                cboWeapons.DisplayMember = "Name";
                cboWeapons.ValueMember = "ID";
                cboWeapons.SelectedIndex = 0;
            }
        }

        private void UpdatePotionListInUI()
        {
            List<HealingPotion> healingPotions = new List<HealingPotion>();

            foreach ( InventoryItem inventoryItem in _player.Inventory )
            {
                if ( inventoryItem.Details is HealingPotion )
                {
                    if ( inventoryItem.Quantity > 0 )
                    {
                        healingPotions.Add( (HealingPotion)inventoryItem.Details );
                    }
                }
            }

            if ( healingPotions.Count == 0 )
            {
                // The player doesn't have any potions, so hide the potion combobox and the "Use" button
                cboPotions.Visible = false;
                btnUsePotion.Visible = false;
            }
            else
            {
                cboPotions.DataSource = healingPotions;
                cboPotions.DisplayMember = "Name";
                cboPotions.ValueMember = "ID";

                cboPotions.SelectedIndex = 0;
            }
        }

        private void UpdateSpellsListInUI()
        {
            List<Spell> spells = new List<Spell>();

            foreach ( SpellBook spellBook in _player.BigSpellBook )
            {
                if ( spellBook.Details is Spell )
                {
                    if ( spellBook.Quantity > 0 )
                    {
                        spells.Add( (Spell)spellBook.Details );
                    }
                }
            }

            if ( spells.Count == 0 )
            {
                // The player doesn't have any spells, so hide the spell combobox and the "Cast" button
                cboSpells.Visible = false;
                btnCastSpell.Visible = false;
            }
            else
            {
                cboSpells.DataSource = spells;
                cboSpells.DisplayMember = "Name";
                cboSpells.ValueMember = "ID";

                cboSpells.SelectedIndex = 0;
            }
        }

        private void btnUseWeapon_Click( object sender , EventArgs e )
        {
        }

        private void btnUsePotion_Click( object sender , EventArgs e )
        {
        }

        private void EduRPG_Load( object sender , EventArgs e )
        {
        }
    }
}