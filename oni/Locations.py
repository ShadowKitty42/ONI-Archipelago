import typing

from BaseClasses import Location, Region
from .Names import LocationNames, TechNames


class ONILocation(Location):
    game = "Oxygen Not Included"


resource_locations: typing.Dict[str, typing.Dict[str, typing.List[str]]] = {
    "terra_base": {
		"basic": [
		    "Muckroot", "Mealwood Seed", "Dirt", "Algae", "Sandstone", "Sand", "Copper Ore", "Coal", "Fertilizer", "Briar Seed", "Hatch", "Blossom Seed", "Igneous Rock", "Water", "Polluted Water"
		],
		"advanced": [
		    "Joya Seed", "Sporechid Seed", "Pincha Pepper Seed", "Sleet Wheat Grain", "Wort Seed", "Waterweed Seed", "Balm Lily Seed", "Mirth Leaf Seed", "Fungal Spore", "Buddy Bud Seed", "Thimble Reed Seed",
            "Obsidian", "Abyssalite", "Fossil", "Granite", "Lead", "Iron Ore", "Diamond",  "Ice", "Polluted Ice", "Sedimentary Rock", "Salt", "Bleach Stone", "Slime", "Gold Amalgam", "Wolframite", "Clay",
            "Phosphorite", "Regolith", "Mafic Rock", 
		]
	},
    "ceres_base": {
		"basic": [
			"Water", "Polluted Water", "Wood", "Oxylite", "Ice", "Snow", "Phosphorite", "Dirt", "Sherberry", "Bammoth", "Warm Coat", "Spigot Seal", "Polluted Ice", "Pikeapple Bush Seed", "Alveo Vera Seed",
            "Idylla Seed", "Cinnabar Ore", "Igneous Rock", 
		],
		"advanced": [
			 "Granite", "Sporechid Seed", "Joya Seed", "Arbor Acorn", "Mirth Leaf Seed", "Hexalent Fruit", "Mealwood Seed", "Oxyfern Seed", "Pincha Pepper Seed", "Nosh Bean", "Dasha Saltvine Seed",
             "Plume Squash Seed", "Carved Lumen Quartz", "Waterweed Seed", "Sleet Wheat Grain", "Wort Seed", "Bonbon Tree Seed", "Obsidian", "Abyssalite", "Sand", "Fossil", "Lead", "Iron Ore", "Diamond",
             "Coal", "Aluminum Ore", "Wolframite", "Salt", "Mafic Rock", "Rust", "Bleach Stone", "Sedimentary Rock", "Sucrose", "Fertilizer", "Sandstone", "Algae", "Slime", "Mercury", 
		]
	},
    "oceania_base": {
		"basic": [
			"Water", "Polluted Water", "Muckroot", "Briar Seed", "Blossom Seed", "Mealwood Seed", "Copper Ore", "Coal", "Dirt", "Sand", "Sandstone", "Fertilizer", "Algae", "Hatch", "Igneous Rock", 
		],
		"advanced": [
			"Joya Seed", "Sporechid Seed", "Balm Lily Seed", "Mirth Leaf Seed", "Pincha Pepper Seed", "Waterweed Seed", "Wort Seed", "Sleet Wheat Grain", "Fungal Spore", "Buddy Bud Seed", "Thimble Reed Seed",
            "Abyssalite", "Granite", "Lead", "Fossil", "Obsidian", "Iron Ore", "Diamond", "Sedimentary Rock", "Phosphorite", "Salt", "Bleach Stone", "Ice", "Wolframite", "Polluted Ice", "Slime", "Clay",
            "Gold Amalgam", "Regolith", "Mafic Rock", 
		]
	},
    "rime_base": {
		"basic": [
			"Water", "Polluted Water", "Sandstone", "Fertilizer", "Ice", "Algae", "Sand", "Dirt", "Copper Ore", "Coal", "Phosphorite", "Muckroot", "Mealwood Seed", "Blossom Seed", "Briar Seed", "Hatch", "Igneous Rock", 
		],
		"advanced": [
			"Iron Ore", "Slime", "Sporechid Seed", "Pincha Pepper Seed", "Waterweed Seed", "Hexalent Fruit", "Arbor Acorn", "Fungal Spore", "Thimble Reed Seed", "Wort Seed", "Obsidian", "Abyssalite", "Fossil",
            "Granite", "Lead", "Diamond", "Iron", "Sedimentary Rock", "Polluted Ice", "Bleach Stone", "Clay", "Gold Amalgam", "Salt", "Mafic Rock", "Rust", "Wolframite", "Aluminum Ore", "Regolith", "Joya Seed",
            "Mirth Leaf Seed", "Buddy Bud Seed", "Nosh Bean", "Oxyfern Seed", "Sleet Wheat Grain", 
		]
	},
    "verdante_base": {
		"basic": [
			"Water", "Polluted Water", "Hexalent Fruit", "Oxyfern Seed", "Mirth Leaf Seed", "Arbor Acorn", "Igneous Rock", "Dirt", "Aluminum Ore", "Mealwood Seed", "Hatch", "Phosphorite", "Sedimentary Rock", 
		],
		"advanced": [
			"Granite", "Balm Lily Seed", "Pincha Pepper Seed", "Waterweed Seed", "Joya Seed", "Sporechid Seed", "Nosh Bean", "Dasha Saltvine Seed", "Fungal Spore", "Buddy Bud Seed", "Thimble Reed Seed", "Obsidian",
            "Abyssalite", "Iron Ore", "Sandstone", "Sand", "Bleach Stone", "Fossil", "Lead", "Coal", "Algae", "Slime", "Clay", "Gold Amalgam", "Diamond", "Salt", "Rust", "Mafic Rock", "Ice", "Wolframite", "Fertilizer", "Regolith", 
		]
	},
    "arboria_base": {
		"basic": [
			"Water", "Polluted Water", "Mealwood Seed", "Oxyfern Seed", "Mirth Leaf Seed", "Arbor Acorn", "Hexalent Fruit", "Dirt", "Igneous Rock", "Aluminum Ore", "Phosphorite", "Hatch", 
		],
		"advanced": [
			"Granite", "Sporechid Seed", "Waterweed Seed", "Joya Seed", "Balm Lily Seed", "Pincha Pepper Seed", "Dasha Saltvine Seed", "Nosh Bean", "Sleet Wheat Grain", "Wort Seed", "Obsidian", "Abyssalite", "Fossil", "Lead", "Coal",
            "Sand", "Bleach Stone", "Iron Ore", "Rust", "Sedimentary Rock", "Mafic Rock", "Iron", "Salt", "Sandstone", "Polluted Ice", "Ice", "Wolframite", "Algae", "Diamond", "Slime", "Fertilizer", "Regolith", "Copper Ore", 
		]
	},
    "volcanea_base": {
		"basic": [
			"Water", "Polluted Water", "Muckroot", "Sandstone", "Sand", "Dirt", "Copper Ore", "Coal", "Hatch", "Blossom Seed", "Mealwood Seed", "Algae", "Fertilizer", "Briar Seed", "Igneous Rock", 
		],
		"advanced": [
			"Joya Seed", "Sporechid Seed", "Pincha Pepper Seed", "Sleet Wheat Grain", "Wort Seed", "Waterweed Seed", "Thimble Reed Seed", "Fungal Spore", "Buddy Bud Seed", "Balm Lily Seed", "Mirth Leaf Seed", "Obsidian", "Abyssalite",
            "Lead", "Fossil", "Granite", "Iron Ore", "Diamond", "Polluted Ice", "Sedimentary Rock", "Wolframite", "Bleach Stone", "Slime", "Ice", "Gold Amalgam", "Salt", "Clay", "Phosphorite", "Regolith", "Mafic Rock", 
		]
	},
    "badlands_base": {
		"basic": [
			"Water", "Polluted Water", "Blossom Seed", "Muckroot", "Mealwood Seed", "Briar Seed", "Sandstone", "Sand", "Dirt", "Algae", "Copper Ore", "Hatch", "Igneous Rock", "Obsidian", "Fertilizer", "Coal", 
		],
		"advanced": [
			"Granite", "Iron Ore", "Mafic Rock", "Rust", "Dasha Saltvine Seed", "Nosh Bean", "Joya Seed", "Pincha Pepper Seed", "Sporechid Seed", "Balm Lily Seed", "Mirth Leaf Seed", "Sleet Wheat Grain", "Wort Seed", "Waterweed Seed",
            "Abyssalite", "Fossil", "Lead", "Iron", "Diamond", "Salt", "Bleach Stone", "Phosphorite", "Ice", "Polluted Ice", "Wolframite", "Slime", "Sedimentary Rock", "Regolith", 
		]
	},
    "aridio_base": {
		"basic": [
			"Water", "Polluted Water", "Hexalent Fruit", "Arbor Acorn", "Mealwood Seed", "Mirth Leaf Seed", "Oxyfern Seed", "Dirt", "Igneous Rock", "Phosphorite", "Aluminum Ore", "Hatch", 
		],
		"advanced": [
			"Joya Seed", "Sporechid Seed", "Pincha Pepper Seed", "Balm Lily Seed", "Dasha Saltvine Seed", "Waterweed Seed", "Fungal Spore", "Buddy Bud Seed", "Thimble Reed Seed", "Obsidian", "Abyssalite", "Fossil", "Lead", "Granite",
            "Diamond", "Ice", "Iron Ore", "Polluted Ice", "Wolframite", "Coal", "Sand", "Iron", "Salt", "Sedimentary Rock", "Bleach Stone", "Sandstone", "Sulfur", "Rust", "Mafic Rock", "Algae", "Slime", "Clay", "Gold Amalgam",
            "Fertilizer", "Regolith", "Wort Seed", "Nosh Bean", 
		]
	},
    "oasisse_base": {
		"basic": [
			"Water", "Polluted Water", "Arbor Acorn", "Mirth Leaf Seed", "Hexalent Fruit", "Mealwood Seed", "Dirt", "Igneous Rock", "Aluminum Ore", "Oxyfern Seed", "Phosphorite", "Hatch", "Sand", 
		],
		"advanced": [
			"Joya Seed", "Sporechid Seed", "Waterweed Seed", "Pincha Pepper Seed", "Fungal Spore", "Buddy Bud Seed", "Thimble Reed Seed", "Muckroot", "Blossom Seed", "Balm Lily Seed", "Briar Seed", "Obsidian", "Abyssalite", "Fossil",
            "Lead", "Granite", "Sedimentary Rock", "Bleach Stone", "Diamond", "Salt", "Iron Ore", "Algae", "Slime", "Gold Amalgam", "Clay", "Coal", "Sandstone", "Copper Ore", "Fertilizer", "Ice", "Wolframite", "Iron", "Mafic Rock", "Regolith", 
		]
	},
    "skewed_base": {
		"basic": [
			"Water", "Polluted Water", "Blossom Seed", "Muckroot", "Mealwood Seed", "Dirt", "Copper Ore", "Sandstone", "Algae", "Sand", "Coal", "Fertilizer", "Hatch", "Briar Seed", "Igneous Rock", 
		],
		"advanced": [
			"Sleet Wheat Grain", "Waterweed Seed", "Wort Seed", "Pincha Pepper Seed", "Balm Lily Seed", "Mirth Leaf Seed", "Sporechid Seed", "Fungal Spore", "Thimble Reed Seed", "Buddy Bud Seed", "Joya Seed", "Obsidian", "Abyssalite",
            "Fossil", "Lead", "Ice", "Polluted Ice", "Granite", "Salt", "Sedimentary Rock", "Bleach Stone", "Slime", "Gold Amalgam", "Wolframite", "Phosphorite", "Iron Ore", "Mafic Rock", "Regolith", "Clay", "Diamond", "Iron", 
		]
	},
    "blasted_base": {
		"basic": [
			"Water", "Polluted Water", "Pikeapple Bush Seed", "Alveo Vera Seed", "Sherberry", "Idylla Seed", "Snow", "Ice", "Crushed Ice", "Solid Carbon Dioxide", "Phosphorite", "Dirt", "Cinnabar Ore", "Oxylite", 
		],
		"advanced": [
			"Joya Seed", "Sporechid Seed", "Mealwood Seed", "Oxyfern Seed", "Arbor Acorn", "Mirth Leaf Seed", "Hexalent Fruit", "Pincha Pepper Seed", "Carved Lumen Quartz", "Plume Squash Seed", "Sleet Wheat Grain", "Wort Seed",
            "Waterweed Seed", "Bonbon Tree Seed", "Obsidian", "Granite", "Igneous Rock", "Coal", "Abyssalite", "Lead", "Iron Ore", "Fossil", "Diamond", "Aluminum Ore", "Iron", "Sand", "Sedimentary Rock", "Rust", "Mafic Rock",
            "Bleach Stone", "Polluted Ice", "Wolframite", "Sucrose", "Salt", "Sandstone", "Slime", "Algae", "Fertilizer", "Mercury", "Copper Ore", "Nosh Bean", 
		]
	},
    "terra": {
        "basic": [
            "Sandstone", "Dirt", "Copper Ore", "Coal", "Algae", "Muckroot", "Mealwood Seed", "Blossom Seed", "Briar Seed", "Sand", "Water", "Fertilizer", "Igneous Rock", "Polluted Water"
        ],
        "advanced": [
            "Granite", "Phosphorite", "Iron Ore", "Gold Amalgam", "Wolframite", "Lead", "Uranium Ore", "Mafic Rock", "Sedimentary Rock", "Fossil", "Obsidian", "Regolith", "Bleach Stone",
            "Salt", "Slime", "Clay", "Ice", "Abyssalite", "Sulfur"
        ],
        "advanced2": [
            "Cobalt Ore", "Mud", "Polluted Dirt", "Polluted Mud", "Aluminum Ore", "Rust", "Bog Bucket Seed", "Swamp Chard Heart", "Oxyfern Seed", "Hexalent Fruit", 
        ],
        "radbolt": [
            "Nosh Bean", "Dasha Saltvine Seed", "Arbor Acorn", "Tranquil Toe Seed", "Saturn Critter Trap Seed", "Mallow Seed", 
        ]
    },
	"ceres": {
		"basic": [
			"Pikeapple Bush Seed", "Sherberry", "Snow", "Ice", "Cinnabar Ore", "Idylla Seed", "Alveo Vera Seed", "Phosphorite", "Oxylite", "Dirt", "Igneous Rock", "Wolframite", "Water", "Polluted Water", "Warm Coat"
		],
		"advanced": [
			"Joya Seed", "Sporechid Seed", "Mealwood Seed", "Hexalent Fruit", "Arbor Acorn", "Mirth Leaf Seed", "Oxyfern Seed", "Pincha Pepper Seed", "Nosh Bean", "Dasha Saltvine Seed",
            "Waterweed Seed", "Plume Squash Seed", "Carved Lumen Quartz", "Sleet Wheat Grain", "Wort Seed", "Bonbon Tree Seed", "Obsidian", "Abyssalite", "Sand", "Fossil", "Lead", "Granite",
            "Diamond", "Coal", "Iron Ore", "Aluminum Ore", "Mafic Rock", "Rust", "Bleach Stone", "Salt", "Sedimentary Rock", "Algae", "Slime", "Sandstone", "Sucrose", "Uranium Ore", "Sulfur",
            "Mercury", "Fertilizer", "Regolith", 
		],
		"advanced2": [
			"Bog Bucket Seed", "Mallow Seed", "Polluted Mud", "Mud", "Polluted Dirt", "Cobalt Ore", "Swamp Chard Heart", 
		],
		"radbolt": [
			"Bliss Burst Seed", "Grubfruit Seed", "Balm Lily Seed", "Fungal Spore", "Buddy Bud Seed", "Thimble Reed Seed", "Copper Ore", "Gold Amalgam", "Clay", 
		]
	},
    "oceania": {
		"basic": [
			"Water", "Polluted Water", "Blossom Seed", "Mealwood Seed", "Muckroot", "Briar Seed", "Sandstone", "Sand", "Dirt", "Algae", "Coal", "Copper Ore", "Hatch", "Fertilizer", 
		],
		"advanced": [
			"Joya Seed", "Sporechid Seed", "Pincha Pepper Seed", "Balm Lily Seed", "Mirth Leaf Seed", "Fungal Spore", "Sleet Wheat Grain", "Wort Seed", "Mallow Seed", "Bog Bucket Seed", "Waterweed Seed", "Swamp Chard Heart",
            "Thimble Reed Seed", "Obsidian", "Abyssalite", "Igneous Rock", "Fossil", "Lead", "Granite", "Diamond", "Iron Ore", "Polluted Ice", "Ice", "Wolframite", "Cobalt Ore", "Slime", "Sedimentary Rock", "Clay",
            "Gold Amalgam", "Polluted Dirt", "Uranium Ore", "Mud", "Polluted Mud", "Phosphorite", "Bleach Stone", "Salt", "Regolith", "Mafic Rock", "Buddy Bud Seed", 
		],
		"advanced2": [
			"Oxyfern Seed", "Hexalent Fruit", "Arbor Acorn", "Aluminum Ore", "Dasha Saltvine Seed", "Rust", 
		],
		"radbolt": [
			"Saturn Critter Trap Seed", "Tranquil Toe Seed", "Nosh Bean", "Grubfruit Seed", "Bliss Burst Seed", "Brine Ice", "Solid Chlorine", "Sulfur", "Sucrose", 
		]
	},
    "squelchy": {
		"basic": [
			"Water", "Polluted Water", "Swamp Chard Heart", "Bog Bucket Seed", "Mallow Seed", "Dirt", "Cobalt Ore", "Polluted Dirt", "Sedimentary Rock", "Mud", "Fertilizer", "Polluted Mud", "Phosphorite", "Fossil", "Sandstone", "Sand", 
		],
		"advanced": [
			"Wort Seed", "Pincha Pepper Seed", "Joya Seed", "Sporechid Seed", "Balm Lily Seed", "Mirth Leaf Seed", "Fungal Spore", "Sleet Wheat Grain", "Thimble Reed Seed", "Grubfruit Seed", "Bliss Burst Seed", "Muckroot",
            "Mealwood Seed", "Briar Seed", "Blossom Seed", "Ice", "Polluted Ice", "Obsidian", "Granite", "Abyssalite", "Wolframite", "Lead", "Igneous Rock", "Iron Ore", "Diamond", "Coal", "Slime", "Uranium Ore", "Clay", "Algae",
            "Gold Amalgam", "Bleach Stone", "Rust", "Salt", "Sucrose", "Sulfur", "Mafic Rock", "Copper Ore", "Regolith", "Buddy Bud Seed", "Hatch", 
		],
		"advanced2": [
			"Oxyfern Seed", "Hexalent Fruit", "Aluminum Ore", "Arbor Acorn", 
		],
		"radbolt": [
			"Waterweed Seed", "Nosh Bean", "Dasha Saltvine Seed", "Solid Chlorine", "Tranquil Toe Seed", "Saturn Critter Trap Seed", 
		]
	},
}

