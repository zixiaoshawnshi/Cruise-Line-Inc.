using System;

namespace CruiseLineInc.Ship.Data
{
    /// <summary>
    /// Lightweight id wrappers keep references stable and type-safe.
    /// </summary>
    [Serializable]
    public readonly struct ZoneId : IEquatable<ZoneId>
    {
        public static readonly ZoneId Invalid = new ZoneId(0);

        public int Value { get; }

        public ZoneId(int value) => Value = value;

        public bool Equals(ZoneId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is ZoneId other && Equals(other);
        public override int GetHashCode() => Value;
        public static bool operator ==(ZoneId left, ZoneId right) => left.Equals(right);
        public static bool operator !=(ZoneId left, ZoneId right) => !(left == right);
        public override string ToString() => Value == 0 ? "ZoneId.Invalid" : $"ZoneId({Value})";
        public bool IsValid => Value != 0;
    }

    [Serializable]
    public readonly struct RoomId : IEquatable<RoomId>
    {
        public static readonly RoomId Invalid = new RoomId(0);

        public int Value { get; }

        public RoomId(int value) => Value = value;

        public bool Equals(RoomId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is RoomId other && Equals(other);
        public override int GetHashCode() => Value;
        public static bool operator ==(RoomId left, RoomId right) => left.Equals(right);
        public static bool operator !=(RoomId left, RoomId right) => !(left == right);
        public override string ToString() => Value == 0 ? "RoomId.Invalid" : $"RoomId({Value})";
        public bool IsValid => Value != 0;
    }

    [Serializable]
    public readonly struct FurnitureNodeId : IEquatable<FurnitureNodeId>
    {
        public static readonly FurnitureNodeId Invalid = new FurnitureNodeId(0);

        public int Value { get; }

        public FurnitureNodeId(int value) => Value = value;

        public bool Equals(FurnitureNodeId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is FurnitureNodeId other && Equals(other);
        public override int GetHashCode() => Value;
        public static bool operator ==(FurnitureNodeId left, FurnitureNodeId right) => left.Equals(right);
        public static bool operator !=(FurnitureNodeId left, FurnitureNodeId right) => !(left == right);
        public override string ToString() => Value == 0 ? "FurnitureNodeId.Invalid" : $"FurnitureNodeId({Value})";
        public bool IsValid => Value != 0;
    }

    [Serializable]
    public readonly struct PortalId : IEquatable<PortalId>
    {
        public static readonly PortalId Invalid = new PortalId(0);

        public int Value { get; }

        public PortalId(int value) => Value = value;

        public bool Equals(PortalId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is PortalId other && Equals(other);
        public override int GetHashCode() => Value;
        public static bool operator ==(PortalId left, PortalId right) => left.Equals(right);
        public static bool operator !=(PortalId left, PortalId right) => !(left == right);
        public override string ToString() => Value == 0 ? "PortalId.Invalid" : $"PortalId({Value})";
        public bool IsValid => Value != 0;
    }

    [Serializable]
    public readonly struct AgentId : IEquatable<AgentId>
    {
        public static readonly AgentId Invalid = new AgentId(0);

        public int Value { get; }

        public AgentId(int value) => Value = value;

        public bool Equals(AgentId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is AgentId other && Equals(other);
        public override int GetHashCode() => Value;
        public static bool operator ==(AgentId left, AgentId right) => left.Equals(right);
        public static bool operator !=(AgentId left, AgentId right) => !(left == right);
        public override string ToString() => Value == 0 ? "AgentId.Invalid" : $"AgentId({Value})";
        public bool IsValid => Value != 0;
    }

    [Serializable]
    public readonly struct RoomArchetypeId : IEquatable<RoomArchetypeId>
    {
        public static readonly RoomArchetypeId Invalid = new RoomArchetypeId(0);

        public int Value { get; }

        public RoomArchetypeId(int value) => Value = value;

        public bool Equals(RoomArchetypeId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is RoomArchetypeId other && Equals(other);
        public override int GetHashCode() => Value;
        public static bool operator ==(RoomArchetypeId left, RoomArchetypeId right) => left.Equals(right);
        public static bool operator !=(RoomArchetypeId left, RoomArchetypeId right) => !(left == right);
        public override string ToString() => Value == 0 ? "RoomArchetypeId.Invalid" : $"RoomArchetypeId({Value})";
        public bool IsValid => Value != 0;
    }

    /// <summary>
    /// Tile coordinate within the ship grid (deck, x, z).
    /// </summary>
    [Serializable]
    public readonly struct TileCoord : IEquatable<TileCoord>
    {
        public int Deck { get; }
        public int X { get; }
        public int Z { get; }

        public TileCoord(int deck, int x, int z)
        {
            Deck = deck;
            X = x;
            Z = z;
        }

        public bool Equals(TileCoord other) => Deck == other.Deck && X == other.X && Z == other.Z;
        public override bool Equals(object obj) => obj is TileCoord other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Deck, X, Z);
        public static bool operator ==(TileCoord left, TileCoord right) => left.Equals(right);
        public static bool operator !=(TileCoord left, TileCoord right) => !(left == right);
        public override string ToString() => $"TileCoord(Deck:{Deck}, X:{X}, Z:{Z})";
    }
}
