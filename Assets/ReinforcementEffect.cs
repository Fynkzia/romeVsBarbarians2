using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReinforcementEffect : MonoBehaviour
{
    public Transform[] points;

    public GameObject spawnFx;

    public GameObject pointsPlayerPrefab;
    public GameObject pointsEnemyPrefab;
    //public GameObject linePrefab;

    public GameObject playerArrow;
    public GameObject enemyArrow;

    public BattleSceneManager scene;
    public ArmyController reinforcementArmy;

    public Transform infoObject;
    public LookAtCamera infoLookAtCamera;
    [SerializeField] public TextMeshPro reinforcementArmyDistance;

    public GameObject enterObject;
    public Transform enterBar;
    public bool enterDelay;

    public float timeToEnter;
    public float enterTime;




    public int lineLength = 5;

    // Start is called before the first frame update


    // Update is called once per frame
    public void CreateEffect(ArmyController army, BattleSceneManager battleScene)
    {
        scene = battleScene;
        reinforcementArmy = army;

        if (army.isPlayer)
        {
            playerArrow.SetActive(true);
        }
        else
        {
            enemyArrow.SetActive(true);
        }

        for (int i = 0; i < army.squadList.Count; i++)
        {
            GameObject newPoint = null;

            if (army.isPlayer)
            {
                newPoint = Instantiate(pointsPlayerPrefab, points[i].transform.position, points[i].transform.rotation, transform);
                //LineRenderer newline = Instantiate(linePrefab, points[i].transform.position, points[i].transform.rotation, transform).GetComponent<LineRenderer>();

            }
            else
            {
                newPoint = Instantiate(pointsEnemyPrefab, points[i].transform.position, points[i].transform.rotation, transform);
            }


            //Vector3 norm = Vector3.Normalize(reinforcementArmy.transform.position - SceneLoader.Instance.mapBattleControllers[scene.sceneIndex].transform.position);

            
        }



        // Направление от объекта к камере
        Vector3 directionToCamera = scene.gameCamera.transform.position - infoObject.transform.position;

        // Проецируем направление на плоскость XZ (убираем компонент по Y)
        directionToCamera.y = 0;

        // Если длина направления нулевая, избегаем ошибок

        // Устанавливаем поворот объекта так, чтобы он смотрел на камеру
        //infoObject.transform.rotation = Quaternion.LookRotation(directionToCamera);

        infoLookAtCamera.InitBattleInfo(scene.gameCamera, scene.cameraController);

        UpdateEffect();

    }


    public void UpdateEffect()
    {
        if (enterDelay)
        {

        }
        else
        {
            if (reinforcementArmy != null)
            {


                reinforcementArmyDistance.text = "" + (int)Vector3.Distance(reinforcementArmy.transform.position, SceneLoader.Instance.mapBattleControllers[scene.sceneIndex].transform.position) * 8 + "km";
            }
        }
    }

    public void SpawnFx(int pointId)
    {
        Instantiate(spawnFx, points[pointId].transform.position, points[pointId].transform.rotation, transform);
    }

    public void EnterToBattleInfo(float delay)
    {
        reinforcementArmyDistance.gameObject.SetActive(false);
        enterObject.SetActive(true);
        enterDelay = true;

        timeToEnter = delay;


    }

    private void Update()
    {
        if (enterDelay)
        {
            enterTime += Time.deltaTime;

            enterBar.localScale = new Vector3(enterTime / timeToEnter, 1, 1);

            if (enterTime > timeToEnter)
            {
                {
                    enterDelay = false;

                    reinforcementArmyDistance.gameObject.SetActive(false);
                    enterObject.SetActive(false);
                }
            }
        }
    }
}
