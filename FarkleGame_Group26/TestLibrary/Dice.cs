/*
 * Program:   FarkleLibrary
 * Module:      Dice.cs
 * Author:      Dustin Taylor, Hongseok Kim, Donghao Tang, Christopher Russell
 * Date:        March 30, 2023
 * Description: The implementation of a Dice used in the Farkle game.  
 */
using System;
using System.Runtime.Serialization;

namespace FarkleLibrary
{
    [DataContract]
    public class Dice
    {
        [DataMember]
        public bool IsScored { get; set; }
        [DataMember]
        public int Value { get; set; }

        public Dice()
        {
            IsScored = false;
        }

        public void Roll()
        {
            var random = new Random();
            Value = random.Next(1, 7);
        }

        public void SetAside()
        {
            IsScored = true;
        }

        public void SetPlayable()
        {
            IsScored = false;
        }


    }
}