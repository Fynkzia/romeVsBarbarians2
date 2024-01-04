using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{

[SerializeField] public List<SquadController> actionQueueArray;

[SerializeField] public List<SquadController> allEnemiesList;
[SerializeField] public List<SquadController> nearEnemyList;
[SerializeField] public float nearEnemyPower;
[SerializeField] public float nearEnemyType0;
[SerializeField] public float nearEnemyType1;
[SerializeField] public float nearEnemyType2;

[SerializeField] public List<SquadController> nearPlayerList;
[SerializeField] public float nearPlayerPower;
[SerializeField] public float nearPlayerType0;
[SerializeField] public float nearPlayerType1;
[SerializeField] public float nearPlayerType2;

 [SerializeField] List<SquadController> playerNearSquads = new List<SquadController>();
[SerializeField]  List<SquadController> playerFarSquads = new List<SquadController>();

[SerializeField]  List<Vector3> battlePoints = new List<Vector3>();
[SerializeField]  List<Vector3> helpPoints = new List<Vector3>();

[SerializeField] public float timeAction;
[SerializeField] public int actions;
 [SerializeField] private LayerMask playerUnitLayer ;

  [SerializeField] private LayerMask enemyUnitLayer ;

 [SerializeField] private GameObject drawingPrefab;

   [Space(10)]
[SerializeField] public int horseCountCoef;

[Header("AI Settings")]
    [Space(10)]
    
[SerializeField] public float difficulty;
[SerializeField] public float timeToGetActions;
[SerializeField] public float farRadius;
[SerializeField] public float nearRadius;
[SerializeField] public float nearEnemyRadius;


[Header("Suport waves Settings")]
    [Space(10)]

[SerializeField] public Transform enemyObject;

[SerializeField] public int waveNow;
[SerializeField] public int waveMax;
[SerializeField] public float waveTimer;

 [Space(10)]
[SerializeField] public float[] waveTiming;
[SerializeField] public SquadController[] wave0;
[SerializeField] public SquadController[] wave1;
[SerializeField] public SquadController[] wave2;
[SerializeField] public SquadController[] wave3;
[SerializeField] public SquadController[] wave4;
[SerializeField] public SquadController[] wave5;


[SerializeField] public Transform[] spawnPoints;





    // Start is called before the first frame update
    void Start()
    {
       SetQueue();
        
        if(wave1.Length > 0){
            waveMax ++;
        }
        if(wave2.Length > 0){
            waveMax ++;
        }
        if(wave3.Length > 0){
            waveMax ++;
        }
        if(wave4.Length > 0){
            waveMax ++;
        }
        if(wave5.Length > 0){
            waveMax ++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeAction += Time.fixedDeltaTime;
        

        if(timeAction > timeToGetActions){
            if(actionQueueArray.Count > 0){
                if(actionQueueArray[actions] != null){
                    AiAction(actionQueueArray[actions]);
           
                    if(actionQueueArray.Count > 0){
                        actionQueueArray.RemoveAt(actions);
                    }
                }
            }

        //actions ++;
        timeAction = 0;

        if(actionQueueArray.Count == 0){
        SetQueue();
        FindHelpPoint ();
        // actions = 0;
        }

        }

        if(waveNow <= waveMax){
             waveTimer += Time.fixedDeltaTime;

            if(waveTimer > waveTiming[waveNow]){
                SpawnWave(waveNow);

                waveNow ++;
                waveTimer = 0;

             }


        }
        

        
    }


     public void AiAction(SquadController squad) {

        if(squad.inBattle){

            NearEnemySquadList(squad);
            NearPlayerSquadList(squad);
            

            if(nearPlayerPower/nearEnemyPower > 2){
                 int random = Random.Range(0,10);

               if(random > 6){
                  Vector3 ceterOfSquads = new Vector3(0,0,0);
                    for (int i = 0; i < nearPlayerList.Count; i++) {
                        ceterOfSquads += nearPlayerList[i].transform.position;
                    }

                    ceterOfSquads /= nearPlayerList.Count;
                    SqauadRetreat(squad,ceterOfSquads);
                    squad.aiActionValue = -1;
                    squad.dangerAlert = true;
                    return;
               }
            }

            if(((int)squad.type == 0  && (int)squad.enemyController[0].type == 2)||
            ((int)squad.type == 1  && (int)squad.enemyController[0].type == 1)||
            ((int)squad.type == 2  && (int)squad.enemyController[0].type == 0)){
               int random = Random.Range(0,10);

               if(random > 8){
                 Vector3 ceterOfSquads = new Vector3(0,0,0);
                    for (int i = 0; i < nearPlayerList.Count; i++) {
                        ceterOfSquads += nearPlayerList[i].transform.position;
                    }

                    ceterOfSquads /= nearPlayerList.Count;
                    squad.aiActionValue = -1;
                    SqauadRetreat(squad,ceterOfSquads);
                    squad.dangerAlert = true;
                    return;
               }
            }
           

            if(squad.unitArray.Count < squad.enemyController[0].unitArray.Count/(2f - difficulty*0.05f)){
                 int random = Random.Range(0,10);

               if(random > 8){
                    Vector3 ceterOfSquads = new Vector3(0,0,0);
                    for (int i = 0; i < nearPlayerList.Count; i++) {
                        ceterOfSquads += nearPlayerList[i].transform.position;
                    }

                    ceterOfSquads /= nearPlayerList.Count;
                    SqauadRetreat(squad,ceterOfSquads);
                    squad.aiActionValue = -1;
                    squad.dangerAlert = true;
                    return;
               }
                

            }

            return;

        }else{

        Collider[] nearColliders = Physics.OverlapSphere(squad.transform.position, nearRadius, playerUnitLayer);
        Collider[] farColliders = Physics.OverlapSphere(squad.transform.position, farRadius, playerUnitLayer);

         playerNearSquads = new List<SquadController>();
         playerFarSquads = new List<SquadController>();

            for (int i = 0; i < nearColliders.Length; i++) {
                SquadController playerSquad = nearColliders[i].gameObject.GetComponent<SquadController>();

                if(!playerNearSquads.Contains(playerSquad) ){
                playerNearSquads.Add(playerSquad);
                }
                
    
            }   
            for (int i = 0; i < farColliders.Length; i++) {
                  SquadController playerSquad = farColliders[i].gameObject.GetComponent<SquadController>();

                if(!playerFarSquads.Contains(playerSquad) ){
                playerFarSquads.Add(playerSquad);
                }
                
    
            }  
 /// * выбираем из ближнего радиусв *
            if(playerNearSquads.Count > 0){
                // отсортировали по близости все отряды игрока
                List<SquadController> nearSquadList = playerNearSquads;
                List<SquadController> nearSquad = new List<SquadController>();

               for (int i = 0; i < playerNearSquads.Count; i++) {
                    SquadController nrSqvd = nearSquadList[0];
                    float dist = Vector3.Distance(nrSqvd.transform.position,squad.transform.position);

                    for (int j = 1; j < nearSquadList.Count; j++) {
                         float d = Vector3.Distance(nearSquadList[j].transform.position,squad.transform.position);
                            if( d < dist){
                                nrSqvd = nearSquadList[j];
                                dist = d;
                            }
                    }
                    nearSquadList.Remove(nrSqvd);
                    nearSquad.Add(nrSqvd);
                    
               }

                playerNearSquads = nearSquad;
                // отсортировали по близости все отряды игрока


          

            if( squad.aiActionValue <= 1){  // есть ли подходящий по типу отряд
                return;
            }

             for (int i = 0; i < playerNearSquads.Count; i++) { 
                if((int)squad.type == 0  && (int)playerNearSquads[i].type == 1){
                    if(UnitCountComparison(squad,playerNearSquads[i])){ // учитывает сложность
                        SquadMoveToSquad(squad,playerNearSquads[i]);
                        squad.aiActionValue = 1;
                        return;
                    }
                     if(NearSquadHelp(squad,playerNearSquads[i],(int)squad.type)){// завем на помошь +1 сквад
                       return;
                    }
                      SquadMoveToSquad(squad,playerNearSquads[i]); // если нет +1 отряда, все ровно атакуем, шо делать
                        squad.aiActionValue = 1;
                        return;
                    
                }

                if((int)squad.type == 1 && (int)playerNearSquads[i].type == 2){
                      if(UnitCountComparison(squad,playerNearSquads[i])){
                        SquadMoveToSquad(squad,playerNearSquads[i]);
                         squad.aiActionValue = 1;
                        return;
                    }
                     if(NearSquadHelp(squad,playerNearSquads[i],(int)squad.type)){// завем на помошь +1 сквад
                       return;
                    }
                    SquadMoveToSquad(squad,playerNearSquads[i]);// если нет +1 отряда, все ровно атакуем, шо делать
                         squad.aiActionValue = 1;
                        return;
                }

                if((int)squad.type == 2 && (int)playerNearSquads[i].type == 0){
                    if(UnitCountComparison(squad,playerNearSquads[i])){
                        SquadMoveToSquad(squad,playerNearSquads[i]);
                         squad.aiActionValue = 1;
                        return;
                    }
                    if(NearSquadHelp(squad,playerNearSquads[i],(int)squad.type)){// завем на помошь +1 сквад
                       return;
                    }
                     SquadMoveToSquad(squad,playerNearSquads[i]);// если нет +1 отряда, все ровно атакуем, шо делать
                         squad.aiActionValue = 1;
                        return;

                }

             }

            if( squad.aiActionValue <= 2){
                return;
            }
             
             for (int i = 0; i < playerNearSquads.Count; i++) {// есть тот же тип отряда?

                if((int)squad.type == (int)playerNearSquads[i].type){
                    if(UnitCountComparison(squad,playerNearSquads[i])){
                    SquadMoveToSquad(squad,playerNearSquads[i]);
                     squad.aiActionValue = 2;
                    return;
                    }
                    if(NearSquadHelp(squad,playerNearSquads[i],(int)squad.type)){// завем на помошь +1 сквад
                       return;
                    }
                     SquadMoveToSquad(squad,playerNearSquads[i]);// если нет +1 отряда, все ровно атакуем, шо делать
                         squad.aiActionValue = 2;
                        return;

                }

              }

            if( squad.aiActionValue <= 3){
                return;
            }

              for (int i = 0; i < playerNearSquads.Count; i++) {// может юнитов больше чем у нас?
               int enemyCountCoef = 0;
               int playerCountCoef = 0;

               if((int)squad.type == 2){
                enemyCountCoef = horseCountCoef;
               }
               if((int)playerNearSquads[i].type == 2){
                playerCountCoef = horseCountCoef;
               }

                if((enemyCountCoef + squad.unitArray.Count)/(1.5f + (difficulty*0.1f)) > (playerCountCoef+ playerNearSquads[i].unitArray.Count)){
                    SquadMoveToSquad(squad,playerNearSquads[i]);
                     squad.aiActionValue = 3;
                    return;
                }

              }

            if( squad.aiActionValue <= 4){ // если один вражеский отряд сражается, надо помочь
                return;
            }

              for (int i = 0; i < playerNearSquads.Count; i++) {
                                                                                                          /// возомжно заменить на функцию для сравенения ???
                if(playerNearSquads[i].inBattle && playerNearSquads[i].enemyController.Count == 1 && playerNearSquads[i].unitArray.Count > playerNearSquads[i].enemyController[0].unitArray.Count){
                    
                    SquadMoveToSquad(squad,playerNearSquads[i]);
                    squad.aiActionValue = 4;
                    return;
                }

              }
               float random = Random.Range(0,10);

               if(random < 5+(difficulty/2.1)){
                     NearEnemySquadList(squad);

                 for (int i = 0; i < nearEnemyList.Count; i++) {
                            if(nearEnemyList[i].dangerAlert){
                                SquadHalfWayToSquad(squad,nearEnemyList[i]);
                                  squad.aiActionValue = 4;
                                   return;

                            }
                        }

               }


             

            if( squad.aiActionValue <= 5){
                return;
            }

             NearEnemySquadList(squad);

            // отсупаем если один отряд вокруг враги
             if(playerNearSquads.Count >= nearEnemyList.Count && nearEnemyList.Count == 1){
                Vector3 ceterOfSquads = new Vector3(0,0,0);
                for (int i = 0; i < playerNearSquads.Count; i++) {
                    ceterOfSquads += playerNearSquads[i].transform.position;
                }

                ceterOfSquads /= playerNearSquads.Count;
                SqauadRetreat(squad,ceterOfSquads);
                 squad.aiActionValue = 5;
                 squad.dangerAlert = true;
                return;
            }
           
            if( squad.aiActionValue <= 6){
                return;
            }


            if((playerNearSquads.Count * (1f  + (difficulty*0.1f))) <= nearEnemyList.Count ){// если отрядов больше чем вражеских - мы атакуем
                
                            int nearCount = playerNearSquads.Count + (3  - (int)(difficulty*0.2f));

                            if(nearCount > nearEnemyList.Count){
                                nearCount = nearEnemyList.Count;
                            }
               
                for (int i = 0; i < nearCount; i++) {
                    if(nearEnemyList[i].aiActionValue > 6  && !nearEnemyList[i].inBattle){
                   SquadMoveToSquad(nearEnemyList[i],playerNearSquads[Random.Range(0,playerNearSquads.Count)]);
                    nearEnemyList[i].aiActionValue = 6;
                    if(actionQueueArray.Contains(nearEnemyList[i])){
                         actionQueueArray.Remove(nearEnemyList[i]);
                         }
                    }
                }
                return;
            }else{
                   if( squad.aiActionValue <= 7){
                    return;
                     }

                     squad.dangerAlert = true;

                     float rand = Random.Range(0,10+difficulty);

                     if(rand <= 3){ // бежим на помошь если есть к кому
                       for (int i = 0; i < nearEnemyList.Count; i++) {
                            if(nearEnemyList[i].dangerAlert){
                                SquadHalfWayToSquad(squad,nearEnemyList[i]);
                                  squad.aiActionValue = 16;
                                   return;

                            }
                        }
                     }

                     if(rand <= 6+difficulty){ // отсупаем
                            
                                Vector3 ceterOfSquads = new Vector3(0,0,0);
                                for (int i = 0; i < playerNearSquads.Count; i++) {
                                    ceterOfSquads += playerNearSquads[i].transform.position;
                                }
                                ceterOfSquads /= playerNearSquads.Count;

                                Vector3 squadOffset = new Vector3(0,0,0);
                                 for (int i = 0; i < nearEnemyList.Count; i++) {
                                    if(nearEnemyList[i].aiActionValue <= 7){
                                        
                                        if(i>0){
                                          squadOffset = nearEnemyList[i].transform.position - nearEnemyList[i-1].transform.position;
                                        }

                                    SqauadRetreat(nearEnemyList[i],ceterOfSquads +squadOffset);
                                    nearEnemyList[i].aiActionValue = 7;
                                        nearEnemyList[i].dangerAlert = true;
                                    if(actionQueueArray.Contains(nearEnemyList[i])){
                                        actionQueueArray.Remove(nearEnemyList[i]);
                                        }
                                    }
                                }
                                return;
                            
                     }

                   

                     

            }

          

            

 //--------------------------// * выбираем из дальнего радиуса * //--------------------------//
            }else if(playerFarSquads.Count > 0){
         
                List<SquadController> farSquadList = playerFarSquads;
                List<SquadController> farSquad = new List<SquadController>();

               for (int i = 0; i < playerFarSquads.Count; i++) {
                    SquadController nrSqvd = farSquadList[0];
                    float dist = Vector3.Distance(nrSqvd.transform.position,squad.transform.position);

                    for (int j = 1; j < farSquadList.Count; j++) {
                         float d = Vector3.Distance(farSquadList[j].transform.position,squad.transform.position);
                            if( d < dist){
                                nrSqvd = farSquadList[j];
                                dist = d;
                            }
                    }
                    farSquadList.Remove(nrSqvd);
                    farSquad.Add(nrSqvd);
                }
            playerFarSquads = farSquad;

            if( squad.aiActionValue <= 11){
                return;
            }

                if(squad.currentStamina/squad.maxStamina < 0.1 + (difficulty*0.05f)){ // отдых
                    return;

                }
                if(squad.currentMorale/squad.maxMorale < 0.1 + (difficulty*0.05f)){ // отдых
                    return;

                }
            

            if( squad.aiActionValue <= 12){
                return;
            }
            for (int i = 0; i < playerFarSquads.Count; i++) {// может нападаем на одного надамаженого отряда
               int enemyCountCoef = 0;
               int playerCountCoef = 0;

               if((int)squad.type == 2){
                enemyCountCoef = horseCountCoef;
               }
               if((int)playerFarSquads[i].type == 2){
                playerCountCoef = horseCountCoef;
               }

                if((enemyCountCoef + squad.unitArray.Count) <= (playerCountCoef+ playerFarSquads[i].unitArray.Count)/(1.5f + (difficulty*0.05f))){
                    NearPlayerSquadList(playerFarSquads[i]);
                    if(nearPlayerList.Count == 1){
                         SquadMoveToSquad(squad,playerFarSquads[i]);
                     squad.aiActionValue = 12;
                    return;
                    }
                   
                }

              }

               
            if( squad.aiActionValue <= 13){
                return;
            }

            NearEnemySquadList(squad);

            for (int i = 0; i < playerFarSquads.Count; i++) {
                NearPlayerSquadList(playerFarSquads[i]);

                   if(nearEnemyType0 >= nearPlayerType1 && nearEnemyType1 >= nearPlayerType2 && nearEnemyType2 >= nearPlayerType0) {
                        if(nearEnemyPower/nearPlayerPower > 0.7+(difficulty * 0.05f)){
                                for (int j = 0; j < nearEnemyList.Count; j++) {
                                     if(nearEnemyList[j].inBattle){
                                                SquadHalfWayToSquad(nearEnemyList[j],playerFarSquads[i]);
                                                nearEnemyList[j].aiActionValue = 13;

                                                if(actionQueueArray.Contains(nearEnemyList[j])){
                                                    actionQueueArray.Remove(nearEnemyList[j]);
                                                    }
                                        }      
                                     }
                                    return;
                                }


                                
                            

                        }
                    }
            

              
                
                
            if( squad.aiActionValue <= 14){
                return;
            }
              
               for (int i = 0; i < playerFarSquads.Count; i++) {
                NearPlayerSquadList(playerFarSquads[i]);

                   Debug.Log("nearEnemyPower/nearPlayerPower" + (nearEnemyPower/nearPlayerPower));

                        if(nearEnemyPower/nearPlayerPower > 1+(difficulty * 0.05f)){
                            int nearCount = nearPlayerList.Count + (int)(nearEnemyPower/nearPlayerPower);
                                if(nearCount > nearEnemyList.Count){
                                        nearCount = nearEnemyList.Count;
                                }
                                

                                for (int j = 0; j < nearCount; j++) {
                                            if(nearEnemyList[j].inBattle){
                                                SquadHalfWayToSquad(nearEnemyList[j],playerFarSquads[i]);
                                                nearEnemyList[j].aiActionValue = 14;

                                                if(actionQueueArray.Contains(nearEnemyList[j])){
                                                    actionQueueArray.Remove(nearEnemyList[j]);
                                                    }
                                                }
                                            }
                                            return;
                                }


                                
                            

                        }


            if( squad.aiActionValue <= 15){
                return;
            }


            if(nearEnemyList.Count  <= 1){
                     SetAllEnemiesList();

                 for (int i = 0; i < allEnemiesList.Count; i++) {
                   
                    if((int)allEnemiesList[i].type != (int)squad.type){
                         SquadMoveToSquad(squad,allEnemiesList[i]);
                         squad.aiActionValue = 1;
                         return;
                    }
                 }
                 
            }   

            if( squad.aiActionValue <= 16){
                return;
            }
                    int rand = Random.Range(0,10);

                    if(rand <= 3){
                        Vector3 ceterOfSquads = new Vector3(0,0,0);
                        for (int i = 0; i < playerFarSquads.Count; i++) {
                            ceterOfSquads += playerFarSquads[i].transform.position;
                        }

                        ceterOfSquads /= playerFarSquads.Count;

                        Vector3 dir = Vector3.Normalize(ceterOfSquads - squad.transform.position);
                        Debug.Log("dir " + dir);

                        SquadHalfWayToPoint(squad, squad.transform.position-(dir*10f));
                        squad.aiActionValue = 16;
                        squad.dangerAlert = true;

                        return;

                    }

                    if(rand <= 7){
                        
                        for (int i = 0; i < nearEnemyList.Count; i++) {
                            if(nearEnemyList[i].dangerAlert){
                                SquadHalfWayToSquad(squad,nearEnemyList[i]);
                                  squad.aiActionValue = 16;
                                   return;

                            }
                        }
                        

                    }
                    if(rand <= 10){

                         if(helpPoints.Count > 0){
                         SquadHalfWayToPoint(squad,helpPoints[Random.Range(0,helpPoints.Count)]);
                         squad.aiActionValue = 21;
                         return;
                }
                    }

                    
            
            
            //  if( squad.aiActionValue <= 16){
            //     return;
            // }
                
            //          Vector3 ceterOfSquads = new Vector3(0,0,0);
            //                     for (int i = 0; i < playerFarSquads.Count; i++) {
            //                         ceterOfSquads += playerFarSquads[i].transform.position;
            //                     }
            //                     ceterOfSquads /= playerFarSquads.Count;


                 

            //       Vector3 dir = Vector3.Normalize(ceterOfSquads - squad.transform.position);

            //       // SquadHalfWayToPoint(nearEnemyList[0],dir);

            //         Vector3 dirLeft = Quaternion.AngleAxis(-90, Vector3.up) * Vector3.Normalize(ceterOfSquads - squad.transform.position);
            //         Vector3 dirRight = Quaternion.AngleAxis(90, Vector3.up) * Vector3.Normalize(ceterOfSquads - squad.transform.position);

            //          for (int i = 1; i < nearEnemyList.Count; i++) {
            //             if(i % 2 == 0){
            //                 dirLeft *=5f;
            //                  SquadHalfWayToPoint(nearEnemyList[i],nearEnemyList[0].transform.position+dirLeft);

            //             }else{
            //                  dirRight *=5f;
            //                  SquadHalfWayToPoint(nearEnemyList[i],nearEnemyList[0].transform.position+dirRight);

            //             }
            //             nearEnemyList[i].aiActionValue = 16;
                           
            //          }

                        
                    
                    

            }else{

            if( squad.aiActionValue <= 20){
                return;
            }

                 SetAllEnemiesList();
                 NearEnemySquadList(squad);

              for (int i = 0; i < allEnemiesList.Count; i++) {
                            if(allEnemiesList[i].dangerAlert){
                             for (int j = 0; j < nearEnemyList.Count; j++) {
                                     if(nearEnemyList[j].inBattle){
                                                SquadHalfWayToSquad(nearEnemyList[j],allEnemiesList[i]);
                                                nearEnemyList[j].aiActionValue = 20;

                                                if(actionQueueArray.Contains(nearEnemyList[j])){
                                                    actionQueueArray.Remove(nearEnemyList[j]);
                                                    }
                                        }      
                                     }
                                   return;

                            }
                        }

                if(helpPoints.Count > 0){
                 SquadHalfWayToPoint(squad,helpPoints[Random.Range(0,helpPoints.Count)]);
                 squad.aiActionValue = 21;
                 return;
                }







        }

     }
     }


public void SqauadRetreat(SquadController enemySquad,Vector3 center) {

    Vector3 dir = Vector3.Normalize(center - enemySquad.transform.position);
    
    SquadHalfWayToPoint(enemySquad,center-dir*nearRadius*2f);

}


public bool NearSquadHelp(SquadController enemySquad,SquadController playerSquad,int enemyType) { // завем на помошь +1 сквад
 NearEnemySquadList(enemySquad);

    if(nearEnemyList.Count > 1){
        Debug.Log("NearSquadHelp");
                      
                for (int i = 0; i < nearEnemyList.Count; i++) { // берем на одно больше чем отрядов игрока
                if((int)nearEnemyList[i].type == enemyType && nearEnemyList[i] != enemySquad && !nearEnemyList[i].inBattle){
                    SquadMoveToSquad(nearEnemyList[i],playerSquad);
                    nearEnemyList[i].aiActionValue = 1;

                    SquadMoveToSquad(enemySquad,playerSquad); // что-то с рисованием линии
                    enemySquad.aiActionValue = 1;

                    if(actionQueueArray.Contains(nearEnemyList[i])){
                    actionQueueArray.Remove(nearEnemyList[i]);
                    }
                         Debug.Log("go v ataku bratya " + nearEnemyList[i].gameObject.name + " " + enemySquad.gameObject.name);
                    return true;

                }


                    
                }
                return false;
                
    }
    return false;
}

public void SquadMoveToSquad(SquadController enemySquad,SquadController playerSquad) {


    GameObject drawing = Instantiate(drawingPrefab);
   LineRenderer lineRenderer = drawing.GetComponent<LineRenderer>();                      
    enemySquad.lineRenderer = lineRenderer;

    Vector3 norm = Vector3.Normalize(playerSquad.transform.position - enemySquad.transform.position);
    
    lineRenderer.positionCount++;
    lineRenderer.SetPosition(lineRenderer.positionCount - 1, enemySquad.transform.position + norm);


    lineRenderer.positionCount++;
    lineRenderer.SetPosition(lineRenderer.positionCount - 1, (enemySquad.transform.position + new Vector3(Random.Range(-3f,3f),0f,Random.Range(-3f,3f)) + playerSquad.transform.position)/2f);

   

   // Debug.Log("enemySquad.transform.position "+ enemySquad.transform.position);
    int r = Random.Range(0,10);

    if(playerSquad.inBattle && r < difficulty){
         Vector3 normLeft = Quaternion.AngleAxis(-90, Vector3.up) * norm;
         Vector3 normRight = Quaternion.AngleAxis(90, Vector3.up) * norm;

          Collider[] left = Physics.OverlapSphere(playerSquad.transform.position + (normLeft*9f), 4.5f, playerUnitLayer);
          Collider[] right = Physics.OverlapSphere(playerSquad.transform.position + (normRight*9f), 4.5f, playerUnitLayer);

          if(left.Length == 0){
             lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, playerSquad.transform.position + (normLeft*9f));

          }else if(right.Length == 0){
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, playerSquad.transform.position + (normRight*9f));

          }
    }
    

    lineRenderer.positionCount++;
    lineRenderer.SetPosition(lineRenderer.positionCount - 1, playerSquad.transform.position + new Vector3(Random.Range(-3f,3f),0f,Random.Range(-3f,3f)));

    enemySquad.SetMoving(true);
    enemySquad.SetBattle(false);

}

