// TEACHING GUIDE: Game3D is OUR game coordinator; Node3D and PackedScene are GODOT.
// GODOT API: _Ready, _PhysicsProcess, GD.Load, GetNode, Instantiate, AddChild.
// SURVIVAL S3: survive 60 seconds; six time-based pressure tiers.
// Rules live in SurvivalRules; combat classes and score values are unchanged.
// WORLD UNITS: speeds are Godot 3D units per second, not pixels per second.

using Godot;
using System;

public partial class Game3D : Node3D
{
	// SURVIVAL S3: keep coordination here, balance in a plain C# object.
	private SurvivalRules _survival = new();
	private SurvivalHud _hud = null!;
	private string _result = "";
	private int _phaseSpawnCount;
	private int _lastPhase;

	private readonly RandomNumberGenerator _rng =
		new();

	private PackedScene _chaserScene = null!;

	private PackedScene _strikerScene = null!;

	private PackedScene _bulletScene = null!;

	private Node3D _entities = null!;

	private Player3D _player = null!;


	private int _kills;

	private int _score;

	private bool _running;

	// GODOT lifecycle callback, called when this scene enters the tree.
	public override void _Ready()
	{
		Engine.TimeScale = 1.0;

		_rng.Randomize();

		_chaserScene =
			GD.Load<PackedScene>(
				"res://Scenes3D/ChaserEnemy3D.tscn");

		_strikerScene =
			GD.Load<PackedScene>(
				"res://Scenes3D/StrikerEnemy3D.tscn");

		_bulletScene =
			GD.Load<PackedScene>(
				"res://Scenes3D/Bullet3D.tscn");

		_entities =
			GetNode<Node3D>("Entities");

		_player =
			GetNode<Player3D>("Player");

		// OUR signal-based design: actors request actions; Game3D owns spawning.
		_player.ShotRequested +=
			SpawnPlayerBullet;

		_player.Died +=
			EndWithDefeat;

		_hud = new SurvivalHud();
		AddChild(_hud);

		StartGame();
	}

	// SURVIVAL S4: use simulation seconds without the old 0.033-second truncation.
    public override void _PhysicsProcess(double deltaValue)
    {
        if (!_running) return;
        bool spawnDue = _survival.Advance(deltaValue);
        if (_survival.Complete)
        {
            FinishGame("VICTORY - Survived 60 seconds!");
            return;
        }
        if (_lastPhase != _survival.PhaseIndex)
        {
            _lastPhase = _survival.PhaseIndex;
            _phaseSpawnCount = 0;
        }
        if (spawnDue) SpawnBatch();
        RefreshHud();
    }