# Basic Research
basic_locations = [
    LocationNames.BasicFarming1,
    LocationNames.BasicFarming2,
    LocationNames.BasicFarming3,
    LocationNames.BasicFarming4,
    LocationNames.MealPrep1,
    LocationNames.MealPrep2,
    LocationNames.MealPrep3,
    LocationNames.MealPrep4,
    LocationNames.PowerRegulation1,
    LocationNames.PowerRegulation2,
    LocationNames.PowerRegulation3,
    LocationNames.Combustion1,
    LocationNames.Combustion2,
    LocationNames.BasicRefinement1,
    LocationNames.BasicRefinement2,
    LocationNames.Jobs1,
    LocationNames.Jobs2,
    LocationNames.Jobs3,                        # Frosty DLC
    LocationNames.AdvancedResearch1,
    LocationNames.AdvancedResearch2,
    LocationNames.AdvancedResearch3,            # Spaced Out DLC
    LocationNames.AdvancedResearch4,            # Spaced Out DLC
    # LocationNames.AdvancedResearch5,
    LocationNames.MedicineI1,
    LocationNames.MedicineII1,
    LocationNames.MedicineII2,
    LocationNames.LiquidPiping1,
    LocationNames.LiquidPiping2,
    LocationNames.LiquidPiping3,
    LocationNames.LiquidPiping4,
    LocationNames.ImprovedOxygen1,
    LocationNames.ImprovedOxygen2,
    LocationNames.SanitationSciences1,
    LocationNames.SanitationSciences2,
    LocationNames.SanitationSciences3,
    LocationNames.SanitationSciences4,
    LocationNames.AdvancedFiltration1,
    LocationNames.AdvancedFiltration2,
    LocationNames.AdvancedFiltration3,          # Spaced Out DLC
    LocationNames.GasPiping1,
    LocationNames.GasPiping2,
    LocationNames.GasPiping3,
    LocationNames.GasPiping4,
    LocationNames.PressureManagement1,
    LocationNames.PressureManagement2,
    LocationNames.PressureManagement3,
    LocationNames.PressureManagement4,
    LocationNames.PortableGasses1,
    LocationNames.PortableGasses2,
    LocationNames.PortableGasses3,
    LocationNames.PortableGasses4,
    LocationNames.PortableGasses5,
    LocationNames.PortableGasses6,              # Frosty DLC
    LocationNames.InteriorDecor1,
    LocationNames.InteriorDecor2,
    LocationNames.InteriorDecor3,
    LocationNames.Artistry1,
    LocationNames.Artistry2,
    LocationNames.Artistry3,
    LocationNames.Artistry4,
    LocationNames.Artistry5,
    LocationNames.Artistry6,
    LocationNames.Artistry7,
    LocationNames.SmartHome1,
    LocationNames.SmartHome2,
    LocationNames.SmartHome3,
    LocationNames.SmartHome4,
    # LocationNames.SmartHome5,
    ]

