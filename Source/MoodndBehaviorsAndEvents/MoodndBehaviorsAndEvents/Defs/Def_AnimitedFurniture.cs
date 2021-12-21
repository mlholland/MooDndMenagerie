using System;
using System.Collections.Generic;
using Verse.Sound;
using Verse;
using RimWorld;
using UnityEngine;

// This Defs are used to programatically generate Animal ThingDefs that
// represent animated furniture animals. To be clear: these defs themselves are NOT the actual defs that
// become animals in-game, they are the precurser defs that are only used in startup code.
namespace MoodndBehaviorsAndEvents
{
    public class Def_AnimatedFurniture : Def
    {
        public ThingDef furnitureDef;
        public string baseDefName;

        public bool makeProgrammaticDefs = true; // only set false for non-material-based defs that have complex elements that can't be easily generated programatically (aka VEF ranged Attacks)

        // Most of the PawnKindDef types
        [NoTranslate]
        public List<string> techHediffsDisallowTags;
        [NoTranslate]
        public List<string> techHediffsTags;
        public FloatRange techHediffsMoney;
        public List<ThingDef> techHediffsRequired;
        public List<SpecificApparelRequirement> specificApparelRequirements;
        public Color? skinColorOverride;
        public Color apparelColor;
        public bool ignoreFactionApparelStuffRequirements;
        public bool apparelIgnoreSeasons;
        public float apparelAllowHeadgearChance;
        [NoTranslate]
        public List<string> apparelDisallowTags;
        [NoTranslate]
        public List<string> apparelTags;
        public List<ThingDef> apparelRequired;
        public FloatRange apparelMoney;
        public ThingStyleDef weaponStyleDef;
        public ThingDef weaponStuffOverride;
        [NoTranslate]
        public List<string> weaponTags;
        public FloatRange weaponMoney;
        public bool forceNormalGearQuality;
        public float techHediffsChance;
        public QualityCategory? forceWeaponQuality;
        public int techHediffsMaxAmount;
        public List<ThingDefCountClass> fixedInventory;
        public float ecoSystemWeight;
        public IntRange wildGroupSize;
        [MustTranslate]
        public string labelFemalePlural;
        [MustTranslate]
        public string labelFemale;
        [MustTranslate]
        public string labelMalePlural;
        [MustTranslate]
        public string labelMale;
        public int minBestSkillLevel;
        public int minTotalSkillLevels;
        public int extraSkillLevels;
        public WorkTags requiredWorkTags;
        public List<SkillRange> skills;
        public bool trader;
        public List<ChemicalDef> forcedAddictions;
        public IntRange combatEnhancingDrugsCount;
        public float combatEnhancingDrugsChance;
        public float chemicalAddictionChance;
        public ThingDef invFoodDef;
        public float invNutrition;
        public PawnInventoryOption inventoryOptions;
        public float biocodeWeaponChance;
        public QualityCategory itemQuality;
        public FloatRange gearHealthRange;
        public FloatRange fleeHealthThresholdRange;
        public int maxGenerationAge;
        public float acceptArrestChanceFactor;
        public FloatRange? chronologicalAgeRange;
        public float backstoryCryptosleepCommonality;
        public RulePackDef nameMakerFemale;
        public RulePackDef nameMaker;
        public List<MissingPart> missingParts;
        public Color? forcedHairColor;
        public HairDef forcedHair;
        public bool factionLeader;
        [LoadAlias("hairTags")]
        public List<StyleItemTagWeighted> styleItemTags;
        public List<TraitDef> disallowedTraits;
        public List<TraitRequirement> forcedTraits;
        public List<AlternateGraphic> alternateGraphics;
        public List<PawnKindLifeStage> lifeStages;
        [MustTranslate]
        public string labelPlural;
        [NoTranslate]
        public List<string> backstoryCategories;
        [NoTranslate]
        public List<BackstoryCategoryFilter> backstoryFiltersOverride;
        [NoTranslate]
        public List<BackstoryCategoryFilter> backstoryFilters;
        public FactionDef defaultFactionType;
        public float alternateGraphicChance;
        public Gender? fixedGender;
        public int minGenerationAge;
        public bool generateInitialNonFamilyRelations;
        public bool aiAvoidCover;
        public float baseRecruitDifficulty;
        public bool isGoodBreacher;
        public bool canBeSapper;
        public bool canArriveManhunter;
        public bool allowOldAgeInjuries;
        public bool isFighter;
        public bool allowRoyalApparelRequirements;
        public bool allowRoyalRoomRequirements;
        public float combatPower;
        public RoyalTitleDef minTitleRequired;
        public List<RoyalTitleDef> titleSelectOne;
        public float defendPointRadius;
        public bool factionHostileOnKill;
        public bool factionHostileOnDeath;
        public bool destroyGearOnDrop;
        public FloatRange? initialWillRange;
        public float royalTitleChance;
        public RoyalTitleDef titleRequired;
        public FloatRange? initialResistanceRange;
        public RaceProperties race;

    }

    public class AnimatedFurnitureThing : ThingDef { }
}
