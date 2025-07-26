using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchipelagoNotIncluded
{
    public class DefaultItem
    {
        public string name { get; set; }
        public string internal_name { get; set; }
        public string research_level { get; set; }
        public string research_level_base { get; set; }
        public string tech { get; set; }
        public string tech_base { get; set; }
        public string internal_tech { get; set; }
        public string internal_tech_base { get; set; }
        public string ap_classification { get; set; }
        public string version { get; set; }

        public DefaultItem()
        { }

        public DefaultItem(TechItem techitem)
        {
            name = ArchipelagoNotIncluded.CleanName(techitem.Name);
            internal_name = techitem.Id;
            tech = ArchipelagoNotIncluded.CleanName(techitem.ParentTech.Name);
            tech_base = tech;
            internal_tech = techitem.parentTechId;
            internal_tech_base = internal_tech;
            ap_classification = "Useful";

            version = "Base";
            techitem.GetRequiredDlcIds()?.ToList().ForEach(id =>
            {
                if (id == DlcManager.DLC4_ID)
                    version = "Dino";
                else if (id == DlcManager.DLC3_ID)
                    version = "Bionic";
                else if (id == DlcManager.DLC2_ID)
                    version = "Frosty";
                else if (id == DlcManager.EXPANSION1_ID)
                    version = "SpacedOut";
                else
                    version = "Base";
            });


            if (techitem.ParentTech.RequiresResearchType("orbital"))
                research_level = "orbital";
            else if (techitem.ParentTech.RequiresResearchType("nuclear"))
                research_level = "radbolt";
            else if (techitem.ParentTech.RequiresResearchType("advanced"))
                research_level = "advanced";
            else
                research_level = "basic";
            research_level_base = research_level;
        }
    }

    public class ModItem
    {
        public string name { get; set; }
        public string internal_name { get; set; }
        public string research_level { get; set; }
        public string tech { get; set; }
        public string internal_tech { get; set; }
        public string ap_classification { get; set; }
        public bool randomized { get; set; }

        public ModItem()
        { }
        public ModItem(TechItem techitem)
        {
            name = ArchipelagoNotIncluded.CleanName(techitem.Name);
            internal_name = techitem.Id;
            tech = ArchipelagoNotIncluded.CleanName(techitem.ParentTech.Name);
            internal_tech = techitem.parentTechId;
            ap_classification = "Useful";
            randomized = false;

            if (techitem.ParentTech.RequiresResearchType("orbital"))
                research_level = "orbital";
            else if (techitem.ParentTech.RequiresResearchType("nuclear"))
                research_level = "radbolt";
            else if (techitem.ParentTech.RequiresResearchType("advanced"))
                research_level = "advanced";
            else
                research_level = "basic";
        }
    }
}
