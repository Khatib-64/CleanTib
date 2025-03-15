namespace CleanTib.Application.Common.Interfaces;

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    /// <summary>
    /// Handles a query.
    /// </summary>
    /// <param name="query">The query to be executed.</param>
    /// <param name="cancellationToken">CancellCancellation Token of the query.</param>
    /// <returns>TResponse of the query.</returns>
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}