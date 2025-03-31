﻿using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using Microsoft.Build.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Archipelago.MultiClient.Net.Packets;
using static STRINGS.ITEMS.BIONIC_BOOSTERS;
using System.Collections.Concurrent;

namespace ArchipelagoNotIncluded
{
    public class APNetworkMonitor : KMonoBehaviour
    {
        public ArchipelagoSession session = null;
        private string URL = "localhost";
        private int Port = 38281;
        public string SlotName = "Shadow";
        private string Password = "";
        private bool initialSyncComplete = false;
        public Dictionary<string, CarePackageInfo> CarePackages = new Dictionary<string, CarePackageInfo>();
        public static ConcurrentQueue<string> packageQueue = new ConcurrentQueue<string>();
        private int MostRecentCount = 0;
        public static int HighestCount = 0;

        public APNetworkMonitor(string URL, int port, string name, string password = "")
        {
            this.URL = URL;
            this.Port = port;
            this.SlotName = name;
            this.Password = password;
            //this.PopulateCarePackages();
        }

        public LoginResult TryConnectArchipelago(ItemsHandlingFlags flags = ItemsHandlingFlags.AllItems)
        {
            MostRecentCount = 0;
            session = ArchipelagoSessionFactory.CreateSession(URL, Port);
            LoginResult result;
            //if (this.Password == "")
            //    result = session.TryConnectAndLogin("Oxygen Not Included", SlotName, flags);
            //else
                result = session.TryConnectAndLogin("Oxygen Not Included", SlotName, flags, password: this.Password);
            if (result.Successful)
            {
                Debug.Log("Connection successful");
                if (flags != ItemsHandlingFlags.AllItems)
                    initialSyncComplete = false;
                session.Items.ItemReceived += OnItemReceived;
                session.Socket.PacketReceived += OnPacketReceived;
                session.Socket.SocketClosed += OnSocketClosed;
                session.Socket.ErrorReceived += OnErrorReceived;
                //UpdateAllItems();
            }
            else
                Debug.Log("Connection failed");
            return result;
        }
        
        private void OnPacketReceived(ArchipelagoPacketBase packet)
        {
            Debug.Log($"Received packet of type: {packet.PacketType.ToString()}");
            if (packet.PacketType.ToString() == "ReceivedItems")
            {
                ReceivedItemsPacket packetReceived = (ReceivedItemsPacket)packet;
                Debug.Log($"The packet has {packetReceived.Items.Length} Items Received");
            }
        }

        private void OnErrorReceived(Exception e, string reason)
        {
            Debug.Log($"OnErrorReceived: {e.Message} {reason}");
            //catch(e Exception).Message = reason;
            if (reason.Contains("closed the WebSocket connection"))
            {
                //session.Socket.DisconnectAsync();
                session = null;
                //StartCoroutine(AttemptToReconnect());
            }
        }

        private void OnSocketClosed(string reason)
        {
            Debug.Log($"OnSocketClosed: {reason}");
            session = null;
            TryConnectArchipelago();
            //StartCoroutine(AttemptToReconnect());
        }

        private IEnumerator AttemptToReconnect()
        {
            yield return new WaitForSeconds(20);
            Debug.Log("Attempting to reconnect...");
            LoginResult result = TryConnectArchipelago();
            //if (!result.Successful)
            //    StartCoroutine(AttemptToReconnect());
        }

