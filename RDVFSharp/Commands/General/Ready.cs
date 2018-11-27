﻿using FChatSharpLib.Entities.Plugin;
using FChatSharpLib.Entities.Plugin.Commands;
using RDVFSharp.DataContext;
using RDVFSharp.Entities;
using RDVFSharp.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDVFSharp.Commands
{
    public class Ready : BaseCommand<RendezvousFighting>
    {
        public override string Description => "Sets a player as ready.";

        public override void ExecuteCommand(string character ,string[] args, string channel)
        {
            if (Plugin.CurrentBattlefield.IsActive || (Plugin.FirstFighter != null && Plugin.SecondFighter != null))
            {
                throw new FightInProgress();
            }

            var fighter = Plugin.Context.Fighters.Find(character);

            if(fighter == null)
            {
                throw new FighterNotFound(character);
            }

            var actualFighter = new Fighter(fighter, Plugin.CurrentBattlefield);

            if(Plugin.FirstFighter == null && Plugin.SecondFighter != null)
            {
                Plugin.FirstFighter = Plugin.SecondFighter;
                Plugin.SecondFighter = null;
            }

            if (Plugin.FirstFighter == null)
            {
                Plugin.FirstFighter = actualFighter;
                Plugin.FChatClient.SendMessageInChannel($"{actualFighter.Name} joined the fight!", channel);
            }
            else
            {
                Plugin.SecondFighter = actualFighter;

                if (!Plugin.CurrentBattlefield.IsActive && (Plugin.FirstFighter != null && Plugin.SecondFighter != null))
                {
                    Plugin.FChatClient.SendMessageInChannel($"{actualFighter.Name} accepted the challenge! Let's get it on!", channel);
                    Plugin.CurrentBattlefield.InitialSetup(Plugin.FirstFighter, Plugin.SecondFighter);
                }
            }

        }
    }
}
