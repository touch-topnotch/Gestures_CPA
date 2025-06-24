namespace Scripts.Static.Definitions
{
    public enum WeaponState: byte
    {
        NONE,
        Initialized,
        Casting,
        Cancelled,
        Deactivated,
        Activated,
        Destroyed
    }

    public enum WeaponClass: ushort
    {
        Melee,
        Magic,
        Range,
        Custom
    }
}