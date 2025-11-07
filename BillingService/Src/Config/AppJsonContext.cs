using System.Text.Json.Serialization;
using BillingService.Application.Common;
using BillingService.Domain.Entities;

namespace BillingService.Config;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ApiResultDto<InvoiceNoteDto>))]
[JsonSerializable(typeof(ApiResultDto<IEnumerable<InvoiceNoteDto>>))]
public partial class AppJsonContext : JsonSerializerContext
{
}