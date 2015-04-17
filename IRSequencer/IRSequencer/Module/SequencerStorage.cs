﻿using System;
using UnityEngine;
using IRSequencer.API;
using IRSequencer.Gui;
using IRSequencer.Core;

namespace IRSequencer.Module
{
    public class SequencerStorage : PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "serializedSequences")]
        public string serializedSequences = "";

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Save Sequences")]
        public void SaveSequencesEvent ()
        {
            if (Sequencer.Instance)
            {
                var message = "";

                foreach(Sequence s in Sequencer.Instance.sequences)
                {
                    message += s.Serialize () + "$";
                }

                ScreenMessages.PostScreenMessage (message);

                serializedSequences = message;
            }
                
        }

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Load Sequences")]
        public void LoadSequencesEvent ()
        {
            Sequencer.Instance.sequences.Clear ();

            var chunks = serializedSequences.Split ('$');

            foreach (string serializedSequence in chunks)
            {
                Sequence s;
                if (TryParseSequence(serializedSequence, out s))
                {
                    
                }
            }
        }

        public bool TryParseSequence(string s, out Sequence seq)
        {
            
        }

        public bool TryParseBasicCommand(string s, out BasicCommand bc)
        {
            var chunks = s.Split ('|');

            if (chunks.Length < 8)
            {
                Logger.Log("Failed parsing BasicCommand from string = " + s, Logger.Level.Debug);
                bc = null;
                return false;
            }

            bc = new BasicCommand(false);

            if (chunks[0] != "null")
            {
                uint servoUID;
                if (!uint.TryParse (chunks [0], out servoUID)) 
                {
                    bc = null;
                    return false;
                }
                else
                {
                    var servoPart = FlightGlobals.ActiveVessel.Parts.Find (p => p.flightID == servoUID);
                    if (part == null)
                    {
                        bc = null;
                        return false;
                    }
                    var servo = new IRWrapper.IServo (servoPart.Modules ["MuMechToggle"]);

                }


            }

            if (!bool.TryParse(chunks[3], out bc.wait))
            {
                bc = null;
                return false;
            }

            if (!float.TryParse(chunks[4], out bc.waitTime))
            {
                bc = null;
                return false;
            }

            if (!int.TryParse(chunks[5], out bc.gotoIndex))
            {
                bc = null;
                return false;
            }

            if (!int.TryParse (chunks [6], out bc.gotoCommandCounter)) 
            {
                bc = null;
                return false;
            } 
            else
                bc.gotoCounter = bc.gotoCommandCounter;

            int temp = 0;
            if (!int.TryParse (chunks [7], out temp)) 
            {
                bc = null;
                return false;
            } 
            else
                bc.ag = (KSPActionGroup)temp;

            return true;
        }

        public SequencerStorage ()
        {
        }

        //returns basic information on kOSProcessor module in Editor
        public override string GetInfo()
        {
            string moduleInfo = "Provides Storage for Seqences";

            return moduleInfo;
        }

        public override void OnStart(StartState state)
        {
            if (state == StartState.Editor)
            {
                
            }

            Logger.Log(string.Format("OnStart: {0}", state), Logger.Level.Debug);
        }


        public override void OnLoad(ConfigNode node)
        {
            try
            {
                // KSP Seems to want to make an instance of my partModule during initial load
                if (vessel == null) return;

                base.OnLoad(node);
            }
            catch (Exception ex) //Intentional Pokemon, if exceptions get out of here it can kill the craft
            {
                Logger.Log("ONLOAD Exception: " + ex.TargetSite);
            }
        }

        public override void OnSave(ConfigNode node)
        {
            try
            {
                base.OnSave(node);
            }
            catch (Exception ex) //Intentional Pokemon, if exceptions get out of here it can kill the craft
            {
                Logger.Log("ONSAVE Exception: " + ex.TargetSite);
            }
        }

    }
}


