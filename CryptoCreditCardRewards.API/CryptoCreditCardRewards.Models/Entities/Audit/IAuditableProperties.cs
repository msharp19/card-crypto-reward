using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Entities.Audit
{
    /// <summary>
    /// Any context poco implementing this will be auditable in database (createdDate,updatedDate,createdBy,updatedBy)
    /// </summary>
    public interface IAuditableProperties
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
    }
}
