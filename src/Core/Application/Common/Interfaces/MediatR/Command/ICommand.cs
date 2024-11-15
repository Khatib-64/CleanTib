namespace CleanTib.Application.Common.Interfaces;

public interface ICommand<T> : IRequest<T>;
public interface ICommand : ICommand<Result<DefaultIdType>>;