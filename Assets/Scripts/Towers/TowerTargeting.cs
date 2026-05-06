namespace SystemBreach.Towers
{
    // Focus modes as defined in GDD section 4.5
    public enum FocusMode
    {
        Nearest,   // closest to the computer (highest path progress)
        Farthest,  // farthest from computer (lowest path progress / just entered)
        MostHP,    // highest current HP
        LeastHP,   // lowest current HP (secure the kill)
        Fastest    // highest move speed
    }
}
