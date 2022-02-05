using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;

namespace SandBox.Player
{

    public class PlayerRollSystem : SystemBase
    {


        protected override void OnUpdate()
        {

            Entities.WithoutBurst().ForEach(
                (
                    in InputControllerComponent inputController

                ) =>
                {

                    bool buttonB = inputController.buttonB_Pressed;


                }
            ).Run();

        }



    }
}


