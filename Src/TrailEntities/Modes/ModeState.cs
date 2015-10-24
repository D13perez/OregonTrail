﻿using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Requires type parameter that is a reference type with a constructor.
    /// </summary>
    public abstract class ModeState<T> : IModeState where T : class, new()
    {
        private IMode _parentMode;
        private T _userData;

        /// <summary>
        ///     This constructor will create new state taking values from old state
        /// </summary>
        internal ModeState(ModeState<T> state) : this(state.ParentMode, state.UserData)
        {
        }

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        protected ModeState(IMode gameMode, T userData)
        {
            ParentMode = gameMode;
            UserData = userData;
        }

        /// <summary>
        ///     Intended to be overridden in abstract class by generics to provide method to return object that contains all the
        ///     data for parent game mode.
        /// </summary>
        public T UserData
        {
            get { return _userData; }
            set { _userData = value; }
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public abstract bool AcceptsInput { get; }

        /// <summary>
        ///     Intended to be overridden in abstract class by generics to provide method to return object that contains all the
        ///     data for parent game mode.
        /// </summary>
        object IModeState.GetUserData()
        {
            return GetUserData();
        }

        /// <summary>
        ///     Current parent game mode which this state is binded to and is doing work on behalf of.
        /// </summary>
        public IMode ParentMode
        {
            get { return _parentMode; }
            set { _parentMode = value; }
        }

        /// <summary>
        ///     Forces the current game mode state to update itself, this typically results in moving to the next state.
        /// </summary>
        public virtual void TickState()
        {
            // Nothing to see here, move along...
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public abstract string GetStateTUI();

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public abstract void OnInputBufferReturned(string input);

        /// <summary>
        ///     Fired when the active game mode has been changed in parent game mode, this is intended for game mode states only so
        ///     they can be aware of these changes and act on them if needed.
        /// </summary>
        /// <param name="modeType">Current mode which the simulation is changing to.</param>
        public virtual void OnParentModeChanged(ModeType modeType)
        {
            // Nothing to see here, move along...
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return GetType().Name;
        }

        /// <summary>
        ///     Actually processes the user data generics request and creates a new class to carry the information over to the next
        ///     game mode state.
        /// </summary>
        /// <returns></returns>
        private static T GetUserData()
        {
            if (!typeof (T).IsClass)
            {
                throw new InvalidCastException("T must be a class type!");
            }

            // TODO: Replace with JSON.net serialization/deserialization setup that works off new game info object.
            var primary = typeof (T);
            //var ofType = primary.GetGenericArguments();
            //var typeDef = primary.GetGenericTypeDefinition();
            return (T) Activator.CreateInstance(primary);
        }
    }
}