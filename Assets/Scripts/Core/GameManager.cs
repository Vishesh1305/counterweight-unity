using System;
using Counterweight.Gameplay;
using UnityEngine;

namespace Counterweight.Core
{
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public int CoinsCollected { get; private set; }
        public int CoinsTotal { get; private set; }
        public int GemsCollected { get; private set; }
        public int GemsTotal { get; private set; }

        public bool AllPickupsCollected => CoinsCollected >= CoinsTotal && GemsCollected >= GemsTotal;

        public GameState State { get; private set; } = GameState.Playing; 


        public event Action PickupCountsChanged;


        public event Action RespawnStarted; 

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("More than one GameManager in the scene. Destroying the duplicate.", this);
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void RegisterPickup(PickupType type)
        {
            switch (type)
            {
                case PickupType.Coin:
                    CoinsTotal++;
                    break;
                case PickupType.Gem:
                    GemsTotal++;
                    break;
            }

            PickupCountsChanged?.Invoke();
        }

        public void CollectPickup(PickupType type)
        {
            switch (type)
            {
                case PickupType.Coin:
                    CoinsCollected++;
                    break;
                case PickupType.Gem:
                    GemsCollected++;
                    break;
            }

            PickupCountsChanged?.Invoke();
        }


        public void ReportPlayerDeath()  
        {   
            BeginRespawn();   
        }  
        
        public void RequestCheckpointRestart()   
        {  
            BeginRespawn();  
        }   

        public void CompleteRespawn() 
        {   
            if (State == GameState.Respawning)   
                State = GameState.Playing; 
        } 

        private void BeginRespawn() 
        { 
            if (State != GameState.Playing)   
                return;   

            State = GameState.Respawning; 
            RespawnStarted?.Invoke();
        } //  
    }
}