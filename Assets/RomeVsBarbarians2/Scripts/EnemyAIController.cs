using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{

[SerializeField] public List<SquadController> actionQueueArray;

[SerializeField] public List<SquadController> nearEnemyList;

 [SerializeField] List<SquadController> playerNearSquads = new List<SquadController>();
[SerializeField]  List<SquadController> playerFarSquads = new List<SquadController>();

[SerializeField]  List<Vector3> battlePoints = new List<Vector3>();

[SerializeField] public float timeAction;
[SerializeField] public int actions;
 [SerializeField] private LayerMask playerUnitLayer ;

  [SerializeField] private LayerMask enemyUnitLayer ;

 [SerializeField] private GameObject drawingPrefab;

[Header("AI Settings")]
    [Space(10)]
[SerializeField] public float timeToGetActions;
[SerializeField] public float farRadius;
[SerializeField] public float nearRadius;
[SerializeField] public float nearEnemyRadius;





    // Start is called before the first frame update
    void Start()
    {
       SetQueue();

    }

    // Update is called once per frame
    void Update()
    {
        timeAction += Time.fixedDeltaTime;

        if(timeAction > timeToGetActions){
            if(actionQueueArray.Count > 0){
            if(actionQueueArray[actions] != null){
            AiAction(actionQueueArray[actions]);
           
            actionQueueArray.RemoveAt(actions);
            
            }
            }

        //actions ++;
        timeAction = 0;

        if(actionQueueArray.Count == 0){
        SetQueue();
        // actions = 0;
        }

        }

        
    }


     public void AiAction(SquadController squad) {

        if(squad.inBattle){


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

            if(playerNearSquads.Count > 0){
            // Debug.Log("playerNearSquads " + playerNearSquads.Count,squad.gameObject);



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

             for (int i = 0; i < playerNearSquads.Count; i++) {

                if((int)squad.type == 0  && (int)playerNearSquads[i].type == 1){
                    SquadMoveToSquad(squad,playerNearSquads[i]);
                    return;
                }

                if((int)squad.type == 2 && (int)playerNearSquads[i].type == 3){
                    SquadMoveToSquad(squad,playerNearSquads[i]);
                    return;
                }

                if((int)squad.type == 1 && (int)playerNearSquads[i].type == 0){
                    SquadMoveToSquad(squad,playerNearSquads[i]);
                    return;
                }

             }
             
             for (int i = 0; i < playerNearSquads.Count; i++) {

                if((int)squad.type == (int)playerNearSquads[i].type){
                    SquadMoveToSquad(squad,playerNearSquads[i]);
                    return;
                }

              }

              for (int i = 0; i < playerNearSquads.Count; i++) {

                if(squad.unitArray.Count/2f > playerNearSquads[i].unitArray.Count){
                    SquadMoveToSquad(squad,playerNearSquads[i]);
                    return;
                }

              }

              for (int i = 0; i < playerNearSquads.Count; i++) {

                if(playerNearSquads[i].inBattle){
                    SquadMoveToSquad(squad,playerNearSquads[i]);
                    return;
                }

              }


             NearEnemySquadList(squad);

                if(playerNearSquads.Count < nearEnemyList.Count){
                    int randomSqaud = Random.Range(0,playerNearSquads.Count);// случайно выбираем отряд к которому будет двигаться група вражеских отрядов
                    for (int i = 0; i < playerNearSquads.Count+1; i++) { // берем на одно больше чем отрядов игрока
                        SquadMoveToSquad(nearEnemyList[i],playerNearSquads[randomSqaud]);

                        if(actionQueueArray.Contains(nearEnemyList[i])){
                        actionQueueArray.Remove(nearEnemyList[i]);
                        }
                    }
                    return;
                }


            }else if(playerFarSquads.Count > 0){
            //Debug.Log("playerFarSquads " + playerFarSquads.Count,squad.gameObject);
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
                    //.Log("farSquad ", nrSqvd);
               }
            playerFarSquads = farSquad;


            for (int i = 0; i < playerFarSquads.Count; i++) {

                if((int)squad.type == 0  && (int)playerFarSquads[i].type == 1){
                    SquadHalfWayToSquad(squad,playerFarSquads[i]);
                    return;
                }

                if((int)squad.type == 2 && (int)playerFarSquads[i].type == 3){
                    SquadHalfWayToSquad(squad,playerFarSquads[i]);
                    return;
                }

                if((int)squad.type == 1 && (int)playerFarSquads[i].type == 0){
                    SquadHalfWayToSquad(squad,playerFarSquads[i]);
                    return;
                }

             }


                for (int i = 0; i < playerFarSquads.Count; i++) {

                if(playerFarSquads[i].inBattle){
                    SquadMoveToSquad(squad,playerFarSquads[i]);
                    return;
                }

              }
                NearEnemySquadList(squad);

                if(playerFarSquads.Count < nearEnemyList.Count){
                    int randomSqaud = Random.Range(0,playerFarSquads.Count);// случайно выбираем отряд к которому будет двигаться група вражеских отрядов
                    for (int i = 0; i < nearEnemyList.Count; i++) {
                        SquadHalfWayToSquad(nearEnemyList[i],playerFarSquads[randomSqaud]);

                        if(actionQueueArray.Contains(nearEnemyList[i])){
                            actionQueueArray.Remove(nearEnemyList[i]);
                        }
                        
                    
                    }
                    return;
                }
              
                if(battlePoints.Count > 0){
                        SquadHalfWayToPoint(squad,battlePoints[Random.Range(0,battlePoints.Count)]);
                        return;
                    }

            }else{

                    if(battlePoints.Count > 0){
                        SquadHalfWayToPoint(squad,battlePoints[Random.Range(0,battlePoints.Count)]);
                    }
                
                return;
            }   



        }

     }