# Advanced Research
advanced_locations = [
    LocationNames.Agriculture1,
    LocationNames.Agriculture2,
    LocationNames.Agriculture3,
    LocationNames.Agriculture4,
    LocationNames.Agriculture5,
    LocationNames.Agriculture6,                 # Spaced Out DLC
    LocationNames.FoodRepurposing1,
    LocationNames.FoodRepurposing2,
    LocationNames.FoodRepurposing3,
    LocationNames.Ranching1,
    LocationNames.Ranching2,
    LocationNames.Ranching3,
    LocationNames.Ranching4,
    LocationNames.Ranching5,
    LocationNames.Ranching6,
    LocationNames.Ranching7,
    LocationNames.AnimalControl1,
    LocationNames.AnimalControl2,
    LocationNames.AnimalControl3,
    LocationNames.AnimalControl4,
    LocationNames.AnimalControl5,
    LocationNames.SpaceCombustion1,             # Spaced Out DLC
    LocationNames.SpaceCombustion2,             # Spaced Out DLC
    LocationNames.FossilFuels1,
    LocationNames.FossilFuels2,
    LocationNames.FossilFuels3,
    LocationNames.Plastics1,
    LocationNames.Plastics2,
    LocationNames.Acoustics1,
    LocationNames.Acoustics2,
    LocationNames.Acoustics3,
    LocationNames.AdvancedPowerRegulation1,
    LocationNames.AdvancedPowerRegulation2,
    LocationNames.AdvancedPowerRegulation3,
    LocationNames.AdvancedPowerRegulation4,
    LocationNames.AdvancedPowerRegulation5,
    LocationNames.AdvancedPowerRegulation6,
    LocationNames.PrettyGoodConductors1,
    LocationNames.PrettyGoodConductors2,
    LocationNames.PrettyGoodConductors3,
    LocationNames.PrettyGoodConductors4,
    LocationNames.PrettyGoodConductors5,
    LocationNames.SmartStorage1,
    LocationNames.SmartStorage2,
    LocationNames.SmartStorage3,
    # LocationNames.SmartStorage4,
    LocationNames.SolidTransport1,
    LocationNames.SolidTransport2,
    LocationNames.SolidTransport3,
    LocationNames.SolidTransport4,
    LocationNames.RefinedObjects1,
    LocationNames.RefinedObjects2,
    LocationNames.RefinedObjects3,              # Spaced Out DLC
    LocationNames.RefinedObjects4,              # Spaced Out DLC
    LocationNames.Smelting1,
    LocationNames.Smelting2,
    LocationNames.ArtificialFriends1,
    LocationNames.ArtificialFriends2,           # Spaced Out DLC
    LocationNames.RoboticTools1,
    LocationNames.RoboticTools2,                # Spaced Out DLC
    LocationNames.SpaceProgram1,                # Spaced Out DLC
    LocationNames.SpaceProgram2,                # Spaced Out DLC
    LocationNames.SpaceProgram3,                # Spaced Out DLC
    LocationNames.SpaceProgram4,                # Spaced Out DLC
    LocationNames.CrashPlan1,                   # Spaced Out DLC
    LocationNames.CrashPlan2,                   # Spaced Out DLC
    LocationNames.CrashPlan3,                   # Spaced Out DLC
    # LocationNames.CrashPlan4,
    LocationNames.NotificationSystems1,
    LocationNames.NotificationSystems2,
    LocationNames.NotificationSystems3,         # Spaced Out DLC
    LocationNames.SkyDetectors1,
    LocationNames.SkyDetectors2,
    LocationNames.NuclearResearch1,             # Spaced Out DLC
    LocationNames.NuclearResearch2,             # Spaced Out DLC
    # LocationNames.NuclearResearch3,
    LocationNames.AdvancedNuclearResearch1,     # Spaced Out DLC
    LocationNames.AdvancedNuclearResearch2,     # Spaced Out DLC
    LocationNames.MedicineIII1,
    LocationNames.MedicineIII2,
    LocationNames.MedicineIII3,
    LocationNames.MedicineIV1,
    LocationNames.MedicineIV2,
    LocationNames.MedicineIV3,                  # Spaced Out DLC
    # LocationNames.MedicineIV4,
    LocationNames.LiquidFiltering1,
    LocationNames.LiquidFiltering2,
    LocationNames.FlowRedirection1,
    LocationNames.FlowRedirection2,
    LocationNames.FlowRedirection3,             # Spaced Out DLC
    LocationNames.FlowRedirection4,             # Spaced Out DLC
    LocationNames.FlowRedirection5,             # Spaced Out DLC
    LocationNames.LiquidDistribution1,
    LocationNames.LiquidDistribution2,          # Spaced Out DLC
    LocationNames.LiquidDistribution3,          # Spaced Out DLC
    LocationNames.LiquidDistribution4,          # Spaced Out DLC
    LocationNames.Distillation1,
    LocationNames.Distillation2,
    LocationNames.Distillation3,
    LocationNames.ImprovedLiquidPiping1,
    LocationNames.ImprovedLiquidPiping2,
    LocationNames.ImprovedLiquidPiping3,
    LocationNames.ImprovedLiquidPiping4,
    # LocationNames.ImprovedLiquidPiping5,
    # LocationNames.ImprovedLiquidPiping6,
    LocationNames.LiquidTemperature1,
    LocationNames.LiquidTemperature2,
    LocationNames.LiquidTemperature3,
    LocationNames.LiquidTemperature4,
    LocationNames.LiquidTemperature5,
    LocationNames.LiquidTemperature6,
    LocationNames.LiquidTemperature7,
    LocationNames.TemperatureModulation1,
    LocationNames.TemperatureModulation2,
    LocationNames.TemperatureModulation3,       # Frosty DLC
    LocationNames.TemperatureModulation4,
    LocationNames.TemperatureModulation5,
    LocationNames.HVAC1,
    LocationNames.HVAC2,
    LocationNames.HVAC3,
    LocationNames.HVAC4,
    LocationNames.HVAC5,
    LocationNames.HVAC6,
    LocationNames.HVAC7,
    LocationNames.Decontamination1,
    LocationNames.Decontamination2,
    LocationNames.Decontamination3,
    LocationNames.SpaceGas1,                    # Spaced Out DLC
    LocationNames.SpaceGas2,                    # Spaced Out DLC
    LocationNames.SpaceGas3,                    # Spaced Out DLC
    LocationNames.SpaceGas4,                    # Spaced Out DLC
    LocationNames.ImprovedGasPiping1,
    LocationNames.ImprovedGasPiping2,
    LocationNames.ImprovedGasPiping3,
    LocationNames.ImprovedGasPiping4,
    LocationNames.GasDistribution1,
    LocationNames.GasDistribution2,             # Spaced Out DLC
    LocationNames.GasDistribution3,             # Spaced Out DLC
    LocationNames.GasDistribution4,             # Spaced Out DLC
    LocationNames.Suits1,
    LocationNames.Suits2,
    LocationNames.Suits3,
    LocationNames.Suits4,
    # LocationNames.Suits5,
    LocationNames.Clothing1,
    LocationNames.Clothing2,
    LocationNames.Clothing3,
    LocationNames.Luxury1,
    LocationNames.Luxury2,
    LocationNames.Luxury3,
    LocationNames.Luxury4,
    LocationNames.Luxury5,                      # Frosty DLC
    LocationNames.FineArt1,
    LocationNames.FineArt2,
    LocationNames.RefractiveDecor1,
    LocationNames.RefractiveDecor2,
    LocationNames.RefractiveDecor3,             # Frosty DLC
    LocationNames.GenericSensors1,
    LocationNames.GenericSensors2,
    LocationNames.GenericSensors3,
    LocationNames.GenericSensors4,
    LocationNames.GenericSensors5,
    LocationNames.GenericSensors6,
    LocationNames.GenericSensors7,
    LocationNames.GenericSensors8,              # Spaced Out DLC
    LocationNames.LogicCircuits1,
    LocationNames.LogicCircuits2,
    LocationNames.LogicCircuits3,
    LocationNames.LogicCircuits4,
    LocationNames.ParallelAutomation1,
    LocationNames.ParallelAutomation2,
    LocationNames.ParallelAutomation3,
    LocationNames.ParallelAutomation4,
    ]

