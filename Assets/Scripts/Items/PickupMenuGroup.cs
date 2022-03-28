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
    public int usedItem;
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
    public int Remain;//how many still available left to choose from pick up list
    public Sprite Image;

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
    public static PowerItemComponent[] useItemComponents = new PowerItemComponent[2];

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
    private Button[] useButton = new Button[2];//blank button with image

    [SerializeField]
    private TextMeshProUGUI[] gameViewUse = new TextMeshProUGUI[2];
    [SerializeField]
    private Button[] gameViewUseButton = new Button[2];//blank button with image
    [SerializeField]

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
        //if (powerItemComponents.Count == 1)
        //{
        //    useButtons[1].interactable = false;
        //}
        if (powerItemComponents.Count == 0)
        {
            useButtons[0].interactable = false;
            useButtons[1].interactable = false;
        }


        //useButton[0].interactable = true;
        //useButton[1].interactable = true;



    }


    public void Count()
    {

        //powerItemComponents.Select(x => x.pickupType).Distinct();
        useSlotIndex1 = -1;
        useSlotIndex2 = -1;

        for (int i = 0; i < menuPickupItem.Length; i++)
        {
            //menuPickupItem[i].CurrentIndex = 0;
        }


        var tempItems = new List<PowerItemComponent>(powerItemComponents);
        powerItemComponents.Clear();
        int powerUps = 3;

        int first = -1;
        for (int j = 0; j < powerUps; j++)
        {
            var ico = menuPickupItem[j].Image;
            var _currentIndex = menuPickupItem[j].CurrentIndex;
            //var menuList = menuPickupItem[j];
            menuPickupItem[j] = new MenuPickupItemData { Image = ico };
            menuPickupItem[j].Image = ico;
            first = -1;
            int count = 0;
            for (int i = 0; i < tempItems.Count; i++)
            {
                if ((int)tempItems[i].pickupType == j + 1 && tempItems[i].useSlot1 == false && tempItems[i].useSlot2 == false)
                {
                    if (first == -1)
                    {
                        first = i;
                    }
                    //var item = tempItems[i];
                    menuPickupItem[j].ItemEntity[count] = tempItems[i].pickupEntity;
                    menuPickupItem[j].ItemIndex[count] = tempItems[i].pickupEntity.Index;
                    count += 1;
                    menuPickupItem[j].Count = count;
                    //menuPickupItem[j].SlotUsed[count] = 0;
                    //    }

                }

            }

            if (first >= 0)
            {
                var item = tempItems[first];
                item.count = count;
                item.menuIndex = j;
                tempItems[first] = item;
                powerItemComponents.Add(tempItems[first]);
                manager.SetComponentData<PowerItemComponent>(item.pickupEntity, item);
            }
            else
            {
                powerItemComponents.Add(new());
            }

        }


        ShowLabels();


    }


    public void ShowLabels()
    {
        // Debug.Log("pu count " + powerItemComponents.Count);


        for (int i = 0; i < uselabel.Length; i++)
        {
            uselabel[i].text = "";
            gameViewUseButton[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < pickuplabel.Length; i++)
        {
            pickuplabel[i].text = "";
        }

        for (int i = 0; i < pickuplabel.Length; i++)
        {

            if (i < powerItemComponents.Count && powerItemComponents[i].count > 0)
            {
                pickuplabel[i].text = powerItemComponents[i].description.ToString() + " " + powerItemComponents[i].count;
                int index = powerItemComponents[i].menuIndex;
                Debug.Log("i " + i + " " + index);
                buttons[i + 1].GetComponent<Image>().sprite = menuPickupItem[index].Image;
                buttons[i + 1].interactable = true;
            }
            else
            {
                buttons[i + 1].interactable = false;
                buttons[i + 1].GetComponent<Image>().sprite = null;
            }
        }




        GameLabels();

        //buttons[1].Select();



    }

    public void GameLabels()
    {
        //Debug.Log("use length " + useItemComponents.Length);
        useButton[0].interactable = false;
        useButton[1].interactable = false;

        for (int i = 0; i < useItemComponents.Length; i++)
        {
            Debug.Log("use 0 " + useItemComponents[i].useSlot1);
            Debug.Log("use 1 " + useItemComponents[i].useSlot2);
            if (useItemComponents[i].useSlot1 && i == 0)//deletes entity after use so now this is still true :( useslotindex = selected power
            {
                useButton[0].interactable = true;
                gameViewUseButton[0].gameObject.SetActive(true);
                uselabel[0].text = useItemComponents[0].description.ToString();
                gameViewUseButton[0].GetComponent<Image>().sprite = menuPickupItem[useItemComponents[0].menuIndex].Image;
                useButton[0].GetComponent<Image>().sprite = menuPickupItem[useItemComponents[0].menuIndex].Image;
                //uselabel[0].GetComponent<Image>().sprite = menuPickupItem[useItemComponents[0].menuIndex].Image;
            }

            if (useItemComponents[i].useSlot2 && i == 1)//deletes entity after use so now this is still true :( useslotindex = selected power
            {
                useButton[1].interactable = true;
                gameViewUseButton[1].gameObject.SetActive(true);
                uselabel[1].text = useItemComponents[1].description.ToString();
                gameViewUseButton[1].GetComponent<Image>().sprite = menuPickupItem[useItemComponents[1].menuIndex].Image;
                useButton[1].GetComponent<Image>().sprite = menuPickupItem[useItemComponents[1].menuIndex].Image;
                //uselabel[1].GetComponent<Image>().sprite = menuPickupItem[useItemComponents[1].menuIndex].Image;
            }



        }

        for (int i = 0; i < gameViewUse.Length; i++)
        {

            gameViewUse[i].text = uselabel[i].text;
            // Debug.Log(gameViewUse[i].text + " game text " + i);

        }

    }

    void ButtonClickedIndex(int index)
    {
        buttonClickedIndex = index;
        Debug.Log("btn idx " + buttonClickedIndex);

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

    public void RemoveUsePower(int use_power)
    {
        //Count();
        int index = useItemComponents[use_power - 1].menuIndex;
        if (index >= powerItemComponents.Count) return;
        var power = powerItemComponents[index];
        if (use_power == 1)
        {
            power.useSlot1 = false;
            power.useSlot2 = false;
            manager.RemoveComponent<UseItem1>(power.pickupEntity);
        }
        else if (use_power == 2)
        {
            power.useSlot1 = false;
            power.useSlot2 = false;
            manager.RemoveComponent<UseItem2>(power.pickupEntity);
        }
        manager.SetComponentData<PowerItemComponent>(power.pickupEntity, power);
        useItemComponents[use_power - 1] = new();
        //powerItemComponents[index] = new();
        Count();
        ShowLabels();


    }

    public void AssignSelectedPower(int use_index)
    {
        //RemoveUsePower(use_index);
        if (powerItemComponents.Count == 0) return;
        int menuIndex = powerItemComponents[selectedPower].menuIndex;
        //int current_index = menuPickupItem[menuIndex].CurrentIndex;
        int current_index = 0;
        int count = menuPickupItem[menuIndex].Count;
        if (use_index <= 0 || current_index >= count) return;
        if (entity == Entity.Null || manager == default) return;
        var pickupEntity = menuPickupItem[menuIndex].ItemEntity[current_index];
        //int slot_used = menuPickupItem[menuIndex].SlotUsed[current_index];
        int slot_used = 0;
        bool pickedUp = powerItemComponents[selectedPower].itemPickedUp;
        var item = powerItemComponents[selectedPower];



        if (pickupEntity != Entity.Null && pickedUp == true && slot_used == 0)
        {
            Debug.Log("use  " + pickupEntity + " " + use_index);
            if (use_index == 1 && useSlotIndex1 >= 0)
            {
                var useItem = powerItemComponents[useSlotIndex1];
                useItem.useSlot1 = false;
                powerItemComponents[useSlotIndex1] = useItem;
            }
            else if (use_index == 2 && useSlotIndex1 >= 0)
            {
                var useItem = powerItemComponents[useSlotIndex2];
                useItem.useSlot2 = false;
                powerItemComponents[useSlotIndex2] = useItem;
            }



            //if (use_index == 1 && slot_used != 1 && item.useSlot2 == false)
            if (use_index == 1 && item.useSlot2 == false)
            {
                slot_used = 1;
                useSlotIndex1 = selectedPower;
                useSlot1 = pickupEntity;
                item.useSlot1 = true;
                useItemComponents[0] = item;
            }

            if (use_index == 2 && item.useSlot1 == false)//item.useslot only one for each item (pickup entity)
                                                         //if (use_index == 2 && slot_used != 2 && item.useSlot1 == false)//item.useslot only one for each item (pickup entity)
            {
                slot_used = 2;
                useSlotIndex2 = selectedPower;
                useSlot2 = pickupEntity;
                item.useSlot2 = true;
                useItemComponents[1] = item;
            }

            if (slot_used > 0)
            {
                UseUpdated = true;
                //menuPickupItem[menuIndex].SlotUsed[current_index] = slot_used;
                //current_index++;
                powerItemComponents[selectedPower] = item;
                //menuPickupItem[menuIndex].Remain -= 1;
                manager.SetComponentData<PowerItemComponent>(pickupEntity, item);
                //Count();
            }


        }
        Count();
        ShowLabels();


    }

    private void ExitClicked()
    {


    }


    public void ShowMenu()
    {

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

        //powerItems.Sort(new PowerItemIndexComparer());

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
            if (pickupMenu.usedItem > 0)
            {
                pickupMenu.usedItem = 0;
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
            if (use1_pressed && HasComponent<PowerItemComponent>(use1_entity))
            {
                ecb.AddComponent<ImmediateUseComponent>(use1_entity);
                var useItem1 = PickupMenuGroup.useItemComponents[0];
                useItem1 = new();
                PickupMenuGroup.useItemComponents[0] = useItem1;
            }

        }
        if (hasUse2)
        {
            var use2_entity = GetSingletonEntity<UseItem2>();
            bool use2_pressed = ReInput.players.GetPlayer(0).GetButtonDown("Use2");
            if (use2_pressed && HasComponent<PowerItemComponent>(use2_entity))
            {
                ecb.AddComponent<ImmediateUseComponent>(use2_entity);
                var useItem2 = PickupMenuGroup.useItemComponents[1];
                useItem2 = new();
                PickupMenuGroup.useItemComponents[1] = useItem2;
            }

        }


        ecb.Playback(EntityManager);
        ecb.Dispose();

    }

}
