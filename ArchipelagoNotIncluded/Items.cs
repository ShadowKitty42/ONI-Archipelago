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
        public string reseaarch_level { get; set; }
        public string reseaarch_level_base { get; set; }
        public string tech { get; set; }
        public string tech_base { get; set; }
        public string internal_tech { get; set; }
        public string internal_tech_base { get; set; }
        public string ap_classssification { get; set; }
        public string version { get; set; }
    }

    public class ModItem
    {
        public string name { get; set; }
        public string internal_name { get; set; }
        public string research_level { get; set; }
        public string tech { get; set; }
        public string internal_tech { get; set; }
        public string ap_classssification { get; set; }

        public ModItem()
        { }
        public ModItem(TechItem techitem)
        {
            name = ArchipelagoNotIncluded.CleanName(techitem.Name);
            internal_name = techitem.Id;
            tech = ArchipelagoNotIncluded.CleanName(techitem.ParentTech.Name);
            internal_tech = techitem.parentTechId;
            ap_classssification = "Useful";

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
