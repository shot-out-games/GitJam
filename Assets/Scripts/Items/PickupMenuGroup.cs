﻿using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public struct PickupMenuComponent : IComponentData
{
    public bool showMenu;
    public bool exitClicked;
    public bool menuStateChanged;
}

public class PowerItemClass
{
    public Entity pickedUpActor;
}


public class PickupMenuGroup : MonoBehaviour, IConvertGameObjectToEntity
{

    public static event Action<bool> PauseGame;

    private EntityManager manager;
    public Entity entity;
    public static List<PowerItemComponent> powerItemComponents = new List<PowerItemComponent>();
    //public SkillTreeComponent player0_skillSet;

    AudioSource audioSource;
    private List<Button> buttons;
    [SerializeField] private Button exitButton;
    public AudioClip clickSound;
    public EventSystem eventSystem;
    private CanvasGroup canvasGroup;

    [SerializeField]
    private TextMeshProUGUI label0;

    //public int speedPts;
    //public int powerPts;
    //public int chinPts;
    //public int availablePoints;

    private int buttonClickedIndex;
    //Player player;

    void Start()
    {
        if (!ReInput.isReady) return;
        //player = ReInput.players.GetPlayer(0);
        audioSource = GetComponent<AudioSource>();
        canvasGroup = GetComponent<CanvasGroup>();
        AddMenuButtonHandlers();
    }



    void Update()
    {

        if (entity == Entity.Null || manager == default) return;
        bool hasComponent = manager.HasComponent(entity, typeof(PickupMenuComponent));
        if (hasComponent == false) return;

        //move to system update below
        //bool stateChange = manager.GetComponentData<PickupMenuComponent>(entity).menuStateChanged;
        //
        //  if (stateChange == true)
        //  {
        var puMenu = manager.GetComponentData<PickupMenuComponent>(entity);
        //skillTreeMenu.menuStateChanged = false;
        manager.SetComponentData(entity, puMenu);

        if (manager.GetComponentData<PickupMenuComponent>(entity).showMenu)
        {
            ShowMenu();
        }
        else
        {
            HideMenu();
        }
        EnableButtons();
        //   }
        ShowLabel0();
    }

    void EnableButtons()
    {
        exitButton.Select();

        //buttons[1].interactable = false;
        //buttons[2].interactable = false;
        //buttons[3].interactable = false;
        //if (availablePoints >= 1 && speedPts == 0)
        //{
        //    buttons[1].interactable = true;
        //    buttons[1].Select();
        //}
        //else if (availablePoints >= 2 && speedPts == 1)
        //{
        //    buttons[2].interactable = true;
        //    buttons[2].Select();
        //}
        //else if (availablePoints >= 3 && speedPts == 2)
        //{
        //    buttons[3].interactable = true;
        //    buttons[3].Select();
        //}


    }

    void ShowLabel0()
    {
        if (powerItemComponents.Count == 0) return;
        label0.text = powerItemComponents[0].ToString(); ;
    }

    void ButtonClickedIndex(int index)
    {
        buttonClickedIndex = index;
        Debug.Log("btn idx " + buttonClickedIndex);
        //if (index >= 1 && index <= 3)
        //{
        //    if (manager.HasComponent<SkillTreeComponent>(entity) == false) return;
        //    availablePoints = availablePoints - index;
        //    speedPts = index;
        //    player0_skillSet.availablePoints = availablePoints;
        //    player0_skillSet.SpeedPts = speedPts;
        //    EnableButtons();

        //}
    }



    public void InitCurrentPowerItems()
    {




        //if (manager.HasComponent<PowerItemComponent>(entity) == false) return;




        //player0_skillSet = manager.GetComponentData<SkillTreeComponent>(entity);

        //speedPts = manager.GetComponentData<SkillTreeComponent>(entity).SpeedPts;
        //powerPts = manager.GetComponentData<SkillTreeComponent>(entity).PowerPts;
        //chinPts = manager.GetComponentData<SkillTreeComponent>(entity).ChinPts;
        //availablePoints = manager.GetComponentData<SkillTreeComponent>(entity).availablePoints;
        //Entity e = manager.GetComponentData<SkillTreeComponent>(entity).e;

        //if (playerSkillSets.Count < 1) //1 for now
        //{
        //    playerSkillSets.Add(player0_skillSet);
        //}

    }



    //public void WriteCurrentPlayerSkillSet()
    //{
    //    if (manager.HasComponent<SkillTreeComponent>(entity) == false) return;
    //    SkillTreeComponent skillTree = player0_skillSet;
    //    Entity playerE = skillTree.e;
    //    manager.SetComponentData<SkillTreeComponent>(playerE, player0_skillSet);

    //}


    private void AddMenuButtonHandlers()
    {
        buttons = GetComponentsInChildren<Button>().ToList();//linq using

        buttons.ForEach((btn) => btn.onClick.AddListener(() =>
            PlayMenuClickSound(clickSound)));//shortcut instead of using inspector to add to each button

        for (int i = 0; i < buttons.Count; i++)
        {
            int temp = i;
            buttons[i].onClick.AddListener(() => { ButtonClickedIndex(temp); });
        }

        exitButton.onClick.AddListener(ExitClicked);


    }

    private void ExitClicked()
    {
        //WriteCurrentPlayerSkillSet();
        //SkillTreeMenuComponent skillTreeMenu = manager.GetComponentData<SkillTreeMenuComponent>(entity);
        //skillTreeMenu.exitClicked = true;
        //manager.SetComponentData(entity, skillTreeMenu);

    }


