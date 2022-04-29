using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using MoralisWeb3ApiSdk;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Web3MultiplayerRPG
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        [Header("General")]
        public List<GameObject> playerPrefabs = new List<GameObject>();
        public Transform initTransform;
        public Transform gmrGateLocation;
        public CinemachineVirtualCamera cineVirtualCamera;
        public MobilePlayerInputController mobileInputController;
        
        [Header("UI Panels")]
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private GameObject transactionPanel;
        [SerializeField] private GameObject leaveRoomPanel;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;

        private GameObject _newPlayer;
        private bool fromGMR = false;

        #region UNITY_LIFECYCLE
        
        private void Start()
        {
            URLParameters.Instance.RegisterOnDone((url)=> {
                if(url.Search.Contains("gmr_traveler_id=")) {
                    fromGMR = true;
                    Debug.Log("from GMR Tunnel");
                } else {
                    fromGMR = false;
                    Debug.Log("from Organic");
                }
            });

            PhotonPlayerController.OnPlayerClicked += PlayerClickedHandler;
            
            if (PhotonNetwork.IsConnectedAndReady)
            {
                OnConnectedToMaster();  
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();   
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                leaveRoomPanel.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            PhotonPlayerController.OnPlayerClicked -= PlayerClickedHandler;
        }

        #endregion

        #region PRIVATE_METHODS

        private void PlayerClickedHandler(PhotonView clickedPhotonView, string walletAddress, string walletAddressFormatted)
        {
            transactionPanel.SetActive(true);
            transactionPanel.GetComponent<TransactionPanel>().SetInformation(clickedPhotonView.Owner.NickName, walletAddress);
        }

        #endregion

        #region PHOTON_NETWORK
        
        public override void OnConnectedToMaster()
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 10;
            PhotonNetwork.JoinOrCreateRoom("Room 1", roomOptions, null);
        }

        public override void OnJoinedRoom()
        {
            GameObject playerPrefab = playerPrefabs[Random.Range(0, playerPrefabs.Count - 1)];

            Vector3 startPosition = initTransform.position;

            if(fromGMR) {
                Debug.Log("Using Gate Location");
                startPosition = new Vector3(gmrGateLocation.position.x, gmrGateLocation.position.y, gmrGateLocation.position.z - 0.6f);
            } else {
                Debug.Log("Using Default Location");
            }

            if (PhotonNetwork.IsConnected)
            {
                _newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, startPosition, Quaternion.identity);
            }
            else
            {
                _newPlayer = Instantiate(playerPrefab, startPosition, Quaternion.identity);
            }

            if (_newPlayer == null)
            {
                Debug.LogError("Player has not been instantiated.");
                return;
            }

            if(fromGMR) {
                var rotation = gmrGateLocation.transform.rotation;
                _newPlayer.transform.Rotate(0.0f, 143.481f, 0.0f);
            }
            
            loadingPanel.SetActive(false);
            audioSource.Play();
            
            //We only want the camera to follow OUR own client player
            PhotonPlayerController photonPlayerController = _newPlayer.GetComponent<PhotonPlayerController>();
            
            if (photonPlayerController.photonView.IsMine)
            {
                cineVirtualCamera.Follow = photonPlayerController.CinemachineCameraTarget.transform;
                cineVirtualCamera.m_LookAt = photonPlayerController.CinemachineCameraTarget.transform;   
            }
            else
            {
                if (Application.isEditor)
                {
                    cineVirtualCamera.Follow = photonPlayerController.CinemachineCameraTarget.transform;
                    cineVirtualCamera.m_LookAt = photonPlayerController.CinemachineCameraTarget.transform; 
                }
            }
            
            if (Application.isMobilePlatform)
            {
                mobileInputController.SetInputController(_newPlayer.GetComponent<PlayerInputController>(), _newPlayer.GetComponent<PlayerInput>());
                mobileInputController.gameObject.SetActive(true);
            }
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("Login");
        }

        #endregion
    }
}

