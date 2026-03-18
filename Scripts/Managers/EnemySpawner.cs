using Godot;
using System;

/// <summary>
/// 刷怪笼：按固定间隔在玩家周围的环形安全区域外生成敌人。
/// 采用"先设坐标再入树"原则，确保敌人进入场景树时已在正确位置，
/// 杜绝物理体在 (0,0) 闪现一帧触发碰撞的问题。
/// </summary>
public partial class EnemySpawner : Node2D
{
    /// 敌人预制场景，在编辑器中拖入 enemy.tscn
    [Export]
    public PackedScene EnemyScene { get; set; }

    /// 刷怪间隔（秒）
    [Export]
    public float SpawnInterval { get; set; } = 1.0f;

    /// 最小生成距离（像素），保证怪物不会出现在玩家视野中心
    [Export]
    public float MinSpawnRadius { get; set; } = 800.0f;

    /// 最大生成距离（像素），需大于半屏对角线以确保在屏幕外刷出
    [Export]
    public float MaxSpawnRadius { get; set; } = 1200.0f;

    private float _timer;

    public override void _Ready()
    {
        // 首次刷怪前留出一个完整间隔，防止第一帧就生成敌人
        _timer = SpawnInterval;
    }

    public override void _PhysicsProcess(double delta)
    {
        _timer -= (float)delta;

        if (_timer <= 0.0f)
        {
            _timer = SpawnInterval;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (EnemyScene == null)
        {
            GD.PrintErr("[EnemySpawner] EnemyScene 未设置，无法生成敌人。");
            return;
        }

        Node2D player = GetTree().GetFirstNodeInGroup("Player") as Node2D;

        if (player == null)
            return;

        // 在 [MinSpawnRadius, MaxSpawnRadius] 环形区域内随机取点
        float angle = GD.Randf() * Mathf.Tau;
        float dist = MinSpawnRadius + GD.Randf() * (MaxSpawnRadius - MinSpawnRadius);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;
        Vector2 spawnPosition = player.GlobalPosition + offset;

        // 【先设坐标再入树】
        // 节点在 AddChild 瞬间会被物理引擎注册，如果此时坐标还是默认值 (0,0)，
        // 物理体会在原点闪现一帧，可能与玩家 Hurtbox 产生错误碰撞。
        // 先设好 Position 再 AddChild，让敌人进入场景树时就在正确位置。
        Node2D enemy = EnemyScene.Instantiate<Node2D>();
        enemy.Position = spawnPosition;
        GetTree().CurrentScene.AddChild(enemy);

        GD.Print($"[EnemySpawner] 怪物生成于安全距离外: {dist:F0}px (角度: {Mathf.RadToDeg(angle):F0}°)");
    }
}