# Radioactive Research
radbolt_locations = [
    LocationNames.Bioengineering1,              # Spaced Out DLC
    LocationNames.AnimalComfort1,
    LocationNames.AnimalComfort2,
    LocationNames.AnimalComfort3,
    LocationNames.FinerDining1,
    LocationNames.FinerDining2,
    LocationNames.FinerDining3,
    LocationNames.FinerDining4,                 # Frosty DLC
    LocationNames.ValveMiniaturization1,
    LocationNames.ValveMiniaturization2,
    LocationNames.SpacePower1,                  # Spaced Out DLC
    LocationNames.SpacePower2,                  # Spaced Out DLC
    LocationNames.SpacePower3,                  # Spaced Out DLC
    LocationNames.RenewableEnergy1,
    LocationNames.RenewableEnergy2,
    LocationNames.RenewableEnergy3,
    LocationNames.RenewableEnergy4,             # Spaced Out DLC
    LocationNames.SolidSpace1,
    LocationNames.SolidSpace2,
    LocationNames.SolidSpace3,
    LocationNames.SolidSpace4,                  # Spaced Out DLC
    LocationNames.SolidSpace5,                  # Spaced Out DLC
    LocationNames.SolidSpace6,                  # Spaced Out DLC
    LocationNames.SolidSpace7,                  # Spaced Out DLC
    LocationNames.SolidSpace8,                  # Spaced Out DLC
    LocationNames.HighTempForging1,
    LocationNames.HighTempForging2,
    LocationNames.HighTempForging3,
    LocationNames.HighTempForging4,
    LocationNames.HighTempForging5,
    LocationNames.DurableLifeSupport1,          # Spaced Out DLC
    LocationNames.DurableLifeSupport2,          # Spaced Out DLC
    LocationNames.DurableLifeSupport3,          # Spaced Out DLC
    LocationNames.DurableLifeSupport4,          # Spaced Out DLC
    LocationNames.DurableLifeSupport5,          # Spaced Out DLC
    LocationNames.NuclearStorage1,              # Spaced Out DLC
    LocationNames.RadiationProtection1,         # Spaced Out DLC
    LocationNames.RadiationProtection2,         # Spaced Out DLC
    LocationNames.RadiationProtection3,         # Spaced Out DLC
    LocationNames.RadiationProtection4,         # Spaced Out DLC
    LocationNames.AdvancedSanitation1,          # Spaced Out DLC
    LocationNames.PrecisionPlumbing1,
    LocationNames.PrecisionPlumbing2,        # Spaced Out DLC
    LocationNames.PrecisionPlumbing3,        # Frosty DLC
    LocationNames.Catalytics1,
    LocationNames.Catalytics2,
    LocationNames.Catalytics3,
    LocationNames.Catalytics4,
    LocationNames.Catalytics5,                  # Spaced Out DLC
    LocationNames.TravelTubes1,
    LocationNames.TravelTubes2,
    LocationNames.TravelTubes3,
    LocationNames.TravelTubes4,
    LocationNames.GlassFurnishings1,
    LocationNames.GlassFurnishings2,
    LocationNames.GlassFurnishings3,
    LocationNames.RenaissanceArt1,
    LocationNames.RenaissanceArt2,
    LocationNames.Computing1,
    LocationNames.Computing2,
    LocationNames.Computing3,
    LocationNames.Computing4,
    LocationNames.Computing5,
    # LocationNames.Computing6,
    ]

# Space Research
orbital_locations = [
    LocationNames.DairyOperation1,
    LocationNames.DairyOperation2,
    LocationNames.DairyOperation3,
    LocationNames.HydrocarbonPropulsion1,           # Spaced Out DLC
    LocationNames.HydrocarbonPropulsion2,           # Spaced Out DLC
    LocationNames.BetterHydroCarbonPropulsion1,     # Spaced Out DLC
    LocationNames.SolidManagement1,
    LocationNames.SolidManagement2,
    LocationNames.SolidManagement3,
    LocationNames.SolidManagement4,
    LocationNames.SolidManagement5,
    LocationNames.SolidManagement6,                 # Spaced Out DLC
    LocationNames.HighVelocityTransport1,           # Spaced Out DLC
    LocationNames.HighVelocityTransport2,           # Spaced Out DLC
    LocationNames.HighVelocityDestruction1,         # Spaced Out DLC
    LocationNames.HighPressureForging1,             # Spaced Out DLC
    LocationNames.CryoFuelPropulsion1,              # Spaced Out DLC
    LocationNames.CryoFuelPropulsion2,              # Spaced Out DLC
    LocationNames.NuclearRefinement1,               # Spaced Out DLC
    LocationNames.NuclearRefinement2,               # Spaced Out DLC
    LocationNames.NuclearRefinement3,               # Spaced Out DLC
    LocationNames.NuclearPropulsion1,               # Spaced Out DLC
    LocationNames.Jetpacks1,                        # Spaced Out DLC
    LocationNames.Jetpacks2,                        # Spaced Out DLC
    LocationNames.Jetpacks3,                        # Spaced Out DLC
    LocationNames.Jetpacks4,                        # Spaced Out DLC
    LocationNames.Jetpacks5,                        # Spaced Out DLC
    LocationNames.Jetpacks6,                        # Spaced Out DLC
    LocationNames.EnvironmentalAppreciation1,
    LocationNames.Screens1,
    LocationNames.Monuments1,
    LocationNames.Monuments2,
    LocationNames.Monuments3,
    LocationNames.AdvancedScanners1,                # Spaced Out DLC
    LocationNames.AdvancedScanners2,                # Spaced Out DLC
    LocationNames.AdvancedScanners3,                # Spaced Out DLC
    LocationNames.Multiplexing1,
    LocationNames.Multiplexing2,
]

# All Research Combined
all_locations = basic_locations + advanced_locations + radbolt_locations + orbital_locations

