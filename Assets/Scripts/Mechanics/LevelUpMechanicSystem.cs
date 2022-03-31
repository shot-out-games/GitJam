using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class LevelUpMechanicSystem : SystemBase
{

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {

        Entities.ForEach(
            (
                Entity e,
                ref ControlBarComponent controlBar,
                ref LevelUpMechanicComponent levelUpMechanic,
                ref RatingsComponent ratings,
                in HealthComponent health,
                in SkillTreeComponent skillTreeComponent

            ) =>
            {
                int pointsNeeded = skillTreeComponent.PointsNextLevel * skillTreeComponent.CurrentLevel;


                float pct = skillTreeComponent.CurrentLevelXp / (float)pointsNeeded;
                controlBar.value = pct;


            }


        ).ScheduleParallel();



    }
}

