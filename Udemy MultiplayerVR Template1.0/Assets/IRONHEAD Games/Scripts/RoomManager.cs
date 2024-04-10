using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TextMeshProUGUI OccupancyRateText_ForSchool;
    [SerializeField]
    private TextMeshProUGUI OccupancyRateText_ForOutdoor;

    private string mapType;

    void Start()
    {
        ConnectToPhoton();
    }

    void ConnectToPhoton()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public void OnEnterButtonClicked_Outdoor()
    {
        AttemptRoomJoin(MultiplayerVRConstants.MAP_TYPE_VALUE_OUTDOOR);
    }

    public void OnEnterButtonClicked_School()
    {
        AttemptRoomJoin(MultiplayerVRConstants.MAP_TYPE_VALUE_SCHOOL);
    }

    void AttemptRoomJoin(string desiredMapType)
    {
        mapType = desiredMapType;
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Leaving current room to join a room of type: " + desiredMapType);
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            JoinOrCreateRoom();
        }
    }

    public override void OnLeftRoom()
    {
        JoinOrCreateRoom();
    }

    void JoinOrCreateRoom()
    {
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join random failed: " + message + ", creating a room...");
        CreateAndJoinRoom();
    }

    void CreateAndJoinRoom()
    {
        string randomRoomName = "Room_" + mapType + "_" + Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = 20,
            CustomRoomPropertiesForLobby = new string[] { MultiplayerVRConstants.MAP_TYPE_KEY },
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } }
        };
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name + ", Map type: " + mapType);
        PhotonNetwork.LoadLevel(mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_SCHOOL ? "World_School" : "World_Outdoor");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master, joining lobby...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateOccupancyUI(roomList);
    }

    void UpdateOccupancyUI(List<RoomInfo> roomList)
    {
        int schoolOccupancy = 0;
        int outdoorOccupancy = 0;

        foreach (RoomInfo room in roomList)
        {
            if (room.CustomProperties[MultiplayerVRConstants.MAP_TYPE_KEY].Equals(MultiplayerVRConstants.MAP_TYPE_VALUE_SCHOOL))
            {
                schoolOccupancy += room.PlayerCount;
            }
            else if (room.CustomProperties[MultiplayerVRConstants.MAP_TYPE_KEY].Equals(MultiplayerVRConstants.MAP_TYPE_VALUE_OUTDOOR))
            {
                outdoorOccupancy += room.PlayerCount;
            }
        }

        OccupancyRateText_ForSchool.text = $"{schoolOccupancy} / 20";
        OccupancyRateText_ForOutdoor.text = $"{outdoorOccupancy} / 20";
    }

    // Add any additional necessary methods here...
}
