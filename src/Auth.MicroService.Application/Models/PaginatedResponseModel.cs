using System.Collections.Generic;

namespace Auth.MicroService.Application.Models
{
    public record PaginatedResponseModel<T>(IEnumerable<T> Data, PaginationMeta Meta);

    public record PaginationMeta(
        int TotalItems,
        int TotalPages,
        int CurrentPage,
        int ItemsPerPage);
}