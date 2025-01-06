using Archipelago.MultiClient.Net;
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

namespace ArchipelagoNotIncluded
{
    public class APNetworkMonitor : KMonoBehaviour
    {
        public ArchipelagoSession session = null;
        private string URL = "localhost";
        private int Port = 38281;
        private string SlotName = "Shadow";

        public APNetworkMonitor(string URL, int port, string name)
        {
            this.URL = URL;
            this.Port = port;
            this.SlotName = name;
        }

        public LoginResult TryConnectArchipelago()
        {
            session = ArchipelagoSessionFactory.CreateSession(URL, Port);
            LoginResult result = session.TryConnectAndLogin("Oxygen Not Included", SlotName, ItemsHandlingFlags.AllItems);
            if (result.Successful)
            {
                Debug.Log("Connection successful");
                session.Items.ItemReceived += OnItemReceived;
                session.Socket.SocketClosed += OnSocketClosed;
                session.Socket.ErrorReceived += OnErrorReceived;
                //UpdateAllItems();
            }
            else
                Debug.Log("Connection failed");
            return result;
        }

        private void OnErrorReceived(Exception e, string reason)
        {
            Debug.Log($"OnErrorReceived: {reason}");
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
            Debug.Log("OnItemReceived Triggered");
            //UpdateAllItems();
            Debug.Log(session.Items.AllItemsReceived.Count);
            //if (session.Items.AllItemsReceived.Count == 4)
            //{
            //ItemInfo item = session.Items.PeekItem();
            AddItem(session.Items.PeekItem());
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

        public void UpdateAllItems()
        {
            Debug.Log("UpdateAllItems Triggered");
            //List<string> techList = new List<string>();
            Debug.Log(this.session.Items.AllItemsReceived.Count);
            for (int i = ArchipelagoNotIncluded.lastItem; i < session.Items.AllItemsReceived.Count - 1; i++)
            {
                AddItem(session.Items.AllItemsReceived[i]);
                /*DefaultItem defItem = AllDefaultItems.SingleOrDefault(i => i.internal_name == item.ItemName);
                string InternalTech = info.spaced_out ? defItem.internal_tech : defItem.internal_tech_base;
                Debug.Log(InternalTech);
                if (!techList.Contains(InternalTech))
                    techList.Add(InternalTech);*/
                //item.LocationName.Substring(0, item.LocationName.IndexOf('-') - 1);
            }
            /*foreach (string tech in techList)
            {
                Game.Instance.Trigger(-107300940, Db.Get().Techs.TryGet(tech));
            }*/
        }

        private static void AddItem(ItemInfo item)
        {
            string name = item.LocationName.Split('-')[0].Trim();
            Debug.Log(name);
            DefaultItem defItem = ArchipelagoNotIncluded.info.spaced_out ? ArchipelagoNotIncluded.AllDefaultItems.Find(i => i.tech == name) : ArchipelagoNotIncluded.AllDefaultItems.Find(i => i.tech_base == name);
            Tech itemTech = Db.Get().Techs.TryGetTechForTechItem(defItem.internal_name);
            if (itemTech != null)
                Game.Instance.Trigger(11390976, (object)itemTech);
        }
    }
}