    public void ShowMenu()
    {
        //InitCurrentPlayerSkillSet();
        PauseGame?.Invoke(true);
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HideMenu()
    {
        PauseGame?.Invoke(false);
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0.0f;
        canvasGroup.blocksRaycasts = false;

    }


    public static void Fill(List<PowerItemComponent> itemList)
    {
        powerItemComponents = itemList;
        for (int i = 0; i < itemList.Count; i++)
        {
            Debug.Log("fill " + powerItemComponents[i].pickedUpActor);
            //bool isPlayer = manager.HasComponent<PlayerComponent>(itemList[i].pickedUpActor);
        }

        //if (manager.HasComponent<PowerItemComponent>(entity) == false) return;
        //Debug.Log("fill " + item[0].pickedUpActor);
    }

    void PlayMenuClickSound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
        Debug.Log("clip " + clip);


    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        this.entity = entity;
        manager = dstManager;

        dstManager.AddComponentData(entity, new PickupMenuComponent()
        {
        });


    }
}


[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(InputControllerSystemUpdate))]

public class PickupSystem : SystemBase
{
    //Player player;

    protected override void OnCreate()
    {



    }
    protected override void OnUpdate()
    {
        //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Persistent);
        //var skillTreeGroup = GetComponentDataFromEntity<SkillTreeComponent>(true);
        //var playerSkillSets = new NativeArray<SkillTreeComponent>(1, Allocator.TempJob);

        //var itemGroup = GetComponentDataFromEntity<PowerItemComponent>(false);
        var itemQuery = GetEntityQuery(ComponentType.ReadOnly<PowerItemComponent>());
        var itemList = itemQuery.ToEntityArray(Allocator.Persistent);
        //NativeArray<TriggerComponent> triggerGroup = triggerQuery.ToComponentDataArray<TriggerComponent>(Allocator.Persistent);
        var itemGroup = itemQuery.ToComponentDataArray<PowerItemComponent>(Allocator.Persistent);

        List<PowerItemComponent> powerItems = new List<PowerItemComponent>();
        for (int i = 0; i < itemGroup.Length; i++)
        {
            if (HasComponent<PlayerComponent>(itemGroup[i].pickedUpActor))
            {
                powerItems.Add(itemGroup[i]);
             
            }

        }

        //PickupMenuGroup.Fill(powerItems);
        PickupMenuGroup.powerItemComponents = powerItems;

        //JobHandle inputDeps0 = Entities.WithReadOnly(skillTreeGroup).ForEach
        //(
        //    (
        //        in SkillTreeComponent skillTree,
        //        in RatingsComponent ratingsComponent,
        //        in PlayerComponent playerComponent,
        //        in Entity e

        //    ) =>
        //    {
        //        if (playerComponent.index == 1)//p1
        //            playerSkillSets[0] = skillTreeGroup[e];
        //    }
        //).Schedule(this.Dependency);

        //inputDeps0.Complete();
        //var playerSkillSet0 = playerSkillSets[0];

        //JobHandle inputDeps1 = Entities.ForEach
        //(
        //    (
        //        in PickupMenuComponent skillTreeMenuComponent,
        //        in Entity e

        //    ) =>
        //    {
        //        ecb.AddComponent<SkillTreeComponent>(e, playerSkillSet0);
        //    }

        //).Schedule(this.Dependency);
        //inputDeps1.Complete();


        ////JobHandle inputDeps2 = Entities.ForEach
        ////(
        ////    (
        ////        ref RatingsComponent ratingsComponent,
        ////        //ref Speed speedPower,
        ////        in SkillTreeComponent skillTree,
        ////        in PlayerComponent playerComponent,
        ////        in Entity e

        ////    ) =>
        ////    {
        ////        //figure out how to do just once so we can use original values
        ////        //ie  speedPower.timeOn = speedPower.timeOn * 2;

        ////        if (skillTree.SpeedPts == 1)
        ////        {
        ////            ratingsComponent.speed = 3.6f;
        ////        }
        ////        else if (skillTree.SpeedPts == 2)
        ////        {
        ////            ratingsComponent.speed = 4.5f;
        ////        }
        ////        else if (skillTree.SpeedPts == 3)
        ////        {
        ////            ratingsComponent.speed = 6f;
        ////            speedPower.timeOn = 12f;
        ////        }

        ////    }

        ////).Schedule(this.Dependency);

        ////inputDeps2.Complete();

        //var inpu = GetComponentDataFromEntity<TriggerComponent>(false);
        //  var inputQuery = GetEntityQuery(ComponentType.ReadOnly<InputControllerComponent>(), 
        //       ComponentType.ReadOnly<PlayerComponent >());
        //   var inputEntityList = inputQuery.ToEntityArray(Allocator.Persistent);


        //Entities.WithoutBurst().ForEach
        Entities.WithoutBurst().ForEach
        (
            (
                ref PickupMenuComponent pickupMenu

            ) =>
            {
                var dpadR = ReInput.players.GetPlayer(0).GetButtonDown("DpadR");
                var select = ReInput.players.GetPlayer(0).GetButtonDown("Select");

                // var inputController = GetComponent<InputControllerComponent>(inputEntityList[0]);


                Debug.Log("pickup menu " + dpadR);
                if (pickupMenu.exitClicked || select && pickupMenu.showMenu == true)
                {
                    pickupMenu.menuStateChanged = true;
                    pickupMenu.exitClicked = false;
                    pickupMenu.showMenu = false;
                }
                else if (dpadR)
                {
                    pickupMenu.menuStateChanged = true;
                    pickupMenu.showMenu = !pickupMenu.showMenu;
                }
            }

        ).Run();

        //inputDeps3.Complete();



        //ecb.Playback(EntityManager);
        //ecb.Dispose();
        //playerSkillSets.Dispose();
        itemGroup.Dispose();
    }

}