public void SquadHalfWayToSquad(SquadController enemySquad,SquadController playerSquad) {

    GameObject drawing = Instantiate(drawingPrefab);
   LineRenderer lineRenderer = drawing.GetComponent<LineRenderer>();                      
    enemySquad.lineRenderer = lineRenderer;

    Vector3 norm = Vector3.Normalize(playerSquad.transform.position - enemySquad.transform.position);
    
    lineRenderer.positionCount++;
    lineRenderer.SetPosition(lineRenderer.positionCount - 1, enemySquad.transform.position + norm);


    lineRenderer.positionCount++;
    lineRenderer.SetPosition(lineRenderer.positionCount - 1, (enemySquad.transform.position + new Vector3(Random.Range(-3f,3f),0f,Random.Range(-3f,3f)) + playerSquad.transform.position)/2f);

    enemySquad.SetMoving(true);
    enemySquad.SetBattle(false);

}

public void SquadHalfWayToPoint(SquadController enemySquad,Vector3 point) {
    GameObject drawing = Instantiate(drawingPrefab);
   LineRenderer lineRenderer = drawing.GetComponent<LineRenderer>();                      
    enemySquad.lineRenderer = lineRenderer;

    Vector3 norm = Vector3.Normalize(point - enemySquad.transform.position);
    
    lineRenderer.positionCount++;
    lineRenderer.SetPosition(lineRenderer.positionCount - 1, enemySquad.transform.position + norm);


    lineRenderer.positionCount++;
    lineRenderer.SetPosition(lineRenderer.positionCount - 1, (enemySquad.transform.position + point)/2f);

    enemySquad.SetMoving(true);
    enemySquad.SetBattle(false);

}



