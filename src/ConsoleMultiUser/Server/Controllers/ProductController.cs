using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services.Interfaces;
using Shared.DTO;
using System.Data;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController (IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [Authorize(Roles = $"{Policies.Customer},{Policies.StoreWorker},{Policies.Admin}")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> Get()
    {
        return Ok(await _productService.Get());
    }


    [HttpPut("{id}")]
    [Authorize(Roles = Policies.Admin)]
    public async Task<ActionResult<ProductDTO>> Update(int id, ProductDTO productDto)
    {
        if (productDto.Id != id)
        {
            return BadRequest();
        }

        if (!await _productService.IsExisting(id)) 
        {
            return NotFound($"The product with id {id} could not be found.");
        }

        try
        {
            ProductDTO updatedProduct = await _productService.Update(id, productDto);
            return Ok(updatedProduct);
        }
        catch (DBConcurrencyException ce)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest($"An unhandled exception happened: {ex.Message}");
        }
    }

    [HttpPost]
    [Authorize(Roles = $"{Policies.StoreWorker},{Policies.Admin}")]
    public async Task<IActionResult> Add(ProductDTO productDto)
    {
        try
        {
            await _productService.Add(productDto);
        }
        catch (Exception e)
        {
            return BadRequest($"An exception occurred. {e.Message}");
        }
        return Ok("The product has been added.");
    }

}