locations_by_tech_category: typing.Dict[str, typing.List[str]] = {
    TechNames.BasicFarming: [
        LocationNames.BasicFarming1,
        LocationNames.BasicFarming2,
        LocationNames.BasicFarming3,
        LocationNames.BasicFarming4,
    ],
    TechNames.MealPrep: [
        LocationNames.MealPrep1,
        LocationNames.MealPrep2,
        LocationNames.MealPrep3,
        LocationNames.MealPrep4,

    ],
    TechNames.FoodRepurposing: [
        LocationNames.FoodRepurposing1,
        LocationNames.FoodRepurposing2,
        LocationNames.FoodRepurposing3,
    ],
    TechNames.FinerDining: [
        LocationNames.FinerDining1,
        LocationNames.FinerDining2,
        LocationNames.FinerDining3,
        LocationNames.FinerDining4,
    ],
    TechNames.Agriculture: [
        LocationNames.Agriculture1,
        LocationNames.Agriculture2,
        LocationNames.Agriculture3,
        LocationNames.Agriculture4,
        LocationNames.Agriculture5,
        LocationNames.Agriculture6,
    ],
    TechNames.Ranching: [
        LocationNames.Ranching1,
        LocationNames.Ranching2,
        LocationNames.Ranching3,
        LocationNames.Ranching4,
        LocationNames.Ranching5,
        LocationNames.Ranching6,
        LocationNames.Ranching7,
    ],
    TechNames.AnimalControl: [
        LocationNames.AnimalControl1,
        LocationNames.AnimalControl2,
        LocationNames.AnimalControl3,
        LocationNames.AnimalControl4,
        LocationNames.AnimalControl5,
    ],
    TechNames.AnimalComfort: [
        LocationNames.AnimalComfort1,
        LocationNames.AnimalComfort2,
        LocationNames.AnimalComfort3,
    ],
    TechNames.DairyOperation: [
        LocationNames.DairyOperation1,
        LocationNames.DairyOperation2,
        LocationNames.DairyOperation3,
    ],
    TechNames.ImprovedOxygen: [
        LocationNames.ImprovedOxygen1,
        LocationNames.ImprovedOxygen2,
    ],
    TechNames.GasPiping: [
        LocationNames.GasPiping1,
        LocationNames.GasPiping2,
        LocationNames.GasPiping3,
        LocationNames.GasPiping4,
    ],
    TechNames.ImprovedGasPiping: [
        LocationNames.ImprovedGasPiping1,
        LocationNames.ImprovedGasPiping2,
        LocationNames.ImprovedGasPiping3,
        LocationNames.ImprovedGasPiping4,
    ],
    TechNames.SpaceGas: [
        LocationNames.SpaceGas1,
        LocationNames.SpaceGas2,
        LocationNames.SpaceGas3,
        LocationNames.SpaceGas4,
    ],
    TechNames.PressureManagement: [
        LocationNames.PressureManagement1,
        LocationNames.PressureManagement2,
        LocationNames.PressureManagement3,
        LocationNames.PressureManagement4,
    ],
    TechNames.Decontamination: [
        LocationNames.Decontamination1,
        LocationNames.Decontamination2,
        LocationNames.Decontamination3,
    ],
    TechNames.LiquidFiltering: [
        LocationNames.LiquidFiltering1,
        LocationNames.LiquidFiltering2,
    ],
    TechNames.MedicineI: [
        LocationNames.MedicineI1,
    ],
    TechNames.MedicineII: [
        LocationNames.MedicineII1,
        LocationNames.MedicineII2,
    ],
    TechNames.MedicineIII: [
        LocationNames.MedicineIII1,
        LocationNames.MedicineIII2,
        LocationNames.MedicineIII3,
    ],
    TechNames.MedicineIV: [
        LocationNames.MedicineIV1,
        LocationNames.MedicineIV2,
        LocationNames.MedicineIV3,
    ],
    TechNames.LiquidPiping: [
        LocationNames.LiquidPiping1,
        LocationNames.LiquidPiping2,
        LocationNames.LiquidPiping3,
        LocationNames.LiquidPiping4,
    ],
    TechNames.ImprovedLiquidPiping: [
        LocationNames.ImprovedLiquidPiping1,
        LocationNames.ImprovedLiquidPiping2,
        LocationNames.ImprovedLiquidPiping3,
        LocationNames.ImprovedLiquidPiping4,
    ],
    TechNames.PrecisionPlumbing: [
        LocationNames.PrecisionPlumbing1,
        LocationNames.PrecisionPlumbing2,
        LocationNames.PrecisionPlumbing3,
    ],
    TechNames.SanitationSciences: [
        LocationNames.SanitationSciences1,
        LocationNames.SanitationSciences2,
        LocationNames.SanitationSciences3,
        LocationNames.SanitationSciences4,
    ],
    TechNames.FlowRedirection: [
        LocationNames.FlowRedirection1,
        LocationNames.FlowRedirection2,
        LocationNames.FlowRedirection3,
        LocationNames.FlowRedirection4,
        LocationNames.FlowRedirection5,
    ],
    TechNames.LiquidDistribution: [
        LocationNames.LiquidDistribution1,
        LocationNames.LiquidDistribution2,
        LocationNames.LiquidDistribution3,
        LocationNames.LiquidDistribution4,
    ],
    TechNames.AdvancedSanitation: [
        LocationNames.AdvancedSanitation1,
    ],
    TechNames.AdvancedFiltration: [
        LocationNames.AdvancedFiltration1,
        LocationNames.AdvancedFiltration2,
        LocationNames.AdvancedFiltration3,
    ],
    TechNames.Distillation: [
        LocationNames.Distillation1,
        LocationNames.Distillation2,
        LocationNames.Distillation3,
    ],
    TechNames.Catalytics: [
        LocationNames.Catalytics1,
        LocationNames.Catalytics2,
        LocationNames.Catalytics3,
        LocationNames.Catalytics4,
        LocationNames.Catalytics5,
    ],
    TechNames.PowerRegulation: [
        LocationNames.PowerRegulation1,
        LocationNames.PowerRegulation2,
        LocationNames.PowerRegulation3,
    ],
    TechNames.AdvancedPowerRegulation: [
        LocationNames.AdvancedPowerRegulation1,
        LocationNames.AdvancedPowerRegulation2,
        LocationNames.AdvancedPowerRegulation3,
        LocationNames.AdvancedPowerRegulation4,
        LocationNames.AdvancedPowerRegulation5,
        LocationNames.AdvancedPowerRegulation6,
    ],
    TechNames.PrettyGoodConductors: [
        LocationNames.PrettyGoodConductors1,
        LocationNames.PrettyGoodConductors2,
        LocationNames.PrettyGoodConductors3,
        LocationNames.PrettyGoodConductors4,
        LocationNames.PrettyGoodConductors5,
    ],
    TechNames.RenewableEnergy: [
        LocationNames.RenewableEnergy1,
        LocationNames.RenewableEnergy2,
        LocationNames.RenewableEnergy3,
        LocationNames.RenewableEnergy4,
    ],
    TechNames.Combustion: [
        LocationNames.Combustion1,
        LocationNames.Combustion2,
    ],
    TechNames.FossilFuels: [
        LocationNames.FossilFuels1,
        LocationNames.FossilFuels2,
        LocationNames.FossilFuels3,
    ],
    TechNames.InteriorDecor: [
        LocationNames.InteriorDecor1,
        LocationNames.InteriorDecor2,
        LocationNames.InteriorDecor3,
    ],
    TechNames.Artistry: [
        LocationNames.Artistry1,
        LocationNames.Artistry2,
        LocationNames.Artistry3,
        LocationNames.Artistry4,
        LocationNames.Artistry5,
        LocationNames.Artistry6,
        LocationNames.Artistry7,
    ],
    TechNames.Clothing: [
        LocationNames.Clothing1,
        LocationNames.Clothing2,
        LocationNames.Clothing3,
    ],
    TechNames.Acoustics: [
        LocationNames.Acoustics1,
        LocationNames.Acoustics2,
        LocationNames.Acoustics3,
    ],
    TechNames.SpacePower: [
        LocationNames.SpacePower1,
        LocationNames.SpacePower2,
        LocationNames.SpacePower3,
    ],
    TechNames.NuclearRefinement: [
        LocationNames.NuclearRefinement1,
        LocationNames.NuclearRefinement2,
        LocationNames.NuclearRefinement3,
    ],
    TechNames.FineArt: [
        LocationNames.FineArt1,
        LocationNames.FineArt2,
    ],
    TechNames.EnvironmentalAppreciation: [
        LocationNames.EnvironmentalAppreciation1,
    ],
    TechNames.Luxury: [
        LocationNames.Luxury1,
        LocationNames.Luxury2,
        LocationNames.Luxury3,
        LocationNames.Luxury4,
        LocationNames.Luxury5,
    ],
    TechNames.RefractiveDecor: [
        LocationNames.RefractiveDecor1,
        LocationNames.RefractiveDecor2,
        LocationNames.RefractiveDecor3,
    ],
    TechNames.GlassFurnishings: [
        LocationNames.GlassFurnishings1,
        LocationNames.GlassFurnishings2,
        LocationNames.GlassFurnishings3,
    ],
    TechNames.Screens: [
        LocationNames.Screens1,
    ],
    TechNames.RenaissanceArt: [
        LocationNames.RenaissanceArt1,
        LocationNames.RenaissanceArt2,
    ],
    TechNames.Plastics: [
        LocationNames.Plastics1,
        LocationNames.Plastics2,
    ],
    TechNames.ValveMiniaturization: [
        LocationNames.ValveMiniaturization1,
        LocationNames.ValveMiniaturization2,
    ],
    TechNames.HydrocarbonPropulsion: [
        LocationNames.HydrocarbonPropulsion1,
        LocationNames.HydrocarbonPropulsion2,
    ],
    TechNames.BetterHydroCarbonPropulsion: [
        LocationNames.BetterHydroCarbonPropulsion1,
    ],
    TechNames.CryoFuelPropulsion: [
        LocationNames.CryoFuelPropulsion1,
        LocationNames.CryoFuelPropulsion2,
    ],
    TechNames.Suits: [
        LocationNames.Suits1,
        LocationNames.Suits2,
        LocationNames.Suits3,
        LocationNames.Suits4,
        # LocationNames.Suits5,
    ],
    TechNames.Jobs: [
        LocationNames.Jobs1,
        LocationNames.Jobs2,
        LocationNames.Jobs3,
    ],
    TechNames.AdvancedResearch: [
        LocationNames.AdvancedResearch1,
        LocationNames.AdvancedResearch2,
        LocationNames.AdvancedResearch3,
        LocationNames.AdvancedResearch4,
        # LocationNames.AdvancedResearch5,
    ],
    TechNames.SpaceProgram: [
        LocationNames.SpaceProgram1,
        LocationNames.SpaceProgram2,
        LocationNames.SpaceProgram3,
        LocationNames.SpaceProgram4,
    ],
    TechNames.CrashPlan: [
        LocationNames.CrashPlan1,
        LocationNames.CrashPlan2,
        LocationNames.CrashPlan3,
        # LocationNames.CrashPlan4,
    ],
    TechNames.DurableLifeSupport: [
        LocationNames.DurableLifeSupport1,
        LocationNames.DurableLifeSupport2,
        LocationNames.DurableLifeSupport3,
        LocationNames.DurableLifeSupport4,
        LocationNames.DurableLifeSupport5,
    ],
    TechNames.NuclearResearch: [
        LocationNames.NuclearResearch1,
        LocationNames.NuclearResearch2,
        # LocationNames.NuclearResearch3,
    ],
    TechNames.AdvancedNuclearResearch: [
        LocationNames.AdvancedNuclearResearch1,
        LocationNames.AdvancedNuclearResearch2,
    ],
    TechNames.NuclearStorage: [
        LocationNames.NuclearStorage1,
    ],
    TechNames.NuclearPropulsion: [
        LocationNames.NuclearPropulsion1,
    ],
    TechNames.NotificationSystems: [
        LocationNames.NotificationSystems1,
        LocationNames.NotificationSystems2,
        LocationNames.NotificationSystems3,
    ],
    TechNames.ArtificialFriends: [
        LocationNames.ArtificialFriends1,
        LocationNames.ArtificialFriends2,
    ],
    TechNames.BasicRefinement: [
        LocationNames.BasicRefinement1,
        LocationNames.BasicRefinement2,
    ],
    TechNames.RefinedObjects: [
        LocationNames.RefinedObjects1,
        LocationNames.RefinedObjects2,
        LocationNames.RefinedObjects3,
        LocationNames.RefinedObjects4,
    ],
    TechNames.Smelting: [
        LocationNames.Smelting1,
        LocationNames.Smelting2,
    ],
    TechNames.HighTempForging: [
        LocationNames.HighTempForging1,
        LocationNames.HighTempForging2,
        LocationNames.HighTempForging3,
        LocationNames.HighTempForging4,
        LocationNames.HighTempForging5,
    ],
    TechNames.HighPressureForging: [
        LocationNames.HighPressureForging1,
    ],
    TechNames.RadiationProtection: [
        LocationNames.RadiationProtection1,
        LocationNames.RadiationProtection2,
        LocationNames.RadiationProtection3,
        LocationNames.RadiationProtection4,
    ],
    TechNames.TemperatureModulation: [
        LocationNames.TemperatureModulation1,
        LocationNames.TemperatureModulation2,
        LocationNames.TemperatureModulation3,
        LocationNames.TemperatureModulation4,
        LocationNames.TemperatureModulation5,
    ],
    TechNames.HVAC: [
        LocationNames.HVAC1,
        LocationNames.HVAC2,
        LocationNames.HVAC3,
        LocationNames.HVAC4,
        LocationNames.HVAC5,
        LocationNames.HVAC6,
        LocationNames.HVAC7,
    ],
    TechNames.LiquidTemperature: [
        LocationNames.LiquidTemperature1,
        LocationNames.LiquidTemperature2,
        LocationNames.LiquidTemperature3,
        LocationNames.LiquidTemperature4,
        LocationNames.LiquidTemperature5,
        LocationNames.LiquidTemperature6,
        LocationNames.LiquidTemperature7,
    ],
    TechNames.SmartHome: [
        LocationNames.SmartHome1,
        LocationNames.SmartHome2,
        LocationNames.SmartHome3,
        LocationNames.SmartHome4,
        # LocationNames.SmartHome5,
    ],
    TechNames.GenericSensors: [
        LocationNames.GenericSensors1,
        LocationNames.GenericSensors2,
        LocationNames.GenericSensors3,
        LocationNames.GenericSensors4,
        LocationNames.GenericSensors5,
        LocationNames.GenericSensors6,
        LocationNames.GenericSensors7,
        LocationNames.GenericSensors8,
    ],
    TechNames.LogicCircuits: [
        LocationNames.LogicCircuits1,
        LocationNames.LogicCircuits2,
        LocationNames.LogicCircuits3,
        LocationNames.LogicCircuits4,
    ],
    TechNames.ParallelAutomation: [
        LocationNames.ParallelAutomation1,
        LocationNames.ParallelAutomation2,
        LocationNames.ParallelAutomation3,
        LocationNames.ParallelAutomation4,
    ],
    TechNames.Computing: [
        LocationNames.Computing1,
        LocationNames.Computing2,
        LocationNames.Computing3,
        LocationNames.Computing4,
        LocationNames.Computing5,
    ],
    TechNames.Multiplexing: [
        LocationNames.Multiplexing1,
        LocationNames.Multiplexing2,
    ],
    TechNames.TravelTubes: [
        LocationNames.TravelTubes1,
        LocationNames.TravelTubes2,
        LocationNames.TravelTubes3,
        LocationNames.TravelTubes4,
    ],
    TechNames.SmartStorage: [
        LocationNames.SmartStorage1,
        LocationNames.SmartStorage2,
        LocationNames.SmartStorage3,
        # LocationNames.SmartStorage4,
    ],
    TechNames.SolidManagement: [
        LocationNames.SolidManagement1,
        LocationNames.SolidManagement2,
        LocationNames.SolidManagement3,
        LocationNames.SolidManagement4,
        LocationNames.SolidManagement5,
        LocationNames.SolidManagement6,
    ],
    TechNames.Jetpacks: [
        LocationNames.Jetpacks1,
        LocationNames.Jetpacks2,
        LocationNames.Jetpacks3,
        LocationNames.Jetpacks4,
        LocationNames.Jetpacks5,
        LocationNames.Jetpacks6,
    ],
    TechNames.HighVelocityTransport: [
        LocationNames.HighVelocityTransport1,
        LocationNames.HighVelocityTransport2,
    ],
    TechNames.SolidTransport: [
        LocationNames.SolidTransport1,
        LocationNames.SolidTransport2,
        LocationNames.SolidTransport3,
        LocationNames.SolidTransport4,
    ],
    TechNames.Monuments: [
        LocationNames.Monuments1,
        LocationNames.Monuments2,
        LocationNames.Monuments3,
    ],
    TechNames.SolidSpace: [
        LocationNames.SolidSpace1,
        LocationNames.SolidSpace2,
        LocationNames.SolidSpace3,
        LocationNames.SolidSpace4,
        LocationNames.SolidSpace5,
        LocationNames.SolidSpace6,
        LocationNames.SolidSpace7,
        LocationNames.SolidSpace8,
    ],
    TechNames.RoboticTools: [
        LocationNames.RoboticTools1,
        LocationNames.RoboticTools2,
    ],
    TechNames.PortableGasses: [
        LocationNames.PortableGasses1,
        LocationNames.PortableGasses2,
        LocationNames.PortableGasses3,
        LocationNames.PortableGasses4,
        LocationNames.PortableGasses5,
        LocationNames.PortableGasses6,
    ],
    TechNames.Bioengineering: [
        LocationNames.Bioengineering1,
    ],
    TechNames.SpaceCombustion: [
        LocationNames.SpaceCombustion1,
        LocationNames.SpaceCombustion2,
    ],
    TechNames.HighVelocityDestruction: [
        LocationNames.HighVelocityDestruction1,
    ],
    TechNames.GasDistribution: [
        LocationNames.GasDistribution1,
        LocationNames.GasDistribution2,
        LocationNames.GasDistribution3,
        LocationNames.GasDistribution4,
    ],
    TechNames.AdvancedScanners: [
        LocationNames.AdvancedScanners1,
        LocationNames.AdvancedScanners2,
        LocationNames.AdvancedScanners3,
    ],
    TechNames.SkyDetectors: [
        LocationNames.SkyDetectors1,
        LocationNames.SkyDetectors2,
    ]

}

