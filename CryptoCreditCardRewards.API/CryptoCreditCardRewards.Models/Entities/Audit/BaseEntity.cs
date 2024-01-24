using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Events;

namespace CryptoCreditCardRewards.Models.Entities.Audit
{
    /// <summary>
    /// Inherit from this is the poco used requires auditing in database
    /// </summary>
    public abstract class BaseEntity : IAuditableProperties
    {
        /// <summary>
        /// When the entity was created
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Who the entity was created by
        /// </summary>
        public int? CreatedById { get; set; }

        /// <summary>
        /// When the entity was updated (if ever)
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Who the entity was updated by (if updated)
        /// </summary>
        public int? UpdatedById { get; set; }

        [NotMapped]
        public List<IDomainEvent> DomainEvents { get; } = new List<IDomainEvent>();

        public void QueueDomainEvent(IDomainEvent @event)
        {
            DomainEvents.Add(@event);
        }
    }
}
