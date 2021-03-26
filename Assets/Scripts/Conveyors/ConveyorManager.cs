﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Pathfinder.tiles;
using HordeSurvivalGame;
using ItemSystem;

namespace Conveyors
{
    public class ConveyorManager : MonoBehaviour
    {
        /* Developer Note:
         *  
         *  each conveyor will connect to neighboring towers and coveyors.
         *  each conveyor has 4 child gameobject that display a conveyor connecting in a particular direction
         *  these are often referred to as  the conveyor's "Arms" in most code relating to conveyors
         *  
         *  each arm will have an arrow displaying the direction of flow of items over the conveyor
         *  these can be overriden by the player in the UI. overridden arrows display as red.
         *  arrows displaying as white are controlled by a number of algorithms below, and will automatically adjust if more inputs and outputs are introduces
         *  arrows displaying as red are controlled by the player only, and may need to be ajusted by said player if new inputs or outputs are created
         */

        public enum IOController
        {
            None,
            Input,
            Output,
            AbsoluteInput // input and always will be input - no overriding
        }

        [SerializeField]
        private string UnityInspectorDebugging = ""; // this exists only for debugging things in unity.

        public GameObject[] conveyorArms; //  assigned in inspector. contains a reference to the conveyors arms in the following order: up, down, left, right
        public Vector2[] cardinalDirections; // assigned in inspector, commonIndex with conveyorArms. contains the direction each arm is facing
        public IOController[] armTypes; // automatically generated inputs and outputs for each conveyor arm
        public IOController[] CustomArmTypes; // user overridden inputs and outputs - so user can change the default functionality of a conveyor
        public IOController[] TrueArmTypes; // a combination of the above two, where CustomArmTypes Overrides armTypes if the value is not None


        public bool hasItemFilters = false;
        public List<Item>[] itemFilters = new List<Item>[4];

        public int visibleArms = 0; // used in algorithm decision making to decide which arms are inputs and which arms are outputs
        public int noOfInputs  = 0; //


        // initialise lists and flood arrays that require a default value
        void Start()
        {
            for(int i = 0; i < itemFilters.Length;i++)
            {
                itemFilters[i] = new List<Item>();
            }
            for(int i =0; i < CustomArmTypes.Length;i++)
            {
                CustomArmTypes[i] = IOController.None;
            }
        }

        // NOTE: the order in which each function is called is ### Super Important ### do not play with this function unless you know what you are doing
        void Update()
        {
            // reset count varables ready for next frame
            visibleArms = 0;
            // if a compatible tower is ajdacent, toggle visibility of the connecting arm
            for (int i = 0; i < conveyorArms.Length; i++)
            {
                UpdateArmVisibility(conveyorArms[i], cardinalDirections[i]);
            }

            // update the number of inputs for output management
            UpdateStats();

            // if the user has overridden any of the I/O controllers these new overridden values need to be taken into account
            CalculateTrueArmTypes();
            // based on the number of inputs and the visible arms, an algorithm is used to determine which arms are outputs
            OutputManagement();
            // now that the auto-generated outputs have been determined, we need to recalculate the true arm types, taking the new outputs into account
            CalculateTrueArmTypes();





            // update arrows on conveyors
            for (int i = 0; i < armTypes.Length; i++)
            {
                UpdateConveyorArrows(i);
            }

        }

        private void CalculateTrueArmTypes()
        {
            for(int i = 0; i < armTypes.Length;i++)
            {
                if(CustomArmTypes[i] != IOController.None)
                {
                    TrueArmTypes[i] = CustomArmTypes[i];
                }
                else
                {
                    TrueArmTypes[i] = armTypes[i];
                }
            }
        }

        /* algoritm that auto-magically decides which arms should be inputs and outputs
         *      How the Algoritm works:
         *          - when a conveyor connects to a tower that outputs items, the connecting arm is set to an input
         *          - if a conveyors neighbor is outputting items into the current conveyor, the arm that connects to the neighbor is labelled an input (their output = my input)
         *              - this is called "Conveyor Chaining"
         *          there are multiple types of conveyor that aid the program's decision making process
         *              - SConveyor - ("Straight" Conveyor with 2 arms and one input)
         *                  if there is only 2 active arms, and one is labelled an input, the other must be an output
         *              - YConveyor - (Conveyor with multiple inputs, but only 1 output)
         *                  like before, if there is only 3 active arms and  2 are labelled input, then the remaining arm must be an output
         *              - XConveyor - ( Conveyor with more than one output)
         *                  - there is a little more decision making for an X conveyor, as the program will have to decide which direction to send the items (which output do we use?)
         *                      - there are currently 2 modes:
         *                          - Split: items are split evenly between the 2 outputs
         *                          - Item Filter: an item filter is used to decide which item should go which direction.
         *                  - with an X conveyor, andything that is not an input will be labelled an output.
         */

        private void OutputManagement()
        {
            if (noOfInputs == 1 && visibleArms == 2)
                SConveyor();
            else if (noOfInputs == visibleArms - 1 && noOfInputs != 0)
                YConveyor();
            else if (noOfInputs < visibleArms - 1 && noOfInputs != 0)
                XConveyor();
            //else if (noOfInputs == 0)
            //{
            //    UnityInspectorDebugging = "NO Conveyor";
            //    Debug.Log("No conveyor");
            //    for (int i = 0; i < conveyorArms.Length; i++)
            //    {
            //        armTypes[i] = IOController.None;
            //    }
            //}
        }