    // Restart is available only after a result; no input-map/scene edit needed.
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (!_running && @event is InputEventKey key
            && key.Pressed && !key.Echo && key.Keycode == Key.R)
            GetTree().ReloadCurrentScene();
    }

    private void StartGame()
    {
        ClearEntities();
        _kills = 0;
        _score = 0;
        _survival = new SurvivalRules();
        _phaseSpawnCount = 0;
        _lastPhase = 0;
        _result = "";
        _running = true;
        _player.ResetPlayer(new Vector3(0f, 0.5f, 0f));
        RefreshHud();
    }

    // SURVIVAL S5: caps count living enemies, never bullets or queued deaths.
    private int CountEnemies(bool strikersOnly = false)
    {
        int count = 0;
        foreach (Node child in _entities.GetChildren())
            if (child is Enemy3D enemy && !enemy.IsQueuedForDeletion()
                && enemy.Health > 0f && (!strikersOnly || enemy is StrikerEnemy3D))
                count++;
        return count;
    }

    private void SpawnBatch()
    {
        SurvivalRules.Phase phase = _survival.Current;
        for (int i = 0; i < phase.BatchSize; i++)
        {
            if (CountEnemies() >= phase.EnemyCap) break;
            // Failed/blocked slots are discarded, not accumulated.
            if (!TryGetSpawnPosition(out Vector3 position)) continue;
            _phaseSpawnCount++;
            bool striker = phase.StrikerEvery > 0
                && _phaseSpawnCount % phase.StrikerEvery == 0
                && CountEnemies(true) < phase.StrikerCap;
            SpawnEnemy(striker ? _strikerScene : _chaserScene, position);
        }
    }

    private void SpawnEnemy(PackedScene scene, Vector3 position)
    {
        Enemy3D enemy = scene.Instantiate<Enemy3D>();
        _entities.AddChild(enemy);
        enemy.GlobalPosition = position;
        enemy.Configure(_player);
        enemy.Destroyed += OnEnemyDestroyed;
        enemy.ShotRequested += SpawnEnemyBullet;
    }

    // SURVIVAL S6: retain edge spawning; reject unfair/overlapping positions.
    private bool TryGetSpawnPosition(out Vector3 position)
    {
        for (int attempt = 0; attempt < 16; attempt++)
        {
            Vector3 candidate = RandomArenaEdgePosition();
            if (candidate.DistanceTo(_player.GlobalPosition)
                < SurvivalRules.MinimumPlayerDistance) continue;
            bool occupied = false;
            foreach (Node child in _entities.GetChildren())
            {
                if (child is Enemy3D enemy && !enemy.IsQueuedForDeletion()
                    && enemy.Health > 0f && candidate.DistanceTo(enemy.GlobalPosition)
                        < SurvivalRules.MinimumEnemySpacing)
                {
                    occupied = true;
                    break;
                }
            }
            if (!occupied) { position = candidate; return true; }
        }
        position = Vector3.Zero;
        return false;
    }

	private Vector3 RandomArenaEdgePosition()
	{
		// OUR arena spawn bounds (world units), slightly inside the walls.
		const float MaxX = 8.5f;
		const float MaxZ = 5.5f;

		int side =
			_rng.RandiRange(0, 3);

		if (side == 0)
		{
			return new Vector3(
				_rng.RandfRange(-MaxX, MaxX),
				0.5f,
				-MaxZ);
		}

		if (side == 1)
		{
			return new Vector3(
				MaxX,
				0.5f,
				_rng.RandfRange(-MaxZ, MaxZ));
		}

		if (side == 2)
		{
			return new Vector3(
				_rng.RandfRange(-MaxX, MaxX),
				0.5f,
				MaxZ);
		}

		return new Vector3(
			-MaxX,
			0.5f,
			_rng.RandfRange(-MaxZ, MaxZ));
	}

	private void SpawnPlayerBullet(
		Vector3 position,
		Vector3 direction)
	{
		// OUR PLAYER BULLET: friendly, 14 units/s, 24 HP damage.
		SpawnBullet(
			position,
			direction,
			false,
			14f,
			24f);
	}

	private void SpawnEnemyBullet(
		Vector3 position,
		Vector3 direction)
	{
		// OUR ENEMY BULLET: hostile, 8 units/s, 11 HP damage.
		SpawnBullet(
			position,
			direction,
			true,
			8f,
			11f);
	}

	private void SpawnBullet(
		Vector3 position,
		Vector3 direction,
		bool hostile,
		float speed,
		float damage)
	{
		Bullet3D bullet =
			_bulletScene.Instantiate<Bullet3D>();

		_entities.AddChild(bullet);

		bullet.GlobalPosition =
			position;

		bullet.Configure(
			direction,
			speed,
			damage,
			hostile);
	}

	// OUR scoring rule: each destroyed enemy counts as one kill and adds its points.
	private void OnEnemyDestroyed(int points)
	{
		if (!_running)
		{
			return;
		}

		_kills++;

		_score += points;

        // SURVIVAL S8: kills award score only; they never end a survival run.
        GD.Print($"Kills: {_kills} | Score: {_score}");
    }

    private void EndWithDefeat()
    {
        FinishGame("DEFEAT - Player defeated");
    }

    // SURVIVAL S9: one result path; freezing is added in the next debug iteration.
    private void FinishGame(string result)
    {
        if (!_running) return;
        _running = false;
        _result = result;
        _player.Active = false;
        GD.Print($"{result} | Time: {_survival.Elapsed:F2}s | Score: {_score}");
        RefreshHud();
    }

    private void RefreshHud()
    {
        _hud.Refresh(_survival, _player.Health, _kills, _score,
            CountEnemies(), _result);
    }

	private void ClearEntities()
	{
		foreach (Node child
			in _entities.GetChildren())
		{
			child.QueueFree();
		}
	}
}
