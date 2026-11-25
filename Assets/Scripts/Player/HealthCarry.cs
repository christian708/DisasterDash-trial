public static class HealthCarry
{
    // -1 = nothing carried over, start with full hearts
    public static int savedHealth = -1;

    public static void Reset()
    {
        savedHealth = -1;
    }
}