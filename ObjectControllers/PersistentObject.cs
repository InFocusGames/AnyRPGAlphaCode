﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnyRPG {
    [RequireComponent(typeof(UUID))]
    public class PersistentObject : MonoBehaviour {

        [Tooltip("If true, this object will save it's position when switching from one scene to another (including the main menu).  It will not save if the game is quit directly from the main menu.")]
        [SerializeField]
        private bool saveOnLevelUnload = false;

        [Tooltip("If true, this object will save it's position when the player saves the game.")]
        [SerializeField]
        private bool saveOnGameSave = false;

        // should this object be moved to its new position on start.  set to false for things with navmeshagent and let the unit spawn node warp them instead
        private bool moveOnStart = true;

        private Vector3 storedPosition;

        private Vector3 storedForwardDirection;

        private string storedUUID;

        protected bool eventSubscriptionsInitialized = false;

        public bool MoveOnStart { get => moveOnStart; set => moveOnStart = value; }

        private void Awake() {
            OrchestratorStart();
        }

        // Start is called before the first frame update
        void Start() {
            if (moveOnStart == true) {
                LoadPersistentState();
            }
        }

        public PersistentState GetPersistentState() {
            //Debug.Log(gameObject.name + "PersistentObject.GetPersistentState()");
            UUID uuid = GetComponent<UUID>();
            if (uuid != null) {
                if (LevelManager.MyInstance != null) {
                    SceneNode activeSceneNode = LevelManager.MyInstance.GetActiveSceneNode();
                    if (activeSceneNode != null && activeSceneNode.PersistentObjects != null) {
                        if (activeSceneNode.PersistentObjects.ContainsKey(uuid.ID)) {
                            storedUUID = activeSceneNode.PersistentObjects[uuid.ID].UUID;
                            storedPosition = new Vector3(activeSceneNode.PersistentObjects[uuid.ID].LocationX, activeSceneNode.PersistentObjects[uuid.ID].LocationY, activeSceneNode.PersistentObjects[uuid.ID].LocationZ);
                            storedForwardDirection = new Vector3(activeSceneNode.PersistentObjects[uuid.ID].DirectionX, activeSceneNode.PersistentObjects[uuid.ID].DirectionY, activeSceneNode.PersistentObjects[uuid.ID].DirectionZ);
                            PersistentState persistentState = new PersistentState();
                            persistentState.Position = storedPosition;
                            persistentState.Forward = storedForwardDirection;
                            return persistentState;
                        }
                    }
                }
            }
            return null;
        }

        public void LoadPersistentState() {
            //Debug.Log(gameObject.name + "PersistentObject.LoadPersistentState()");
            PersistentState persistentState = GetPersistentState();
            if (persistentState != null) {
                transform.position = persistentState.Position;
                transform.forward = persistentState.Forward;
            }
        }

        public void OrchestratorStart() {
            //GetComponentReferences();
            CreateEventSubscriptions();
        }

        public virtual void CreateEventSubscriptions() {
            //Debug.Log(gameObject.name + "CharacterAbilityManager.CreateEventSubscriptions()");
            if (eventSubscriptionsInitialized) {
                return;
            }
            if (SystemEventManager.MyInstance == null) {
                Debug.LogError(gameObject.name + ".PersistentObject.CreateEventSubscriptions: Could not find SystemEventManager.  Is GameManager in the scene?");
            } else {
                SystemEventManager.StartListening("OnLevelUnload", HandleLevelUnload);
                SystemEventManager.StartListening("OnSaveGame", HandleSaveGame);
            }
            eventSubscriptionsInitialized = true;
        }

        public virtual void CleanupEventSubscriptions() {
            if (!eventSubscriptionsInitialized) {
                return;
            }
            if (SystemEventManager.MyInstance != null) {
                SystemEventManager.StopListening("OnLevelUnload", HandleLevelUnload);
                SystemEventManager.StopListening("OnSaveGame", HandleSaveGame);
            }
            eventSubscriptionsInitialized = false;
        }

        private void OnEnable() {
        }

        private void OnDisable() {
            CleanupEventSubscriptions();
        }

        public void HandleLevelUnload(string eventName, EventParamProperties eventParamProperties) {
            if (saveOnLevelUnload == true) {
                SaveProperties();
            }
        }

        public void HandleSaveGame(string eventName, EventParamProperties eventParamProperties) {
            if (saveOnGameSave == true) {
                SaveProperties();
            }
        }

        public void SaveProperties() {
            //Debug.Log(gameObject.name + "PersistentObject.SaveProperties()");
            storedPosition = transform.position;
            storedForwardDirection = transform.forward;
            UUID uuid = GetComponent<UUID>();
            if (uuid != null) {
                storedUUID = uuid.ID;
            }

            // save this data to the scene node that is active
            if (LevelManager.MyInstance != null) {
                SceneNode currentSceneNode = LevelManager.MyInstance.GetActiveSceneNode();
                if (currentSceneNode != null) {
                    currentSceneNode.PersistentObjects[storedUUID] = MakeSaveData();
                }
            }
        }

        public PersistentObjectSaveData MakeSaveData() {
            //Debug.Log(gameObject.name + "PersistentObject.MakeSaveData()");
            PersistentObjectSaveData returnValue = new PersistentObjectSaveData();
            returnValue.UUID = storedUUID;
            returnValue.LocationX = storedPosition.x;
            returnValue.LocationY = storedPosition.y;
            returnValue.LocationZ = storedPosition.z;
            returnValue.DirectionX = storedForwardDirection.x;
            returnValue.DirectionY = storedForwardDirection.y;
            returnValue.DirectionZ = storedForwardDirection.z;

            return returnValue;
        }
    }

    public class PersistentState {
        public Vector3 Position;
        public Vector3 Forward;
    }
}

