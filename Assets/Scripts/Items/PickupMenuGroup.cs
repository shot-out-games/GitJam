using Rewired;
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
    private List<Button> buttons;
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
    Entity useSlot1, useSlot2;

    //Player player;
    int useSlots = 2;
    int selectedPower;
    int useSlotIndex1, useSlotIndex2;


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
        //Entity pickupEntity1 = powerItemComponents[0].pickupEntity;
        //bool pickedUp1 = powerItemComponents[0].itemPickedUp;
        //var item1 = manager.GetComponentData<PowerItemComponent>(pickupEntity1);
        ////item1.useSlot1 = false;
        //if (pickupEntity1 != Entity.Null && pickedUp1 == true && useSlot1 != pickupEntity1 && useSlot2 != pickupEntity1)
        //{
        //    Debug.Log("use 1 " + pickupEntity1);
        //    useSlot1 = pickupEntity1;
        //    item1.useSlot1 = true;
        //}
        //manager.SetComponentData<PowerItemComponent>(pickupEntity1, item1);

        //Entity pickupEntity2 = powerItemComponents[1].pickupEntity;
        //bool pickedUp2 = powerItemComponents[1].itemPickedUp;
        //var item2 = manager.GetComponentData<PowerItemComponent>(pickupEntity2);
        ////item2.useSlot2 = false;
        //if (pickupEntity2 != Entity.Null && pickedUp2 == true && useSlot1 != pickupEntity2 && useSlot2 != pickupEntity2)
        //{
        //    Debug.Log("use 2 " + pickupEntity2);
        //    useSlot2 = pickupEntity2;
        //    item2.useSlot2 = true;
        //}
        //manager.SetComponentData<PowerItemComponent>(pickupEntity2, item2);


    }

    void Update()
    {
        UpdateItemEntities();
        GameObject selected = eventSystem.currentSelectedGameObject;

        //if (entity == Entity.Null || manager == default) return;
        //bool hasComponent = manager.HasComponent(entity, typeof(PickupMenuComponent));
        //if (hasComponent == false) return;

        ////move to system update below
        ////bool stateChange = manager.GetComponentData<PickupMenuComponent>(entity).menuStateChanged;
        ////
        ////  if (stateChange == true)
        ////  {
        //var puMenu = manager.GetComponentData<PickupMenuComponent>(entity);
        ////skillTreeMenu.menuStateChanged = false;
        //manager.SetComponentData(entity, puMenu);

        //if (manager.GetComponentData<PickupMenuComponent>(entity).showMenu)
        //{
        //    ShowMenu();
        //}
        //else
        //{
        //    HideMenu();
        //}
        //EnableButtons();
        //   }
        //ShowLabels();
        //FillUseLabels();
    }

    //  void FillUseLabels()
    //  {
    // for (int  i = 0;  i < useItemComponents.Count;  i++)
    //  {

    //  }



    // }
    public void EnableButtons()
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

    public void ShowLabels()
    {


        for (int i = 0; i < pickuplabel.Length; i++)
        {
            if (i >= powerItemComponents.Count)
            {
                pickuplabel[i].text = "";//enable disable later
            }
            else
            {
                pickuplabel[i].text = powerItemComponents[i].description.ToString();
            }
        }

        uselabel[0].text = powerItemComponents[useSlotIndex1].description.ToString();
        uselabel[1].text = powerItemComponents[useSlotIndex2].description.ToString();



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

    public void SelectedPower(int index)
    {
        selectedPower = index;
        Debug.Log("sel " + selectedPower);
    }

    public void AssignSelectedPower(int use_index)
    {
        if (use_index <= 0) return;
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
                item.useSlot1 = true;
            }
            else if (use_index == 2)
            {
                useSlotIndex1 = selectedPower;
                useSlot2 = pickupEntity;
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
        PowerItemComponent[] useItems = new PowerItemComponent[2];




        for (int i = 0; i < itemGroup.Length; i++)
        {
            powerItems.Add(itemGroup[i]);

        }

        powerItems.Sort();

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
            ecb.AddComponent<UseItem1>(pickupEntity1);
            //Debug.Log("add use1 " + pickupEntity1);
        }

        if (pickupEntity2 != Entity.Null && pickedUp2 == true)
        {
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