        private void XConveyor()
        {
            UnityInspectorDebugging = "XConveyor";
            for (int i = 0; i < conveyorArms.Length; i++)
            {

                // if a conveyor arm is visible and NOT and input, make it an output
                if (TrueArmTypes[i] != IOController.Input)
                {
                    //Debug.Log("y conveyor");
                    if (conveyorArms[i].activeInHierarchy == true)
                    {

                        //Debug.Log("y conveyor active");
                        armTypes[i] = IOController.Output;
                    }
                }
            }
        }

        private void YConveyor()
        {
            UnityInspectorDebugging = "YConveyor";
            for (int i = 0; i < conveyorArms.Length; i++)
            {

                // if a conveyor arm is visible and NOT and input, make it an output
                if (TrueArmTypes[i] != IOController.Input)
                {
                    //Debug.Log("y conveyor");
                    if (conveyorArms[i].activeInHierarchy == true)
                    {

                        //Debug.Log("y conveyor active");
                        armTypes[i] = IOController.Output;
                    }
                }
            }
        }

        private void SConveyor()
        {
            UnityInspectorDebugging = "SConveyor";
            for (int i = 0; i < conveyorArms.Length;i++)
            {

                // if a conveyor arm is visible and NOT and input, make it an output
                if (TrueArmTypes[i] != IOController.Input)
                {
                    //Debug.Log("straight conveyor");
                    if (conveyorArms[i].activeInHierarchy == true)
                    {

                        //Debug.Log("straight conveyor active");
                        armTypes[i] = IOController.Output;
                    }
                }
            }
        }

        private void UpdateStats()
        {
            noOfInputs  = 0;
            foreach(IOController i in armTypes)
            {
                if (i == IOController.Input)  noOfInputs++;
            }
        }
        private void UpdateConveyorArrows(int i)
        {
            // if conveyor arm type set to none, hide the arrow
            if (TrueArmTypes[i] == IOController.None)
            {
                conveyorArms[i].GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
            // if conveyor arm type set to anything other than none, show the arrow again
            else
            {   
                conveyorArms[i].GetComponentInChildren<SpriteRenderer>().enabled = true;

                // if controller arm type is "Output", the arrow will point away from the centre of the conveyor
                if (TrueArmTypes[i] == IOController.Output)
                {
                    conveyorArms[i].GetComponentInChildren<SpriteRenderer>().gameObject.transform.localRotation = Quaternion.Euler(90, 90, 0);
                }

                // if controller arm type is "Input", the arrow will point towards the centre of the conveyor
                if (TrueArmTypes[i] == IOController.Input)
                {
                    conveyorArms[i].GetComponentInChildren<SpriteRenderer>().gameObject.transform.localRotation = Quaternion.Euler(90, 270, 0);
                }
            }
        }
        private void UpdateArmVisibility(GameObject arm, Vector2 cardinalDirection)
        {
            Tile t = Tile.Vector3ToTile(transform.position);
            //if (Tile.tileMap[t.x + (int)cardinalDirection.x, t.y + (int)cardinalDirection.y].GetTower() != null)
            arm.SetActive(CanConveyorConnect(t, cardinalDirection));
        }
        private bool CanConveyorConnect(Tile t, Vector2 cardinalDirection)
        {
            int armIndex = System.Array.IndexOf(cardinalDirections, cardinalDirection);
            armTypes[armIndex] = IOController.None;

            // dont bother running any of the below code if there is no tower adjacent to the conveyor
            if (Tile.tileMap[t.x + (int)cardinalDirection.x, t.y + (int)cardinalDirection.y].GetTower() == null)
            {
                return false;
            }
                
            // conveyor can connect to other conveyors
            ConveyorManager conv;
            if(Tile.tileMap[t.x + (int)cardinalDirection.x, t.y + (int)cardinalDirection.y].GetTower().TryGetComponent<ConveyorManager>(out conv))
            {
                ConveyorChaining(cardinalDirection, conv);
                visibleArms++;
                return true;
            }

            // conveyor can connect to mine
            Mine m;
            if (Tile.tileMap[t.x + (int)cardinalDirection.x, t.y + (int)cardinalDirection.y].GetTower().TryGetComponent<Mine>(out m))
            {
                int index = System.Array.IndexOf(cardinalDirections, cardinalDirection);
                armTypes[armIndex] = IOController.Input;
                Debug.Log("test");
                visibleArms++;
                return true;
            }

            return false;
        }
        private void ConveyorChaining(Vector2 cardinalDirection, ConveyorManager otherConveyor)
        {
            Vector2 oppositeDirection = new Vector2(-cardinalDirection.x, -cardinalDirection.y);
            int theirIndex = System.Array.IndexOf(otherConveyor.cardinalDirections, oppositeDirection);
            int myIndex = System.Array.IndexOf(cardinalDirections, cardinalDirection);
            if (otherConveyor.TrueArmTypes[theirIndex] == IOController.Output)
            {
                armTypes[myIndex] = IOController.Input;
            }
        }
    }
}