public void FindHelpPoint(){ //хз надо ли оно нам

    helpPoints = new List<Vector3>();

        for (int i = 0; i < actionQueueArray.Count; i++) {
            if(actionQueueArray[i].inBattle){
                NearPlayerSquadList(actionQueueArray[i]);
                NearEnemySquadList(actionQueueArray[i]);

                  if((int)nearPlayerList[0].type == 0 && (int)actionQueueArray[i].type == 1 ){
                    helpPoints.Add(actionQueueArray[i].transform.position);
                  }
                  if((int)nearPlayerList[0].type == 1 && (int)actionQueueArray[i].type == 2 ){
                    helpPoints.Add(actionQueueArray[i].transform.position);
                  }
                  if((int)nearPlayerList[0].type == 2 && (int)actionQueueArray[i].type == 0 ){
                    helpPoints.Add(actionQueueArray[i].transform.position);
                  }
                
                
                if(nearPlayerList.Count < actionQueueArray[i].enemyController.Count){
                    helpPoints.Add(actionQueueArray[i].transform.position);
                }

                 if(nearEnemyPower/nearPlayerPower < 0.8+(difficulty * 0.02f)){
                    helpPoints.Add(actionQueueArray[i].transform.position);

                 }

            }
        }


}


public void NearEnemySquadList(SquadController squad ){
                int enemyCountCoef = 0;
            

               if((int)squad.type == 2){
                enemyCountCoef = horseCountCoef;
               }
              

     Collider[] nearColliders = Physics.OverlapSphere(squad.transform.position, nearEnemyRadius, enemyUnitLayer);
        
        nearEnemyList = new List<SquadController>();
        nearEnemyPower = 0;

        nearEnemyType0 = 0;
        nearEnemyType1 = 0;
        nearEnemyType2 = 0;
        

            for (int i = 0; i < nearColliders.Length; i++) {
                SquadController SquadNear = nearColliders[i].gameObject.GetComponent<SquadController>();
                
                if(!nearEnemyList.Contains(SquadNear) ){
                nearEnemyList.Add(SquadNear);

                nearEnemyPower += SquadNear.powerSquad*(SquadNear.unitArray.Count+enemyCountCoef);

                if((int)SquadNear.type == 0){
                    nearEnemyType0 ++;
                }
                if((int)SquadNear.type == 1){
                    nearEnemyType1 ++;
                }
                if((int)SquadNear.type == 2){
                    nearEnemyType2 ++;
                }
                }
                
    
            }   
        

    }

    public void NearPlayerSquadList(SquadController squad ){
           
               int playerCountCoef = 0;

              
               if((int)squad.type == 2){
                playerCountCoef = horseCountCoef;
               }

     Collider[] nearColliders = Physics.OverlapSphere(squad.transform.position, nearEnemyRadius, playerUnitLayer);
        
        nearPlayerList = new List<SquadController>();
        nearPlayerPower = 0;

         nearPlayerType0 = 0;
        nearPlayerType1 = 0;
        nearPlayerType2 = 0;
        

            for (int i = 0; i < nearColliders.Length; i++) {
                SquadController SquadNear = nearColliders[i].gameObject.GetComponent<SquadController>();

                if(!nearPlayerList.Contains(SquadNear) ){
                nearPlayerList.Add(SquadNear);

                 nearPlayerPower += SquadNear.powerSquad*(SquadNear.unitArray.Count+playerCountCoef);

                 if((int)SquadNear.type == 0){
                    nearPlayerType0 ++;
                }
                if((int)SquadNear.type == 1){
                    nearPlayerType1 ++;
                }
                if((int)SquadNear.type == 2){
                    nearPlayerType2 ++;
                }
                }
                
    
            }   
        

    }
    

    public bool UnitCountComparison(SquadController squad,SquadController playerSquad ){
                int enemyCountCoef = 0;
               int playerCountCoef = 0;

               if((int)squad.type == 2){
                enemyCountCoef = horseCountCoef;
               }
               if((int)playerSquad.type == 2){
                playerCountCoef = horseCountCoef;
               }

        if(difficulty <= 3){
            return true;
        }
        
        if (difficulty <= 6){
            if(squad.unitArray.Count + enemyCountCoef  >= playerSquad.unitArray.Count +playerCountCoef){
                return true;
            }
                return false;
            
        }
        
        if(difficulty > 6){
            if(squad.unitArray.Count + enemyCountCoef > playerSquad.unitArray.Count + (difficulty/2) + playerCountCoef ){
                return true;
            }
                return false;

        }
        return false;
    }

