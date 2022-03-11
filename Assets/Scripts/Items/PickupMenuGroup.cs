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
    public List<PowerItemComponent> powerItemComponents = new List<PowerItemComponent>();
    public PowerItemComponent[] useItemComponents = new PowerItemComponent[2];

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

    //Player player;

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

    //public void UpdateUseEntities()
    //{

    //    if (entity == Entity.Null || manager == default) return;
    //    Entity pickupEntity1 = useItemComponents[0].pickupEntity;
    //    bool pickedUp1 = useItemComponents[0].itemPickedUp;
    //    if (pickupEntity1 != Entity.Null && pickedUp1 == true)
    //    {
    //        manager.AddComponent<UseItem1>(pickupEntity1);
    //    }
    //    Entity pickupEntity2 = useItemComponents[1].pickupEntity;
    //    bool pickedUp2 = useItemComponents[1].itemPickedUp;
    //    if (pickupEntity2 != Entity.Null && pickedUp2 == true)
    //    {
    //        manager.AddComponent<UseItem2>(pickupEntity2);
    //    }


    //}

    void Update()
    {

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

        for (int i = 0; i < useItemComponents.Length; i++)
        {
            //Debug.Log("use " + uselabel.Length + " " + i);
            //if (uselabel.Length == 0) continue;
            if (useItemComponents[i].buttonAssigned == false && i < powerItemComponents.Count)
            {
                useItemComponents[i].buttonAssigned = true;
                useItemComponents[i] = powerItemComponents[i];
            }
            uselabel[i].text = useItemComponents[i].description.ToString();
            //uselabel[i].text = "test" + useItemComponents[i].itemPickedUp;

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

    protected override void OnCreate()
    {
    }


    protected override void OnUpdate()
    {
        var itemQuery = GetEntityQuery(ComponentType.ReadOnly<PowerItemComponent>());
        var itemList = itemQuery.ToEntityArray(Allocator.Persistent);
        var itemGroup = itemQuery.ToComponentDataArray<PowerItemComponent>(Allocator.Persistent);

        List<PowerItemComponent> powerItems = new List<PowerItemComponent>();
        for (int i = 0; i < itemGroup.Length; i++)
        {
            if (HasComponent<PlayerComponent>(itemGroup[i].pickedUpActor))
            {
                if (itemGroup[i].itemPickedUp)
                {
                    powerItems.Add(itemGroup[i]);
                }

            }

        }

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.WithoutBurst().ForEach
       (
           (
               PickupMenuGroup pickupMenuGroup,
               in Entity e,
               in PickupMenuComponent pickupMenu

           ) =>
           {

               pickupMenuGroup.powerItemComponents = powerItems;

             
               if (pickupMenu.showMenu)
               {
                   pickupMenuGroup.ShowMenu();
               }
               else
               {
                   pickupMenuGroup.HideMenu();
               }

               Entity pickupEntity1 = pickupMenuGroup.useItemComponents[0].pickupEntity;
               bool pickedUp1 = pickupMenuGroup.useItemComponents[0].itemPickedUp;
               if (pickupEntity1 != Entity.Null && pickedUp1 == true && HasComponent<UseItem1>(pickupEntity1) == false)
               {
                   ecb.AddComponent<UseItem1>(pickupEntity1);
               }
               Entity pickupEntity2 = pickupMenuGroup.useItemComponents[1].pickupEntity;
               bool pickedUp2 = pickupMenuGroup.useItemComponents[1].itemPickedUp;
               if (pickupEntity2 != Entity.Null && pickedUp2 == true && HasComponent<UseItem2>(pickupEntity2) == false)
               {
                   ecb.AddComponent<UseItem2>(pickupEntity2);
               }

               pickupMenuGroup.EnableButtons();
               pickupMenuGroup.ShowLabels();
           }

       ).Run();



        Entities.WithoutBurst().ForEach
        (
            (
                ref PickupMenuComponent pickupMenu

            ) =>
            {
                var dpadR = ReInput.players.GetPlayer(0).GetButtonDown("DpadR");
                var select = ReInput.players.GetPlayer(0).GetButtonDown("Select");

                // var inputController = GetComponent<InputControllerComponent>(inputEntityList[0]);


                //Debug.Log("pickup menu " + dpadR);
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



        ecb.Playback(EntityManager);
        ecb.Dispose();
        //playerSkillSets.Dispose();
        itemGroup.Dispose();
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
