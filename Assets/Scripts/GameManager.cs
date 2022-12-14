using Assets.GameplayControl;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;

using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour//, IDataPersistence
{
    public List<Sprite> cardSprites;
    //public GameObject shipGameObject;
    public GameObject cardButton;
    public GameObject cardButtonWithSync;

    private GameObject spawnedCardGameObject;
    public List<GameObject> shipGameObjectList = new();
    public List<TMP_Text> cardStackCounterList { set; get; } = new();
    private Vector3 spaceshipsBase = new(-8, -4, -1);
    public TMP_Text spaceshipCounter;
    private TMP_Text satelliteCounter;

    public bool iSendSpawnCardsServerRpc;
    GameObject drawCardsPanel;
    GameObject cardGoal;

    public int playerId = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("GameManager Client ID:"+OwnerClientId);
        drawCardsPanel = GameObject.Find("DrawCardsPanel");
        cardGoal = GameObject.Find("CardGoal");

        spaceshipCounter = GameObject.Find("SpaceshipCounter").GetComponent<TMP_Text>();
        spaceshipCounter.text = "50";//Server.allPlayersInfo.Single(p => p.Id == PlayerGameData.Id).SpaceshipsLeft.ToString();

        satelliteCounter = GameObject.Find("SatelliteCounter").GetComponent<TMP_Text>();
        satelliteCounter.text = "3";
    }

    // Update is called once per frame
    void Update()
    { 
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnShipsServerRpc(int playerId, Vector3 position, Quaternion rotation,ServerRpcParams serverRpcParams = default)
    {
        //Debug.Log("playerId: " + playerId);
        
        float angle = CalculateAngle(position,spaceshipsBase);
        var spawnedShipGameObject = Instantiate(shipGameObjectList[playerId], spaceshipsBase, Quaternion.Euler(new Vector3(0, 0, -angle)));
        // spawnuje si??? dla wszystkich graczy bo network object
        spawnedShipGameObject.GetComponent<NetworkObject>().Spawn(true);
        var spawnedShip = spawnedShipGameObject.GetComponent<Move>();
        spawnedShip.goalPosition = position;
        spawnedShip.goalRotation = rotation;
    }

    public void SpawnCards(Transform t, int color, string name)
    {
        cardButton.GetComponent<Image>().sprite = cardSprites[color];
        spawnedCardGameObject = Instantiate(cardButton, t);
        spawnedCardGameObject.name = name;
        var spawnedCard = spawnedCardGameObject.GetComponent<Move>();
        spawnedCard.speed = 700;
        Transform cardsStack = GameObject.Find(name + "s").GetComponent<Transform>();
        spawnedCard.goalPosition = cardsStack.position;
        spawnedCard.goalRotation = cardsStack.rotation;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnCardsServerRpc(Vector3 position,int color,string name,int index)
    {
       SpawnCardsClientRpc(position, color, name,index);
    }

    [ClientRpc]
    public void SpawnCardsClientRpc(Vector3 position, int color, string name,int index)
    {
        if (!iSendSpawnCardsServerRpc)
        {
            cardButton.GetComponent<Image>().sprite = cardSprites[color];
            spawnedCardGameObject = Instantiate(cardButton, drawCardsPanel.transform.GetChild(index));
            spawnedCardGameObject.name = name;
            spawnedCardGameObject.transform.localScale = new(0.5f, 0.5f);
            //spawnedCardGameObject.transform.SetParent(canvas.transform);
            var spawnedCard = spawnedCardGameObject.GetComponent<Move>();

            spawnedCard.speed = 1000;
            spawnedCard.goalPosition = cardGoal.transform.position;
            spawnedCard.goalRotation = Quaternion.identity;
        }
        else
            iSendSpawnCardsServerRpc = false;
    }

    public float CalculateAngle(Vector3 position1, Vector3 position2)
    {
        return Mathf.Atan2(
                          position1.x - position2.x,
                          position1.y - position2.y
                          ) * Mathf.Rad2Deg;
    }


    [ServerRpc(RequireOwnership = false)]
    public void SetBuildPathDataServerRpc(int pathId, int playerId)
    {
        SetBuildPathDataClientRpc(pathId, playerId);
    }

    [ClientRpc]
    public void SetBuildPathDataClientRpc(int pathId, int playerId)
    {
        Path path = Map.mapData.paths.Where(p => p.Id == pathId).First();
        path.isBuilt = true;
        path.builtById = playerId;
    }

    public void SetPopUpWindow(string message)
    {
        var popUp = GameObject.Find("Canvas").transform.Find("PopUpPanel");//transform.parent.GetChild(0);
        popUp.transform.Find("InfoText").GetComponent<TMP_Text>().text = message;
        popUp.gameObject.SetActive(true);
    }

    public void ShowFadingPopUpWindow(string message)
    {
        var popUp = GameObject.Find("Canvas").transform.Find("FadingPopUpPanel");
        popUp.transform.Find("InfoText").GetComponent<TMP_Text>().text = message;
        StartCoroutine(ShowFadingPopUpWindowCoroutine(popUp.transform.Find("FadeScript").GetComponent<FadePanel>()));
    }

    IEnumerator ShowFadingPopUpWindowCoroutine(FadePanel fade)
    {
        fade.ShowUp();
        yield return new WaitForSeconds(2);
        fade.FadeOut();
    }

    public void DelayAiMove(ArtificialPlayer ai)
    {
        StartCoroutine(DelayAiMoveCoroutine(ai));
    }

    IEnumerator DelayAiMoveCoroutine(ArtificialPlayer ai)
    {
        yield return new WaitForSeconds(3);
        ai.BestMove();
    }

    public void EndAiTurn(ArtificialPlayer ai)
    {
        StartCoroutine(EndAiTurnCoroutine(ai));
    }

    IEnumerator EndAiTurnCoroutine(ArtificialPlayer ai)
    {
        yield return new WaitForSeconds(1);
        Communication.EndAITurn(ai);
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndGameServerRpc() // wy??wietlanie ekranu ko??cowego
    {
        EndGameClientRpc();
    }

    [ClientRpc]
    private void EndGameClientRpc()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void MarkMissionDone(Mission mission)
    {
        GameObject missionButton = GameObject.Find(mission.start.name + "-" + mission.end.name);
        missionButton.transform.GetChild(1).gameObject.SetActive(true);
    }

}