public void DeleteEnemySquad(SquadController squad) {

if(actionQueueArray.Contains(squad)){
    actionQueueArray.Remove(squad);
}

if(nearEnemyList.Contains(squad)){
    nearEnemyList.Remove(squad);
}

}

public void DeletePlayerSquad(SquadController squad) {

if(playerFarSquads.Contains(squad)){
    playerFarSquads.Remove(squad);
}

if(playerNearSquads.Contains(squad)){
    playerNearSquads.Remove(squad);
}

}

public void SetQueue() {

actionQueueArray = new List<SquadController>();

        GameObject[] allEnemy = GameObject.FindGameObjectsWithTag("Enemy");

            for (int i = 0; i < allEnemy.Length; i++) {
            actionQueueArray.Add(allEnemy[i].GetComponent<SquadController>());
        }

        
    }


public void SetAllEnemiesList() {

allEnemiesList = new List<SquadController>();

        GameObject[] allEnemy = GameObject.FindGameObjectsWithTag("Enemy");

            for (int i = 0; i < allEnemy.Length; i++) {
            allEnemiesList.Add(allEnemy[i].GetComponent<SquadController>());
        }

        
    }

  public void SpawnWave(int wave){

    Transform point = spawnPoints[Random.Range(0,spawnPoints.Length)]; 

     SquadController[] squads = wave0;

    if(wave == 0){
         squads = wave0;
    }
    if(wave == 1){
        squads = wave1;
    }
    if(wave == 2){
         squads = wave2;
    }
    if(wave == 3){
         squads = wave3;
    }
    if(wave == 4){
         squads = wave4;
    }
    if(wave == 5){
         squads = wave4;
    }

                 GameObject[] allPlayerSquads = GameObject.FindGameObjectsWithTag("Squad");

                Vector3 ceterOfSquads = new Vector3(0,0,0);
                    for (int i = 0; i < allPlayerSquads.Length; i++) {
                        ceterOfSquads += allPlayerSquads[i].transform.position;
                    }

                    ceterOfSquads /= allPlayerSquads.Length;

        Vector3 dirLeft = Quaternion.AngleAxis(point.rotation.eulerAngles.y, Vector3.up) * new Vector3(5f,0f,0f);
        Vector3 dirRight = Quaternion.AngleAxis(point.rotation.eulerAngles.y, Vector3.up) * new Vector3(-5f,0f,0f);

        GameObject sq = Instantiate(squads[0],point.position,point.rotation).gameObject;
        sq.transform.parent = enemyObject;
        SquadController spawnedSquad = sq.GetComponent<SquadController>();

        SquadHalfWayToPoint (spawnedSquad,ceterOfSquads);

        for (int i = 1; i < squads.Length; i++) {
            if(i % 2 == 0){
                dirLeft += dirLeft;
                    sq = Instantiate(squads[i],point.position+dirLeft,point.rotation).gameObject;
                    sq.transform.parent = enemyObject;
                    spawnedSquad = sq.GetComponent<SquadController>();
                    SquadHalfWayToPoint (spawnedSquad,ceterOfSquads);

            }else{
                dirRight +=dirRight;
                sq = Instantiate(squads[i],point.position + dirRight,point.rotation).gameObject;
                    sq.transform.parent = enemyObject;
                    spawnedSquad = sq.GetComponent<SquadController>();
                    SquadHalfWayToPoint (spawnedSquad,ceterOfSquads);
                   

            }
            }

    }
}
