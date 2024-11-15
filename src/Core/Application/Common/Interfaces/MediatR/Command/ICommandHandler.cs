namespace CleanTib.Application.Common.Interfaces;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
 where TCommand : ICommand<TResponse>
 where TResponse : notnull;

public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Result<DefaultIdType>>
    where TCommand : ICommand;