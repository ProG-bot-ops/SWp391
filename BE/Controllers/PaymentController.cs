using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Service;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var payments = await _paymentService.GetAllAsync();
        return Ok(payments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment == null)
            return NotFound();
        return Ok(payment);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Payment payment)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdPayment = await _paymentService.CreatePaymentAsync(payment);
        return CreatedAtAction(nameof(GetById), new { id = createdPayment.Id }, createdPayment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Payment payment)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        if (id != payment.Id)
            return BadRequest("Id không khớp");

        var existingPayment = await _paymentService.GetByIdAsync(id);
        if (existingPayment == null)
            return NotFound();

        await _paymentService.UpdatePaymentAsync(payment);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment == null)
            return NotFound();

        await _paymentService.DeletePaymentAsync(id);
        return NoContent();
    }
}