        private void OnItemReceived(ReceivedItemsHelper helper)
        {
            //Debug.Log("OnItemReceived Triggered");
            //UpdateAllItems();
            //Debug.Log(session.Items.AllItemsReceived.Count);
            //if (session.Items.AllItemsReceived.Count == 4)
            //{
            //ItemInfo item = session.Items.PeekItem();

            ItemInfo item = session.Items.PeekItem();
            //if (item.Player.Name == SlotName)
            AddItem(item);
            //Debug.Log($"Current Player: {SlotName} - Found {item.ItemName} for {item.Player.Name}");

            /*Debug.Log(item.ItemName);

            //char[] delimiters = { '<', '>' };
            string name = item.LocationName.Split('-')[0].Trim();
            Debug.Log(name);
            DefaultItem defItem = info.spaced_out ? AllDefaultItems.Find(i => i.tech == name) : AllDefaultItems.Find(i => i.tech_base == name);
            string InternalTech = info.spaced_out ? defItem.internal_tech : defItem.internal_tech_base;
            Debug.Log(InternalTech);
            Tech itemTech = Db.Get().Techs.TryGetTechForTechItem(defItem.internal_name);
            //Tech itemTech = Db.Get().Techs.TryGet("Jobs");
            //Tech techToAdd = new Tech(defItem.internal_name, new List<string> { defItem.internal_name }, Db.Get().Techs);
            //Db.Get().Techs.resources.Add(techToAdd);

            //OnResearchComplete_Patch.AddTechItem(PlanScreen.Instance, techToAdd);
            //Tech itemTech = Db.Get().Techs.TryGet(InternalTech);
            //Debug.Log(itemTech.Name);
            //TechInstance techInstance = Research.Instance.Get(itemTech);
            //if (techInstance != null && !techInstance.IsComplete())
            //Debug.Log("Item exists and isn't complete");
            /*if (itemTech == null)
            {
                Debug.Log("Tech was null");
                return;
            }
            if (!itemTech.IsComplete())
            {
                Debug.Log("Tech is not complete");
                Game.Instance.Trigger(-107300940, itemTech);
            }
            BuildingDef buildingDef = Assets.GetBuildingDef(defItem.internal_name);
            if ((UnityEngine.Object)buildingDef == (UnityEngine.Object)null)
            {
                DebugUtil.LogWarningArgs((object)string.Format("Tech '{0}' unlocked building '{1}' but no such building exists", (object)tech.Name, (object)current.Id));
            }
            else
            {
                HashedString tagCategory = tagCategoryMap[buildingDef.Tag];BuildMenu.Instance
                hashedStringSet.Add(tagCategory);
                this.AddParentCategories(tagCategory, (ICollection<HashedString>)hashedStringSet);
            }
            //}
            Debug.Log($"ItemName: {item.ItemName}, ItemId: {item.ItemId}, ItemDisplayName: {item.ItemDisplayName}");
            //PlanScreen.Instance.ScreenUpdate(true);
            Game.Instance.Trigger(11390976, (object)itemTech);
            //Game.Instance.Trigger(-107300940, (object)itemTech);*/

            session.Items.DequeueItem();
        }

        public void UpdateAllItems(bool force = false)
        {
            /*if (!force && initialSyncComplete)
                return;*/
            Debug.Log("UpdateAllItems Triggered");
            Debug.Log(this.session.Items.AllItemsReceived.Count);
            MostRecentCount = 0;
            if (!initialSyncComplete)
            {
                PopulateCarePackages();
                //session.ConnectionInfo.UpdateConnectionOptions(ItemsHandlingFlags.AllItems);
                ConnectUpdatePacket packet = new ConnectUpdatePacket
                {
                    ItemsHandling = ItemsHandlingFlags.AllItems
                };
                session.Socket.SendPacket(packet);
                initialSyncComplete = true;
            }
            else
                session.Socket.SendPacketAsync(new SyncPacket());
            //List<string> techList = new List<string>();
            //for (int i = ArchipelagoNotIncluded.lastItem; i < session.Items.AllItemsReceived.Count - 1; i++)
            /*foreach(ItemInfo item in session.Items.AllItemsReceived)
            {
                //AddItem(session.Items.AllItemsReceived[i]);
                AddItem(item);
                DefaultItem defItem = AllDefaultItems.SingleOrDefault(i => i.internal_name == item.ItemName);
                string InternalTech = info.spaced_out ? defItem.internal_tech : defItem.internal_tech_base;
                Debug.Log(InternalTech);
                if (!techList.Contains(InternalTech))
                    techList.Add(InternalTech);
                //item.LocationName.Substring(0, item.LocationName.IndexOf('-') - 1);
            }*/
            /*foreach (string tech in techList)
            {
                Game.Instance.Trigger(-107300940, Db.Get().Techs.TryGet(tech));
            }*/
        }

