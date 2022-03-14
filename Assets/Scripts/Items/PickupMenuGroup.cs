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
    //public PowerItemComponent[] useItemComponents = new PowerItemComponent[2];

    //public SkillTreeComponent player0_skillSet;

    AudioSource audioSource;
    [SerializeField]
    private List<Button> buttons;
    [SerializeField]
    private List<Button> useButtons;
    [SerializeField] private Button exitButton;
    public AudioClip clickSound;
    public EventSystem eventSystem;
    private CanvasGroup canvasGroup;

    [SerializeField]
    private TextMeshProUGUI[] pickuplabel;
    [SerializeField]
    private TextMeshProUGUI[] uselabel;

    //public int speedPts;
    //public int powerPts;
    //public int chinPts;
    //public int availablePoints;

    private int buttonClickedIndex;
    public static bool UpdateMenu = true;
    //private bool[] inUsedItemList = new bool[30];


    //Player player;
    int useSlots = 2;
    int selectedPower;
    int useSlotIndex1 = -1, useSlotIndex2 = -1;
    Entity useSlot1, useSlot2;


    void Start()
    {
        if (!ReInput.isReady) return;
        //player = ReInput.players.GetPlayer(0);
        audioSource = GetComponent<AudioSource>();
        canvasGroup = GetComponent<CanvasGroup>();
        AddMenuButtonHandlers();
        //useItemComponents.Add(new PowerItemComponent());
        //useItemComponents.Add(new PowerItemComponent());
    }

    public void UpdateItemEntities()
    {

        if (entity == Entity.Null || manager == default) return;



    }

    void Update()
    {
        UpdateItemEntities();
        GameObject selected = eventSystem.currentSelectedGameObject;


    }




    // }
    public void EnableButtons()
    {
        buttons[0].Select();
        useButtons[0].interactable = true;
        useButtons[1].interactable = true;
        if (powerItemComponents.Count == 1)
        {
            useButtons[1].interactable = false;
        }
        if (powerItemComponents.Count == 0)
        {
            useButtons[0].interactable = false;
            useButtons[1].interactable = false;
        }

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

    public void ShowLabels()
    {
        for (int i = 0; i < uselabel.Length; i++)
        {
            uselabel[i].text = "";
        }

        for (int i = 0; i < pickuplabel.Length; i++)
        {
            pickuplabel[i].text = "";
        }

        for (int i = 0; i < powerItemComponents.Count; i++)
        {
            pickuplabel[i].text = powerItemComponents[i].description.ToString();
        }

        for (int i = 0; i < powerItemComponents.Count; i++)
        {
            if (i == 0 && powerItemComponents.Count > 0 && useSlotIndex1 >= 0)
            {
                uselabel[i].text = powerItemComponents[useSlotIndex1].description.ToString();
            }
            else if (i == 1 && powerItemComponents.Count > 1 && useSlotIndex2 >= 0)
            {
                uselabel[i].text = powerItemComponents[useSlotIndex2].description.ToString();
            }
        }




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

    public void SelectedPower(int index)
    {
        selectedPower = index;
        Debug.Log("sel " + selectedPower);
    }

    public void AssignSelectedPower(int use_index)
    {
        if (use_index <= 0 || powerItemComponents.Count < use_index) return;
        if (entity == Entity.Null || manager == default) return;
        Debug.Log("assign " + selectedPower);
        Entity pickupEntity = powerItemComponents[selectedPower].pickupEntity;
        bool pickedUp = powerItemComponents[selectedPower].itemPickedUp;
        var item = manager.GetComponentData<PowerItemComponent>(pickupEntity);
        //item1.useSlot1 = false;
        if (pickupEntity != Entity.Null && pickedUp == true)
        {
            Debug.Log("use  " + pickupEntity + " " + use_index);

            if (use_index == 1)
            {

                useSlotIndex1 = selectedPower;
                useSlot1 = pickupEntity;
                item.useSlot2 = false;
                item.useSlot1 = true;
            }
            else if (use_index == 2)
            {
                useSlotIndex2 = selectedPower;
                useSlot2 = pickupEntity;
                item.useSlot1 = false;
                item.useSlot2 = true;
            }
            ShowLabels();
        }
        manager.SetComponentData<PowerItemComponent>(pickupEntity, item);



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
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(InputControllerSystemUpdate))]
//[UpdateAfter(typeof(PickupPowerUpRaycastSystem))]


public class PickupSystem : SystemBase
{



    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Persistent);

        var itemQuery = GetEntityQuery(ComponentType.ReadOnly<PowerItemComponent>());
        var itemList = itemQuery.ToEntityArray(Allocator.Persistent);
        var itemGroup = itemQuery.ToComponentDataArray<PowerItemComponent>(Allocator.Persistent);

        List<PowerItemComponent> powerItems = new List<PowerItemComponent>();
        List<PowerItemComponent> useItems = new List<PowerItemComponent>();

        for (int i = 0; i < 2; i++)
        {
            useItems.Add(new PowerItemComponent());
        }

        for (int i = 0; i < itemGroup.Length; i++)
        {
            if (itemGroup[i].itemPickedUp)
            {
                powerItems.Add(itemGroup[i]);
            }
        }

        powerItems.Sort(new PowerItemIndexComparer());

        PickupMenuGroup.powerItemComponents = powerItems;

        for (int i = 0; i < itemGroup.Length; i++)
        {
            if (itemGroup[i].useSlot1)
            {
                useItems[0] = itemGroup[i];
            }
            else if (itemGroup[i].useSlot2)
            {
                useItems[1] = itemGroup[i];
            }
        }




        //for (int i = 0; i < useItems.Length; i++)
        //{
        //    if (i < powerItems.Count)
        //        //if (useItems[i].buttonAssigned == false && i < powerItems.Count)
        //        //{
        //        //useItems[i].buttonAssigned = true;
        //        useItems[i] = powerItems[i];
        //    //}

        //}


        var pickupMenu = GetSingleton<PickupMenuComponent>();

        var dpadR = ReInput.players.GetPlayer(0).GetButtonDown("DpadR");
        var select = ReInput.players.GetPlayer(0).GetButtonDown("Select");
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
        SetSingleton(pickupMenu);


        Entities.WithoutBurst().ForEach((PickupMenuGroup pickupMenuGroup) =>
        {

            //pickupMenuGroup.powerItemComponents = powerItems;
            //pickupMenuGroup.useItemComponents = useItems;
            pickupMenuGroup.EnableButtons();
            pickupMenuGroup.ShowLabels();
            if (pickupMenu.showMenu)
            {
                pickupMenuGroup.ShowMenu();
            }
            else
            {
                pickupMenuGroup.HideMenu();
            }
        }

        ).Run();











        Entity pickupEntity1 = useItems[0].pickupEntity;
        bool pickedUp1 = useItems[0].itemPickedUp;

        Entity pickupEntity2 = useItems[1].pickupEntity;
        bool pickedUp2 = useItems[1].itemPickedUp;




        if (pickupEntity1 != Entity.Null && pickedUp1 == true)
        {
            ecb.RemoveComponent<UseItem2>(pickupEntity1);
            ecb.AddComponent<UseItem1>(pickupEntity1);
            //Debug.Log("add use1 " + pickupEntity1);
        }

        if (pickupEntity2 != Entity.Null && pickedUp2 == true)
        {
            ecb.RemoveComponent<UseItem1>(pickupEntity2);
            ecb.AddComponent<UseItem2>(pickupEntity2);
            //Debug.Log("add use2 " + pickupEntity2);
        }







        ecb.Playback(EntityManager);
        ecb.Dispose();
        itemGroup.Dispose();
        itemList.Dispose();
    }

}


