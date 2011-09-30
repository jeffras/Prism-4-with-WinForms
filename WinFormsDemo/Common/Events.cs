using System;
using Common.dto;
using Microsoft.Practices.Prism.Events;

namespace Common.Events
{
    public class ProductSavedEvent : CompositePresentationEvent<Product>
    {
    }

    public class CreateProductEvent : CompositePresentationEvent<Product>
    {
    }

    public class CreateProductFailedEvent : CompositePresentationEvent<String>
    {
    }

    public class ValidateProductEvent : CompositePresentationEvent<Product>
    {
    }
}
