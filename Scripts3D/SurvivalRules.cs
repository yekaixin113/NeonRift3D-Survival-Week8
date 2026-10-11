using System;

// SURVIVAL S1: Pure C# timing and balance; no Godot types or combat behaviour.
public sealed class SurvivalRules
{
    public const double DurationSeconds = 60.0;
    public const float MinimumPlayerDistance = 5f;
    public const float MinimumEnemySpacing = 1.4f;

    public readonly record struct Phase(double Interval, int BatchSize,
        int EnemyCap, int StrikerCap, int StrikerEvery);

    // Each row lasts ten seconds. Caps bound pressure even if nothing is killed.
    private static readonly Phase[] Phases =
    {
        new(2.5, 1, 6, 0, 0),
        new(2.0, 1, 8, 1, 5),
        new(2.0, 2, 10, 2, 4),
        new(1.5, 2, 12, 3, 4),
        new(1.25, 3, 14, 4, 3),
        new(1.0, 3, 16, 4, 3)
    };

    public double Elapsed { get; private set; }
    public double Remaining => Math.Max(0, DurationSeconds - Elapsed);
    public bool Complete => Elapsed >= DurationSeconds;
    public int PhaseIndex => Math.Min(5, (int)(Elapsed / 10.0));
    public Phase Current => Phases[PhaseIndex];
    private double _nextSpawn = 1.5;

    // At most one batch per tick: missed batches are never repaid as a burst.
    public bool Advance(double delta)
    {
        if (Complete || !double.IsFinite(delta) || delta <= 0) return false;
        Elapsed = Math.Min(DurationSeconds, Elapsed + delta);
        if (Complete) return false; // No spawn at or after the victory boundary.
        // A phase change only changes the parameters used after the next scheduled
        // spawn. It never starts an immediate replacement wave or touches enemies
        // that are already alive.
        if (Elapsed + 1e-9 < _nextSpawn) return false;
        _nextSpawn = Elapsed + Current.Interval;
        return true;
    }
}
