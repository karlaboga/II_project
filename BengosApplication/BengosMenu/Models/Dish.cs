using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Required for [NotMapped] 

namespace Bengos.Models
{
    public class Dish
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public double Price { get; set; }
        public string? Category { get; set; }

        // Properties from BillingAndPayment's Dish.cs (additional details)
        public string PreparationTime { get; set; } = "--";
        public string Alergies { get; set; } = "None";
        public string Steps { get; set; } = "No steps provided.";

        // Calculated properties for UI
        public string DisplayText => $"{Name} — {Price:0.00} RON";
        public string PriceDisplay => $"{Price:0.00} RON";

        // Navigation property for ingredients
        public virtual ICollection<DishIngredient> DishIngredients { get; set; } = new List<DishIngredient>();

        // Note: Properties like Unit, Quantity, MinStock from Produs.cs are not included here
        // as Dish represents a menu item, not an ingredient/stock item.
    }
}
