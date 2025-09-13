using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public bool isPlayer;     // индекс в сетке (строка, колонка)
    public Vector3 worldPos;     // центр клетки в мире
    public SpriteRenderer sprite;// визуальный спрайт клетки
    public float size;

    public Color enemyColor;
    public Color playerColor;

    void Start()
    {
        //index = idx;
        worldPos = transform.position;
        //size = size;


        sprite = GetComponent<SpriteRenderer>();




    }

    // Проверка, находится ли точка внутри клетки
    public bool Contains(Vector3 worldPoint)
    {
        float half = size / 2f;
        return (worldPoint.x >= worldPos.x - half && worldPoint.x < worldPos.x + half &&
                worldPoint.z >= worldPos.z - half && worldPoint.z < worldPos.z + half);
    }

    public void Capture(bool player)
    {
        if (player)
        {
            sprite.color = playerColor;
        }
        else
        {
            sprite.color = enemyColor;
        }

        isPlayer = player;
    }
}
