using System;

namespace EventPublisher.Model
{
    public abstract class EventBase
    {
        private Guid Id { get; set; }
        private string SpecVersion { get; set; }
        private string Source { get; set; }
        private DateTime Time { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid AggregateId { get; set; }
        public Guid XeroUserId { get; set; }
        public Guid XeroOrganisationId { get; set; }
        public Guid XeroPartnerId { get; set; }
    }
}