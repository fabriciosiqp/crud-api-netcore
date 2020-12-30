using crud.Data;
using crud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace crud.Controllers
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("v1/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly DataContext _context;

        public CategoryController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        ///  Get all categories
        /// </summary>
        /// <response code="200">The category list was successfully obtained.</response>
        /// <response code="500">There was an error fetching the category list.</response>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Category>>> Get()
        {
            try
            {
                var categories = await _context.Categories
                    .AsNoTracking()
                    .ToListAsync();

                return categories;
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

        }

        /// <summary>
        ///  Get category by ID
        /// </summary>
        /// <param name="id">Category Id</param>
        /// <response code="200">The category was successfully obtained.</response>
        /// <response code="404">Category not found with specified ID.</response>
        /// <response code="500">An error occurred while obtaining the category.</response>
        [HttpGet]
        [Route("{id::Guid}")]
        public async Task<ActionResult<Category>> GetById(Guid id)
        {
            try
            {
                var category = await _context.Categories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (category is null) return NotFound();

                return category;
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

        }

        /// <summary>
        /// Register category
        /// </summary>
        /// <param name="model">Category model</param>
        /// <response code="201">  The category was successfully registered.</response>
        /// <response code="400">The category model is invalid.</response>
        /// <response code="500">An error occurred while registering the category.</response>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Category>> Post([FromBody] Category model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Categories.Add(model);
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
        /// Update category
        /// </summary>
        /// <param name="model">Category model</param>
        /// <response code="200">The category has been successfully updated.</response>
        /// <response code="400">The category model is invalid.</response>
        /// <response code="404">Category not found with specified ID.</response>
        /// <response code="500">An error occurred while updating the category.</response>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Category>> Put([FromBody] Category model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Categories.Attach(model);

                    _context.Entry(model).State = EntityState.Modified;

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
        /// Delete category
        /// </summary>
        /// <param name="id">Category Id</param>
        /// <response code="200">The category has been successfully deleted.</response>
        /// <response code="400">The category model is invalid.</response>
        /// <response code="404">Category not found with specified ID.</response>
        /// <response code="500">There was an error deleting the category.</response>
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<Category>> Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                    if (category is null)
                    {
                        return NotFound();
                    }

                    _context.Categories.Remove(category);

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
