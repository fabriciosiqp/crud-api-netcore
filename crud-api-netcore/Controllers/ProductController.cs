using crud.Data;
using crud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace crud.Controllers
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("v1/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        ///  Get all products
        /// </summary>
        /// <response code="200">The product list was successfully obtained.</response>
        /// <response code="500">There was an error fetching the product list.</response>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Product>>> Get()
        {
            try
            {
                var products = await _context.Products
                    .Include(x => x.Category)
                    .AsNoTracking()
                    .ToListAsync();

                return products;
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }


        }

        /// <summary>
        ///  Get product by ID
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <response code="200">The product was successfully obtained.</response>
        /// <response code="404">Product not found with specified ID.</response>
        /// <response code="500">An error occurred while obtaining the product.</response>
        [HttpGet]
        [Route("{id::Guid}")]
        public async Task<ActionResult<Product>> GetById(Guid id)
        {
            try
            {
                var product = await _context.Products
                           .Include(x => x.Category)
                           .AsNoTracking()
                           .FirstOrDefaultAsync(x => x.Id == id);

                if (product is null) return NotFound();

                return product;
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

        }

        /// <summary>
        ///  Get product by category ID
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <response code="200">The product list by category was obtained.</response>
        /// <response code="404">Products not found with the specified category.</response>
        /// <response code="500">There was an error fetching the product list.</response>
        [HttpGet]
        [Route("category/{id::Guid}")]
        public async Task<ActionResult<List<Product>>> GetByCategory(Guid id)
        {
            try
            {
                if (Guid.Empty == id)
                {
                    return NotFound();
                }

                var products = await _context.Products
                    .Include(x => x.Category)
                    .Where(x => x.CategoryId == id)
                    .AsNoTracking()
                    .ToListAsync();

                return products;
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

        }



        /// <summary>
        /// Register product
        /// </summary>
        /// <param name="model">Product model</param>
        /// <response code="201">  The product was successfully registered.</response>
        /// <response code="400">The product model is invalid.</response>
        /// <response code="500">An error occurred while registering the product.</response>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Product>> Post([FromBody] Product model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Products.Add(model);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
                }
                catch (Exception)
                {

                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }

            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Update product
        /// </summary>
        /// <param name="model">Product model</param>
        /// <response code="200">The product has been successfully updated.</response>
        /// <response code="400">The product model is invalid.</response>
        /// <response code="404">Product not found with specified ID.</response>
        /// <response code="500">An error occurred while updating the product.</response>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Product>> Put([FromBody] Product model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == model.Id);

                    if (product is null) return NotFound();

                    product.Title = model.Title;
                    product.Description = model.Description;
                    product.Price = model.Price;

                    await _context.SaveChangesAsync();

                    return model;
                }
                catch (Exception)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }


            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Delete product
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <response code="200">The product has been successfully deleted.</response>
        /// <response code="400">The product model is invalid.</response>
        /// <response code="404">Product not found with specified ID.</response>
        /// <response code="500">There was an error deleting the product.</response>
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<Product>> Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

                    if (product is null)
                    {
                        return NotFound();
                    }

                    _context.Products.Remove(product);

                    await _context.SaveChangesAsync();

                    return Ok();
                }
                catch (Exception)
                {

                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }

            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