public void SquadMoveToSquad(SquadController enemySquad,SquadController playerSquad) {


    GameObject drawing = Instantiate(drawingPrefab);
   LineRenderer lineRenderer = drawing.GetComponent<LineRenderer>();                      
    enemySquad.lineRenderer = lineRenderer;

    Vector3 norm = Vector3.Normalize(playerSquad.transform.position - enemySquad.transform.position);
    
    lineRenderer.positionCount++;
    lineRenderer.SetPosition(lineRenderer.positionCount - 1, enemySquad.transform.position + norm);


    lineRenderer.positionCount++;
    lineRenderer.SetPosition(lineRenderer.positionCount - 1, (enemySquad.transform.position + playerSquad.transform.position)/2f);

   

   // Debug.Log("enemySquad.transform.position "+ enemySquad.transform.position);

    

    lineRenderer.positionCount++;
    lineRenderer.SetPosition(lineRenderer.positionCount - 1, playerSquad.transform.position);

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
    lineRenderer.SetPosition(lineRenderer.positionCount - 1, (enemySquad.transform.position + playerSquad.transform.position)/2f);

   

   // Debug.Log("enemySquad.transform.position "+ enemySquad.transform.position);

    

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

   

   // Debug.Log("enemySquad.transform.position "+ enemySquad.transform.position);

    

    enemySquad.SetMoving(true);
    enemySquad.SetBattle(false);

}
public void NearPlayerSquadList(){


}

public void SetBattlePoint(Vector3 point){

battlePoints.Add(point);

}

public void NearEnemySquadList(SquadController squad ){


     Collider[] nearColliders = Physics.OverlapSphere(squad.transform.position, nearEnemyRadius, enemyUnitLayer);
        
        nearEnemyList = new List<SquadController>();
        

            for (int i = 0; i < nearColliders.Length; i++) {
                SquadController SquadNear = nearColliders[i].gameObject.GetComponent<SquadController>();

                if(!nearEnemyList.Contains(SquadNear) ){
                nearEnemyList.Add(SquadNear);
                }
                
    
            }   
        

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
}
