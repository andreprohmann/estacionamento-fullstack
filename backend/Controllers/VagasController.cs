
using Estacionamento.Api.Dtos;
using Estacionamento.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estacionamento.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VagasController : ControllerBase
    {
        private readonly IVagaService _service;
        public VagasController(IVagaService service) { _service = service; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VagaResponseDto>>> GetAll([FromQuery] bool abertas = false)
            => Ok(await _service.GetAllAsync(abertas));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VagaResponseDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<VagaResponseDto>> Create([FromBody] VagaCreateDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<VagaResponseDto>> Update(int id, [FromBody] VagaUpdateDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (updated is null) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}/checkout")]
        public async Task<ActionResult<VagaResponseDto>> Checkout(int id, [FromBody] VagaCheckoutDto dto)
        {
            try
            {
                var closed = await _service.CheckoutAsync(id, dto);
                if (closed is null) return NotFound();
                return Ok(closed);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
