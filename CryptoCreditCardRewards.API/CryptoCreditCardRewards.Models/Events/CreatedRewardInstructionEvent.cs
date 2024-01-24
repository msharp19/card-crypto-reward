using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;

namespace CryptoCreditCardRewards.Models.Events
{
    public class CreatedRewardInstructionEvent : IDomainEvent
    {
        public Instruction Instruction { get; set; }

        public CreatedRewardInstructionEvent(Instruction instruction)
        {
            Instruction = instruction;
        }
    }
}
