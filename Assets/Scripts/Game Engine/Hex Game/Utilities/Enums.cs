using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine
{
    public class Enums 
    {
       
    }

    // Commonly Shared
    #region
    public enum KeyWordType
    {
        None = 0,
        Accuracy = 11,
        Aura = 6,
        Armour = 30,
        BackStrike = 7,
        Constitution = 13,
        CriticalChance = 19,
        CriticalStrike = 20,
        DebuffResistance = 16,
        Dodge = 12,
        ActionPoints = 2,
        Flanking = 26,
        FreeStrike = 25,
        Initiative = 24,
        InjuryResistance = 17,
        Intelligence = 10,
        KnockBack = 23,
        MagicResistance = 28,
        MeleeAttack = 3,
        Perk = 1,
        PhysicalResistance = 27, 
        RangedAttack = 4,
        Resolve = 14,
        Skill = 5,
        Strength = 9,
        StressResistance = 15,
        ActionPointRecovery = 8,
        Teleport = 22,
        Vision = 21,
        Wits = 29,
        ZoneOfControl = 18,
        Might = 31,
        
    }
    public enum CoreAttribute
    {
        Might = 0,
        Accuracy = 2,
        Dodge = 3,
        Constitution = 4,
        Resolve = 5,
        Wits = 6,
        Fatigue = 7,
    }
    public enum ItemCoreAttribute
    {
        Might = 19,
        PhysicalDamage = 0,
        MagicDamage = 1,
        Accuracy = 2,
        Dodge = 3,
        Constituition = 4,
        Resolve = 5,
        Wits = 6,
        Fatigue = 19,
        CriticalChance = 7,
        CriticalModifier = 14,
        Initiative = 12,
        ActionPointRecovery = 13,
        MaxActionPoints = 18,
        Vision = 15,
        AuraSize = 16,
        FatigueRecovery = 20,
        StressResistance = 8,
        DebuffResistance = 9,
        PhysicalResistance = 10,
        MagicResistance = 11,
        InjuryResistance = 17,
        
    }
    public enum Rarity
    {
        None = 0,
        Common = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4,
    }
    #endregion

    // Hex + Mapping
    #region
    public enum TileElevation
    {
        Ground = 0,
        Elevated = 1,
    }
    public enum HexMapShape
    {
        Hexagonal = 0,
        Rectangular = 1,
    }
    public enum HexDirection
    {
        None = 0,
        North = 1,
        NorthEast = 2,
        SouthEast = 3,
        South = 4,
        SouthWest = 5,
        NorthWest = 6,
        East = 7,
        West = 8,
    }
    public enum WorldTileType
    {
        Grass = 0,
        Dirt = 1,
    }
    public enum FogOfWarState
    {
        Hidden = 0,
        Revealed = 1,
    }

    #endregion

    // Dungeon Map
    #region
    public enum NodeStates
    {
        Locked,
        Visited,
        Attainable
    }
    public enum MapOrientation
    {
        BottomToTop,
        TopToBottom,
        RightToLeft,
        LeftToRight
    }
    #endregion

    // World Map
    #region
    /*
    public enum HexGridType
    {
        even_q,
        odd_q,
        even_r,
        odd_r
    };*/
    #endregion

    // Talents
    #region
    public enum TalentSchool
    {
        None = 0,
        Neutral = 1,
        Warfare = 2,
        Scoundrel = 3,
        Guardian = 5,
        Pyromania = 6,
        Ranger = 8,
        Manipulation = 9,
        Divinity = 10,
        Shadowcraft = 11,
        Naturalism = 13,
        Metamorph = 14,
    }
    public enum TargetRequirement
    {
        NoTarget = 0,
        Ally = 1,
        AllyOrSelf = 2,
        Enemy = 3,
        AllCharacters = 4,
        AllCharactersExceptSelf = 6,
        Hex = 5,
    };
    public enum SecondaryTargetRequirement
    {
        None = 0,
        Ally = 1,
        AllyOrSelf = 2,
        Enemy = 3,
        AllCharacters = 4,
        UnoccupiedHexWithinRangeOfTarget = 5,
    };
    public enum AbilityRequirementType
    {
        None = 0,
        TargetHasUnoccupiedBackTile = 1,
        TargetIsTeleportable = 2,
        CasterIsTeleportable = 3,
        CasterHasEnoughHealth = 4,

    }
    public enum AbilitySelectionPhase
    {
        None = 0,
        First = 1,
        Second = 2,
    }
    #endregion

    // Items
    #region
    public enum ItemArmourClass
    {
        None = 0,
        Light = 1,
        Medium = 2,
        Heavy = 3,
    }
    public enum ItemType
    {
        None = 0,
        Head = 1,
        Body = 2,
        Weapon = 3,
        Trinket = 4,
    }
    public enum WeaponSlot
    {
        MainHand = 0,
        Offhand = 1,
    }
    public enum RosterSlotType
    {
        None = 0,
        Trinket = 1,
        MainHand = 2,
        OffHand = 3,
        Head = 4,
        Body = 5,
    }
    public enum HandRequirement
    {
        OneHanded = 0,
        TwoHanded = 1,
    }
    public enum WeaponClass
    {
        None = 0,
        Axe = 3,
        Bow = 11,
        Dagger = 1,
        HandWeapon = 12,
        Hammer = 4,
        Holdable = 10,
        Polearm = 7,
        Shield = 9,
        Spear = 5,
        Staff = 6,
        Sword = 2,     
        ThrowingNet = 13,
      
      
    }
    public enum WeaponRequirement
    {
        None = 0,
        MeleeWeapon = 1,
        RangedWeapon = 2,
        Bow = 3,
        Shield = 4,
        ThrowingNet = 5,
        EmptyOffhand = 6,

    }

    #endregion

    // Abilities
    #region
    public enum DamageType
    {
        None = 0,
        Physical = 1,
        Magic = 2,
    }
    public enum AbilityType
    {
        None = 0,
        Skill = 1,
        MeleeAttack = 2,
        RangedAttack = 3,
    }
    public enum AbilityEffectRange
    {
        Target = 0,
        Self = 1,
        ZoneOfControl = 2,
        Aura = 3,
        AoeAtTarget = 4,
    }
    public enum AoeType
    {
        AtTarget = 0,
        ZoneOfControl = 1,
        Aura = 2,
        Global = 3,
    }
    public enum AbilityEffectType
    {
        None = 0,
        ApplyPassiveSelf = 1,
        ApplyPassiveTarget = 10,
        ApplyPassiveAoe = 12,
        ApplyPassiveInLine = 19,
        DamageTarget = 2,
        DamageAoe = 3,
        GainActionPoints = 4,
        GainActionPointsTarget = 11,
        HealSelf = 20,
        KnockBack = 5,
        LoseHealthSelf = 17,
        MoveToTile = 6,
        MoveInLine = 7,
        //RemoveArmour = 21,
        RemovePassiveTarget = 18,
        StressCheck = 9,
        SummonCharacter = 16,       
        TeleportSelf = 8,
        TeleportTargetToTile = 13,
        TeleportSelfBehindTarget = 14,
        TeleportSwitchWithTarget = 15,
       
     
     
    }
    public enum AbilityDamageSource
    {
        Ability = 0,
        Weapon = 1,
    }
    #endregion

    // Character Related
    #region
    public enum CharacterRace
    {
        None = 0,
        Human = 1,
        Orc = 2,
        Undead = 3,
        Elf = 4,
        Goblin = 5,
        Satyr = 6,
        Gnoll = 7,
        Ent = 8,
        Demon = 9,
    }
    public enum ActivationPhase
    {
        NotActivated = 0,
        StartPhase = 1,
        ActivationPhase = 2,
        EndPhase = 3,
    }
    public enum Allegiance
    {
        Player = 0,
        Enemy = 1,
    }
    public enum Controller
    {
        Player = 0,
        AI = 1,
    }
    public enum LivingState
    {
        Alive = 0,
        Dead = 1,
    }
    #endregion

    // Perks + Passives
    #region
    public enum Perk
    {
        None = 0,
        Abusive = 107,
        Ailing = 161,
        ArmsMaster = 126,
        Barrier = 47,
        Berserk = 46,
        BigMuscles = 87,
        Bleeding = 7,
        Blinded = 4,
        Guard = 74,
        BloodOfTheAncients = 66,
        BloodThirsty = 63,  
        Brave = 91,
        Brawny = 110,
        Brute = 115,
        Burning = 1,
        Bully = 67,
        Cannibalism = 23,
        Clairvoyant = 65,
        Clotter = 113,
        ClutchHitter = 99,
        Combo = 154,
        Contagious = 151,
        Courage = 155,
        Coward = 92,
        Crippled = 5,
        Cunning = 22,
        Dazed = 11,
        DeadHeart = 24,
        DimWitted = 102,
        DivineFavour = 10,
        DragonAspect = 145,
        Eager = 19,

        TrueSight = 9,           
       
        Evasion = 76,
        Executioner = 16,
        FastLearner = 101,
        Fat = 117,
        FearOfUndead = 108,
        Finesse = 121,
        Fit = 160,
        FlamingPresence = 152,
        Flight = 147,
        Focus = 75,
        Footwork = 13,
        Fortified = 156,
        Frail = 94,
        FragileBinding = 85,            
        FlamingWeapon = 8,
        HardNoggin = 112,
        HymnOfFellowship = 138,
        HymnOfPurity = 139,
        HymnOfVengeance = 140,
        HymnOfCourage = 141,
        DeadEye = 109,
        Implaccable = 60,
        Indecisive = 96,
        Inattentive = 103,
        Inoculated = 118,
        InspiringLeader = 72,
        IronWill = 128,
        Irrational = 116,
        Locomotion = 150,
        LoomingPresence = 125,
        Masochist = 82,
        Mirage = 52,
        Motivated = 122,
        Nimble = 18,        
        Opportunist = 71,
        Overcharged = 149,
        Outfitter = 157,
        Overload = 73,
        Paranoid = 114,
        PathFinder = 124,
        Perceptive = 95,
        PointBlank = 58,
        Poisoned = 80,
        PoisonedWeapon = 84,
        PoorReflexes = 98,
        PunchDrunk = 106,
        Quick = 97,
        QuickDraw = 120,
        RecentlyStunned = 6,
        Regeneration = 123,
        Resolute = 17,
        Riposte = 119,
        Rune = 26,
        Rooted = 79,
        Sadistic = 64,
        SavageLeader = 127,
        Drunkard = 25,
        Shed = 146,
        ShieldWall = 12,
        SpearWall = 162,
        ShortSighted = 105,
        Slippery = 56,
        Sloppy = 100,
        Slow = 90,
        Sniper = 70,
        SoulToken = 69,
        Squeamish = 104,
        Stealth = 77,
        StormShield = 148,
        StrikingPresence = 57,
        Strong = 93,
        StrongLungs = 158,
        Stunned = 3,       
        StunImmunity = 86,
        Survivor = 61,
        Taunt = 78,
        ThickSkinned = 81,
        Thorns = 142,
        ThrillOfTheHunt = 20,
        TigerAspect = 144,
        Torturer = 83,
        Tough = 68,  
        TurtleAspect = 143,
        UndeadHater = 111,
        Vulnerable = 48,
        Weakened = 49, 
        Wimp = 88,
        Wise = 89,
        Wheezy = 159,
        Wrath = 50,
        Volatile = 153,

        // Injuries
        BrokenArm = 27,
        BruisedLeg = 28,
        BrokenLeg = 29,
        BrokenNose = 30,
        BrokenRibs = 31,
        BrokenFinger = 32,
        CrushedWindpipe = 33,
        CutArm = 34,
        CutArmSinew = 35,
        CutArtery = 36,
        CutNeckVein = 37,
        CutLegMuscles = 38,
        DeepAbdominalCut = 39,
        DeepChestCut = 40,
        DeepFaceCut = 41,
        DislocatedShoulder = 42,
        ExposedRibs = 43,
        FracturedElbow = 44,
        FracturedHand = 157,
        FracturedRibs = 158,
        FracturedSkull = 159,
        CutEyeSocket = 160,
        StabbedKidney = 161,
        StabbedCheek = 162,
        StabbedLegMuscles = 163,
        PiercedLung = 164,
        TornEar = 165,
        Concussed = 166,
        StabbedGuts = 167,
        TornRotatorCuff = 168,

        /*
        BrokenNose = 27,
        BrokenRibs = 28,
        BrokenFinger = 29,
        GrazedEyeSocket = 31,
        DislocatedShoulder = 32,
        BrokenArm = 33,
        Concussion = 34,
        FracturedSkull = 35,
        TornKneeLigament = 36,
        BruisedLeg = 37,
        BrokenHip = 38,
        BruisedKidney = 39,
        DeepAbdominalCut = 40,
        ExposedRibs = 43,
        CutArtery = 44,*/

        // Permanent Injuries
        CompromisedLiver = 129,
        CrippledShoulder = 130,
        DeeplyDisturbed = 131,
        MissingEye = 132,
        MissingFingers = 133,
        MissingEar = 134,
        MissingNose = 135,
        PermanentlyConcussed = 136,
        TornKneeLigament = 137,
        ScarredLung = 169,

        /*
        MissingEye = 129,
        ScarredLung = 130,
        MissingFingers = 131,
        MissingNose = 132,
        PermanentlyConcussed = 133,
        CrippledKnee = 134,
        CrippledShoulder = 135,
        DeeplyDisturbed = 136,
        CompromisedLiver = 137,
        */
    }
    public enum PerkQuality
    {
        Negative = 0,
        Positive = 1,
        Neutral = 2,
    }
    #endregion

    // Town Related
    #region
    public enum TownActivity
    {
        None = 0,
        BedRest = 1,
        Surgery = 2,
        Therapy = 3,
    }
    #endregion

    // Stress + Combat + Injuries
    #region
    public enum InjurySeverity
    {
        None = 0,
        Mild = 1,
        Severe = 2,
    }
    public enum InjuryType
    {
        None = 0,
        Sharp = 1,
        Blunt = 2,
        Magical = 3,
        Mental = 4,
    }
    public enum HitRollResult
    {
        Hit = 0,
        Miss = 1,
        //Dodge = 2,
        // in future, maybe we have block, parry, etc
    }
    public enum StressEventType
    {
        EnemyMovedIntoMelee = 0,
        EnemyMovedBehindMe = 1,
        HealthLost = 2,
        InjuryGained = 3,
        AllyLosesHealth = 4,
        AllyInjured = 5,
        AllyKilled = 6,
        AllyShattered = 7,
        AllyMoraleStateWorsened = 12,
        AllyMoraleStateImproved = 13,
        EnemyKilled = 8,
        EnemyInjured = 9,
        LandedCriticalStrike = 10,
        DebuffApplied = 11,
    }
    public enum StressState
    {
        None = 0,
        Confident = 1,
        Nervous = 2,
        Wavering = 3,
        Panicking = 4,
        Shattered = 5,
    }
    #endregion

    // UI + Visual Events
    #region
    public enum TextColor
    {
        None = 0,
        White = 1,
        BlueNumber = 2,
        KeyWordYellow = 3,
        PhysicalBrown = 4,
        MagicPurple = 9,
        FireRed = 5,
        ShadowPurple = 6,
        AirBlue = 7,
        PoisonGreen = 8,
        LightRed = 10,
        LightGreen = 12,
        RareTextBlue = 11,
    }
    public enum QueuePosition
    {
        Front = 0,
        Back = 1,
    }

    #endregion

    // Animation Event Data
    #region
    public enum AnimationEventType
    {
        None = 0,
        Movement = 1,
        CharacterAnimation = 2,
        ParticleEffect = 3,
        CameraShake = 4,
        SoundEffect = 5,
        ScreenOverlay = 7,
        Delay = 6,

    }
    public enum ScreenOverlayType
    {
        None = 0,
        EdgeOverlay = 1,
    }
    public enum ScreenOverlayColor
    {
        White = 0,
        Fire = 1,
        Purple = 2,
        PoisonGreen = 3,
    }
    public enum MovementAnimEvent
    {
        None = 0,
        MoveTowardsTarget = 1,
        MoveToCentre = 2,
        MoveToMyNode = 3,
    }
    public enum CharacterAnimation
    {
        None = 0,
        MeleeAttack = 1,
        AoeMeleeAttack = 5,
        ShootBow = 2,
        ShootProjectileWithAttackAnim = 3,
        ShootProjectile = 7,
        Skill = 4,
        Resurrect = 6,
    }
    public enum ParticleEffect
    {
        None = 0,
        GeneralBuff = 14,
        GeneralDebuff = 15,
        HealEffect = 24,
        ApplyBurning = 16,
        ApplyPoisoned = 17,
        SmokeExplosion = 25,
        GainOverload = 18,
        LightningExplosion = 1,
        FireExplosion = 4,
        PoisonExplosion = 5,
        ShadowExplosion = 6,
        FrostExplosion = 7,
        BloodExplosion = 2,
        GhostExplosionPurple = 22,
        ConfettiExplosionRainbow = 23,
        SmallMeleeImpact = 21,
        AoeMeleeArc = 3,
        FireNova = 8,
        PoisonNova = 9,
        ShadowNova = 10,
        LightningNova = 11,
        FrostNova = 12,
        HolyNova = 13,
        RitualCircleYellow = 19,
        RitualCirclePurple = 20,
        ToxicRain = 26,

    }
    public enum ProjectileFired
    {
        None = 4,
        Arrow = 0,
        FireBall1 = 1,
        ShadowBall1 = 2,
        PoisonBall1 = 3,
        FrostBall1 = 6,
        LightningBall1 = 7,
        HolyBall1 = 8,
        FireMeteor = 9,
        Javelin = 10,

    }
    public enum ProjectileStartPosition
    {
        Shooter = 0,
        SkyCentreOffScreen = 1,
        AboveTargetOffScreen
    }
    public enum CameraShakeType
    {
        None = 0,
        Small = 1,
        Medium = 2,
        Large = 3,
    }
    public enum CreateOnCharacter
    {
        None,
        Self,
        Target,
        All,
        AllAllies,
        AllEnemies,
    }
    #endregion

    // UCM + Model Related
    #region
    public enum CharacterModelSize
    {
        Small = 1,
        Normal = 0,
        Large = 2,
        Massive = 3,
    }
    #endregion

    // Journey + Encounter Related
    #region
    public enum EncounterType
    {
        None = 0,
        BasicEnemy = 1,
        EliteEnemy = 2,
        BossEnemy = 3,
        DraftEvent = 5,
        CampSite = 6,
        Story = 8,
        TreasureRoom = 9,
        MysteryCombat = 10,
        Town = 11,
        Shop = 12,
    }
    public enum CombatDifficulty
    {
        Basic = 0,
        Elite = 1,
        Boss = 2,
    }
    public enum SaveCheckPoint
    {
        None = 0,
        CombatStart = 1,
        CombatEnd = 2,
        Town = 3,
    }
    #endregion

    // Misc
    #region
    public enum CombatGameState
    {
        None = 0,
        CombatInactive = 1,
        CombatActive = 2,
    }
    #endregion


}