location_to_tech_name: typing.Dict[str, str] = {
    LocationNames.BasicFarming1: TechNames.BasicFarming,
    LocationNames.BasicFarming2: TechNames.BasicFarming,
    LocationNames.BasicFarming3: TechNames.BasicFarming,
    LocationNames.BasicFarming4: TechNames.BasicFarming,
    LocationNames.MealPrep1: TechNames.MealPrep,
    LocationNames.MealPrep2: TechNames.MealPrep,
    LocationNames.MealPrep3: TechNames.MealPrep,
    LocationNames.MealPrep4: TechNames.MealPrep,
    LocationNames.PowerRegulation1: TechNames.PowerRegulation,
    LocationNames.PowerRegulation2: TechNames.PowerRegulation,
    LocationNames.PowerRegulation3: TechNames.PowerRegulation,
    LocationNames.Combustion1: TechNames.Combustion,
    LocationNames.Combustion2: TechNames.Combustion,
    LocationNames.BasicRefinement1: TechNames.BasicRefinement,
    LocationNames.BasicRefinement2: TechNames.BasicRefinement,
    LocationNames.Jobs1: TechNames.Jobs,
    LocationNames.Jobs2: TechNames.Jobs,
    LocationNames.Jobs3: TechNames.Jobs,
    LocationNames.AdvancedResearch1: TechNames.AdvancedResearch,
    LocationNames.AdvancedResearch2: TechNames.AdvancedResearch,
    LocationNames.AdvancedResearch3: TechNames.AdvancedResearch,
    LocationNames.AdvancedResearch4: TechNames.AdvancedResearch,
    # LocationNames.AdvancedResearch5: TechNames.AdvancedResearch,
    LocationNames.MedicineI1: TechNames.MedicineI,
    LocationNames.MedicineII1: TechNames.MedicineII,
    LocationNames.MedicineII2: TechNames.MedicineII,
    LocationNames.LiquidPiping1: TechNames.LiquidPiping,
    LocationNames.LiquidPiping2: TechNames.LiquidPiping,
    LocationNames.LiquidPiping3: TechNames.LiquidPiping,
    LocationNames.LiquidPiping4: TechNames.LiquidPiping,
    LocationNames.ImprovedOxygen1: TechNames.ImprovedOxygen,
    LocationNames.ImprovedOxygen2: TechNames.ImprovedOxygen,
    LocationNames.SanitationSciences1: TechNames.SanitationSciences,
    LocationNames.SanitationSciences2: TechNames.SanitationSciences,
    LocationNames.SanitationSciences3: TechNames.SanitationSciences,
    LocationNames.SanitationSciences4: TechNames.SanitationSciences,
    LocationNames.AdvancedFiltration1: TechNames.AdvancedFiltration,
    LocationNames.AdvancedFiltration2: TechNames.AdvancedFiltration,
    LocationNames.AdvancedFiltration3: TechNames.AdvancedFiltration,
    LocationNames.GasPiping1: TechNames.GasPiping,
    LocationNames.GasPiping2: TechNames.GasPiping,
    LocationNames.GasPiping3: TechNames.GasPiping,
    LocationNames.GasPiping4: TechNames.GasPiping,
    LocationNames.PressureManagement1: TechNames.PressureManagement,
    LocationNames.PressureManagement2: TechNames.PressureManagement,
    LocationNames.PressureManagement3: TechNames.PressureManagement,
    LocationNames.PressureManagement4: TechNames.PressureManagement,
    LocationNames.PortableGasses1: TechNames.PortableGasses,
    LocationNames.PortableGasses2: TechNames.PortableGasses,
    LocationNames.PortableGasses3: TechNames.PortableGasses,
    LocationNames.PortableGasses4: TechNames.PortableGasses,
    LocationNames.PortableGasses5: TechNames.PortableGasses,
    LocationNames.PortableGasses6: TechNames.PortableGasses,
    LocationNames.InteriorDecor1: TechNames.InteriorDecor,
    LocationNames.InteriorDecor2: TechNames.InteriorDecor,
    LocationNames.InteriorDecor3: TechNames.InteriorDecor,
    LocationNames.Artistry1: TechNames.Artistry,
    LocationNames.Artistry2: TechNames.Artistry,
    LocationNames.Artistry3: TechNames.Artistry,
    LocationNames.Artistry4: TechNames.Artistry,
    LocationNames.Artistry5: TechNames.Artistry,
    LocationNames.Artistry6: TechNames.Artistry,
    LocationNames.Artistry7: TechNames.Artistry,
    LocationNames.SmartHome1: TechNames.SmartHome,
    LocationNames.SmartHome2: TechNames.SmartHome,
    LocationNames.SmartHome3: TechNames.SmartHome,
    LocationNames.SmartHome4: TechNames.SmartHome,
    # LocationNames.SmartHome5: TechNames.SmartHome,

    LocationNames.Agriculture1: TechNames.Agriculture,
    LocationNames.Agriculture2: TechNames.Agriculture,
    LocationNames.Agriculture3: TechNames.Agriculture,
    LocationNames.Agriculture4: TechNames.Agriculture,
    LocationNames.Agriculture5: TechNames.Agriculture,
    LocationNames.Agriculture6: TechNames.Agriculture,
    LocationNames.FoodRepurposing1: TechNames.FoodRepurposing,
    LocationNames.FoodRepurposing2: TechNames.FoodRepurposing,
    LocationNames.FoodRepurposing3: TechNames.FoodRepurposing,
    LocationNames.Ranching1: TechNames.Ranching,
    LocationNames.Ranching2: TechNames.Ranching,
    LocationNames.Ranching3: TechNames.Ranching,
    LocationNames.Ranching4: TechNames.Ranching,
    LocationNames.Ranching5: TechNames.Ranching,
    LocationNames.Ranching6: TechNames.Ranching,
    LocationNames.Ranching7: TechNames.Ranching,
    LocationNames.AnimalControl1: TechNames.AnimalControl,
    LocationNames.AnimalControl2: TechNames.AnimalControl,
    LocationNames.AnimalControl3: TechNames.AnimalControl,
    LocationNames.AnimalControl4: TechNames.AnimalControl,
    LocationNames.AnimalControl5: TechNames.AnimalControl,
    LocationNames.SpaceCombustion1: TechNames.SpaceCombustion,
    LocationNames.SpaceCombustion2: TechNames.SpaceCombustion,
    LocationNames.FossilFuels1: TechNames.FossilFuels,
    LocationNames.FossilFuels2: TechNames.FossilFuels,
    LocationNames.FossilFuels3: TechNames.FossilFuels,
    LocationNames.Plastics1: TechNames.Plastics,
    LocationNames.Plastics2: TechNames.Plastics,
    LocationNames.Acoustics1: TechNames.Acoustics,
    LocationNames.Acoustics2: TechNames.Acoustics,
    LocationNames.Acoustics3: TechNames.Acoustics,
    LocationNames.AdvancedPowerRegulation1: TechNames.AdvancedPowerRegulation,
    LocationNames.AdvancedPowerRegulation2: TechNames.AdvancedPowerRegulation,
    LocationNames.AdvancedPowerRegulation3: TechNames.AdvancedPowerRegulation,
    LocationNames.AdvancedPowerRegulation4: TechNames.AdvancedPowerRegulation,
    LocationNames.AdvancedPowerRegulation5: TechNames.AdvancedPowerRegulation,
    LocationNames.AdvancedPowerRegulation6: TechNames.AdvancedPowerRegulation,
    LocationNames.PrettyGoodConductors1: TechNames.PrettyGoodConductors,
    LocationNames.PrettyGoodConductors2: TechNames.PrettyGoodConductors,
    LocationNames.PrettyGoodConductors3: TechNames.PrettyGoodConductors,
    LocationNames.PrettyGoodConductors4: TechNames.PrettyGoodConductors,
    LocationNames.PrettyGoodConductors5: TechNames.PrettyGoodConductors,
    LocationNames.SmartStorage1: TechNames.SmartStorage,
    LocationNames.SmartStorage2: TechNames.SmartStorage,
    LocationNames.SmartStorage3: TechNames.SmartStorage,
    # LocationNames.SmartStorage4: TechNames.SmartStorage,
    LocationNames.SolidTransport1: TechNames.SolidTransport,
    LocationNames.SolidTransport2: TechNames.SolidTransport,
    LocationNames.SolidTransport3: TechNames.SolidTransport,
    LocationNames.SolidTransport4: TechNames.SolidTransport,
    LocationNames.RefinedObjects1: TechNames.RefinedObjects,
    LocationNames.RefinedObjects2: TechNames.RefinedObjects,
    LocationNames.RefinedObjects3: TechNames.RefinedObjects,
    LocationNames.RefinedObjects4: TechNames.RefinedObjects,
    LocationNames.Smelting1: TechNames.Smelting,
    LocationNames.Smelting2: TechNames.Smelting,
    LocationNames.ArtificialFriends1: TechNames.ArtificialFriends,
    LocationNames.ArtificialFriends2: TechNames.ArtificialFriends,
    LocationNames.RoboticTools1: TechNames.RoboticTools,
    LocationNames.RoboticTools2: TechNames.RoboticTools,
    LocationNames.SpaceProgram1: TechNames.SpaceProgram,
    LocationNames.SpaceProgram2: TechNames.SpaceProgram,
    LocationNames.SpaceProgram3: TechNames.SpaceProgram,
    LocationNames.SpaceProgram4: TechNames.SpaceProgram,
    LocationNames.CrashPlan1: TechNames.CrashPlan,
    LocationNames.CrashPlan2: TechNames.CrashPlan,
    LocationNames.CrashPlan3: TechNames.CrashPlan,
    # LocationNames.CrashPlan4: TechNames.CrashPlan,
    LocationNames.NotificationSystems1: TechNames.NotificationSystems,
    LocationNames.NotificationSystems2: TechNames.NotificationSystems,
    LocationNames.NotificationSystems3: TechNames.NotificationSystems,
    LocationNames.SkyDetectors1: TechNames.SkyDetectors,
    LocationNames.SkyDetectors2: TechNames.SkyDetectors,
    LocationNames.NuclearResearch1: TechNames.NuclearResearch,
    LocationNames.NuclearResearch2: TechNames.NuclearResearch,
    # LocationNames.NuclearResearch3: TechNames.NuclearResearch,
    LocationNames.AdvancedNuclearResearch1: TechNames.AdvancedNuclearResearch,
    LocationNames.AdvancedNuclearResearch2: TechNames.AdvancedNuclearResearch,
    LocationNames.MedicineIII1: TechNames.MedicineIII,
    LocationNames.MedicineIII2: TechNames.MedicineIII,
    LocationNames.MedicineIII3: TechNames.MedicineIII,
    LocationNames.MedicineIV1: TechNames.MedicineIV,
    LocationNames.MedicineIV2: TechNames.MedicineIV,
    LocationNames.MedicineIV3: TechNames.MedicineIV,
    LocationNames.LiquidFiltering1: TechNames.LiquidFiltering,
    LocationNames.LiquidFiltering2: TechNames.LiquidFiltering,
    LocationNames.FlowRedirection1: TechNames.FlowRedirection,
    LocationNames.FlowRedirection2: TechNames.FlowRedirection,
    LocationNames.FlowRedirection3: TechNames.FlowRedirection,
    LocationNames.FlowRedirection4: TechNames.FlowRedirection,
    LocationNames.FlowRedirection5: TechNames.FlowRedirection,
    LocationNames.LiquidDistribution1: TechNames.LiquidDistribution,
    LocationNames.LiquidDistribution2: TechNames.LiquidDistribution,
    LocationNames.LiquidDistribution3: TechNames.LiquidDistribution,
    LocationNames.LiquidDistribution4: TechNames.LiquidDistribution,
    LocationNames.AdvancedSanitation1: TechNames.AdvancedSanitation,
    LocationNames.Distillation1: TechNames.Distillation,
    LocationNames.Distillation2: TechNames.Distillation,
    LocationNames.Distillation3: TechNames.Distillation,
    LocationNames.ImprovedLiquidPiping1: TechNames.ImprovedLiquidPiping,
    LocationNames.ImprovedLiquidPiping2: TechNames.ImprovedLiquidPiping,
    LocationNames.ImprovedLiquidPiping3: TechNames.ImprovedLiquidPiping,
    LocationNames.ImprovedLiquidPiping4: TechNames.ImprovedLiquidPiping,
    LocationNames.LiquidTemperature1: TechNames.LiquidTemperature,
    LocationNames.LiquidTemperature2: TechNames.LiquidTemperature,
    LocationNames.LiquidTemperature3: TechNames.LiquidTemperature,
    LocationNames.LiquidTemperature4: TechNames.LiquidTemperature,
    LocationNames.LiquidTemperature5: TechNames.LiquidTemperature,
    LocationNames.LiquidTemperature6: TechNames.LiquidTemperature,
    LocationNames.LiquidTemperature7: TechNames.LiquidTemperature,
    LocationNames.TemperatureModulation1: TechNames.TemperatureModulation,
    LocationNames.TemperatureModulation2: TechNames.TemperatureModulation,
    LocationNames.TemperatureModulation3: TechNames.TemperatureModulation,
    LocationNames.TemperatureModulation4: TechNames.TemperatureModulation,
    LocationNames.TemperatureModulation5: TechNames.TemperatureModulation,
    LocationNames.HVAC1: TechNames.HVAC,
    LocationNames.HVAC2: TechNames.HVAC,
    LocationNames.HVAC3: TechNames.HVAC,
    LocationNames.HVAC4: TechNames.HVAC,
    LocationNames.HVAC5: TechNames.HVAC,
    LocationNames.HVAC6: TechNames.HVAC,
    LocationNames.HVAC7: TechNames.HVAC,
    LocationNames.Decontamination1: TechNames.Decontamination,
    LocationNames.Decontamination2: TechNames.Decontamination,
    LocationNames.Decontamination3: TechNames.Decontamination,
    LocationNames.SpaceGas1: TechNames.SpaceGas,
    LocationNames.SpaceGas2: TechNames.SpaceGas,
    LocationNames.SpaceGas3: TechNames.SpaceGas,
    LocationNames.SpaceGas4: TechNames.SpaceGas,
    LocationNames.ImprovedGasPiping1: TechNames.ImprovedGasPiping,
    LocationNames.ImprovedGasPiping2: TechNames.ImprovedGasPiping,
    LocationNames.ImprovedGasPiping3: TechNames.ImprovedGasPiping,
    LocationNames.ImprovedGasPiping4: TechNames.ImprovedGasPiping,
    LocationNames.GasDistribution1: TechNames.GasDistribution,
    LocationNames.GasDistribution2: TechNames.GasDistribution,
    LocationNames.GasDistribution3: TechNames.GasDistribution,
    LocationNames.GasDistribution4: TechNames.GasDistribution,
    LocationNames.Suits1: TechNames.Suits,
    LocationNames.Suits2: TechNames.Suits,
    LocationNames.Suits3: TechNames.Suits,
    LocationNames.Suits4: TechNames.Suits,
    # LocationNames.Suits5: TechNames.Suits,
    LocationNames.Clothing1: TechNames.Clothing,
    LocationNames.Clothing2: TechNames.Clothing,
    LocationNames.Clothing3: TechNames.Clothing,
    LocationNames.Luxury1: TechNames.Luxury,
    LocationNames.Luxury2: TechNames.Luxury,
    LocationNames.Luxury3: TechNames.Luxury,
    LocationNames.Luxury4: TechNames.Luxury,
    LocationNames.Luxury5: TechNames.Luxury,
    LocationNames.FineArt1: TechNames.FineArt,
    LocationNames.FineArt2: TechNames.FineArt,
    LocationNames.RefractiveDecor1: TechNames.RefractiveDecor,
    LocationNames.RefractiveDecor2: TechNames.RefractiveDecor,
    LocationNames.RefractiveDecor3: TechNames.RefractiveDecor,
    LocationNames.GenericSensors1: TechNames.GenericSensors,
    LocationNames.GenericSensors2: TechNames.GenericSensors,
    LocationNames.GenericSensors3: TechNames.GenericSensors,
    LocationNames.GenericSensors4: TechNames.GenericSensors,
    LocationNames.GenericSensors5: TechNames.GenericSensors,
    LocationNames.GenericSensors6: TechNames.GenericSensors,
    LocationNames.GenericSensors7: TechNames.GenericSensors,
    LocationNames.GenericSensors8: TechNames.GenericSensors,
    LocationNames.LogicCircuits1: TechNames.LogicCircuits,
    LocationNames.LogicCircuits2: TechNames.LogicCircuits,
    LocationNames.LogicCircuits3: TechNames.LogicCircuits,
    LocationNames.LogicCircuits4: TechNames.LogicCircuits,
    LocationNames.ParallelAutomation1: TechNames.ParallelAutomation,
    LocationNames.ParallelAutomation2: TechNames.ParallelAutomation,
    LocationNames.ParallelAutomation3: TechNames.ParallelAutomation,
    LocationNames.ParallelAutomation4: TechNames.ParallelAutomation,

    LocationNames.FinerDining1: TechNames.FinerDining,
    LocationNames.FinerDining2: TechNames.FinerDining,
    LocationNames.FinerDining3: TechNames.FinerDining,
    LocationNames.FinerDining4: TechNames.FinerDining,
    LocationNames.AnimalComfort1: TechNames.AnimalComfort,
    LocationNames.AnimalComfort2: TechNames.AnimalComfort,
    LocationNames.AnimalComfort3: TechNames.AnimalComfort,
    LocationNames.PrecisionPlumbing1: TechNames.PrecisionPlumbing,
    LocationNames.PrecisionPlumbing2: TechNames.PrecisionPlumbing,
    LocationNames.PrecisionPlumbing3: TechNames.PrecisionPlumbing,
    LocationNames.Catalytics1: TechNames.Catalytics,
    LocationNames.Catalytics2: TechNames.Catalytics,
    LocationNames.Catalytics3: TechNames.Catalytics,
    LocationNames.Catalytics4: TechNames.Catalytics,
    LocationNames.Catalytics5: TechNames.Catalytics,
    LocationNames.RenewableEnergy1: TechNames.RenewableEnergy,
    LocationNames.RenewableEnergy2: TechNames.RenewableEnergy,
    LocationNames.RenewableEnergy3: TechNames.RenewableEnergy,
    LocationNames.RenewableEnergy4: TechNames.RenewableEnergy,
    LocationNames.SpacePower1: TechNames.SpacePower,
    LocationNames.SpacePower2: TechNames.SpacePower,
    LocationNames.SpacePower3: TechNames.SpacePower,
    LocationNames.GlassFurnishings1: TechNames.GlassFurnishings,
    LocationNames.GlassFurnishings2: TechNames.GlassFurnishings,
    LocationNames.GlassFurnishings3: TechNames.GlassFurnishings,
    LocationNames.RenaissanceArt1: TechNames.RenaissanceArt,
    LocationNames.RenaissanceArt2: TechNames.RenaissanceArt,
    LocationNames.ValveMiniaturization1: TechNames.ValveMiniaturization,
    LocationNames.ValveMiniaturization2: TechNames.ValveMiniaturization,
    LocationNames.DurableLifeSupport1: TechNames.DurableLifeSupport,
    LocationNames.DurableLifeSupport2: TechNames.DurableLifeSupport,
    LocationNames.DurableLifeSupport3: TechNames.DurableLifeSupport,
    LocationNames.DurableLifeSupport4: TechNames.DurableLifeSupport,
    LocationNames.DurableLifeSupport5: TechNames.DurableLifeSupport,
    LocationNames.NuclearStorage1: TechNames.NuclearStorage,
    LocationNames.HighTempForging1: TechNames.HighTempForging,
    LocationNames.HighTempForging2: TechNames.HighTempForging,
    LocationNames.HighTempForging3: TechNames.HighTempForging,
    LocationNames.HighTempForging4: TechNames.HighTempForging,
    LocationNames.HighTempForging5: TechNames.HighTempForging,
    LocationNames.RadiationProtection1: TechNames.RadiationProtection,
    LocationNames.RadiationProtection2: TechNames.RadiationProtection,
    LocationNames.RadiationProtection3: TechNames.RadiationProtection,
    LocationNames.RadiationProtection4: TechNames.RadiationProtection,
    LocationNames.Computing1: TechNames.Computing,
    LocationNames.Computing2: TechNames.Computing,
    LocationNames.Computing3: TechNames.Computing,
    LocationNames.Computing4: TechNames.Computing,
    LocationNames.Computing5: TechNames.Computing,
    LocationNames.TravelTubes1: TechNames.TravelTubes,
    LocationNames.TravelTubes2: TechNames.TravelTubes,
    LocationNames.TravelTubes3: TechNames.TravelTubes,
    LocationNames.TravelTubes4: TechNames.TravelTubes,
    LocationNames.SolidSpace1: TechNames.SolidSpace,
    LocationNames.SolidSpace2: TechNames.SolidSpace,
    LocationNames.SolidSpace3: TechNames.SolidSpace,
    LocationNames.SolidSpace4: TechNames.SolidSpace,
    LocationNames.SolidSpace5: TechNames.SolidSpace,
    LocationNames.SolidSpace6: TechNames.SolidSpace,
    LocationNames.SolidSpace7: TechNames.SolidSpace,
    LocationNames.SolidSpace8: TechNames.SolidSpace,
    LocationNames.Bioengineering1: TechNames.Bioengineering,

    LocationNames.DairyOperation1: TechNames.DairyOperation,
    LocationNames.DairyOperation2: TechNames.DairyOperation,
    LocationNames.DairyOperation3: TechNames.DairyOperation,
    LocationNames.HydrocarbonPropulsion1: TechNames.HydrocarbonPropulsion,
    LocationNames.HydrocarbonPropulsion2: TechNames.HydrocarbonPropulsion,
    LocationNames.BetterHydroCarbonPropulsion1: TechNames.BetterHydroCarbonPropulsion,
    LocationNames.SolidManagement1: TechNames.SolidManagement,
    LocationNames.SolidManagement2: TechNames.SolidManagement,
    LocationNames.SolidManagement3: TechNames.SolidManagement,
    LocationNames.SolidManagement4: TechNames.SolidManagement,
    LocationNames.SolidManagement5: TechNames.SolidManagement,
    LocationNames.SolidManagement6: TechNames.SolidManagement,
    LocationNames.HighVelocityTransport1: TechNames.HighVelocityTransport,
    LocationNames.HighVelocityTransport2: TechNames.HighVelocityTransport,
    LocationNames.HighVelocityDestruction1: TechNames.HighVelocityDestruction,
    LocationNames.HighPressureForging1: TechNames.HighPressureForging,
    LocationNames.CryoFuelPropulsion1: TechNames.CryoFuelPropulsion,
    LocationNames.CryoFuelPropulsion2: TechNames.CryoFuelPropulsion,
    LocationNames.NuclearRefinement1: TechNames.NuclearRefinement,
    LocationNames.NuclearRefinement2: TechNames.NuclearRefinement,
    LocationNames.NuclearRefinement3: TechNames.NuclearRefinement,
    LocationNames.NuclearPropulsion1: TechNames.NuclearPropulsion,
    LocationNames.Jetpacks1: TechNames.Jetpacks,
    LocationNames.Jetpacks2: TechNames.Jetpacks,
    LocationNames.Jetpacks3: TechNames.Jetpacks,
    LocationNames.Jetpacks4: TechNames.Jetpacks,
    LocationNames.Jetpacks5: TechNames.Jetpacks,
    LocationNames.Jetpacks6: TechNames.Jetpacks,
    LocationNames.EnvironmentalAppreciation1: TechNames.EnvironmentalAppreciation,
    LocationNames.Screens1: TechNames.Screens,
    LocationNames.Monuments1: TechNames.Monuments,
    LocationNames.Monuments2: TechNames.Monuments,
    LocationNames.Monuments3: TechNames.Monuments,
    LocationNames.Multiplexing1: TechNames.Multiplexing,
    LocationNames.Multiplexing2: TechNames.Multiplexing,
    LocationNames.AdvancedScanners1: TechNames.AdvancedScanners,
    LocationNames.AdvancedScanners2: TechNames.AdvancedScanners,
    LocationNames.AdvancedScanners3: TechNames.AdvancedScanners,
}
