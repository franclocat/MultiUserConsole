﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO;

public class ProductDTO
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Title { get; set; }

    [MaxLength(100)]
    public string Description { get; set; }

    [Timestamp]
    public byte[]? Timestamp { get; set; }

    public ProductDTO Clone()
    {
        return new ProductDTO
        {
            Id = Id,
            Title = Title,
            Description = Description
        };
    }

    public override string ToString()
    {
        return $"Id: {Id}   |Title: {Title} |Description: {Description}";
    }
}
