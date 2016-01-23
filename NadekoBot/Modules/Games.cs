﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Modules;
using NadekoBot.Extensions;

namespace NadekoBot.Modules
{
    class Games : DiscordModule
    {
        public Games() : base() {
            commands.Add(new Trivia());
            //commands.Add(new SpeedTyping());
        }

        public override void Install(ModuleManager manager)
        {
            manager.CreateCommands("", cgb =>
            {
                commands.ForEach(cmd => cmd.Init(cgb));
                cgb.CreateCommand(">")
                    .Description("Attack a person. Supported attacks: 'splash', 'strike', 'burn', 'surge'.\n**Usage**: > strike @User")
                    .Parameter("attack_type",Discord.Commands.ParameterType.Required)
                    .Parameter("target",Discord.Commands.ParameterType.Required)
                    .Do(async e =>
                    {
                        
                        var usr = e.Server.FindUsers(e.GetArg("target")).FirstOrDefault();
                        var usrType = GetType(usr.Id);
                        string response = "";
                        int dmg = GetDamage(usrType, e.GetArg("attack_type").ToLowerInvariant());
                        response = e.GetArg("attack_type") + (e.GetArg("attack_type")=="splash"?"es ":"s ") + usr.Mention + " for " + dmg+".\n";
                        if (dmg >= 65)
                        {
                            response += "It's super effective!";
                        }
                        else if (dmg <= 35) {
                            response += "Ineffective!";
                        }
                        await e.Send(NadekoBot.botMention + " " + response);
                    });

                cgb.CreateCommand("poketype")
                    .Parameter("target", Discord.Commands.ParameterType.Required)
                    .Description("Gets the users element type. Use this to do more damage with strike")
                    .Do(async e =>
                    {
                        var usr = e.Server.FindUsers(e.GetArg("target")).FirstOrDefault();
                        if (usr == null) {
                            await e.Send("No such person.");
                        }

                        await e.Send(usr.Name + "'s type is " + GetType(usr.Id));
                    });
            });
        }

        private int GetDamage(PokeType targetType, string v)
        {
            var rng = new Random();
            switch (v)
            {
                case "splash": //water
                    if (targetType == PokeType.FIRE)
                        return rng.Next(65, 100);
                    else if (targetType == PokeType.ELECTRICAL)
                        return rng.Next(0, 35);
                    else
                        return rng.Next(40, 60);
                case "strike": //grass
                    if (targetType == PokeType.ELECTRICAL)
                        return rng.Next(65, 100);
                    else if (targetType == PokeType.FIRE)
                        return rng.Next(0, 35);
                    else
                        return rng.Next(40, 60);
                case "burn": //fire
                case "flame":
                    if (targetType == PokeType.GRASS)
                        return rng.Next(65, 100);
                    else if (targetType == PokeType.WATER)
                        return rng.Next(0, 35);
                    else
                        return rng.Next(40, 60);
                case "surge": //electrical
                case "electrocute":
                    if (targetType == PokeType.WATER)
                        return rng.Next(65, 100);
                    else if (targetType == PokeType.GRASS)
                        return rng.Next(0, 35);
                    else
                        return rng.Next(40, 60);
                default:
                    return 0;
            }
        }

        private PokeType GetType(ulong id) {
            var remainder = id % 10;
            if (remainder < 3)
                return PokeType.WATER;
            else if (remainder >= 3 && remainder < 5)
            {
                return PokeType.GRASS;
            }
            else if (remainder >= 5 && remainder < 8)
            {
                return PokeType.FIRE;
            }
            else {
                return PokeType.ELECTRICAL;
            }
        }

        private enum PokeType
        {
            WATER, GRASS, FIRE, ELECTRICAL
        }
    }
}
