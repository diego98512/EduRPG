using Engine;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace EduRPG
{
    public partial class EduRPG : Form
    {

        //TODO Implement custom actions
        //TODO Implement spell casting (w/ mana consumption, spell leveling, etc.)
        //TODO Implement shops (weapon shops, general item shops, spell shops)
        //TODO Implement more monsters
        //TODO Add more locations and quests
        //TODO Add visuals?

        private Player _player;
        private Monster _currentMonster;
        private const string PLAYER_DATA_FILE_NAME = "PlayerData.xml";

        public EduRPG()
        {
            InitializeComponent();
            if ( File.Exists( PLAYER_DATA_FILE_NAME ) )
            {
                _player = Player.CreatePlayerFromXmlString(
                File.ReadAllText( PLAYER_DATA_FILE_NAME ) );
            }
            else
            {
                _player = Player.CreateDefaultPlayer();
            }
            MoveTo( _player.CurrentLocation );
            UpdatePlayerStats();
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
                ScrollToBottomOfMessages();
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

            //Restore player's mana
            _player.CurrentMana = _player.MaximumMana;

            //Update hit points in UI.
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();

            //Update mana in UI
            lblMana.Text = _player.CurrentMana.ToString();

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
                            ScrollToBottomOfMessages();

                            // Remove quest items from inventory
                            _player.RemoveQuestCompletionItems( newLocation.QuestAvailableHere );

                            //Give quest rewards.
                            rtbMessages.Text += "You've received " + Environment.NewLine;
                            ScrollToBottomOfMessages();
                            rtbMessages.Text +=
                           newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() +
                           " experience points and " + Environment.NewLine;
                            rtbMessages.Text +=
                           newLocation.QuestAvailableHere.RewardGold.ToString() +
                           " gold!" + Environment.NewLine;
                            rtbMessages.Text +=
                           newLocation.QuestAvailableHere.RewardItem.Name +
                           Environment.NewLine;
                            ScrollToBottomOfMessages();
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
                    ScrollToBottomOfMessages();
                    rtbMessages.Text += newLocation.QuestAvailableHere.Description +
                   Environment.NewLine;
                    ScrollToBottomOfMessages();
                    rtbMessages.Text += "To complete it, return with:" +
                   Environment.NewLine;
                    ScrollToBottomOfMessages();
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
                    ScrollToBottomOfMessages();

                    // Add the quest to the player's quest list
                    _player.Quests.Add( new PlayerQuest( newLocation.QuestAvailableHere ) );
                }
            }

            //Does the location have a monster?
            if ( newLocation.MonsterLivingHere != null )
            {
                rtbMessages.Text += "You see a " + newLocation.MonsterLivingHere.Name +
               Environment.NewLine;
                ScrollToBottomOfMessages();

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

            //Refresh player's stats
            UpdatePlayerStats();

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

            foreach ( SpellBook spellBook in _player.Spells )
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

        private void btnCastSpell_Click( object sender , EventArgs e )
        {
            //Get the currently selected spell from the cboSpells ComboBox
            Spell currentSpell = (Spell)cboSpells.SelectedItem;

            //Subtract mana cost of spell from player's current mana
            _player.CurrentMana -= currentSpell.ManaCost;

            //Determine the amount of damage to do to the monster
            int damageToMonster = RandomNumberGenerator.NumberBetween( currentSpell.MinimumDamage , currentSpell.MaximumDamage );

            //Apply the damage to the monster
            _currentMonster.CurrentHitPoints -= damageToMonster;

            //Display message
            rtbMessages.Text += "You cast " + currentSpell.Name + " for " + damageToMonster.ToString() + " points of damage to " + _currentMonster.Name + Environment.NewLine;

            //Reward the player casting experience
            currentSpell.CastingExperience += 5;

            MonsterAfterAttack();
        }

        private void btnUseWeapon_Click( object sender , EventArgs e )
        {
            //Get the currently selected weapon from the cboWeapons ComboBox
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;

            //Determine the amount of damage to do to the monster
            int damageToMonster = RandomNumberGenerator.NumberBetween( currentWeapon.MinimumDamage , currentWeapon.MaximumDamage );

            //Apply the damage to the monster's CurrentHitPoints
            _currentMonster.CurrentHitPoints -= damageToMonster;

            //Display message
            rtbMessages.Text += "You deal " + damageToMonster.ToString() + " points of damage to " + _currentMonster.Name + Environment.NewLine;
            ScrollToBottomOfMessages();

            MonsterAfterAttack();
        }

        private void btnUsePotion_Click( object sender , EventArgs e )
        {
            //Get the current selected potion from the combobox
            HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;

            //Add healing amount to the player's CurrentHitPoints
            _player.CurrentHitPoints = ( _player.CurrentHitPoints + potion.AmountToHeal );

            //CurrentHitPoints cannot exceed player's MaximumHitPoints
            if(_player.CurrentHitPoints > _player.MaximumHitPoints )
            {
                _player.CurrentHitPoints = _player.MaximumHitPoints;
            }

            //Remove the potion from the player's inventory
            foreach(InventoryItem ii in _player.Inventory )
            {
                if(ii.Details.ID == potion.ID )
                {
                    ii.Quantity--;
                    break;
                }
            }

            //Display message
            rtbMessages.Text += "You drink a " + potion.Name + " and heal yourself for " + potion.AmountToHeal.ToString() + " hit points." + Environment.NewLine;
            ScrollToBottomOfMessages();

            //Monster gets their turn to attack

            //Determine the amount of damage the monster does to the player
            int damageToPlayer = RandomNumberGenerator.NumberBetween( 0 , _currentMonster.MaximumDamage );

            //Display message
            rtbMessages.Text += "The " + _currentMonster.Name + " dealt " + damageToPlayer.ToString() + " points of damage." + Environment.NewLine;
            ScrollToBottomOfMessages();

            //Subtract damage from player
            _player.CurrentHitPoints -= damageToPlayer;

            if(_player.CurrentHitPoints <= 0 )
            {
                //We died
                rtbMessages.Text += "The " + _currentMonster.Name + "killed you." + Environment.NewLine;
                ScrollToBottomOfMessages();

                //Send home
                MoveTo( World.LocationByID( World.LOCATION_ID_HOME ) );
            }

            //Refresh player data in UI
            UpdatePlayerStats();
            UpdateInventoryListInUI();
            UpdatePotionListInUI();
        }

        private void ScrollToBottomOfMessages()
        {
            rtbMessages.SelectionStart = rtbMessages.Text.Length;
            rtbMessages.ScrollToCaret();
        }

        private void UpdatePlayerStats()
        {
            //Refresh player information and inventory controls
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();
            lblMana.Text = _player.CurrentMana.ToString();
        }

        private void MonsterAfterAttack()
        {
            //Check if the monster is dead
            if ( _currentMonster.CurrentHitPoints <= 0 )
            {
                //Monster is dead
                rtbMessages.Text += Environment.NewLine;
                rtbMessages.Text += "You have slain " + _currentMonster.Name + Environment.NewLine;
                ScrollToBottomOfMessages();

                //Give the player experience points for their victory
                _player.ExperiencePoints += _currentMonster.RewardExperiencePoints;
                rtbMessages.Text += "You earned " + _currentMonster.RewardExperiencePoints.ToString() + " experience points. Nice!" + Environment.NewLine;
                ScrollToBottomOfMessages();

                //Give the player gold for their victory
                _player.Gold += _currentMonster.RewardGold;
                rtbMessages.Text += "You looted the " + _currentMonster.Name + "'s corpse and found " + _currentMonster.RewardGold.ToString() + " gold. Who cares if you get your hands a little dirty? Gold is gold..." + Environment.NewLine;
                ScrollToBottomOfMessages();

                //Get random loot items from the monster
                List<InventoryItem> lootedItems = new List<InventoryItem>();

                //Add items to the lootedItms list, comparing a random number to the drop percentage
                foreach ( LootItem lootItem in _currentMonster.LootTable )
                {
                    if ( RandomNumberGenerator.NumberBetween( 1 , 100 ) <= lootItem.DropPercentage )
                    {
                        lootedItems.Add( new InventoryItem( lootItem.Details , 1 ) );
                    }
                }

                //If no items were randomly selected, then add the default loot item(s)
                if ( lootedItems.Count == 0 )
                {
                    foreach ( LootItem lootItem in _currentMonster.LootTable )
                    {
                        if ( lootItem.IsDefaultItem )
                        {
                            lootedItems.Add( new InventoryItem( lootItem.Details , 1 ) );
                        }
                    }
                }

                //Add the looted items to the player's inventory
                foreach ( InventoryItem inventoryItem in lootedItems )
                {
                    _player.AddItemToInventory( inventoryItem.Details );

                    if ( inventoryItem.Quantity == 1 )
                    {
                        rtbMessages.Text += "You also looted " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.Name + Environment.NewLine;
                        ScrollToBottomOfMessages();
                    }

                    else
                    {
                        rtbMessages.Text += "You also looted " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.NamePlural + Environment.NewLine;
                        ScrollToBottomOfMessages();
                    }
                }

                //Refresh player information and inventory controls
                UpdatePlayerStats();

                UpdateInventoryListInUI();
                UpdatePotionListInUI();
                UpdateWeaponListInUI();
                UpdateSpellsListInUI();

                //Add a blank line to the messages box, just for appearance
                rtbMessages.Text += Environment.NewLine;

                //Move the player to current location (to heal player and create a new monster to fight)
                MoveTo( _player.CurrentLocation );
            }

            else
            {
                //Monster is still alive

                //Determine the amount of damage the monster does to the player
                int damageToPlayer = RandomNumberGenerator.NumberBetween( 0 , _currentMonster.MaximumDamage );

                //Display message
                rtbMessages.Text += _currentMonster.Name + " dealt " + damageToPlayer.ToString() + " points of damage to you. " + Environment.NewLine;
                ScrollToBottomOfMessages();

                //Subtract damage from player
                _player.CurrentHitPoints -= damageToPlayer;

                //Refresh player data in UI
                UpdatePlayerStats();

                if ( _player.CurrentHitPoints <= 0 )
                {
                    //Display message
                    rtbMessages.Text += "The " + _currentMonster.Name + " has killed you..." + Environment.NewLine;
                    ScrollToBottomOfMessages();

                    //Move player to "Home"
                    MoveTo( World.LocationByID( World.LOCATION_ID_HOME ) );
                }
            }
        }

        private void EduRPG_Load( object sender , EventArgs e )
        {
        }

        private void EduRPG_FormClosing( object sender , FormClosingEventArgs e )
        {
            File.WriteAllText( PLAYER_DATA_FILE_NAME , _player.ToXMLString() );
        }
    }
}