[UpdateInGroup(typeof(SimulationSystemGroup))]

//[AlwaysUpdateSystem]
public class InputUseItemSystem : SystemBase
{
    //Player player;

    protected override void OnCreate()
    {


        Debug.Log("use1 ");

    }
    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Persistent);
        bool hasUse1 = HasSingleton<UseItem1>();
        bool hasUse2 = HasSingleton<UseItem2>();
        if (hasUse1)
        {
            var use1_entity = GetSingletonEntity<UseItem1>();
            bool use1_pressed = ReInput.players.GetPlayer(0).GetButtonDown("Use1");
            Debug.Log("use1 " + use1_pressed);
            if (use1_pressed && HasComponent<PowerItemComponent>(use1_entity))
            {
                ecb.AddComponent<ImmediateUseComponent>(use1_entity);
                //ecb.RemoveComponent<UseItem1>(use1_entity);
            }

        }
        if (hasUse2)
        {
            var use2_entity = GetSingletonEntity<UseItem2>();
            bool use2_pressed = ReInput.players.GetPlayer(0).GetButtonDown("Use2");
            Debug.Log("use2 " + use2_pressed);
            if (use2_pressed && HasComponent<PowerItemComponent>(use2_entity))
            {
                ecb.AddComponent<ImmediateUseComponent>(use2_entity);
                //ecb.RemoveComponent<UseItem2>(use2_entity);
            }

        }


        ecb.Playback(EntityManager);
        ecb.Dispose();

    }

}
