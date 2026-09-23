using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Application.ViewModels;

namespace TeamProjectManagement.Application.Mappings
{
    public static partial class MappingExtensions
    {
        public static PagedViewModel<TViewModel> ToPagedViewModel<TEntity, TViewModel>(
            this PagedResult<TEntity> pagedResult,
            Func<TEntity, TViewModel> mapFunc)
        {
            return new PagedViewModel<TViewModel>
            {
                Items = pagedResult.Items.Select(mapFunc).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages,
                HasNext = pagedResult.HasNext,
                HasPrevious = pagedResult.HasPrevious
            };
        }
    }
}
