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
    public bool usedItem;
}

public class PowerItemClass
{
    public Entity pickedUpActor;
}

[Serializable]
public class MenuPickupItemData
{
    public int[] ItemIndex = new int[4];
    public int[] SlotUsed = new int[4];
    public Entity[] ItemEntity = new Entity[4];
    public int CurrentIndex;
    public int Count;

}

public class PickupMenuGroup : MonoBehaviour, IConvertGameObjectToEntity
{

    //public static event Action<bool> PauseGame;
    public static event Action<bool> HideSubscriberMenu;
    public static bool UseUpdated;

    private EntityManager manager;
    public Entity entity;
    public static List<PowerItemComponent> powerItemComponents = new List<PowerItemComponent>();
    //public static List<PowerItemComponent> tempItems = new List<PowerItemComponent>();
    public PowerItemComponent[] useItemComponents = new PowerItemComponent[2];

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

    [SerializeField]
    private TextMeshProUGUI[] gameViewUse;

    //public int speedPts;
    //public int powerPts;
    //public int chinPts;
    //public int availablePoints;

    private int buttonClickedIndex;
    public static bool UpdateMenu = true;
    private bool[] inUsedItemList = new bool[30];
    public MenuPickupItemData[] menuPickupItem = new MenuPickupItemData[10];//10 items (buttons) max currently

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




    }


    public void Count()
    {

        //powerItemComponents.Select(x => x.pickupType).Distinct();
        useSlotIndex1 = -1;
        useSlotIndex2 = -1;

        var tempItems = new List<PowerItemComponent>(powerItemComponents);
        //for (int i = 0; i < tempItems.Count; i++)
        //{
        //    if (tempItems[i].useSlot1)
        //    {
        //        useSlotIndex1 = i;
        //    }
        //    else if (tempItems[i].useSlot2)
        //    {
        //        useSlotIndex2 = i;
        //    }
        //}

        //Debug.Log("temp count " + tempItems.Count);
        powerItemComponents.Clear();
        //Debug.Log("temp count after " + tempItems.Count);
        int powerUps = 2;
        //if (powerUps > tempItems.Count) powerUps = tempItems.Count;

        int first = -1;
        for (int j = 0; j < powerUps; j++)
        {
            menuPickupItem[j] = new MenuPickupItemData();
            first = -1;
            int count = 0;
            for (int i = 0; i < tempItems.Count; i++)
            {
                if ((int)tempItems[i].pickupType == j + 1)
                {
                    if (first == -1) first = i;
                    //var item = tempItems[i];
                    menuPickupItem[j].ItemEntity[count] = tempItems[i].pickupEntity;
                    menuPickupItem[j].ItemIndex[count] = tempItems[i].pickupEntity.Index;
                    count += 1;
                    menuPickupItem[j].Count = count;
                    //tempItems[i] = item;
                    //Debug.Log("count " + item.count);
                    //Debug.Log("Entity " + tempItems[i].pickupEntity);
                }

            }
            if (first >= 0)
            {
                var item = tempItems[first];
                item.count = count;
                tempItems[first] = item;
                powerItemComponents.Add(tempItems[first]);
            }

        }


        //ShowLabels();


    }


    public void ShowLabels()
    {
        // Debug.Log("pu count " + powerItemComponents.Count);


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
            pickuplabel[i].text = powerItemComponents[i].description.ToString() + " " + powerItemComponents[i].count;
            // Debug.Log("pu text show labels " + pickuplabel[i].text);
        }


        //Debug.Log("use length " + useItemComponents.Length);
        for (int i = 0; i < useItemComponents.Length; i++)
        {
            //Debug.Log("use " + useItemComponents[i].description.ToString());
            for (int j = 0; j < powerItemComponents.Count; j++)
            {
                if (powerItemComponents[j].useSlot1 && i == 0)//deletes entity after use so now this is still true :( useslotindex = selected power
                {
                    uselabel[0].text = useItemComponents[0].description.ToString();
                }

                if (powerItemComponents[j].useSlot2 && i == 1)//deletes entity after use so now this is still true :( useslotindex = selected power
                {
                    uselabel[1].text = useItemComponents[1].description.ToString();
                }

            }

        }

        GameLabels();

        buttons[0].Select();



    }

    public void GameLabels()
    {
        for (int i = 0; i < gameViewUse.Length; i++)
        {

            gameViewUse[i].text = uselabel[i].text;
            // Debug.Log(gameViewUse[i].text + " game text " + i);

        }
        //Canvas.ForceUpdateCanvases();
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
        Count();
        int current_index = menuPickupItem[selectedPower].CurrentIndex;
        int count = menuPickupItem[selectedPower].Count;
        if (use_index <= 0 || powerItemComponents.Count < use_index || current_index > 1) return;
        if (entity == Entity.Null || manager == default) return;
        //Debug.Log("assign " + selectedPower);
        var pickupEntity = menuPickupItem[selectedPower].ItemEntity[current_index];
        int slot_used = menuPickupItem[selectedPower].SlotUsed[current_index];

        //Entity pickupEntity = powerItemComponents[selectedPower].pickupEntity;
        bool pickedUp = powerItemComponents[selectedPower].itemPickedUp;
        //var item = manager.GetComponentData<PowerItemComponent>(pickupEntity);
        var item = powerItemComponents[selectedPower];
        
        //item1.useSlot1 = false;
        if (pickupEntity != Entity.Null && pickedUp == true && slot_used == 0)
        {
            Debug.Log("use  " + pickupEntity + " " + use_index);


            slot_used = use_index;
            UseUpdated = true;


            if (use_index == 1)
            {

                useSlotIndex1 = selectedPower;
                useSlot1 = pickupEntity;
                //item.useSlot2 = false;
                item.useSlot1 = true;
                useItemComponents[0] = item;
            }

            if (use_index == 2)
            {
                useSlotIndex2 = selectedPower;
                useSlot2 = pickupEntity;
                //item.useSlot1 = false;
                item.useSlot2 = true;
                useItemComponents[1] = item;
            }
            menuPickupItem[selectedPower].SlotUsed[current_index] = slot_used;
            current_index++;
            menuPickupItem[selectedPower].CurrentIndex = current_index;
            powerItemComponents[selectedPower] = item;
            manager.SetComponentData<PowerItemComponent>(pickupEntity, item);
            ShowLabels();


        }



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
        //PauseGame?.Invoke(true);
        HideSubscriberMenu?.Invoke(false);
        GameInterface.Paused = true;
        GameInterface.StateChange = true;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Count();

    }

    public void HideMenu()
    {
        //PauseGame?.Invoke(false);
        GameInterface.Paused = false;
        GameInterface.StateChange = true;
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



        List<PowerItemComponent> powerItems = new();
        List<PowerItemComponent> useItems = new();

        for (int i = 0; i < 2; i++)
        {
            useItems.Add(new PowerItemComponent());
        }

        for (int i = 0; i < itemGroup.Length; i++)
        {
            if (itemGroup[i].itemPickedUp)
            {
                powerItems.Add(itemGroup[i]);
                //Debug.Log("use slot1 i " + i + " " + itemGroup[i].useSlot1);
                //Debug.Log("use slot2 i " + i + " " + itemGroup[i].useSlot2);
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
                if (itemGroup[i].useSlot2)
                {
                    useItems[1] = itemGroup[i];
                }
            }

        if (PickupMenuGroup.UseUpdated)
        {
            PickupMenuGroup.UseUpdated = false;

            Entity pickupEntity1 = useItems[0].pickupEntity;
            bool pickedUp1 = useItems[0].itemPickedUp;
            bool use1 = useItems[0].useSlot1;

            Entity pickupEntity2 = useItems[1].pickupEntity;
            bool pickedUp2 = useItems[1].itemPickedUp;
            bool use2 = useItems[1].useSlot2;




            if (pickupEntity1 != Entity.Null && pickedUp1 == true && use1)
            {
                var power = GetComponent<PowerItemComponent>(pickupEntity1);
                power.useSlot1 = true;
                power.useSlot2 = false;
                ecb.SetComponent<PowerItemComponent>(pickupEntity1, power);
                ecb.RemoveComponent<UseItem2>(pickupEntity1);
                ecb.AddComponent<UseItem1>(pickupEntity1);
                //Debug.Log("add use1 " + pickupEntity1);
            }

            if (pickupEntity2 != Entity.Null && pickedUp2 == true && use2)
            {
                var power = GetComponent<PowerItemComponent>(pickupEntity2);
                power.useSlot2 = true;
                power.useSlot1 = false;
                ecb.SetComponent<PowerItemComponent>(pickupEntity2, power);
                ecb.RemoveComponent<UseItem1>(pickupEntity2);
                ecb.AddComponent<UseItem2>(pickupEntity2);
                //Debug.Log("add use2 " + pickupEntity2);
            }

        }

        itemGroup.Dispose();
        itemList.Dispose();

        var pickupMenu = GetSingleton<PickupMenuComponent>();


        pickupMenu.menuStateChanged = false;

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



        Entities.WithoutBurst().ForEach((PickupMenuGroup pickupMenuGroup) =>
        {
            if (pickupMenu.usedItem)
            {
                pickupMenu.usedItem = false;
                pickupMenu.menuStateChanged = true;
                pickupMenuGroup.Count();
                pickupMenuGroup.ShowLabels();
                Debug.Log("pu sho " + pickupMenu.showMenu);
            }
            if (pickupMenu.menuStateChanged == false) return;


            if (pickupMenu.showMenu)
            {
                pickupMenuGroup.ShowMenu();
                pickupMenuGroup.EnableButtons();
                pickupMenuGroup.ShowLabels();
            }
            else
            {
                pickupMenuGroup.HideMenu();
            }
        }

        ).Run();



        //SetSingleton<PickupMenuComponent>(pickupMenu);
        SetSingleton(pickupMenu);




        ecb.Playback(EntityManager);
        ecb.Dispose();

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
            //Debug.Log("use1 " + use1_pressed);
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
            //Debug.Log("use2 " + use2_pressed);
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
