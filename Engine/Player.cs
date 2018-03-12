using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System;

namespace Engine
{
    public class Player : LivingCreature
    {
        public int Gold { get; set; }

        public int ExperiencePoints { get; set; }

        public int Level { get { return ( ( ExperiencePoints / 100 ) + 1 ); } }

        public List<InventoryItem> Inventory { get; set; }

        public List<SpellBook> Spells { get; set; }

        public List<PlayerQuest> Quests { get; set; }

        public Location CurrentLocation { get; set; }

        private Player(int currentHitPoints , int maximumHitPoints , int currentMana , int maximumMana , int gold , int experiencePoints , int statusEffect) : base(currentHitPoints , maximumHitPoints , currentMana , maximumMana , statusEffect)
        {
            Gold = gold;
            ExperiencePoints = experiencePoints;   

            Inventory = new List<InventoryItem>();
            Quests = new List<PlayerQuest>();
            Spells = new List<SpellBook>();
        }

        public static Player CreateDefaultPlayer()
        {
            Player player = new Player( 10 , 10 , 20 , 20 , 20 , 0 , 0 );
            player.Inventory.Add( new InventoryItem( World.ItemByID( World.ITEM_ID_RUSTY_SWORD ) , 1 ) );
            player.Spells.Add( new SpellBook( World.SpellByID( World.SPELL_ID_FIRE ) , 1 ) );
            player.CurrentLocation = World.LocationByID( World.LOCATION_ID_HOME );

            return player;
        }

        public static Player CreatePlayerFromXmlString (string xmlPlayerData )
        {
            try
            {
                XmlDocument playerData = new XmlDocument();

                playerData.LoadXml( xmlPlayerData );

                int currentHitPoints = Convert.ToInt32( playerData.SelectSingleNode( "/Player/Stats/CurrentHitPoints" ).InnerText );

                int maximumHitPoints = Convert.ToInt32( playerData.SelectSingleNode( "/Player/Stats/MaximumHitPoints" ).InnerText );

                int currentMana = Convert.ToInt32( playerData.SelectSingleNode( "/Player/Stats/CurrentMana" ).InnerText );

                int maximumMana = Convert.ToInt32( playerData.SelectSingleNode( "/Player/Stats/MaximumMana" ).InnerText );

                int gold = Convert.ToInt32( playerData.SelectSingleNode( "/Player/Stats/Gold" ).InnerText );

                int experiencePoints = Convert.ToInt32( playerData.SelectSingleNode( "/Player/Stats/ExperiencePoints" ).InnerText );

                int statusEffect = Convert.ToInt32( playerData.SelectSingleNode( "/Player/Stats/StatusEffect" ).InnerText );

                Player player = new Player( currentHitPoints , maximumHitPoints , currentMana , maximumMana , gold , experiencePoints , statusEffect );

                int currentLocationID = Convert.ToInt32( playerData.SelectSingleNode( "/Player/Stats/CurrentLocation" ).InnerText );

                player.CurrentLocation = World.LocationByID( currentLocationID );

                foreach ( XmlNode node in playerData.SelectNodes( "/Player/InventoryItems/InventoryItem" ) )
                {
                    int id = Convert.ToInt32( node.Attributes[ "ID" ].Value );

                    int quantity = Convert.ToInt32( node.Attributes[ "Quantity" ].Value );

                    for ( int i = 0; i < quantity; i++ )
                    {
                        player.AddItemToInventory( World.ItemByID( id ) );
                    }
                }

                foreach ( XmlNode node in playerData.SelectNodes( "/Player/PlayerQuests/PlayerQuest" ) )
                {
                    int id = Convert.ToInt32( node.Attributes[ "ID" ].Value );

                    bool isCompleted = Convert.ToBoolean( node.Attributes[ "IsCompleted" ].Value );
                    
                    PlayerQuest playerQuest = new PlayerQuest( World.QuestByID( id ) );

                    playerQuest.IsCompleted = isCompleted;
                    
                    player.Quests.Add( playerQuest );
                }
                
                 return player;
            }

            catch
            {
                //If there's an error with the XML data, return a default player
                return Player.CreateDefaultPlayer();
            }
        }

