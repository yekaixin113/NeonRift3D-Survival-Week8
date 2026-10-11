using Godot;
using System;

// SURVIVAL S2: Presentation only; Game3D supplies all values and the result.
public partial class SurvivalHud : CanvasLayer
{
    private Label _status = null!;
    public override void _Ready()
    {
        _status = new Label { Position = new Vector2(20, 16) };
        _status.AddThemeFontSizeOverride("font_size", 24);
        _status.AddThemeColorOverride("font_color", Colors.White);
        _status.AddThemeColorOverride("font_shadow_color", Colors.Black);
        _status.AddThemeConstantOverride("shadow_offset_x", 2);
        _status.AddThemeConstantOverride("shadow_offset_y", 2);
        AddChild(_status);
    }

    public void Refresh(SurvivalRules rules, float health, int kills, int score,
        int enemies, string result)
    {
        _status.Text = $"SURVIVAL | {rules.Remaining:F1}s remaining | HP {health:0}\n"
            + $"Pressure {rules.PhaseIndex + 1}/6 | Enemies {enemies}/{rules.Current.EnemyCap}"
            + $" | Kills {kills} | Score {score}\n"
            + (result.Length == 0
                ? "Survive 60 seconds. WASD: move | Mouse: aim / fire"
                : result + " | Press R to restart");
    }
}