        private void AddItem(ItemInfo item)
        {
            //string name = item.LocationName.Split('-')[0].Trim();
            Debug.Log($"AddItem: {item.ItemName} MostRecentCount: {MostRecentCount} lastItem: {ArchipelagoNotIncluded.lastItem}");
            if (MostRecentCount == ArchipelagoNotIncluded.lastItem || ArchipelagoNotIncluded.lastItem == 0)
            {
                ArchipelagoNotIncluded.lastItem++;
                if (item.ItemName.StartsWith("Care Package"))
                {
                    Debug.Log($"Sending Care Package: {item.ItemName}");
                    SendCarePackage(item);
                }
            }
            MostRecentCount++;
            //DefaultItem defItem = ArchipelagoNotIncluded.info.spaced_out ? ArchipelagoNotIncluded.AllDefaultItems.Find(i => i.tech == name) : ArchipelagoNotIncluded.AllDefaultItems.Find(i => i.tech_base == name);
            

            DefaultItem defItem = ArchipelagoNotIncluded.AllDefaultItems.Find(i => i.name == item.ItemName);
            ModItem modItem = ArchipelagoNotIncluded.AllModItems.Find(i => i.name == item.ItemName);
            Tech itemTech = null;
            if (defItem != null)
                itemTech = Db.Get().Techs.TryGetTechForTechItem(defItem.internal_name);
            if (modItem != null)
                itemTech = Db.Get().Techs.TryGetTechForTechItem(modItem.internal_name);
            if (itemTech != null)
            {
                Game.Instance.Trigger(11390976, (object)itemTech);
                //PlanScreen.Instance.RefreshBuildableStates(true);
                //PlanScreen.Instance.ForceRefreshAllBuildingToggles();
            }
        }

        public void SendResourceCheck(string ResourceName)
        {
            long id = ArchipelagoNotIncluded.netmon.session.Locations.GetLocationIdFromName("Oxygen Not Included", ResourceName);
            ArchipelagoNotIncluded.netmon.session.Locations.CompleteLocationChecks([id]);
        }

