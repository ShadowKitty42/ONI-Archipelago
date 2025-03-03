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

        public APNetworkMonitor(string URL, int port, string name, string password = "")
        {
            this.URL = URL;
            this.Port = port;
            this.SlotName = name;
            this.Password = password;
        }

        public LoginResult TryConnectArchipelago(ItemsHandlingFlags flags = ItemsHandlingFlags.AllItems)
        {
            session = ArchipelagoSessionFactory.CreateSession(URL, Port);
            LoginResult result;
            //if (this.Password == "")
            //    result = session.TryConnectAndLogin("Oxygen Not Included", SlotName, flags);
            //else
                result = session.TryConnectAndLogin("Oxygen Not Included", SlotName, flags, password: this.Password);
            if (result.Successful)
            {
                Debug.Log("Connection successful");
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
            if (!initialSyncComplete)
            {
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

        private static void AddItem(ItemInfo item)
        {
            //string name = item.LocationName.Split('-')[0].Trim();
            //Debug.Log(item.ItemName);
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

        private static void SendCarePackage()
        {

        }
    }
}
