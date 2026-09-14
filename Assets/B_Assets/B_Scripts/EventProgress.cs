using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventProgress : MonoBehaviour
{
    //[SerializeField] private string startText;
    [SerializeField] private string changeText;
    [SerializeField] private GameObject warpPoint;
    private List<Enemy> enemies = new List<Enemy>();
    private WarpPointerController warpPointerController;
    private DestinationUI destinationUI;

    private void Start()
    {
        destinationUI = DestinationUI.Instance;

        Enemy[] e = GetComponentsInChildren<Enemy>();

        foreach (Enemy enemy in e)
        {
            enemies.Add(enemy);
        }
    }

    private void OnEnable()
    {
        if (destinationUI != null)
        {
            if (warpPointerController == null)
            {
                warpPointerController = GameObject.FindWithTag("Player").GetComponentInChildren<WarpPointerController>();
            }
            //destinationUI.SetDestinationText(startText);
            warpPointerController.DestroyWarpPoint();
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

        if (enemies.Count == 0)
        {
            destinationUI.SetDestinationText(changeText);

            if (warpPoint != null)
            {
                if (warpPointerController == null)
                {
                    warpPointerController = GameObject.FindWithTag("Player").GetComponentInChildren<WarpPointerController>();
                }
                warpPointerController.SetWarpPoint(warpPoint);
            }
        }
    }

}
