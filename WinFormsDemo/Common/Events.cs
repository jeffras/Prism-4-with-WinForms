using Common.dto;
using Microsoft.Practices.Prism.Events;

namespace Common.Events
{
    public class ProductSavedEvent : CompositePresentationEvent<Product> //CompositeEvent<Product>
    {
    }
}
