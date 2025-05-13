using System;
using System.Collections;
using UnityEngine;
using Zenject;

public sealed class RoomManager : MonoBehaviour
{
  [SerializeField] private float _roomLoadingTime = 2.0f;

  //--------------------------------------

  private LevelManager levelManager;
  private RoomLoadingUI roomLoadingUI;

  private Coroutine coroutineChangeRoom;

  //======================================

  public int CurrentIndexRoom { get; private set; }

  public Room CurrentRoom { get; private set; }

  //======================================

  public event Action<Room> OnCreateRoom;
  public event Action<Room> OnRoomCreated;

  public event Action OnRoomChangeCompleted;

  public event Action OnRoomsAreOver;

  //======================================

  [Inject]
  private void Construct(LevelManager parLevelManager, RoomLoadingUI parRoomLoadingUI)
  {
    levelManager = parLevelManager;
    roomLoadingUI = parRoomLoadingUI;
  }

  //======================================

  private void Awake()
  {
    CurrentIndexRoom = 0;
  }

  private void OnEnable()
  {
    OnCreateRoom += CreateRoom;

    OnRoomChangeCompleted += RoomChangeCompleted;
  }

  private void OnDestroy()
  {
    OnCreateRoom -= CreateRoom;
  }

  //======================================

  public void CreateRoom(Room parRoom)
  {
    if (parRoom == null)
    {
      Debug.LogError($"CreateRoom = null");
      return;
    }

    CurrentRoom = Instantiate(parRoom, Vector3.zero, Quaternion.identity);
    CurrentRoom.RoomPortal.OnPlayerEnteredPortal += RemoveRoom;

    levelManager.Player.CharacterController.enabled = false;
    levelManager.Player.transform.SetPositionAndRotation(CurrentRoom.SpawnPointPlayer.position, Quaternion.Euler(Vector3.zero));
    levelManager.Player.CharacterController.enabled = true;

    CurrentRoom.Initialize(levelManager);

    OnRoomCreated?.Invoke(CurrentRoom);
  }

  public void NextRoom()
  {
    if (coroutineChangeRoom != null)
      return;

    var roomPrefabs = levelManager.LocationData.ListRoomPrefabs;

    if (CurrentIndexRoom >= roomPrefabs.Count)
    {
      Debug.Log("All rooms have been created");
      return;
    }

    coroutineChangeRoom = StartCoroutine(ChangeRoom(roomPrefabs[CurrentIndexRoom]));
    CurrentIndexRoom++;
  }

  public void RemoveRoom()
  {
    var roomPrefabs = levelManager.LocationData.ListRoomPrefabs;

    if (CurrentRoom != null)
      CurrentRoom.RoomPortal.OnPlayerEnteredPortal -= RemoveRoom;

    if (CurrentIndexRoom + 1 > roomPrefabs.Count)
    {
      Debug.Log("All rooms have been created");
      //OnRoomsAreOver?.Invoke();
      return;
    }

    NextRoom();
  }

  //======================================

  private void RoomChangeCompleted()
  {
    CurrentRoom.RoomLoaded();
  }

  //======================================

  private IEnumerator ChangeRoom(Room parRoom)
  {
    levelManager.Player.CharacterController.enabled = false;
    roomLoadingUI.SetActive(true);

    yield return new WaitUntil(() => !roomLoadingUI.IsActive);

    Destroy(CurrentRoom.gameObject);

    OnCreateRoom?.Invoke(parRoom);

    yield return new WaitUntil(() => CurrentRoom.IsRoomCreated);

    roomLoadingUI.SetActive(false);

    yield return new WaitUntil(() => !roomLoadingUI.IsActive);

    OnRoomChangeCompleted?.Invoke();
    coroutineChangeRoom = null;
  }

  //======================================
}