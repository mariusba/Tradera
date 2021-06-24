using Tradera.Models.ServiceResolvers;

namespace Tradera.Models
{
    public class ProcessorIdentifier
    {
        public ExchangeName Name { get; init; }
        public string Pair { get; init; }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + Name.ToString().GetHashCode();
            hash = (hash * 7) + Pair.GetHashCode();
            return hash;
        }

        public override bool Equals(object? obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }
    }
}