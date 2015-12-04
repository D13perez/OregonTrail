﻿using System;
using System.Text;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Allows the player to alter how many 'miles' their vehicle will attempt to travel in a given day, this also changes
    ///     the rate at which random events that are considered bad will occur along with other factors in the simulation such
    ///     as making players more susceptible to disease and also making them hungry more often.
    /// </summary>
    [RequiredMode(Mode.Travel)]
    public sealed class ChangePaceState : StateProduct<TravelInfo>
    {
        /// <summary>
        ///     String builder for the changing pace text.
        /// </summary>
        private StringBuilder _pace;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ChangePaceState(IModeProduct gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnStatePostCreate()
        {
            base.OnStatePostCreate();

            _pace = new StringBuilder();
            _pace.Append($"{Environment.NewLine}Change pace{Environment.NewLine}");
            _pace.Append(
                $"(currently \"{GameSimulationApp.Instance.Vehicle.Pace}\"){Environment.NewLine}{Environment.NewLine}");
            _pace.Append($"The pace at which you travel{Environment.NewLine}");
            _pace.Append($"can change. Your choices are:{Environment.NewLine}{Environment.NewLine}");
            _pace.Append($"1. a steady pace{Environment.NewLine}");
            _pace.Append($"2. a strenuous pace{Environment.NewLine}");
            _pace.Append($"3. a grueling pace{Environment.NewLine}");
            _pace.Append($"4. find out what these{Environment.NewLine}");
            _pace.Append($"   different paces mean");
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            return _pace.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            switch (input.ToUpperInvariant())
            {
                case "1":
                    GameSimulationApp.Instance.Vehicle.ChangePace(TravelPace.Steady);
                    ClearState();
                    break;
                case "2":
                    GameSimulationApp.Instance.Vehicle.ChangePace(TravelPace.Strenuous);
                    ClearState();
                    break;
                case "3":
                    GameSimulationApp.Instance.Vehicle.ChangePace(TravelPace.Grueling);
                    ClearState();
                    break;
                case "4":
                    SetState(typeof (PaceAdviceState));
                    break;
                default:
                    SetState(typeof (ChangePaceState));
                    break;
            }
        }
    }
}