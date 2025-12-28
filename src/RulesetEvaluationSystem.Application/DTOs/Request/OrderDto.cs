using System;
using System.Collections.Generic;
using System.Text;

namespace RulesetEvaluationSystem.Application.DTOs.Request
{
    public class OrderDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string PublisherNumber { get; set; } = string.Empty;
        public string PublisherName { get; set; } = string.Empty;
        public string OrderMethod { get; set; } = string.Empty;

        public List<ShipmentDto> Shipments { get; set; } = new();
        public List<ItemDto> Items { get; set; } = new();
    }

    public class ShipmentDto
    {
        public ShipToDto ShipTo { get; set; } = new();
    }

    public class ShipToDto
    {
        public string IsoCountry { get; set; } = string.Empty;
    }

    public class ItemDto
    {
        public string Sku { get; set; } = string.Empty;
        public int PrintQuantity { get; set; }
        public List<ComponentDto> Components { get; set; } = new();
    }

    public class ComponentDto
    {
        public string Code { get; set; } = string.Empty;
        public Dictionary<string, string> Attributes { get; set; } = new();
    }
}