        public bool HasRequiredItemToEnterThisLocation( Location location )
        {
            if ( location.ItemRequiredToEnter == null )
            {
                //There is no item required to enter so return true.
                return true;
            }

            //See if the player has required item in their inventory
            return Inventory.Exists( ii => ii.Details.ID == location.ItemRequiredToEnter.ID );
        }

        public bool HasThisQuest( Quest quest )
        {
            //Just like with required item, check to see if player has this quest.
            return Quests.Exists( pq => pq.Details.ID == quest.ID );
        }

        public bool CompletedThisQuest( Quest quest )
        {
            foreach ( PlayerQuest playerQuest in Quests )
            {
                if ( playerQuest.Details.ID == quest.ID )
                {
                    return playerQuest.IsCompleted;
                }
            }
            return false;
        }

        public bool HasAllQuestCompletionItems( Quest quest )
        {
            // See if the player has all the items needed to complete the quest here
            foreach ( QuestCompletionItem qci in quest.QuestCompletionItems )
            {
                //Check each item in the player's inventory to see if they have it and if they have enough of it
                if ( !Inventory.Exists( ii => ii.Details.ID == qci.Details.ID && ii.Quantity >= qci.Quantity ) )
                {
                    return false;
                }
            }

            // If we got here, then the player must have all the required items, and enough of them, to complete the quest.
            return true;
        }

        public void RemoveQuestCompletionItems( Quest quest )
        {
            foreach ( QuestCompletionItem qci in quest.QuestCompletionItems )
            {
                InventoryItem item = Inventory.SingleOrDefault( ii => ii.Details.ID == qci.Details.ID );

                if(item != null )
                {
                    //Subtract the quantity from the player's inventory that was needed to complete the quest
                    item.Quantity -= qci.Quantity;
                }
            }
        }

        public void AddItemToInventory( Item itemToAdd )
        {
            InventoryItem item = Inventory.SingleOrDefault( ii => ii.Details.ID == itemToAdd.ID );

            if(item == null )
            {
                //They didn't have the item so add it to their inventory
                Inventory.Add( new InventoryItem( itemToAdd , 1 ) );
            }

            else
            {
                //They have the item in their inventory, so increase the quantity by 1
                item.Quantity++;
            }
        }

        public void MarkQuestCompleted( Quest quest )
        {
            // Find the quest in the player's quest list
            PlayerQuest playerQuest = Quests.SingleOrDefault( pq => pq.Details.ID == quest.ID );

            if(playerQuest != null )
            {
                playerQuest.IsCompleted = true;
            }
        }