        public void PopulateCarePackages()
        {
            CarePackages.Add("Sandstone", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SandStone).tag.ToString(), 2000f, null));
            CarePackages.Add("Dirt", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Dirt).tag.ToString(), 1000f, null));
            CarePackages.Add("Algae", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Algae).tag.ToString(), 1000f, null));
            CarePackages.Add("Oxylite", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag.ToString(), 200f, null));
            CarePackages.Add("Water", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Water).tag.ToString(), 4000f, null));
            CarePackages.Add("Sand", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Sand).tag.ToString(), 6000f, null));
            CarePackages.Add("Coal", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Carbon).tag.ToString(), 6000f, null));
            CarePackages.Add("Fertilizer", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Fertilizer).tag.ToString(), 6000f, null));
            CarePackages.Add("Ice", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ice).tag.ToString(), 8000f, null));
            CarePackages.Add("Brine", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Brine).tag.ToString(), 4000f, null));
            CarePackages.Add("Salt Water", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SaltWater).tag.ToString(), 4000f, null));
            CarePackages.Add("Rust", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Rust).tag.ToString(), 2000f, null));
            CarePackages.Add("Copper Ore", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag.ToString(), 4000f, null));
            CarePackages.Add("Gold Amalgam", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag.ToString(), 4000f, null));
            CarePackages.Add("Refined Copper", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Copper).tag.ToString(), 800f, null));
            CarePackages.Add("Refined Iron", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Iron).tag.ToString(), 800f, null));
            CarePackages.Add("Lime", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Lime).tag.ToString(), 800f, null));
            CarePackages.Add("Plastic", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag.ToString(), 1000f, null));
            CarePackages.Add("Glass", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Glass).tag.ToString(), 400f, null));
            CarePackages.Add("Steel", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Steel).tag.ToString(), 200f, null));
            CarePackages.Add("Ethanol", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag.ToString(), 200f, null));
            CarePackages.Add("Aluminum Ore", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag.ToString(), 200f, null));
            CarePackages.Add("Oxyfern Seed", new CarePackageInfo("OxyfernSeed", 2f, null));
            CarePackages.Add("Arbor Tree Seed", new CarePackageInfo("ForestTreeSeed", 2f, null));
            CarePackages.Add("Thimble Reed Seed", new CarePackageInfo(BasicFabricMaterialPlantConfig.SEED_ID, 6f, null));
            CarePackages.Add("Wort Seed", new CarePackageInfo("ColdBreatherSeed", 2f, null));
            CarePackages.Add("Nutrient Bar", new CarePackageInfo("FieldRation", 10f, null));
            CarePackages.Add("Omelettes", new CarePackageInfo("CookedEgg", 6f, null));
            CarePackages.Add("Barbecue", new CarePackageInfo("CookedMeat", 6f, null));
            CarePackages.Add("Spicy Tofu", new CarePackageInfo("SpicyTofu", 6f, null));
            CarePackages.Add("Fried Mushroom", new CarePackageInfo("FriedMushroom", 3f, null));
            CarePackages.Add("Hatch", new CarePackageInfo("HatchBaby", 2f, null));
            CarePackages.Add("Hatch Egg", new CarePackageInfo("HatchEgg", 6f, null));
            CarePackages.Add("Pip", new CarePackageInfo("SquirrelBaby", 2f, null));
            CarePackages.Add("Pip Egg", new CarePackageInfo("SquirrelEgg", 4f, null));
            CarePackages.Add("Drecko", new CarePackageInfo("DreckoBaby", 2f, null));
            CarePackages.Add("Drecko Egg", new CarePackageInfo("DreckoEgg", 6f, null));
            CarePackages.Add("Pacu", new CarePackageInfo("Pacu", 8f, null));

            if (DlcManager.IsContentSettingEnabled("DLC2_ID"))
            {
                CarePackages.Add("Cinnabar", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Cinnabar).tag.ToString(), 4000f, null));
                CarePackages.Add("Wood", new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.WoodLog).tag.ToString(), 400f, null));
                CarePackages.Add("Flox", new CarePackageInfo("WoodDeerBaby", 2f, null));
                CarePackages.Add("Spigot Seal", new CarePackageInfo("SealBaby", 2f, null));
                CarePackages.Add("Bammoth", new CarePackageInfo("IceBellyEgg", 2f, null));
                CarePackages.Add("Squash Fries", new CarePackageInfo("FriesCarrot", 6f, null));
                CarePackages.Add("Idylla Seed", new CarePackageInfo("IceFlowerSeed", 6f, null));
                CarePackages.Add("Alveo Vera Seed", new CarePackageInfo("BlueGrassSeed", 6f, null));
                CarePackages.Add("Plume Squash Seed", new CarePackageInfo("CarrotPlantSeed", 2f, null));
                CarePackages.Add("Bonbon Tree Seed", new CarePackageInfo("SpaceTreeSeed", 2f, null));
                CarePackages.Add("Pikeapple Bush Seed", new CarePackageInfo("HardSkinBerryPlantSeed", 6f, null));
            }
            if (DlcManager.IsContentSettingEnabled("DLC3_ID"))
            {
                CarePackages.Add("Metal Power Bank", new CarePackageInfo("DisposableElectrobank_RawMetal", 6f, null));
                CarePackages.Add("Construction Booster", new CarePackageInfo("Booster_Construct1", 2f, null));
                CarePackages.Add("Digging Booster", new CarePackageInfo("Booster_Dig1", 2f, null));
                CarePackages.Add("Electrical Engineering Booster", new CarePackageInfo("Booster_Op1", 2f, null));
                CarePackages.Add("Suit Training Booster", new CarePackageInfo("Booster_Suits1", 2f, null));
                CarePackages.Add("Grilling Booster", new CarePackageInfo("Booster_Cook1", 2f, null));
                CarePackages.Add("Advanced Medical Booster", new CarePackageInfo("Booster_Medicine1", 2f, null));
                CarePackages.Add("Strength Booster", new CarePackageInfo("Booster_Carry1", 2f, null));
                CarePackages.Add("Masterworks Art Booster", new CarePackageInfo("Booster_Art1", 2f, null));
                CarePackages.Add("Crop Tending Booster", new CarePackageInfo("Booster_Farm1", 2f, null));
                CarePackages.Add("Ranching Booster", new CarePackageInfo("Booster_Ranch1", 2f, null));
                CarePackages.Add("Researching Booster", new CarePackageInfo("Booster_Research1", 2f, null));
                if (DlcManager.IsExpansion1Active())
                    CarePackages.Add("Piloting Booster", new CarePackageInfo("Booster_Pilot1", 2f, null));
                if (DlcManager.IsPureVanilla())
                    CarePackages.Add("Piloting Booster", new CarePackageInfo("Booster_PilotVanilla1", 2f, null));
            }
        }

        public void SendCarePackage(ItemInfo item)
        {
            string packageName = item.ItemName.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries)[1];
            if (!CarePackages.ContainsKey(packageName))
            {
                Debug.Log($"Unknown Care Package: {packageName}");
                return;
            }
            
            packageQueue.Enqueue(packageName);
        }
    }
}
