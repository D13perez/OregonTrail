﻿using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Primary game mode of the simulation, used to show simulation advancing through linear time. Shows all major stats
    ///     of party and vehicle, plus climate and other things like distance traveled and distance to next point.
    /// </summary>
    public sealed class TravelingMode : GameMode<TravelCommands>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.TravelingMode" /> class.
        /// </summary>
        public TravelingMode() : base(false)
        {
            // Keep track of basic information about menu choices, vehicle and party stats, trades, advice, etc.
            TravelInfo = new TravelInfo();

            // Update menu with proper choices.
            UpdateLocation();
        }

        /// <summary>
        ///     Traveling game mode has a mode state information object that is used to keep track of any important info about the
        ///     state like how many days we should rest.
        /// </summary>
        private TravelInfo TravelInfo { get; }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override ModeType ModeType
        {
            get { return ModeType.Travel; }
        }

        /// <summary>
        ///     Attaches state that picks strings from array at random to show from point of interest.
        /// </summary>
        private void TalkToPeople()
        {
            CurrentState = new TalkToPeopleState(this, TravelInfo);
        }

        /// <summary>
        ///     Attached store game mode on top of existing game mode for purchasing items from this location.
        /// </summary>
        private void BuySupplies()
        {
            GameSimulationApp.Instance.AddMode(ModeType.Store);
        }

        /// <summary>
        ///     Resumes the simulation and progression down the trail towards the next point of interest.
        /// </summary>
        private void ContinueOnTrail()
        {
            CurrentState = new ContinueOnTrailState(this, TravelInfo);
        }

        /// <summary>
        ///     Shows current load out for vehicle and player inventory items.
        /// </summary>
        private void CheckSupplies()
        {
            CurrentState = new CheckSuppliesState(this, TravelInfo);
        }

        /// <summary>
        ///     Shows players current position on the total trail along with progress indicators so they know how much more they
        ///     have and what they have accomplished.
        /// </summary>
        private void LookAtMap()
        {
            CurrentState = new LookAtMapState(this, TravelInfo);
        }

        /// <summary>
        ///     Changes the number of miles the vehicle will attempt to move in a single day.
        /// </summary>
        private void ChangePace()
        {
            CurrentState = new ChangePaceState(this, TravelInfo);
        }

        /// <summary>
        ///     Changes the amount of food in pounds the vehicle party members will consume each day of the simulation.
        /// </summary>
        private void ChangeFoodRations()
        {
            CurrentState = new ChangeRationsState(this, TravelInfo);
        }

        /// <summary>
        ///     Attaches state that will ask how many days should be ticked while sitting still, if zero is entered then nothing
        ///     happens.
        /// </summary>
        private void StopToRest()
        {
            CurrentState = new RestQuestionState(this, TravelInfo);
        }

        /// <summary>
        ///     Looks through the traveling information data for any pending trades that people might want to make with you.
        /// </summary>
        private void AttemptToTrade()
        {
            CurrentState = null;
            GameSimulationApp.Instance.AddMode(ModeType.Trade);
        }

        /// <summary>
        ///     Attaches a new mode on top of this one that allows the player to hunt for animals and kill them using bullets for a
        ///     specified time limit.
        /// </summary>
        public void Hunt()
        {
            CurrentState = null;
            GameSimulationApp.Instance.AddMode(ModeType.Hunt);
        }

        /// <summary>
        ///     Determines if there is a store, people to get advice from, and a place to rest, what options are available, etc.
        /// </summary>
        private void UpdateLocation()
        {
            // Header text for above menu comes from travel info object.
            var headerText = new StringBuilder();
            headerText.Append(TravelInfo.TravelStatus);
            headerText.Append("You may:");
            MenuHeader = headerText.ToString();

            // Reset and calculate what commands are allowed at this current point of interest on the trail.
            ClearCommands();
            AddCommand(ContinueOnTrail, TravelCommands.ContinueOnTrail, "Continue on trail");
            AddCommand(CheckSupplies, TravelCommands.CheckSupplies, "Check supplies");
            AddCommand(LookAtMap, TravelCommands.LookAtMap, "Look at map");
            AddCommand(ChangePace, TravelCommands.ChangePace, "Change pace");
            AddCommand(ChangeFoodRations, TravelCommands.ChangeFoodRations, "Change food rations");
            AddCommand(StopToRest, TravelCommands.StopToRest, "Stop to rest");
            AddCommand(AttemptToTrade, TravelCommands.AttemptToTrade, "Attempt to trade");
            AddCommand(TalkToPeople, TravelCommands.TalkToPeople, "Talk to people");
            AddCommand(BuySupplies, TravelCommands.BuySupplies, "Buy supplies");
            AddCommand(HuntForFood, TravelCommands.HuntForFood, "Hunt for food");
        }

        /// <summary>
        /// </summary>
        private void HuntForFood()
        {
        }

        /// <summary>
        ///     Fired when trail simulation has determined the vehicle and player party has reached the next point of interest in
        ///     the trail.
        /// </summary>
        /// <param name="nextPoint"></param>
        protected override void OnReachPointOfInterest(Location nextPoint)
        {
            base.OnReachPointOfInterest(nextPoint);

            // Check if the point is us, and a location we need to welcome player into.
            if (nextPoint != CurrentPoint)
                return;

            // On the first point we are going to force the look around state onto the traveling mode without asking.
            if (GameSimulationApp.Instance.Trail.IsFirstPointOfInterest())
            {
                CurrentState = new LookAroundState(this, TravelInfo);
            }
            else if (!GameSimulationApp.Instance.Trail.IsFirstPointOfInterest() &&
                     GameSimulationApp.Instance.Vehicle.Odometer > 0 &&
                     GameSimulationApp.Instance.TotalTurns > 0)
            {
                // Ensure we only ask if the player wants to stop when it is really not the first turn.
                CurrentState = new LookAroundQuestionState(this, TravelInfo);
            }
        }

        /// <summary>
        ///     Fired when the current game modes state is altered, it could be removed and null or a new one added up to
        ///     implementation to check.
        /// </summary>
        protected override void OnStateChanged()
        {
            base.OnStateChanged();

            // Skip if current state is not null.
            if (CurrentState != null)
                return;

            // Update menu with proper choices.
            UpdateLocation();
        }
    }
}