        public string ToXMLString()
        {
            XmlDocument playerData = new XmlDocument();

            //Create the top-level XML node
            XmlNode player = playerData.CreateElement( "Player" );
            playerData.AppendChild( player );

            //Create the "Stats" child node to hold the other player statistics nodes
            XmlNode stats = playerData.CreateElement( "Stats" );
            player.AppendChild( stats );

            //Create the child nodes for the "Stats" node            
            //!CurrentHitPoints
            XmlNode currentHitPoints = playerData.CreateElement( "CurrentHitPoints" );

            currentHitPoints.AppendChild( playerData.CreateTextNode( this.CurrentHitPoints.ToString() ) );

            stats.AppendChild( currentHitPoints );

            //!MaximumHitPoints
            XmlNode maximumHitPoints = playerData.CreateElement( "MaximumHitPoints" );

            maximumHitPoints.AppendChild( playerData.CreateTextNode( this.MaximumHitPoints.ToString() ) );

            stats.AppendChild( maximumHitPoints );

            //!CurrentMana
            XmlNode currentMana = playerData.CreateElement( "CurrentMana" );

            currentMana.AppendChild( playerData.CreateTextNode( this.CurrentMana.ToString() ) );

            stats.AppendChild( currentMana );

            //!MaximumMana
            XmlNode maximumMana = playerData.CreateElement( "MaximumMana" );

            maximumMana.AppendChild( playerData.CreateTextNode( this.MaximumMana.ToString() ) );

            stats.AppendChild( maximumMana );

            //!Gold
            XmlNode gold = playerData.CreateElement( "Gold" );

            gold.AppendChild( playerData.CreateTextNode( this.Gold.ToString() ) );

            stats.AppendChild( gold );

            //!ExperiencePoints
            XmlNode experiencePoints = playerData.CreateElement( "ExperiencePoints" );

            experiencePoints.AppendChild( playerData.CreateTextNode( this.ExperiencePoints.ToString() ) );

            stats.AppendChild( experiencePoints );

            //!CurrentLocation
            XmlNode currentLocation = playerData.CreateElement( "CurrentLocation" );

            currentLocation.AppendChild( playerData.CreateTextNode( this.CurrentLocation.ToString() ) );

            stats.AppendChild( currentLocation );

            //!StatusEffect
            XmlNode statusEffect = playerData.CreateElement( "StatusEffect" );

            statusEffect.AppendChild( playerData.CreateTextNode( this.StatusEffect.ToString() ) );

            stats.AppendChild( statusEffect );

            //Create the "InventoryItems" child node to hold each InventoryItem node
            XmlNode inventoryItems = playerData.CreateElement( "InventoryItems" );
            player.AppendChild( inventoryItems );

            //Create an "InventoryItem" node for each item in the player's inventory
            foreach(InventoryItem item in this.Inventory )
            {
                XmlNode inventoryItem = playerData.CreateElement( "InventoryItem" );

                XmlAttribute idAttribute = playerData.CreateAttribute( "ID" );
                idAttribute.Value = item.Details.ID.ToString();
                inventoryItem.Attributes.Append( idAttribute );

                XmlAttribute quantityAttribute = playerData.CreateAttribute( "Quantity" );
                quantityAttribute.Value = item.Quantity.ToString();
                inventoryItem.Attributes.Append( quantityAttribute );

                inventoryItems.AppendChild( inventoryItem );
            }

            //Create the "SpellBook" child node to hold each Spell node
            XmlNode spells = playerData.CreateElement( "Spells" );
            player.AppendChild( spells );

            //Create a "Spell" node for each spell in the player's spellbook
            foreach(SpellBook spell in this.Spells )
            {
                XmlNode spellBook = playerData.CreateElement( "SpellBook" );

                XmlAttribute idAttribute = playerData.CreateAttribute( "ID" );
                idAttribute.Value = spell.Details.ID.ToString();
                spellBook.Attributes.Append( idAttribute );

                XmlAttribute castingExperienceAttribute = playerData.CreateAttribute( "CastingExperience" );
                castingExperienceAttribute.Value = spell.Details.CastingExperience.ToString();
                spellBook.Attributes.Append( castingExperienceAttribute );

                spells.AppendChild( spellBook );
            }

            //Create the "PlayerQuests" child node to hold each PlayerQuest node
            XmlNode playerQuests = playerData.CreateElement( "PlayerQuests" );
            player.AppendChild( playerQuests );

            //Create a "PlayerQuest" child node for each quest the player has acquired
            foreach(PlayerQuest quest in this.Quests )
            {
                XmlNode playerQuest = playerData.CreateElement( "PlayerQuest" );

                XmlAttribute idAttribute = playerData.CreateAttribute( "ID" );
                idAttribute.Value = quest.Details.ID.ToString();
                playerQuest.Attributes.Append( idAttribute );

                XmlAttribute isCompletedAttribute = playerData.CreateAttribute( "IsCompleted" );
                isCompletedAttribute.Value = quest.IsCompleted.ToString();
                playerQuest.Attributes.Append( isCompletedAttribute );

                playerQuests.AppendChild( playerQuest );
            }

            return playerData.InnerXml; //The XML document, as a string, so we can save the data to the disk
        }
    }
}