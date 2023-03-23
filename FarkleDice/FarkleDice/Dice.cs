using System;
using System.Collections.Generic;
namespace Farkle
{
    public class Dice
    {
        public bool IsScored { get; set;}

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


    }
}