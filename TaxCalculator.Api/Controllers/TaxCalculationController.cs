using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaxCalculator.Application.TaxCalculation.Commands;

namespace TaxCalculator.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaxCalculationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TaxCalculationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Calculate([FromBody] CalculateTaxCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(command, cancellationToken);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}