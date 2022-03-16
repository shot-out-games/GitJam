using System;
using Unity.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using Unity.Jobs;
using Unity.Physics.Systems;
using Rewired;

public struct GameInterfaceComponent : IComponentData
{
    public bool paused;
}

public struct Pause : IComponentData
{
    
}

public class GameInterface : MonoBehaviour, IConvertGameObjectToEntity
{

    
    public static event Action HideMenuEvent;
    public static event Action SelectClickedEvent;


    public static bool Paused = false;
    public static bool SelectPressed = false;
    public static bool StateChange = false;

    public Player player;
    public int playerId = 0; // The Rewired player id of this character

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
       

    }

    void Start()
    {
        if (!ReInput.isReady) return;
        player = ReInput.players.GetPlayer(playerId);
    }

    private void Update()
    {

        SelectPressed = false;
        if (player.GetButtonDown("select"))
        {
            StateChange = true;
            Paused = !Paused;
            SelectPressed = true;
            SelectClicked();
         
        }




    }


    public void SelectClicked()//only called with button from system no menu item currently
    {
     
        SelectClickedEvent?.Invoke();//pause menu subscribes to this event to show pause menu
    }

  
  

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<GameInterfaceComponent>(entity, new GameInterfaceComponent { paused = Paused });
    }
}



[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(InputControllerSystemUpdate))]

public class GameInterfaceSystem : SystemBase
{

    StepPhysicsWorld stepPhysicsWorld;

    protected override void OnCreate()
    {
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }



    protected override void OnUpdate()
    {

        bool paused = GameInterface.Paused;
        bool stateChange = GameInterface.StateChange;

        

        bool required = HasSingleton<DeadMenuComponent>() && HasSingleton<WinnerMenuComponent>();
        if (required == false) return;

        bool deadMenuDisplayed = !GetSingleton<DeadMenuComponent>().hide;
        bool winnerMenuDisplayed = !GetSingleton<WinnerMenuComponent>().hide;


        stepPhysicsWorld.Enabled = !paused;


        if (stateChange)
        {
            GameInterface.StateChange = false;
            Entities.WithoutBurst().WithAll<RatingsComponent>().WithStructuralChanges().ForEach((Entity entity) =>
            {

                if (paused)
                {
                    EntityManager.AddComponent<Pause>(entity);
                }
                else
                {
                    EntityManager.RemoveComponent<Pause>(entity);
                }
            }
            ).Run();
        }



    }
}


