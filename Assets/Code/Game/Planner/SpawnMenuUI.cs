using UnityEngine;

public class SpawnMenuUI : MonoBehaviour
{
    public SpawnManager spawnManager;
    public EntityData allyData;
    public EntityData neutralData;
    public EntityData enemyData;

    public void SpawnAlly()
    {
        spawnManager.SelectEntity(allyData, EntityThreat.Ally);
    }
    
    public void SpawnNeutral()
    {
        spawnManager.SelectEntity(neutralData, EntityThreat.Neutral);
    }

    public void SpawnEnemy()
    {
        spawnManager.SelectEntity(enemyData, EntityThreat.Enemy);
    }
}