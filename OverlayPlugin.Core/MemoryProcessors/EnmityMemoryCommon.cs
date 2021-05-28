using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainbowMage.OverlayPlugin.MemoryProcessors
{
    public enum ObjectType : byte
    {
        Unknown = 0x00,
        PC = 0x01,
        Monster = 0x02,
        NPC = 0x03,
        Aetheryte = 0x05,
        Gathering = 0x06,
        Minion = 0x09
    }

    /// <summary>An unsigned byte offset by 0x94 from the combatant's data that describes its status.</summary>
    public enum ObjectStatus : byte
    {
        /// <summary>Indicates that a targetable PC, NPC, monster, minion, or object is at an effective distance of 97 or less from the player.</summary>
        // A dead PC keeps status 191.
        NormalActorStatus = 191,

        /// <summary>Indicates that a targetable pet, chocobo, or a monster belonging to another one (add) is in the vicinity.</summary>
        // It hasn't been tested, but it probably works off the 97 effective distance as well.
        // While doing some tests in Titan (Extreme), all of the targetable adds that spawned had this status (jails, heart, bombs, adds).
        // What this could mean is that in order to finish an encounter, all actors with status 191 must die.
        NormalSubActorStatus = 190,

        /// <summary>Indicates that a previously targetable actor or sub-actor is now untargetable.</summary>
        Untargetable = 189,

        /// <summary>Indicates an untargetable, invisible, or a dead actor or sub-actor.</summary>
        // This includes the invisible "helper" boss actors in instances, and enemies that show up on the enmity list that are never targetable.
        // When an enemy's HP reaches 0, their nameplates grays out and then disappears. The status is only updated to 188 after the nameplate's gone.
        // It is not fully known whether an invisible actor that loads on the MobArray that becomes targetable later on will have status 188 or 189.
        // It is likely that they'll have status 189. TEA could be a great example to confirm this as all bosses load from the get-go.
        Uninteractable = 188

        /* More statuses do exist, and a lot of them are not fully tested or understood:
         * Status 47 indicates that an actor is updating from status 191 to 175.
         * Status 175 indicates that an actor is at an effective distance of between 98 and 109 (inclusive).
         * A combatant object doesn't get updated anymore by the game once the effective distance is at 110 or greater,
         * going over that distance and then walking back to ~108 effective distance gets the status set to 171,
         * which could signify that the (targetable?) actor has just loaded in and is pending an update on its status.
         * Watching an unskippable cutscene sets the status to 255.
         * Watching a skippable cuscene from the inn sets the status to 111.
         * And more.
         */
    }

    /// <summary>An unsigned byte offset by 0x105 from the combatant's data that describes the 3D model visibility.</summary>
    // In a fight like O1N, where the main boss (Alte Roite) momentarily disappears and appears at the edge of the map to do his knockback,
    // the ModelStatus gets set to 64 (Hidden), but the ObjectStatus remains at 191. The fact that the ObjectStatus doesn't change to 189 could mean that 
    // Alte Roite is still accepting damage in this invisibility period, and that no ghosting will occur. Checking the logs shows that
    // there is indeed no presence of log line 34 that toggles the targetability.
    // Not a lot of testing has been done to this field, but doing Titan (Extreme) confirms the behavior seen in O1N:
    // When Titan's model disappears from the jumps, the field gets set to 64. When the model reappears, the field gets set back to 0 (Visible).
    // And when Titan's Heart spawns, Titan's ModelStatus remains at 0 (while its ObjectStatus is set to 189).
    public enum ModelStatus
    {
        // The status of the invisible "helper" actors is set to 0 as well.
        Visible = 0,
        Hidden = 64

        /* This field definitely has more statuses:
         * Status 8 that functions the same way as with ObjectStatus 171.
         */
    }

    [Serializable]
    public class Combatant
    {
        public uint ID;
        public uint OwnerID;
        public ObjectType Type;
        public ObjectStatus Status;
        public ModelStatus ModelStatus;
        public uint TargetID;

        public byte Job;
        public string Name;

        public int CurrentHP;
        public int MaxHP;

        public Single PosX;
        public Single PosY;
        public Single PosZ;
        public Single Rotation;

        public string Distance;
        public byte EffectiveDistance;

        public List<EffectEntry> Effects;

        public string DistanceString(Combatant target)
        {
            var distanceX = (float)Math.Abs(PosX - target.PosX);
            var distanceY = (float)Math.Abs(PosY - target.PosY);
            var distanceZ = (float)Math.Abs(PosZ - target.PosZ);
            var distance = (float)Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY) + (distanceZ * distanceZ));
            return distance.ToString("0.00");
        }
    }

    [Serializable]
    public class EnmityEntry
    {
        public uint ID;
        public uint OwnerID;
        public string Name;
        public uint Enmity;
        public bool isMe;
        public int HateRate;
        public byte Job;
    }

    [Serializable]
    public class AggroEntry
    {
        public uint ID;
        public string Name;
        public int HateRate;
        public int Order;
        public bool isCurrentTarget;
        public bool isTargetable;

        public int CurrentHP;
        public int MaxHP;

        // Target of Enemy
        public EnmityEntry Target;

        // Effects
        public List<EffectEntry> Effects;
    }

    [Serializable]
    public class EffectEntry
    {
        public ushort BuffID;
        public ushort Stack;
        public float Timer;
        public uint ActorID;
        public bool isOwner;
    }

    public abstract class EnmityMemory
    {
        abstract public bool IsValid();
        abstract public Combatant GetTargetCombatant();
        abstract public Combatant GetSelfCombatant();
        abstract public Combatant GetFocusCombatant();
        abstract public Combatant GetHoverCombatant();
        abstract public List<Combatant> GetCombatantList();
        abstract public List<EnmityEntry> GetEnmityEntryList(List<Combatant> combatantList);
        abstract public unsafe List<AggroEntry> GetAggroList(List<Combatant> combatantList);
        abstract public bool GetInCombat();
    }
}
