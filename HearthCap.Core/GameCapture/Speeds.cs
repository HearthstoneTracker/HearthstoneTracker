namespace HearthCap.Core.GameCapture
{
    public enum Speeds : int
    {
        Slow = 1000 / 5, // 200 ms
        Medium = 1000 / 10, // 100 ms
        Fast = 1000 / 20, // 50 ms
        VeryFast = 1000 / 30, // 33.3 ms
        InsanelyFast = 1000 / 60, // 16.6 ms
        NoDelay = 0,
        Default = VeryFast
    }
}