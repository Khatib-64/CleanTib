namespace CleanTib.Application.Common.Interfaces;

public interface ICommand<T> : IRequest<T>;
public interface ICommand : IRequest<Result<DefaultIdType>>;
public interface IQuery<T> : IRequest<Result<T>>;
