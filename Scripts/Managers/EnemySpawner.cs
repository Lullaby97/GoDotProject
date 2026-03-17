using Godot;
using System;

/// <summary>
/// 刷怪笼：按固定间隔在玩家周围屏幕外的环形区域生成敌人。
/// 挂载为 World 场景下的 Node2D 节点。
/// </summary>
public partial class EnemySpawner : Node2D
{
    /// 敌人预制场景，在编辑器中拖入 enemy.tscn
    [Export]
    public PackedScene EnemyScene { get; set; }

    /// 刷怪间隔（秒）
    [Export]
    public float SpawnInterval { get; set; } = 1.0f;

    /// 生成半径（像素），需大于半屏对角线以确保在屏幕外刷出
    [Export]
    public float SpawnRadius { get; set; } = 1200.0f;

    private float _timer;

    public override void _Process(double delta)
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

        // 在玩家周围 SpawnRadius 距离的圆环上随机取一个点
        float angle = GD.Randf() * Mathf.Tau;
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * SpawnRadius;
        Vector2 spawnPosition = player.GlobalPosition + offset;

        Node2D enemy = EnemyScene.Instantiate<Node2D>();
        AddChild(enemy);
        enemy.GlobalPosition = spawnPosition;
    }
}
