using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventProgress : MonoBehaviour
{
    [SerializeField] private string changeText;
    private List<Enemy> enemies = new List<Enemy>();
    private DestinationUI destinationUI;

    private void Start()
    {
        destinationUI = DestinationUI.Instance;

        Enemy[] e = GetComponentsInChildren<Enemy>();

        foreach (Enemy enemy in e)
        enemies.Add(enemy);
    }

    private void Update()
    {
        if (enemies.Count == 0)
        {
            destinationUI.SetDestinationText(changeText);
        }
    }

    public void DestroyEnemy(Enemy enemy)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemy == enemies[i])
            {
                enemies.RemoveAt(i);
            }
        }
    }

}
